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
        // exposed to: booking controller [v2]
        public bool CheckD1ApprovalStatus(string userid);

        // exposed to: booking controller [v2]
        public List<HospitalDetailsDto_HospitalDetails> FetchAvailableHospitalsListForBooking();

        // exposed to: booking controller [v2]
        public bool V2BookSlot(BookingDetailsDto_V2SlotBook submittedDetails, string userid);

        /* exposed to: booking controller, user profile service [v2]
        */
        public BookingDetailsDto_UserBookingDetails FetchBookingDetails(string userid);

        // exposed to: booking controller [v2]
        public DateTime FetchD1BookingDate(string userid);

        // exposed to: booking controller [v2]
        public bool CheckUserBookingStatus(string userid);

        
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

        
        // service method: V2 

        public bool CheckD1ApprovalStatus(string userid)
        {
            bool isD1Approved = FetchBookingDetails(userid).Dose1ApproveDate != DateTime.MinValue ? true : false;             

            return isD1Approved;

        }

        public List<HospitalDetailsDto_HospitalDetails> FetchAvailableHospitalsListForBooking()
        {
            List<HospitalDetailsDto_HospitalDetails> availableHospitalsList = _hospitalService.FetchAvailableHospitalsList();

            return availableHospitalsList.Count > 0 ? availableHospitalsList : new List<HospitalDetailsDto_HospitalDetails>();
        }

        public bool V2BookSlot(BookingDetailsDto_V2SlotBook submittedDetails, string userid)
        {
            BookingDetailsModel bookingDetails = new BookingDetailsModel();
            string hospitalId = _hospitalService.FetchHospitalIdyName(submittedDetails.HospitalName);
            HospitalDetailsModel hospitalDetails = _hospitalService.FetchHospitalDetailsById(hospitalId);
            int availableSlot = -1;

            if (hospitalDetails != null) 
            {
                availableSlot = hospitalDetails.HospitalAvailableSlots;
            }

            BookingDetailsModel fetchedDetails = _v1RemDb.BookingDetails.FirstOrDefault(record => record.UserId == userid);
            
            if (fetchedDetails == null)
            {
                // map booking id
                bookingDetails.BookingId = GenerateBookingId(userid); 

                // map user id
                bookingDetails.UserId = userid;

                // map user vaccination id
                bookingDetails.UserVaccinationId = _userVaccineDeailsService.FetchUserVaccinationID(userid);

                if (!string.IsNullOrEmpty(submittedDetails.Dose1BookDate.ToString()) && (submittedDetails.Dose1BookDate != DateTime.MinValue))
                {
                    bookingDetails.Dose1BookDate = submittedDetails.Dose1BookDate;
                    bookingDetails.D1HospitalId = hospitalId;
                    bookingDetails.D1SlotNumber = availableSlot;
                
                    _v1RemDb.BookingDetails.Add(bookingDetails);
                }
            }
            else
            {
                // Update the existing fetchedDetails object
                if (!string.IsNullOrEmpty(submittedDetails.Dose2BookDate.ToString()) && (submittedDetails.Dose2BookDate != DateTime.MinValue))
                {
                    fetchedDetails.Dose2BookDate = submittedDetails.Dose2BookDate;
                    fetchedDetails.D2HospitalId = hospitalId;
                    fetchedDetails.D2SlotNumber = availableSlot;
                    
                    _v1RemDb.BookingDetails.Update(fetchedDetails);
                }
            }

            // save to booking details DB
            int bookingDetailsStatus = _v1RemDb.SaveChanges();

            if (bookingDetailsStatus <= 0)
            {
                Console.WriteLine($"-------Booking Service: Something went wrong while saving booking details-------");
                return false;
            }
            else 
            {
                if (hospitalDetails != null)
                {
                    // map updated available slot of hospital
                    hospitalDetails.HospitalAvailableSlots = hospitalDetails.HospitalAvailableSlots - 1;

                    // save details to DB
                    _v1RemDb.HospitalDetails.Update(hospitalDetails);

                    // update DB
                    int hospitalDetailsUpdateStatus = _v1RemDb.SaveChanges();

                    return hospitalDetailsUpdateStatus > 0;
                }
                else
                {
                    return false;
                }
            }
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

        // service method: to fetch dose 1 booking date
        public DateTime FetchD1BookingDate(string userid)
        {
            var fetchedDetails = _v1RemDb.BookingDetails.FirstOrDefault(record=>record.UserId == userid);
            if(fetchedDetails != null)
            {
                return fetchedDetails.Dose1BookDate;
            }
            return new DateTime();
        }

        // service method: to check if user already booked both slots
        public bool CheckUserBookingStatus(string userid)
        {
            var fetchedDetails = _v1RemDb.BookingDetails.FirstOrDefault(record=>record.UserId == userid);
            if(fetchedDetails != null)
            {
                if(fetchedDetails.Dose1BookDate != DateTime.MinValue && fetchedDetails.Dose2BookDate != DateTime.MinValue)
                {
                    return false;
                }
                return true;
            }
            return true;
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