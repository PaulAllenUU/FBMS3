using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using FBMS3.Core.Models;

namespace FBMS3.Web.ViewModels
{
    public class UserProfileViewModel
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string SecondName { get; set; }
 
        [Required]
        [EmailAddress]
        [Remote(action: "VerifyEmailAvailable", controller: "User", AdditionalFields = nameof(Id))]
        public string Email { get; set; }

        public string FoodBankStreetName { get; set; }

        [Required]
        public Role Role { get; set; }

    }
}