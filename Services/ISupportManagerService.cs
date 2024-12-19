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
        public List<SupportDetailsDto_SupportTicketsList> FetchTicketsList();
    }

    public class SupportManagerService: ISupportManagerService
    {
        private readonly AppDbContext _v1RemDb;
        
        public SupportManagerService(AppDbContext v1RemDb)
        {
            _v1RemDb = v1RemDb;
        }

        public List<SupportDetailsDto_SupportTicketsList> FetchTicketsList()
        {
            return new List<SupportDetailsDto_SupportTicketsList>();
        }
    }
}