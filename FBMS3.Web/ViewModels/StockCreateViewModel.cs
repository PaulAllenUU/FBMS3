using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FBMS3.Web.ViewModels
{
    public class StockCreateViewModel
    {
        public SelectList FoodBanks { get; set; }

        [Required(ErrorMessage = "Please select a foodbank")]
        [Display(Name = "Select which foodbank you are adding the stock item to")]
        public int FoodBankId { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set ;}

        public DateTime ExpiryDate { get; set; }
    }
}