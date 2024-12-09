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
                return fetchedDetails.Dose1BookDate == DateTime.MinValue && fetchedDetails.Dose1ApproveDate == DateTime.MinValue; 
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
                if(d1Status)
                {
                    return fetchedDetails.Dose2BookDate == DateTime.MinValue && fetchedDetails.Dose2ApproveDate == DateTime.MinValue;
                }

                return false;
            }

            return false;
        }
    } 

}