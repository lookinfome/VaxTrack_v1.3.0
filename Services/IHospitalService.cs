using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using v1Remastered.Services;
using v1Remastered.Models;
using v1Remastered.Dto;
using System.Linq;


namespace v1Remastered.Services
{
    public interface IHospitalService
    {
        // exposed to: admin service, user profile service
        public string FetchHospitalNameById(string hospitalId);

        // exposed to: admin service, admin controller
        public List<HospitalDetailsModel> FetchHospitalsList();

        // exposed to: admin service
        
        // exposed to: booking service
        public string FetchHospitalIdyName(string hospitalName);

        // exposed to: booking service, booking controller
        public List<HospitalDetailsDto_HospitalDetails> FetchAvailableHospitalsList();


        /*******************************************************/

        // exposed to: user profile controller
        public List<string> FetchAvailableHospitalLocations();

        // exposed to: booking service, admin service 
        public HospitalDetailsModel FetchHospitalDetailsById(string hospitalId);

        
        // exposed to: user profile controller
        public List<HospitalDetailsDto_HospitalSlotBooking> FetchAvailableHospitalsByLocation(string hospitalLocation);
    }

    public class HospitalService : IHospitalService
    {
        private readonly AppDbContext _v1RemDb;

        public HospitalService(AppDbContext v1RemDb)
        {
            _v1RemDb = v1RemDb;
        }

        // fetch hospital name by id
        public string FetchHospitalNameById(string hospitalId)
        {
            var hospitalRecord = _v1RemDb.HospitalDetails.FirstOrDefault(record => record.HospitalId == hospitalId);
            
            if (hospitalRecord != null)
            {
                return !string.IsNullOrEmpty(hospitalRecord.HospitalName) ? hospitalRecord.HospitalName : "NA";
            }
            
            return "NA";
        }

        // fetch all hospital details
        public List<HospitalDetailsModel> FetchHospitalsList()
        {
            var hospitalList = _v1RemDb.HospitalDetails.ToList();
            if(hospitalList != null)
            {
                return hospitalList;
            }
            return new List<HospitalDetailsModel>();
        }

        // fetch all available hospitals details
        public List<HospitalDetailsDto_HospitalDetails> FetchAvailableHospitalsList()
        {
            var hospitalList = _v1RemDb.HospitalDetails
                                .Where(record=>record.HospitalAvailableSlots >= 1)
                                .Select(record => new HospitalDetailsDto_HospitalDetails 
                                    {
                                        HospitalName = record.HospitalName,
                                        HospitalAvailableSlots = record.HospitalAvailableSlots,
                                        HospitalLocation = record.HospitalLocation
                                    }
                                )
                                .ToList();
            if(hospitalList != null)
            {
                return hospitalList;
            }
            return new List<HospitalDetailsDto_HospitalDetails>();
        }

        // fetch hospital id by name
        public string FetchHospitalIdyName(string hospitalName)
        {
            var hospitalDetails = _v1RemDb.HospitalDetails.FirstOrDefault(record=>record.HospitalName == hospitalName);
            if(hospitalDetails != null)
            {
                return hospitalDetails.HospitalId;
            }

            return "";
        }

        // fetch hospital details by id
        public HospitalDetailsModel FetchHospitalDetailsById(string hospitalId)
        {
            var hospitalDetails = _v1RemDb.HospitalDetails.FirstOrDefault(record=>record.HospitalId == hospitalId);
            if(hospitalDetails != null)
            {
                return hospitalDetails;
            }
            return new HospitalDetailsModel();
        }
        
    
        // fetch available hospital locations
        public List<string> FetchAvailableHospitalLocations()
        {
            var fetchedDetails = _v1RemDb.HospitalDetails.Where(record=>record.HospitalAvailableSlots >=1).Select(record=>record.HospitalLocation).ToList();

            if(fetchedDetails != null)
            {
                return fetchedDetails;
            }

            return new List<string>();

        }

        // fetch available hospital names by location
        public List<HospitalDetailsDto_HospitalSlotBooking> FetchAvailableHospitalsByLocation(string hospitalLocation)
        {
            var fetchedDetails = _v1RemDb.HospitalDetails.Where(record=>record.HospitalLocation == hospitalLocation)
                                .Select(record => new HospitalDetailsDto_HospitalSlotBooking 
                                {
                                    HospitalId = record.HospitalId,
                                    HospitalName = record.HospitalName
                                }).ToList();

            if(fetchedDetails != null)
            {
                return fetchedDetails;
            }

            return new List<HospitalDetailsDto_HospitalSlotBooking>();
        }
    
    
    }

}