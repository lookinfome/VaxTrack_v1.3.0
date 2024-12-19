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
        public SupportManagerController(UserManager<AppUserIdentityModel> userManager)
        {
            _userManager = userManager;
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
            
            return PartialView("_SupportManagerPage");
        }


    }


    
}