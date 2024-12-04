using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using v1Remastered.Services;
using v1Remastered.Models;
using v1Remastered.Dto;
using System.Linq;

namespace v1Remastered.Services
{
    public interface IUserVaccineDetailsService
    {
        // exposed to: account service
        public bool SaveNewUserVaccinationDetails(string userid);

        // exposed to: booking service
        public string FetchUserVaccinationID(string userid);

        // exposed to: user profile service
        public UserVaccineDetailsDto_VaccineDetails FetchUserVaccinationDetails(string userid);
    }

    public class UserVaccineDetailsService : IUserVaccineDetailsService
    {
        private readonly AppDbContext _v1RemDb;

        public UserVaccineDetailsService(AppDbContext v1RemDb)
        {
            _v1RemDb = v1RemDb;
        }

        // service method: to save new user vaccination details
        public bool SaveNewUserVaccinationDetails(string userid)
        {
            // generate fresh new user vaccine id
            string userVaccinationId = GenerateUserVaccineId(userid);

            // map user vaccination details in model
            UserVaccineDetailsModel userVaccineDetails = new UserVaccineDetailsModel()
            {
                UserId = userid,
                UserVaccinationId = userVaccinationId,
                UserVaccinationStatus = 0
            };

            // save record to DB
            _v1RemDb.UserVaccineDetails.Add(userVaccineDetails);

            // update DB
            int userVaccineDetailsSavedStatus= _v1RemDb.SaveChanges();

            return userVaccineDetailsSavedStatus <=0 ? false : true;
        }

        // service method: to fetch user vaccination details
        public UserVaccineDetailsDto_VaccineDetails FetchUserVaccinationDetails(string userid)
        {
            var userVaccineDetails = _v1RemDb.UserVaccineDetails.FirstOrDefault(record => record.UserId == userid);

            if (userVaccineDetails != null && !string.IsNullOrEmpty(userVaccineDetails.UserId))
            {
                return new UserVaccineDetailsDto_VaccineDetails()
                {
                    UserId = userVaccineDetails.UserId,
                    UserVaccinationId = userVaccineDetails.UserVaccinationId,
                    UserVaccinationStatus = userVaccineDetails.UserVaccinationStatus == 0 ? "Not Vaccinated" : 
                                            userVaccineDetails.UserVaccinationStatus == 1 ? "Partially Vaccinated" : "Vaccinated"
                };
            }

            return new UserVaccineDetailsDto_VaccineDetails();
        }

        // service method: fetch user vaccination id
        public string FetchUserVaccinationID(string userid)
        {
            var userVaccinationId = _v1RemDb.UserVaccineDetails.FirstOrDefault(record=>record.UserId == userid);

            if(userVaccinationId != null && !string.IsNullOrEmpty(userVaccinationId.UserVaccinationId))
            {
                return userVaccinationId.UserVaccinationId;
            }

            return "";
        
        }

        // utility method: generate user vaccine id
        private string GenerateUserVaccineId(string userid)
        {
            Random rnd = new Random();
            int randomNum = rnd.Next(0,99);

            return $"{userid}_{randomNum}";
        }
    }
}