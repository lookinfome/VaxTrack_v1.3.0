using Microsoft.AspNetCore.Mvc;
using v1Remastered.Services;
using v1Remastered.Dto;

namespace v1Remastered.Controllers
{
    [Route("UserFeedback")]
    public class UserFeedback:Controller
    {
        private readonly IUserProfileService _userProfileService;

        public UserFeedback(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }


        [HttpGet("{userid}")]
        public IActionResult UserFeedbackPage([FromRoute] string userid)
        {
            ViewBag.UserName = _userProfileService.FetchUserName(userid);
            
            return View();
        }
    }
}