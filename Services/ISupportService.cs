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
        public List<SupportDetailsDto_SupportTicketsList> FetchTicketsListByUserId(string userid);
    }

    public class SupportService: ISupportService
    {
        private static int _ticketCounters = 0;
        private readonly AppDbContext _v1RemDb;
        

        public SupportService(AppDbContext v1RemDb)
        {
            _v1RemDb = v1RemDb;
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

        public List<SupportDetailsDto_SupportTicketsList> FetchTicketsListByUserId(string userid)
        {
            if (_v1RemDb.SupportDetails == null)
            {
                return new List<SupportDetailsDto_SupportTicketsList>();
            }

            var fetchedDetails = _v1RemDb.SupportDetails
                .Where(record => record.UserId == userid)
                .ToList() // Fetch data first
                .Select(record => new SupportDetailsDto_SupportTicketsList
                {
                    SupportId = record.SupportId,
                    SupportStatus = GetTicketStatus(record.SupportStatus), // Map status after fetching data
                    SupportTitle = record.SupportTitle,
                    SupportRaisedDate = record.SupportRaisedDate.ToString("yyyy-MM-dd HH:mm:ss")
                }).ToList();

            return fetchedDetails;
        }

        private string CreateNewTicketId()
        {
            return $"INC{++SupportService._ticketCounters}";
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