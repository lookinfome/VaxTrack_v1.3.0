using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using v1Remastered.Services;
using v1Remastered.Dto;
using v1Remastered.Models;
using Microsoft.AspNetCore.Identity;

namespace v1Remastered.Controllers
{
    [Authorize]
    [Route("Admin/v2")]
    public class AdminV2Controller:Controller
    {
        private readonly IAdminV2Service _adminV2Service;
        private readonly IAuthService _authService;
        private readonly UserManager<AppUserIdentityModel> _userManager;

        public AdminV2Controller(
            IAdminV2Service adminV2Service,
            IAuthService authService,
            UserManager<AppUserIdentityModel> userManager
        )
        {
            _adminV2Service = adminV2Service;
            _userManager = userManager;
            _authService = authService;
        }

        [HttpGet("{userid}")]
        public async Task<IActionResult> V2AdminPage()
        {
            // fetch user details from asp-net-user table for authentication
            var loggedInUser = await _userManager.GetUserAsync(User);

            // if user's record not found
            if (loggedInUser == null || !await _userManager.IsInRoleAsync(loggedInUser, "admin"))
            {
                TempData["UnauthorizedAction"] = "Hey hey hey, you can't really do that... ";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.LoginMessage = TempData["userAdminMsgSuccess"];
            ViewBag.TotalUserCount = _adminV2Service.FetchTotalUserCount();
            ViewBag.TotalUserWithSlotBookedCount = _adminV2Service.FetchTotalUserWithSlotBookedCount();
            ViewBag.MaleUsersCount = _adminV2Service.FetchTotalMaleUsersCount();
            ViewBag.FemaleUsersCount = _adminV2Service.FetchTotalFemaleUsersCount();
            ViewBag.FullyVaccinatedUsersCount = _adminV2Service.FetchTotalVaccinatedUsersCount();
            ViewBag.PartiallyVaccinatedUsersCount = _adminV2Service.FetchTotalPartiallyVaccinatedUsersCount();
            
            return View();
        }

        [HttpGet("{userid}/admin-usages")]
        public IActionResult V2AdminUsages()
        {
            ViewBag.TotalUserCount = _adminV2Service.FetchTotalUserCount();
            
            return PartialView("_V2AdminDashUsagesPartial");
        }

        [HttpGet("{userid}/admin-actions")]
        public IActionResult V2AdminActions()
        {
            // fetch users list with pending approvals
            ViewBag.UsersListWithPendingApprovals = _adminV2Service.FetchPendingApprovalsList();
            ViewBag.FetchedHospitalsDetails =  _adminV2Service.FetchHospitalsList();

            return PartialView("_V2AdminDashActionsPartial");
        }

        [HttpPost("ApproveSlots")]
        public IActionResult ApproveSlotBook(string userId, string bookingId)
        {

            bool approvalStatus = _adminV2Service.ApproveSlotBookV2(userId, bookingId);

            if(approvalStatus)
            {
                List<AdminDetailsDto_UserWithPendingApproval> _userswWithPendingApproval = _adminV2Service.FetchPendingApprovalsList();

                if(_userswWithPendingApproval.Count >= 1)
                {
                    TempData["approvalSuccessMsg"] = $"{userId} is now vaccinated";
                    return PartialView("_V2UsersPendingAppovalTablePartial", _userswWithPendingApproval.ToList());
                }
                return PartialView("_V2AdminErrorMsgPartial", new List<AdminDetailsDto_UserWithPendingApproval>());

            }
            else
            {
                // return error message
                return NotFound($"Approval failed due to unexpected error for user - {userId}"); 
            }

        } 

        [HttpGet("UpdateSlots")]
        public IActionResult UpdateSlots(string hospitalId, int increaseBy)
        {
            _adminV2Service.UpdateAvailableSlotsById(hospitalId, increaseBy);

            List<AdminDetailsDto_HospitalsList> _fetchedHospitalsDetails = _adminV2Service.FetchHospitalsList();
            ViewBag.FetchedHospitalsDetails = _fetchedHospitalsDetails;

            if(_fetchedHospitalsDetails.Count > 0)
            {
                return PartialView("_V2UpdatedHospitalTablePartial", _fetchedHospitalsDetails);
            }

            // return error page
            return PartialView("_V2AdminErrorMsgPartial", _fetchedHospitalsDetails);
        
        }
    
    
    }
}