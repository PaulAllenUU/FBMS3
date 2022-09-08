using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FBMS3.Web.ViewModels
{
    public class StockCreateViewModel
    {
        public SelectList FoodBanks { get; set; }

        [Required(ErrorMessage = "Please select a foodbank")]
        [Display(Name = "Please confirm where you are adding the stock to")]
        public int FoodBankId { get; set; }

        [Required]
        [Display(Name = "Please include the description of the stock item")]
        public string Description { get; set; }

        [Required]
        public int Quantity { get; set ; }

        [Required]
        public DateTime ExpiryDate { get; set; } 

        public SelectList Categorys { get; set; }

        [Required(ErrorMessage = "Please select a category for your stock")]
        public int CategoryId { get; set; }

    }
}