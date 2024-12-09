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
            return Ok("Hit successfull");

            // return message page
            // ViewBag.BookingErrorMsg = $"{userid} - already booked both slots";
            // return PartialView("_BookingErrorPartial");

        }

        
        

    }
}