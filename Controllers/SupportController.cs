using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using v1Remastered.Services;
using v1Remastered.Dto;
using Microsoft.AspNetCore.Identity;

namespace v1Remastered.Controllers
{
    [Route("Support/{userid}")]
    [Authorize]
    public class SupportController:Controller
    {
        private readonly ISupportService _supportService;
        private readonly IUserProfileService _userProfileService;
        private readonly UserManager<AppUserIdentityModel> _userManager;

        public SupportController(ISupportService supportService, IUserProfileService userProfileService, UserManager<AppUserIdentityModel> userManager)
        {
            _userManager = userManager;
            _supportService = supportService;
            _userProfileService = userProfileService;
        }


        [HttpGet("")]
        public async Task<IActionResult> SupportPage([FromRoute] string userid)
        {
            // fetch user details from asp-net-user table for authentication
                var loggedInUser = await _userManager.GetUserAsync(User);

            // if user's record not found
            if (loggedInUser == null || loggedInUser.UserName != userid)
            {
                TempData["UnauthorizedAction"] = "Hey hey hey, you can't really do that... ";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.UserId = userid;
            ViewBag.UserName = _userProfileService.FetchUserName(userid);
            ViewBag.TicketRaiseSuccessMsg = TempData["TicketRaiseSuccessMsg"];
            ViewBag.TicketsList = _supportService.FetchTicketsListByUserId(userid);
            
            return View();
        }
        
        [HttpPost("RaiseNewTicket")]
        public IActionResult RaiseNewTicket(SupportDetailsDto_SupportForm submitedDetails, [FromRoute] string userid)
        {
            if (ModelState.IsValid)
            {
                
                bool isTicketRaised = _supportService.SaveNewTicket(submitedDetails, userid);

                if (isTicketRaised)
                {
                    TempData["TicketRaiseSuccessMsg"] = $"Thanks for your ticket, we are on it.";
                    return RedirectToAction("SupportPage", "Support", new { userid = userid });
                }
            }
            
            ViewBag.UserName = _userProfileService.FetchUserName(userid);
            return View("SupportPage", submitedDetails);
        }

        [HttpPost("TicketDetails/{supportid}/AddNewComment")]
        public IActionResult AddNewComment([FromRoute] string userid, string supportid, string submittedComment)
        {
            try
            {
                bool isNewCommentAdded = _supportService.SaveNewComment(userid, supportid, submittedComment);

                if (isNewCommentAdded)
                {
                    ViewBag.TicketDetails = _supportService.FetchTicketDetailsByUserIdTicketId(userid, supportid);
                    return PartialView("_TicketCommentsPartial", ViewBag.TicketDetails);
                }
                return Ok("Failed to add comment.");
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("TicketDetails/{supportId}")]
        public IActionResult GetTicketDetailsById([FromRoute] string userId, [FromRoute] string supportId)
        {
            ViewBag.TicketDetails = _supportService.FetchTicketDetailsByUserIdTicketId(userId, supportId);

            // You can add logic here to fetch the ticket details using userId and supportId
            return PartialView("_TicketDetailsPartial");
        }
    
    }
}