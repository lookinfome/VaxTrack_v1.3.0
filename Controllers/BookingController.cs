using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using v1Remastered.Services;
using v1Remastered.Dto;

namespace v1Remastered.Controllers
{
    [Route("Booking")]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IHospitalService _hospitalService;
        
        public BookingController(IBookingService bookingService, IHospitalService hospitalService)
        {
            _bookingService = bookingService;
            _hospitalService = hospitalService;
        }

        [Authorize]
        [HttpPost("v2/{userid}")]
        public IActionResult V2Booking(string userId, DateTime dose1Date, string hospitalId)
        {

            if(!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(dose1Date.ToString()) && !string.IsNullOrEmpty(hospitalId))
            {
                // check for existing booking record
                var fetchedDetails = _bookingService.FetchBookingDetails(userId);

                // if record not found
                if(string.IsNullOrEmpty(fetchedDetails.BookingId))
                {
                    bool isSlotBooked = _bookingService.SaveNewBookingDetails(userId, dose1Date, hospitalId);

                    if(isSlotBooked)
                    {
                        TempData["BookingStatusMsg"] = $"Booking successfull, see you soon at the vaccination center";
                    }
                }
                return RedirectToAction("UserProfile", "UserProfile", new { userId = userId });
            }
            else
            {
                return Ok($"--------- Dose 1 is already booked for user: {userId} ---------");
            }

        }

        [Authorize]
        [HttpPost("v2/{userid}/Update")]
        public IActionResult V2BookingUpdate(string userId, DateTime dose2Date, string hospitalId)
        {   
            if(!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(dose2Date.ToString()) && !string.IsNullOrEmpty(hospitalId))
            {
                // check for existing booking record
                var fetchedDetails = _bookingService.FetchBookingDetails(userId);

                if(!string.IsNullOrEmpty(fetchedDetails.BookingId))
                {
                    bool isSlotBooked = _bookingService.UpdateBookingDetails(userId, dose2Date, hospitalId);

                    if(isSlotBooked)
                    {
                        TempData["BookingStatusMsg"] = $"Booking successfull, see you soon at the vaccination center";
                    }
                }
                return RedirectToAction("UserProfile", "UserProfile", new { userId = userId });
            }
            else
            {
                return Ok($"--------- Dose 2 is already booked for user: {userId} ---------");
            }
        }

        
        

    }
}