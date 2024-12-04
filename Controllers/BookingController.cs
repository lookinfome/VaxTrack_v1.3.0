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
        [HttpGet("v2/{userid}")]
        public IActionResult V2Booking([FromRoute] string userid)
        {
            bool isBookingAllowed = _bookingService.CheckUserBookingStatus(userid);

            if(isBookingAllowed)
            {
                ViewBag.IsD1Approved = _bookingService.CheckD1ApprovalStatus(userid);
                ViewBag.Dose1BookingDate = _bookingService.FetchD1BookingDate(userid); 
                ViewBag.AvailableHospitalsLit = _bookingService.FetchAvailableHospitalsListForBooking();
                
                ViewBag.BookingStatusMsg = TempData["BookingSubmitMsg"];                
                return View();

            }

            // fetch booking details for user
            var _userBookingDetails = _bookingService.FetchBookingDetails(userid);

            // return message page
            ViewBag.BookingErrorMsg = $"{userid} - already booked both slots, booking Id: {_userBookingDetails.BookingId}";
            return View("~/Views/Booking/BookingError.cshtml");


        }

        [Authorize]
        [HttpPost("v2/{userid}")]
        public IActionResult V2Booking(BookingDetailsDto_V2SlotBook submittedDetails, [FromRoute] string userid)
        {
            bool case1 = (!string.IsNullOrEmpty(submittedDetails.Dose1BookDate.ToString()) && submittedDetails.Dose1BookDate != DateTime.MinValue);
            bool case2 = (!string.IsNullOrEmpty(submittedDetails.Dose2BookDate.ToString()) && submittedDetails.Dose2BookDate != DateTime.MinValue);

            if((case1 || case2) && !string.IsNullOrEmpty(submittedDetails.HospitalName))
            {
                bool isSlotBooked = _bookingService.V2BookSlot(submittedDetails, userid);
            
                if(isSlotBooked)
                {
                    TempData["BookingStatusMsg"] = $"Booking successfull, see you soon at the vaccination center";
                    return RedirectToAction("UserProfile", "UserProfile", new { userid = userid });
                }
            }
            
            TempData["BookingSubmitMsg"] = $"Need to provide more details dose date/hospital name";
            return RedirectToAction("V2Booking", "Booking", new {userid = userid});

        }
        

    }
}