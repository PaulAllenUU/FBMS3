
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;

using FBMS3.Core.Models;
using FBMS3.Core.Services;
using FBMS3.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using FBMS3.Core.Security;

/**
 *  User Management Controller providing registration and login functionality
 */
namespace FBMS3.Web.Controllers
{
    public class UserController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly IUserService _svc;

        public UserController(IUserService svc, IConfiguration config)
        {        
            _config = config;    
            _svc = svc;
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email,Password")] UserLoginViewModel m)
        {
            var user = _svc.Authenticate(m.Email, m.Password);
            // check if login was unsuccessful and add validation errors
            if (user == null)
            {
                ModelState.AddModelError("Email", "Invalid Login Credentials");
                ModelState.AddModelError("Password", "Invalid Login Credentials");
                return View(m);
            }

            // Login Successful, so sign user in using cookie authentication
            await SignInCookie(user);

            Alert("Successfully Logged in", AlertType.info);

            return Redirect("/");
        }

        public IActionResult Register()
        {
            var foodbanks = _svc.GetFoodBanks();

            var ucvm = new UserRegisterViewModel
            {
                FoodBanks = new SelectList(foodbanks,"Id","StreetName")
            };

            return View(ucvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Register a new user and bind the below attributes to that user 
        public IActionResult Register([Bind("FirstName,SecondName,Email,Password,PasswordConfirm,FoodBankId,Role")] UserRegisterViewModel m)       
        {
            //pass the properties from the SelectList in again in case of error
            var foodbanks = _svc.GetFoodBanks();

            //if not passed in this causes an error when the form is passed in the 2nd name
            var ucvm = new UserRegisterViewModel
            {
                FoodBanks = new SelectList(foodbanks,"Id","StreetName")
            };

            if (!ModelState.IsValid)
            {
                //ensure the view model above is returned so that the foreign key select list property appears in case of first time error
                return View(ucvm);
            }
            // add user via service
            var user = _svc.AddUser(m.FirstName, m.SecondName, m.Email,m.Password, m.FoodBankId, m.Role);
            // check if error adding user and display warning
            if (user == null) {
                Alert("There was a problem Registering. Please try again", AlertType.warning);
                return View(ucvm);
            }

            Alert("Successfully Registered. Now login", AlertType.info);

            return RedirectToAction(nameof(Login));
        }

        public IActionResult Create()
        {
            var foodbanks = _svc.GetFoodBanks();

            var ucvm = new UserRegisterViewModel
            {
                FoodBanks = new SelectList(foodbanks,"Id","StreetName")
            };

            return View(ucvm);
        }
        [Authorize(Roles = "admin")]

        public IActionResult Index(UserSearchViewModel u)
        {
            u.Users = _svc.SearchUsers(u.Query);

            return View(u);
        }

        [Authorize(Roles="admin")]
        public IActionResult Details(int id)
        {
            var u = _svc.GetUser(id);

            if(u == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(u);

        }

        [Authorize(Roles = "admin")]
        public IActionResult Edit(int id)
        {
            //load all foodbanks into memmory using the SelectList feature on the ViewModel
            var foodbanks = _svc.GetFoodBanks();
           // use BaseClass helper method to retrieve Id of signed in user 
            var user = _svc.GetUser(id);

            if(user == null || foodbanks == null) 
            { 
                return null; 
            }

            var userViewModel = new UserProfileViewModel { 

                FoodBanks = new SelectList(foodbanks, "Id","StreetName"),
                Id = user.Id, 
                FirstName = user.FirstName,
                SecondName = user.SecondName,
                FoodBankId = user.FoodBankId, 
                Email = user.Email,                 
                Role = user.Role

            }; 

            //pass to the view for editing
            return View(userViewModel);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult Edit(int id, [Bind("Id,FirstName,SecondName,Email,FoodBankId,Role")] UserProfileViewModel u)
        {
            var user = _svc.GetUser(u.Id);  

            if(!ModelState.IsValid || user == null)
            {
                return View(u);         
            }

            user.FirstName = u.FirstName;
            user.SecondName = u.SecondName;
            user.Email = u.Email;
            user.FoodBankId = u.FoodBankId;
            user.Role = u.Role;

            var updated = _svc.UpdateUser(user);

            if (updated == null) 
            {
                Alert("There was a problem Updating. Please try again", AlertType.warning);
                return View(u);
            }

            Alert($"Successfully updated the account details for {user.FirstName}", AlertType.info);


            return RedirectToAction(nameof(Index));

        } 

        [Authorize]
        public IActionResult UpdateProfile()
        {
            //instantiate food banks from selectlist viewmodel
            var foodbanks = _svc.GetFoodBanks();
           // use BaseClass helper method to retrieve Id of signed in user 
            var user = _svc.GetUser(User.GetSignedInUserId());
            var userViewModel = new UserProfileViewModel { 

                FoodBanks = new SelectList(foodbanks, "Id","StreetName"),
                Id = user.Id, 
                FirstName = user.FirstName,
                SecondName = user.SecondName,
                FoodBankId = user.FoodBankId, 
                Email = user.Email,                 
                Role = user.Role
            };

            return View(userViewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile([Bind("Id,FirstName,SecondName,Email,FoodBankId,Role")] UserProfileViewModel m)       
        {
            var user = _svc.GetUser(m.Id);
            // check if form is invalid and redisplay
            if (!ModelState.IsValid || user == null)
            {
                return View(m);
            } 

            // update user details and call service
            user.FirstName = m.FirstName;
            user.SecondName = m.SecondName;
            user.Email = m.Email;
            user.FoodBankId = m.FoodBankId;
            user.Role = m.Role;        
            var updated = _svc.UpdateUser(user);

            // check if error updating service
            if (updated == null) {
                Alert("There was a problem Updating. Please try again", AlertType.warning);
                return View(m);
            }

            Alert("Successfully Updated Account Details", AlertType.info);
            
            // sign the user in with updated details)
            await SignInCookie(user);

            return RedirectToAction("Index","Home");
        }

        // Change Password
        [Authorize]
        public IActionResult UpdatePassword()
        {
            // use BaseClass helper method to retrieve Id of signed in user 
            var user = _svc.GetUser(User.GetSignedInUserId());
            var passwordViewModel = new UserPasswordViewModel { 
                Id = user.Id, 
                Password = user.Password, 
                PasswordConfirm = user.Password, 
            };
            return View(passwordViewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword([Bind("Id,OldPassword,Password,PasswordConfirm")] UserPasswordViewModel m)       
        {
            var user = _svc.GetUser(m.Id);
            if (!ModelState.IsValid || user == null)
            {
                return View(m);
            }  
            // update the password
            user.Password = m.Password; 
            // save changes      
            var updated = _svc.UpdateUser(user);
            if (updated == null) {
                Alert("There was a problem Updating the password. Please try again", AlertType.warning);
                return View(m);
            }

            Alert("Successfully Updated Password", AlertType.info);
            // sign the user in with updated details
            await SignInCookie(user);

            return RedirectToAction("Index","Home");
        }

        [Authorize(Roles ="admin")]
        public IActionResult Delete(int id)
        {
            var u = _svc.GetUser(id);

            if(u == null)
            {
                Alert($"User {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }
            
            return View(u);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult DeleteConfirm(int id)
        {
            var u = _svc.DeleteUser(id);

            Alert("User deleted successfully", AlertType.info);

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        // Return not authorised and not authenticated views
        public IActionResult ErrorNotAuthorised() => View();
        public IActionResult ErrorNotAuthenticated() => View();

        // -------------------------- Helper Methods ------------------------------

        // Called by Remote Validation attribute on RegisterViewModel to verify email address is available
        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyEmailAvailable(string email, int id)
        {
            // check if email is available, or owned by user with id 
            if (!_svc.IsEmailAvailable(email,id))
            {
                return Json($"A user with this email address {email} already exists.");
            }
            return Json(true);                  
        }

        // Called by Remote Validation attribute on ChangePassword to verify old password
        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyPassword(string oldPassword)
        {
            // use BaseClass helper method to retrieve Id of signed in user 
            var id = User.GetSignedInUserId();            
            // check if email is available, unless already owned by user with id
            var user = _svc.GetUser(id);
            if (user == null || !Hasher.ValidateHash(user.Password, oldPassword))
            {
                return Json($"Please enter current password.");
            }
            return Json(true);                  
        }

        // Sign user in using Cookie authentication scheme
        private async Task SignInCookie(User user)
        {
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                AuthBuilder.BuildClaimsPrincipal(user)
            );
        }
    }
}