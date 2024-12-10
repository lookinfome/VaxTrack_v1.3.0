using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using v1Remastered.Services;
using v1Remastered.Models;
using v1Remastered.Dto;
using System.Linq;


namespace v1Remastered.Services
{
    public interface IBookingService
    {
        // exposed to: user profile controller
        public bool IsD1Booked(string userId, string bookingId);
        public bool IsD2Booked(string userId, string bookingId);

        // exposed to: booking controller, user profile service
        public BookingDetailsDto_UserBookingDetails FetchBookingDetails(string userid);

        // exposed to: user profile controller
        public bool SaveNewBookingDetails(string userId, DateTime dose1Date, string hospitalId);
        
    }

    public class BookingService : IBookingService
    {
        private readonly AppDbContext _v1RemDb;
        private readonly IUserVaccineDetailsService _userVaccineDeailsService;
        private readonly IHospitalService _hospitalService;
        public BookingService(AppDbContext v1RemDb, IUserVaccineDetailsService userVaccineDetailsService, IHospitalService hospitalService)
        {
            _v1RemDb = v1RemDb;
            _userVaccineDeailsService = userVaccineDetailsService;
            _hospitalService = hospitalService;
        }

        
        
        // service method: to fetch user booking details 
        public BookingDetailsDto_UserBookingDetails FetchBookingDetails(string userid)
        {
            var bookingDetails = _v1RemDb.BookingDetails.FirstOrDefault(record => record.UserId == userid);

            if (bookingDetails != null)
            {
                var fetchedBookingDetails = new BookingDetailsDto_UserBookingDetails()
                {
                    BookingId = bookingDetails.BookingId,
                    Dose1BookDate = bookingDetails.Dose1BookDate, // Corrected this line
                    Dose2BookDate = bookingDetails.Dose2BookDate,
                    D1SlotNumber = bookingDetails.D1SlotNumber,
                    D2SlotNumber = bookingDetails.D2SlotNumber,
                    D1HospitalId = bookingDetails.D1HospitalId,
                    D2HospitalId = bookingDetails.D2HospitalId,
                    Dose1ApproveDate = bookingDetails.Dose1ApproveDate,
                    Dose2ApproveDate = bookingDetails.Dose2ApproveDate,
                };

                return fetchedBookingDetails;
            }

            return new BookingDetailsDto_UserBookingDetails();
        }

        // service method: to fetch dose 1 booking status
        public bool IsD1Booked(string userId, string bookingId)
        {
            var fetchedDetails = _v1RemDb.BookingDetails.FirstOrDefault(record=>record.UserId == userId && record.BookingId == bookingId);
            if(fetchedDetails != null)
            {
                return fetchedDetails.Dose1BookDate != DateTime.MinValue; 
            }
            return false;
        }

        // service method: to fetch dose 2 booking status
        public bool IsD2Booked(string userId, string bookingId)
        {
            var fetchedDetails = _v1RemDb.BookingDetails.FirstOrDefault(record=>record.UserId == userId && record.BookingId == bookingId);
            if(fetchedDetails != null)
            {
                bool d1Status = IsD1Booked(userId, bookingId);
                if(d1Status && fetchedDetails.Dose1ApproveDate != DateTime.MinValue)
                {
                    return fetchedDetails.Dose2BookDate != DateTime.MinValue;
                }

                return true;
            }

            return false;
        }
    
        // service method: to save new booking details
        public bool SaveNewBookingDetails(string userId, DateTime dose1Date, string hospitalId)
        {
            int availableSlot = -1;
            BookingDetailsModel bookingDetails = new BookingDetailsModel();
            HospitalDetailsModel hospitalDetails = _hospitalService.FetchHospitalDetailsById(hospitalId);

            if (hospitalDetails != null) 
            {
                availableSlot = hospitalDetails.HospitalAvailableSlots;
            }

            // map booking id
            bookingDetails.BookingId = GenerateBookingId(userId); 

            // map user id
            bookingDetails.UserId = userId;

            // map user vaccination id
            bookingDetails.UserVaccinationId = _userVaccineDeailsService.FetchUserVaccinationID(userId);

            // map dose 1 date
            bookingDetails.Dose1BookDate = dose1Date;

            // map hospital id
            bookingDetails.D1HospitalId = hospitalId;

            // map available details
            bookingDetails.D1SlotNumber = availableSlot;

            // save and update db
            _v1RemDb.BookingDetails.Add(bookingDetails);
            int saveBookingStatus = _v1RemDb.SaveChanges();

            if(saveBookingStatus > 0)
            {
                hospitalDetails.HospitalAvailableSlots = availableSlot - 1;
                _v1RemDb.HospitalDetails.Update(hospitalDetails);
                int updateHospitalStatus = _v1RemDb.SaveChanges();

                return updateHospitalStatus > 0;
            }
            else
            {
                return false; // booking failed
            }
        }

        // utility method: generate new booking id
        private string GenerateBookingId(string userid)
        {
            Random rnd = new Random();
            int randomNum = rnd.Next(100,1000);

            return $"{userid}_B{randomNum}";
        }
    
    } 

}