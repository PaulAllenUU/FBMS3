using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FBMS3.Web.ViewModels
{
    public class RecipeStockViewModel
    {
        public string Title { get; set; }

        public string NoOfIngredients { get; set; }

        public int CookingTimeMins { get; set; }
        
    }
}