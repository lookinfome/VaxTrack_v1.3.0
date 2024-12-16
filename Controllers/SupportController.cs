using Microsoft.AspNetCore.Mvc;
using v1Remastered.Services;
using v1Remastered.Dto;

namespace v1Remastered.Controllers
{
    [Route("Support")]
    public class SupportController:Controller
    {
        private readonly ISupportService _supportService;
        private readonly IUserProfileService _userProfileService;

        public SupportController(ISupportService supportService, IUserProfileService userProfileService)
        {
            _supportService = supportService;
            _userProfileService = userProfileService;
        }


        [HttpGet("{userid}")]
        public IActionResult SupportPage([FromRoute] string userid)
        {
            ViewBag.UserId = userid;
            ViewBag.UserName = _userProfileService.FetchUserName(userid);
            ViewBag.TicketRaiseSuccessMsg = TempData["TicketRaiseSuccessMsg"];
            ViewBag.TicketsList = _supportService.FetchTicketsListByUserId(userid);
            
            return View();
        }
        
        [HttpPost]
        [Route("{userid}/RaiseNewTicket")]
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

    
        [HttpGet("{userId}/TicketDetails/{supportId}")]
        public IActionResult GetTicketDetailsById([FromRoute] string userId, [FromRoute] string supportId)
        {
            ViewBag.TicketDetails = _supportService.FetchTicketDetailsByUserIdTicketId(userId, supportId);

            // You can add logic here to fetch the ticket details using userId and supportId
            return PartialView("_TicketDetailsPartial");
        }
    
    }
}