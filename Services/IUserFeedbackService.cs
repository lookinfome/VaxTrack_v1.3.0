using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using v1Remastered.Services;
using v1Remastered.Models;
using v1Remastered.Dto;
using System.Linq;

namespace v1Remastered.Services
{
    public interface IUserFeedbackService
    {

    }

    public class UserFeedbackService: IUserFeedbackService
    {
        private readonly AppDbContext _v1RemDb;

        public UserFeedbackService(AppDbContext v1RemDb)
        {
            _v1RemDb = v1RemDb;
        }

        
    }
}