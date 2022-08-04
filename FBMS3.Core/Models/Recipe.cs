using System;

using System.ComponentModel.DataAnnotations;
using FBMS3.Core.Validators;

namespace FBMS3.Core.Models
{
    public enum RecipeRange { ALL, VEGETARIAN, COELIAC, LESSTHAN20MINS }
    public class Recipe
    {

        //suitable properties for each recipe
        public int Id { get; set; }

        public string Title { get; set; }
        
        public int NoOfIngredients { get; set; }

        public int CookingTimeMins { get; set; }

        public bool Vegetarian { get; set; }

        public bool CoeliacFriendly { get; set; }

        public IList<RecipeStock> RecipeStock { get; set; } = new List<RecipeStock>();

    }
}