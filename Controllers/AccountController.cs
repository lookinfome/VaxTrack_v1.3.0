using Microsoft.AspNetCore.Mvc;
using v1Remastered.Services;
using v1Remastered.Dto;

namespace v1Remastered.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        // variable: to access account service methods
        private readonly IAccountService _accountService;

        // variable: to access auth service methods
        private readonly IAuthService _authService;
        
        // variable: to access user profile service methods
        private readonly IUserProfileService _userProfileService;

        // constructor: to initialize class variables
        public AccountController(IAccountService accountService, IAuthService authService, IUserProfileService userProfileService)
        {
            _accountService = accountService;
            _authService = authService;
            _userProfileService = userProfileService;
        }

        [HttpGet("LoginUser")]
        public IActionResult LoginUser()
        {
            return PartialView("_LoginUserPartial");
        }

        [HttpPost("LoginUser")]
        public IActionResult LoginUser(UserDetailsDto_Login submittedDetails)
        {
            if (ModelState.IsValid)
            {
                string userId = _accountService.LoginUser(submittedDetails);
                if (!string.IsNullOrEmpty(userId))
                {
                    string userName = _userProfileService.FetchUserName(userId);
                    string userRole = _userProfileService.FetchUserRoleById(userId);

                    if (userRole.ToLower() == "user")
                    {
                        TempData["userLoginMsgSuccess"] = $"Welcome again, {userName}";
                        return Json(new { success = true, redirectUrl = Url.Action("UserProfile", "UserProfile", new { userid = submittedDetails.UserId }) });
                    }
                    else
                    {
                        TempData["userAdminMsgSuccess"] = $"Welcome again admin, {userName}";
                        return Json(new { success = true, redirectUrl = Url.Action("AdminPage", "Admin", new { userid = submittedDetails.UserId }) });
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return PartialView("_LoginUserPartial", submittedDetails);
                }
            }
            else
            {
                return PartialView("_LoginUserPartial", submittedDetails);
            }
        }


        [HttpGet("RegisterUser")]
        public IActionResult RegisterUser()
        {
            ViewBag.SignUpFailedMsg = TempData["SignUpFailedMsg"];
            return PartialView("_RegisterUserPartial");
        }

        [HttpPost("RegisterUser")]
        public IActionResult RegisterUser(UserDetailsDto_Register submittedDetails)
        {
            if (ModelState.IsValid)
            {
                string userId = _accountService.RegisterUser(submittedDetails);
                if (!string.IsNullOrEmpty(userId))
                {
                    string username = _userProfileService.FetchUserName(userId);

                    TempData["SignUpSuccessMsg"] = $"Welcome user, {username}";
                    return Json(new { success = true, redirectUrl = Url.Action("UserProfile", "UserProfile", new { userid = userId }) });
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return PartialView("_RegisterUserPartial", submittedDetails);
                }
            }
            else
            {
                return PartialView("_RegisterUserPartial", submittedDetails);
            }
        }
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutUser()
        {
            var result = await _authService.LogoutUserAsync();

            return result!=false ? RedirectToAction("Index", "Home") : NotFound(new {systemMessage = "something went wrong while logging you out"});
        }

    }
}