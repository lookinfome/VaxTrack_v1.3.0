using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using v1Remastered.Services;
using v1Remastered.Dto;
using v1Remastered.Models;
using Microsoft.AspNetCore.Identity;

namespace v1Remastered.Controllers
{
    [Authorize]
    [Route("Admin")]
    public class AdminController:Controller
    {
        private readonly IAuthService _authService;
        private readonly IAdminService _adminService;
        private readonly UserManager<AppUserIdentityModel> _userManager;
        public AdminController(IAuthService authService, IAdminService adminService, UserManager<AppUserIdentityModel> userManager)
        {
            _authService = authService;
            _adminService = adminService;
            _userManager = userManager;
        }

        [HttpGet("{userid}")]
        public async Task<IActionResult> AdminPage()
        {
            // fetch user details from asp-net-user table
            var loggedInUser = await _userManager.GetUserAsync(User);

            // check user autheticity and role
            if (loggedInUser == null || !await _userManager.IsInRoleAsync(loggedInUser, "admin"))
            {
                await _authService.LogoutUserAsync();
                return RedirectToAction("LoginUser", "Account");
            }

            // users list with pending approval
            List<AdminDetailsDto_UserWithPendingApproval> _userswWithPendingApproval = _adminService.FetchUsersWithPendingApproval();
            ViewBag.UsersWithPendingApproval = _userswWithPendingApproval;

            // hospitals list with name and slots details
            List<AdminDetailsDto_HospitalsList> _fetchedHospitalsDetails = _adminService.FetchHospitalsList();
            ViewBag.FetchedHospitalsDetails = _fetchedHospitalsDetails;

            // total users count
            int _totalUsersCount = _adminService.FetchTotalUsersCount();
            ViewBag.TotalUsersCount = _totalUsersCount;

            // total users count with slot booked
            int _totalUsersCountWithSlotBooked = _adminService.FetchTotalUsersCountWithSlotBooked();
            ViewBag.TotalUsersCountWithSlotBooked = _totalUsersCountWithSlotBooked;

            // total users count with approved slots
            int _totalUsersCountWithApprovedSlots = _adminService.FetchTotalUsersCountWithApprovedSlots();
            ViewBag.TotalUsersCountWithApprovedSlots = _totalUsersCountWithApprovedSlots;

            // total male users count
            int _totalMaleUsersCount = _adminService.FetchTotalMaleUsersCount();
            int _totalFemaleUsersCount = _adminService.FetchTotalFemaleUsersCount();
            ViewBag.TotalMaleUsersCount = _totalMaleUsersCount;
            ViewBag.TotalFemaleUsersCount = _totalFemaleUsersCount;
            if(!string.IsNullOrEmpty(TempData["userAdminMsgSuccess"]?.ToString()))
            {
                ViewBag.WelcomeMessage = TempData["userAdminMsgSuccess"];
            }

            return View();
        }

        [HttpPost("BookUserSlot")]
        public IActionResult ApproveSlotBook(string userId, string bookingId)
        {
            // bool approvalStatus = _adminService.ApproveSlotBook(userId, bookingId);

            bool approvalStatus = _adminService.ApproveSlotBookV2(userId, bookingId);

            if(approvalStatus)
            {
                List<AdminDetailsDto_UserWithPendingApproval> _userswWithPendingApproval = _adminService.FetchUsersWithPendingApproval();

                if(_userswWithPendingApproval.Count >= 1)
                {
                    return PartialView("_FilteredUsersTablePartial", _userswWithPendingApproval.ToList());
                }
                return PartialView("_AdminErrorMsgPartial", new List<AdminDetailsDto_UserWithPendingApproval>());

            }
            else
            {
                // return error message
                return NotFound($"Approval failed due to unexpected error for user - {userId}"); 
            }

        } 
    

        [HttpGet("IncreaseSlots")]
        public IActionResult IncreaseSlots(string hospitalId, int increaseBy)
        {
            _adminService.UpdateAvailableSlotsById(hospitalId, increaseBy);

            List<AdminDetailsDto_HospitalsList> _fetchedHospitalsDetails = _adminService.FetchHospitalsList();
            ViewBag.FetchedHospitalsDetails = _fetchedHospitalsDetails;

            if(_fetchedHospitalsDetails.Count > 0)
            {
                return PartialView("_UpdatedSlotHospitalTablePartial", _fetchedHospitalsDetails);
            }

            // return error page
            return PartialView("_AdminErrorMsgPartial", _fetchedHospitalsDetails);
        
        }

    
    }
}