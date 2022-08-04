using FBMS3.Core.Models;
using System.Collections.Generic;

namespace FBMS3.Web.ViewModels
{
    public class RecipeSearchViewModel
    {
        //always create the list even when it is empty
        public IList<Recipe> Recipes { get; set; } = new List<Recipe>();

        //search options for looking for specific recipes
        public string Query { get; set; } ="";

        //add functionality for enums for veggie, coeliac or less than 20 min
        public RecipeRange Range { get; set; } = RecipeRange.ALL;
    }
}