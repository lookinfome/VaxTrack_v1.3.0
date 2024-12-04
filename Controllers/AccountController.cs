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
            return View();
        }

        [HttpPost("LoginUser")]
        public IActionResult LoginUser(UserDetailsDto_Login submittedDetails)
        {
            // if model is valid
            if(ModelState.IsValid)
            {
                // fetch user id post logging in
                string userId = _accountService.LoginUser(submittedDetails);

                // if login successfull
                if(!string.IsNullOrEmpty(userId))
                {
                    // fetch user name
                    string userName = _userProfileService.FetchUserName(userId);

                    // fetch user role
                    string userRole = _userProfileService.FetchUserRoleById(userId);

                    // if user is not admin
                    if(userRole.ToLower() == "user")
                    {
                        TempData["userLoginMsgSuccess"] = $"Welcome again, {userName}";
                        return RedirectToAction("UserProfile", "UserProfile", new {userid = submittedDetails.UserId});
                    }

                    // if user is admin
                    else
                    {
                        TempData["userAdminMsgSuccess"] = $"Welcome again admin, {userName}";
                        return RedirectToAction("AdminPage", "Admin", new {userid = submittedDetails.UserId});
                    }

                }

                // if login failed
                else
                {
                    return View(submittedDetails);
                }

            }

            // if model is not valid
            else
            {
                return View(submittedDetails);
            }
        }

        [HttpGet("RegisterUser")]
        public IActionResult RegisterUser()
        {
            ViewBag.SignUpFailedMsg = TempData["SignUpFailedMsg"];
            return View();
        }

        [HttpPost("RegisterUser")]
        public IActionResult RegisterUser(UserDetailsDto_Register submittedDetails)
        {
        
            if(ModelState.IsValid)
            {
                // Task<string> userId1 = _accountService.RegisterUser(submittedDetails);



                string userId= _accountService.RegisterUser(submittedDetails);

                if(!string.IsNullOrEmpty(userId))
                {
                    string username = _userProfileService.FetchUserName(userId);

                    TempData["SignUpSuccessMsg"] = $"Welcom user, {username}";
                    return RedirectToAction("UserProfile", "UserProfile", new {userid = submittedDetails.UserId});
                }
                else
                {
                    TempData["SignUpFailedMsg"] = $"Oops somthing went wrong, but don't worry and try again";
                    return View(submittedDetails);
                }
            }
            else
            {
                return View(submittedDetails);
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