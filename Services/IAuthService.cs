using Microsoft.AspNetCore.Identity;
using v1Remastered.Models;

namespace v1Remastered.Services
{
    // interface: to expose the services
    public interface IAuthService
    {
        // exposed to: account service
        public Task<string> LoginUserAsync(string userid, string password);

        // exposed to: user profile controller
        public Task<bool> CheckUserAuthenticity(string userid, string password);
        
        // exposed to: account service
        public Task<string> RegisterUserAsync(string userid, string password, string role);

        /* exposed to: 
            account controller, 
            admin controller, 
            user profile controller 
        */
        public Task<bool> LogoutUserAsync();
    }

    // class: to define the implementation of services
    public class AauthService : IAuthService
    {
        // variable: to handle signup operations
        private readonly UserManager<AppUserIdentityModel> _userManager;

        // variable: to handle signin operations
        private readonly SignInManager<AppUserIdentityModel> _signInManager;

        // variable: to handle role creation and assigning operation
        private readonly RoleManager<IdentityRole> _roleManager;

        // contructor: to initialize the class variables
        public AauthService (SignInManager<AppUserIdentityModel> signInManager, UserManager<AppUserIdentityModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        // service: login existing user
        public async Task<string> LoginUserAsync(string userid, string password)
        {
            var user = await _userManager.FindByNameAsync(userid);
            if (user?.UserName == null)
            {
                return "";
            }

            await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
            
            return userid;
        }
        
        // service: register new user
        public async Task<string> RegisterUserAsync(string userid, string password, string role)
        {
            
            // mapping username fom submitted details to app user model
            var user = new AppUserIdentityModel
            {
                UserName = userid,
            };

            // saving app user model in asp-net-users table
            var result = await _userManager.CreateAsync(user, password);

            // if successful
            if(result.Succeeded)
            {
                // add role to asp-ne-users
                if(!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }

                // add role to the user
                await _userManager.AddToRoleAsync(user, role);

                // Sign in the user
                await _signInManager.SignInAsync(user, isPersistent: false);

                return userid;
                
            }

            // return result
            return "";

        }

        // service: log out user
        public async Task<bool> LogoutUserAsync()
        {
            // handling sign-out operation
            await _signInManager.SignOutAsync();

            return true;
        }

        // service: user authentic state
        public async Task<bool> CheckUserAuthenticity(string userid, string password)
        {

            var user = await _userManager.FindByNameAsync(userid);
            if(user == null)
            {
                return false;
            }

            return await _userManager.CheckPasswordAsync(user, password);
        }
    
    }
}