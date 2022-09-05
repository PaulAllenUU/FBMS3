using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FBMS3.Web.ViewModels
{
    public class ParcelCreateViewModel
    {
        public SelectList Clients { get; set; }

        public int ClientId { get; set; }

        public SelectList Users { get; set; }
        public int UserId { get; set; }

        public SelectList FoodBanks { get; set; }

        public int FoodBankId { get; set; }
    }
}