using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using FBMS3.Core.Models;

namespace FBMS3.Web.ViewModels
{
    public class ClientProfileViewModel
    {
        public int Id { get; set; }

        [Required]
        public string SecondName { get; set; }

        public string PostCode { get; set;}

        public string EmailAddress { get; set; }

        public int NoOfPeople { get; set; }

        public int FoodBankId { get; set; }

    }
}