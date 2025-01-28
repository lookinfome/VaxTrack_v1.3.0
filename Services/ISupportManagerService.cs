using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using v1Remastered.Services;
using v1Remastered.Models;
using v1Remastered.Dto;
using System.Linq;

namespace v1Remastered.Services
{
    public interface ISupportManagerService
    {
        // exposed to: support manager controller
        public int FetchTicketsCount();

        // exposed to: support manager controller
        public List<SupportDetailsDto_SupportTicketsList> FetchTicketList();

        // exposed to: support manager controller
        public SupportDetailsDto_SupportTicketDetailsView FetchTicketDetails(string supportid);

    }

    public class SupportManagerService: ISupportManagerService
    {
        private readonly AppDbContext _v1RemDb;
        private readonly IUserProfileService _userProfileService;

        private readonly List<string> TicketStatusList = new List<string>(){
            "new",
            "work in progress",
            "pending",
            "resolved",
            "closed"
        };
        
        public SupportManagerService(AppDbContext v1RemDb, IUserProfileService userProfileService)
        {
            _v1RemDb = v1RemDb;
            _userProfileService = userProfileService;
        }

        // service method: to fetch total ticket count
        public int FetchTicketsCount()
        {
            var fetchedDetails = _v1RemDb.SupportDetails.ToList();

            if(fetchedDetails != null)
            {
                return fetchedDetails.Count();
            }

            return 0;
        }

        // service method: to fetch list of tickets
        public List<SupportDetailsDto_SupportTicketsList> FetchTicketList()
        {
            var fetchedDetails = _v1RemDb.SupportDetails
                                    .ToList().OrderBy(record=>record.SupportRaisedDate).Reverse()
                                    .Select(record => new SupportDetailsDto_SupportTicketsList
                                    {
                                        SupportId = record.SupportId,
                                        SupportStatus = GetTicketStatus(record.SupportStatus), // Map status after fetching data
                                        SupportTitle = record.SupportTitle,
                                        SupportRaisedDate = record.SupportRaisedDate.ToString("yyyy-MM-dd HH:mm:ss")
                                    }).ToList();

            if(fetchedDetails.Count > 0)
            {
                return fetchedDetails;
            }

            return new List<SupportDetailsDto_SupportTicketsList>();

        }

        // service method: to fetch ticket details
        public SupportDetailsDto_SupportTicketDetailsView FetchTicketDetails(string supportid)
        {
            string userid = GetUserId(supportid);

            // var fetchedTicketDetails = _v1RemDb.SupportDetails.FirstOrDefault(record => record.UserId == userid && record.SupportId == supportid);
            var fetchedTicketDetails = _v1RemDb.SupportDetails.FirstOrDefault(record => record.SupportId == supportid);
            if (fetchedTicketDetails == null)
            {
                return new SupportDetailsDto_SupportTicketDetailsView();
            }

            var fetchedCommentDetails = _v1RemDb.SupportConversations
                                                .Where(record => record.SupportId == supportid)
                                                .OrderByDescending(record => record.SupportCommentDate)
                                                .ToList();

            var mappedCommentDetailsList = fetchedCommentDetails.Select(comment => new SupportDetailsDto_SupportCommentsDetails
            {
                UserName = _userProfileService.FetchUserName(comment.UserId),
                SupportCommentId = comment.SupportCommentId,
                SupportId = comment.SupportId,
                SupportComment = comment.SupportComment,
                SupportCommentDate = comment.SupportCommentDate
            }).ToList();

            var mappedDetails = new SupportDetailsDto_SupportTicketDetailsView
            {
                SupportId = fetchedTicketDetails.SupportId,
                SupportStatus = GetTicketStatus(fetchedTicketDetails.SupportStatus),
                SupportTitle = fetchedTicketDetails.SupportTitle,
                SupportDescription = fetchedTicketDetails.SupportDescription,
                SupportRaisedDate = fetchedTicketDetails.SupportRaisedDate,
                SupportComments = mappedCommentDetailsList
            };

            return mappedDetails;
        }

        // utility method: to get ticket status
        private string GetTicketStatus(int status)
        {
            switch (status)
            {
                case 0: return "new";
                case 1: return "work in progress";
                case 2: return "pending";
                case 3: return "resolved";
                case 4: return "closed";
                default: return "unknown status"; // Optional: handle unexpected status values
            }
        }

        // utility method: to get userid of ticket
        private string GetUserId(string supportid)
        {
            var supportDetail = _v1RemDb.SupportDetails.FirstOrDefault(record => record.SupportId == supportid);
            return supportDetail?.UserId;
        }

    }
}