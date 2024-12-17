using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using v1Remastered.Services;
using v1Remastered.Models;
using v1Remastered.Dto;
using System.Linq;

namespace v1Remastered.Services
{
    public interface ISupportService
    {
        // exposed to: support controller
        public bool SaveNewTicket(SupportDetailsDto_SupportForm submitedDetails, string userid);

        // exposed to: support controller
        public bool SaveNewComment(string userid, string supportid, string submittedComment);
        
        // exposed to: support controller
        public List<SupportDetailsDto_SupportTicketsList> FetchTicketsListByUserId(string userid);
    
        // exposed to: support controller
        public SupportDetailsDto_SupportTicketDetailsView FetchTicketDetailsByUserIdTicketId(string userid, string supportid);

    }

    public class SupportService: ISupportService
    {
        private readonly AppDbContext _v1RemDb;
        private readonly IUserProfileService _userProfileService;
        private static int _ticketCounters = 0;
        private static int _commentCounters = 0;

        public SupportService(AppDbContext v1RemDb, IUserProfileService userProfileService)
        {
            _v1RemDb = v1RemDb;
            _userProfileService = userProfileService;
        }
        
        public bool SaveNewTicket(SupportDetailsDto_SupportForm submitedDetails, string userid)
        {
            SupportDetailsModel newIncident = new SupportDetailsModel();
            newIncident.SupportId = CreateNewTicketId();
            newIncident.SupportStatus = 0;
            newIncident.SupportTitle = submitedDetails.SupportTitle;
            newIncident.SupportDescription = submitedDetails.SupportDescription;
            newIncident.SupportRaisedDate = DateTime.UtcNow;
            newIncident.UserId = userid;

            _v1RemDb.SupportDetails.Add(newIncident);
            int newIncidentSaveStatus = _v1RemDb.SaveChanges();

            return newIncidentSaveStatus <=0 ? false : true;
        }

        public bool SaveNewComment(string userid, string supportid, string submittedComment)
        {
            // generate new comment id
            string newCommentId = CreateNewCommentId();

            // map details
            SupportConversationsModel mappedDetails = new SupportConversationsModel()
            {
                SupportCommentId = newCommentId,
                SupportId = supportid,
                SupportComment = submittedComment,
                SupportCommentDate = DateTime.UtcNow,
                UserId = userid
            }; 
            
            // save to DB
            _v1RemDb.SupportConversations.Add(mappedDetails);

            // update DB
            int saveNewCommentStatus = _v1RemDb.SaveChanges();

            return saveNewCommentStatus >0 ? true : false;
        }

        public List<SupportDetailsDto_SupportTicketsList> FetchTicketsListByUserId(string userid)
        {
            if (_v1RemDb.SupportDetails == null)
            {
                return new List<SupportDetailsDto_SupportTicketsList>();
            }

            var fetchedDetails = _v1RemDb.SupportDetails
                .Where(record => record.UserId == userid)
                .ToList().OrderBy(record=>record.SupportRaisedDate).Reverse() // Fetch data first
                .Select(record => new SupportDetailsDto_SupportTicketsList
                {
                    SupportId = record.SupportId,
                    SupportStatus = GetTicketStatus(record.SupportStatus), // Map status after fetching data
                    SupportTitle = record.SupportTitle,
                    SupportRaisedDate = record.SupportRaisedDate.ToString("yyyy-MM-dd HH:mm:ss")
                }).ToList();

            return fetchedDetails;
        }

        public SupportDetailsDto_SupportTicketDetailsView FetchTicketDetailsByUserIdTicketId(string userid, string supportid)
        {
            var fetchedTicketDetails = _v1RemDb.SupportDetails.FirstOrDefault(record => record.UserId == userid && record.SupportId == supportid);
            var fetchedCommentDetails = _v1RemDb.SupportConversations
                                                .Where(record => record.UserId == userid && record.SupportId == supportid)
                                                .OrderByDescending(record => record.SupportCommentDate)
                                                .ToList();

            List<SupportDetailsDto_SupportCommentsDetails> mappedCommentDetailsList = new List<SupportDetailsDto_SupportCommentsDetails>();

            if (fetchedCommentDetails.Count > 0)
            {
                foreach (var comment in fetchedCommentDetails)
                {
                    string _username = _userProfileService.FetchUserName(comment.UserId);

                    SupportDetailsDto_SupportCommentsDetails mappedCommentDetails = new SupportDetailsDto_SupportCommentsDetails
                    {
                        UserName = _username,
                        SupportCommentId = comment.SupportCommentId,
                        SupportId = comment.SupportId,
                        SupportComment = comment.SupportComment,
                        SupportCommentDate = comment.SupportCommentDate
                    };

                    mappedCommentDetailsList.Add(mappedCommentDetails);
                }
            }

            SupportDetailsDto_SupportTicketDetailsView mappedDetails = new SupportDetailsDto_SupportTicketDetailsView
            {
                SupportId = fetchedTicketDetails.SupportId,
                SupportStatus = GetTicketStatus(fetchedTicketDetails.SupportStatus),
                SupportTitle = fetchedTicketDetails.SupportTitle,
                SupportDescription = fetchedTicketDetails.SupportDescription,
                SupportRaisedDate = fetchedTicketDetails.SupportRaisedDate,
                SupportComments = mappedCommentDetailsList
            };

            if (!string.IsNullOrEmpty(mappedDetails.SupportId))
            {
                return mappedDetails;
            }

            return new SupportDetailsDto_SupportTicketDetailsView();
        }
        


        private string CreateNewTicketId()
        {
            string _newSupportId;
            bool isDuplicate;

            do
            {
                _newSupportId = $"INC{(++_ticketCounters).ToString("D6")}";
                var fetchedDetails = _v1RemDb.SupportDetails.FirstOrDefault(record => record.SupportId == _newSupportId);
                isDuplicate = fetchedDetails != null;
            } while (isDuplicate);

            return _newSupportId;
        }

        private string CreateNewCommentId()
        {
            string _newCommentId;
            bool isDuplicate;

            do
            {
                _newCommentId = $"CMT{(++_commentCounters).ToString("D6")}";
                var fetchedCommentDetails = _v1RemDb.SupportConversations.FirstOrDefault(record=>record.SupportCommentId == _newCommentId);
                isDuplicate = fetchedCommentDetails != null;
            } while (isDuplicate);

            return _newCommentId;
        }

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
        
    }
}