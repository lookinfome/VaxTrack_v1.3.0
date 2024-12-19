using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using v1Remastered.Services;
using v1Remastered.Models;
using v1Remastered.Dto;
using System.Linq;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;



namespace v1Remastered.Services
{
    public interface IAccountService
    {

        // exposed to: account controller
        public string LoginUser(UserDetailsDto_Login submittedDetails);

        // exposed to: account controller
        public string RegisterUser(UserDetailsDto_Register submittedDetails);
    }

    public class AccountService : IAccountService
    {
        private readonly AppDbContext _v1RemDb;
        private readonly IAuthService _authService;
        private readonly IUserVaccineDetailsService _userVaccineDetailsService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private static int _userSequenceCounter = 0;
        
        public AccountService(AppDbContext v1RemDb, IAuthService authService, IUserVaccineDetailsService userVaccineDetailsService, IWebHostEnvironment webHostEnvironment)
        {
            _v1RemDb = v1RemDb;
            _authService = authService;
            _webHostEnvironment = webHostEnvironment;
            _userVaccineDetailsService = userVaccineDetailsService;
        }

        // service method: login new user
        public string LoginUser(UserDetailsDto_Login submittedDetails)
        {
            Task<string> result = _authService.LoginUserAsync(submittedDetails.UserId, submittedDetails.UserPassword);

            return !string.IsNullOrEmpty(result.Result.ToString()) && result.Result.ToString() == submittedDetails.UserId ? submittedDetails.UserId : "";
        }

        // service method: register new user
        public string RegisterUser(UserDetailsDto_Register submittedDetails)
        {
            // user id
            submittedDetails.UserId = GenerateUserId(submittedDetails.UserName, submittedDetails.UserUid);

            // user role
            string userRole = submittedDetails.UserRole == false ? "user" : "admin";

            // register user and login
            Task<string> result = _authService.RegisterUserAsync(submittedDetails.UserId, submittedDetails.UserPassword, userRole);

            // save user details in DB
            if(!string.IsNullOrEmpty(result.Result.ToString()) && result.Result.ToString() == submittedDetails.UserId)
            {

                bool isUserDetailsSaved = SaveNewUserDetails(submittedDetails);

                bool isUserVaccineDetailsSaved = isUserDetailsSaved ? _userVaccineDetailsService.SaveNewUserVaccinationDetails(submittedDetails.UserId) : false;

                return isUserVaccineDetailsSaved ? submittedDetails.UserId : "";
            }

            return "";
        
        }

        // utility method: generate new user id
        private string GenerateUserId(string username, string userUid)
        {
            string _newUserId;
            bool isDuplicate;

            // Extract initials
            string[] nameParts = username.Split(' ');
            string initials = string.Empty;
            foreach (string part in nameParts)
            {
                if (!string.IsNullOrEmpty(part))
                {
                    initials += part[0];
                }
            }

            do
            {
                _newUserId = $"{initials}_U{(++_userSequenceCounter).ToString("D2")}";
                var fetchedDetails = _v1RemDb.UserDetails.FirstOrDefault(record => record.UserId == _newUserId);
                isDuplicate = fetchedDetails != null;
            } while (isDuplicate);

            return _newUserId.ToUpper();
        }

        // utility method: save new user record
        private bool SaveNewUserDetails(UserDetailsDto_Register submittedDetails)
        {

            // map details to user details model
            UserDetailsModel userDetails = new UserDetailsModel()
            {
                UserId = submittedDetails.UserId,
                UserName = submittedDetails.UserName,
                UserUid = submittedDetails.UserUid,
                UserPhone = submittedDetails.UserPhone,
                UserGender = submittedDetails.UserGender,
                UserBirthdate = submittedDetails.UserBirthdate,
                UserRole = submittedDetails.UserRole
            };

            // save image details
            if (submittedDetails.ProfilePicture != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "assets");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + submittedDetails.ProfilePicture.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    submittedDetails.ProfilePicture.CopyTo(fileStream);
                }
                userDetails.ProfilePicturePath = "/assets/" + uniqueFileName;
            }
            else 
            {
                if(submittedDetails.UserGender.ToUpper() == "M")
                {
                    userDetails.ProfilePicturePath = "/assets/man.png";
                }
                else if(submittedDetails.UserGender.ToUpper() == "F")
                {
                    userDetails.ProfilePicturePath = "/assets/woman.png";
                }
                else 
                {
                    userDetails.ProfilePicturePath = "/assets/default-0.png";
                }
            }


            // save record to DB and update DB
            _v1RemDb.UserDetails.Add(userDetails);
            int userDetailsSavedStatus = _v1RemDb.SaveChanges();

            return userDetailsSavedStatus > 0;
        }



    }
}