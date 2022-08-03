using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FBMS3.Web.ViewModels
{
    public class RecipeCreateViewModel
    {
        public string Title { get; set; }

        public string NoOfIngredients { get; set; }

        
    }
}