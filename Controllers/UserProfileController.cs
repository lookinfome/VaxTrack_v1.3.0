
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using v1Remastered.Services;
using v1Remastered.Dto;
using Microsoft.AspNetCore.Identity;

namespace v1Remastered.Controllers
{
    [Route("UserProfile")]
    public class UserProfileController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IBookingService _bookingService;
        private readonly IHospitalService _hospitalService;
        private readonly IUserProfileService _userProfileService;
        private readonly UserManager<AppUserIdentityModel> _userManager;
        public UserProfileController(IAuthService authService, IHospitalService hospitalService, IBookingService bookingService, IUserProfileService userProfileService, UserManager<AppUserIdentityModel> userManager)
        {
            _authService = authService;
            _userManager = userManager;
            _bookingService = bookingService;
            _hospitalService = hospitalService;
            _userProfileService = userProfileService;
        }

        [Authorize]
        [HttpGet("{userid}")]
        public async Task<IActionResult> UserProfile(string userid)
        {
            if (!string.IsNullOrEmpty(userid))
            {

                // fetch user details from asp-net-user table for authentication
                var loggedInUser = await _userManager.GetUserAsync(User);

                // if user's record not found
                if (loggedInUser == null || loggedInUser.UserName != userid)
                {
                    TempData["UnauthorizedAction"] = "Hey hey hey, you can't really do that... ";
                    return RedirectToAction("Index", "Home");
                }

                // fetch hospital details
                UserDetailsDto_UserProfile userProfileDetails = _userProfileService.FetchUserProfileDetails(userid);
                ViewBag.D1HospitalName = _userProfileService.FetchAdditionalUserDetails(userProfileDetails)["D1HospitalName"];
                ViewBag.D2HospitalName = _userProfileService.FetchAdditionalUserDetails(userProfileDetails)["D2HospitalName"];
                ViewBag.AvailableHospitalLocations = _hospitalService.FetchAvailableHospitalLocations();

                // slot booking details
                ViewBag.IsD1Booked = _bookingService.IsD1Booked(userid, userProfileDetails.UserBookingDetails.BookingId);
                ViewBag.IsD2Booked = _bookingService.IsD2Booked(userid, userProfileDetails.UserBookingDetails.BookingId);
                ViewBag.IsD1Approved = _bookingService.IsD1Approved(userid, userProfileDetails.UserBookingDetails.BookingId);
                ViewBag.IsD2Approved = _bookingService.IsD2Approved(userid, userProfileDetails.UserBookingDetails.BookingId);
                ViewBag.D1BookedDate = _bookingService.FetchD1BookedDate(userid, userProfileDetails.UserBookingDetails.BookingId);
                
                // welcome message: login
                if(!string.IsNullOrEmpty(TempData["userLoginMsgSuccess"]?.ToString()))
                {
                    ViewBag.WelcomeMessage = TempData["userLoginMsgSuccess"];
                }

                // welcome message: register
                if(!string.IsNullOrEmpty(TempData["SignUpSuccessMsg"]?.ToString()))
                {
                    ViewBag.WelcomeMessage = TempData["SignUpSuccessMsg"];
                }

                // update message: edit profile
                if(!string.IsNullOrEmpty(TempData["userProfileUpdateSuccessMsg"]?.ToString()))
                {
                    ViewBag.UserProfileUpdateSuccessMsg = TempData["userProfileUpdateSuccessMsg"];
                }

                if(!string.IsNullOrEmpty(TempData["userProfileUpdateErrorMsg"]?.ToString()))
                {
                    ViewBag.UserProfileUpdateErrorMsg = TempData["userProfileUpdateErrorMsg"];
                }

                // update message: slot booking
                if(!string.IsNullOrEmpty(TempData["BookingStatusMsg"]?.ToString()))
                {
                    ViewBag.BookingStatusMsg = TempData["BookingStatusMsg"];
                }
 
                // final call to view
                if (userProfileDetails.UserId != null)
                {
                    return View(userProfileDetails);
                }
            }

            return View("~/Views/UserProfile/UserProfileError.cshtml");
        }


        [Authorize]
        [HttpPost("{userid}/Edit")]
        public async Task<IActionResult> UserProfileEdit(string userid, string phoneNumber, string dateOfBirth, IFormFile profileImage, string password)
        {
            // Fetch user details from asp-net-user table for authentication
            var loggedInUser = await _userManager.GetUserAsync(User);

            // If user's record not found
            if (loggedInUser == null || loggedInUser.UserName != userid)
            {
                return Json(new { success = false, message = "Unauthorized action." });
            }

            // Convert dateOfBirth string to DateTime
            DateTime _dateOfBirth;
            if (!string.IsNullOrEmpty(dateOfBirth))
            {
                _dateOfBirth = DateTime.Parse(dateOfBirth);
            }
            else
            {
                _dateOfBirth = DateTime.MinValue;
            }

            // Add your logic to handle the form submission here
            string userPassword = string.IsNullOrEmpty(password) ? "" : password;

            bool isUserAuthentic = _authService.CheckUserAuthenticity(userid, userPassword).Result;

            if (isUserAuthentic)
            {
                bool result = _userProfileService.UpdateUserProfile(userid, phoneNumber, _dateOfBirth, profileImage);
                string userName = _userProfileService.FetchUserName(userid);

                if (result)
                {
                    TempData["userProfileUpdateSuccessMsg"] = $"Looks good, {userName}";
                    return Json(new { success = true, message = $"{userName}'s profile has been updated successfully." });
                }
                else
                {
                    TempData["userProfileUpdateErrorMsg"] = $"Oops something wrong";
                    return Json(new { success = false, message = "Oops, something went wrong." });
                }
            }

            TempData["userProfileUpdateErrorMsg"] = $"Oops something wrong";
            return Json(new { success = false, message = "Incorrect password, hence update failed." });
        }

        [HttpGet]
        public JsonResult AvailableHospitalsByLocation(string hospitalLocation)
        {
            var fetchedDetails = _hospitalService.FetchAvailableHospitalsByLocation(hospitalLocation);
            return Json(fetchedDetails);
        }

    }

}