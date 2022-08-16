using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using FBMS3.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FBMS3.Web.ViewModels
{
    public class ClientCreateViewModel
    {
        [Required]
        public string SecondName { get; set; }

        [Required]
        public string PostCode { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [Range(0, 15)]
        public int NoOfPeople { get; set; }

        public SelectList FoodBanks { get; set; }

        public int FoodBankId { get; set; }

    }
}