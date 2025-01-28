using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using v1Remastered.Services;
using v1Remastered.Dto;
using Microsoft.AspNetCore.Identity;

namespace v1Remastered.Controllers
{
    [Route("SupportManager/{userid}")]
    public class SupportManagerController:Controller
    {
        private readonly UserManager<AppUserIdentityModel> _userManager;
        private readonly ISupportManagerService _supportManagerService;
        private readonly ISupportService _supportService;
        public SupportManagerController(UserManager<AppUserIdentityModel> userManager, ISupportService supportService, ISupportManagerService supportManagerService)
        {
            _userManager = userManager;
            _supportService = supportService;
            _supportManagerService = supportManagerService;
        }

        [HttpGet("")]
        public async Task<IActionResult> SupportManagerPage([FromRoute] string userid)
        {
            // fetch user details from asp-net-user table for authentication
            var loggedInUser = await _userManager.GetUserAsync(User);

            // if user's record not found
            if (loggedInUser == null || !await _userManager.IsInRoleAsync(loggedInUser, "admin"))
            {
                TempData["UnauthorizedAction"] = "Hey hey hey, you can't really do that... ";
                return RedirectToAction("Index", "Home");
            }


            ViewBag.TotalTicketsCount = _supportManagerService.FetchTicketsCount();
            ViewBag.SupportTicketManagerList = _supportManagerService.FetchTicketList();

            
            return PartialView("_SupportManagerPage");
        }


        [HttpGet("TicketDetails/{supportid}")]
        public IActionResult ViewTicketDetails(string supportid)
        {
            ViewBag.SupportId = supportid;

            ViewBag.FetchedDetails = _supportManagerService.FetchTicketDetails(supportid);

            return PartialView("_TicketDetailsManagePartial");
        }

        [HttpPost("TicketDetails/{supportid}/AddNewComment")]
        public IActionResult AddNewComment([FromRoute] string userid, string supportid, string submittedComment)
        {
            try
            {
                bool isNewCommentAdded = _supportService.SaveNewComment(userid, supportid, submittedComment);

                if (isNewCommentAdded)
                {
                    ViewBag.TicketDetails = _supportService.FetchTicketDetailsByTicketId(supportid);
                    return PartialView("_TicketAdminCommentsPartial", ViewBag.TicketDetails);
                }
                return Ok("Failed to add comment.");

            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    
    }


    
}