using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using FBMS3.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace FBMS3.Web.ViewModels
{
    public class UserRegisterViewModel
    { 
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string SecondName { get; set; }
 
        [Required]
        [EmailAddress]
        [Remote(action: "VerifyEmailAvailable", controller: "User")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string PasswordConfirm  { get; set; }

        public SelectList FoodBanks { get; set ; }

        [Display(Name = "Please confirm which food bank you are working at")]
        public int FoodBankId { get; set; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Role Role { get; set; }

    }
}