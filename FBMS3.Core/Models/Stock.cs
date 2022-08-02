using System;
using System.Text.Json.Serialization;

namespace FBMS3.Core.Models
{

    public enum StockRange { ALL, NONFOOD, MEAT, VEGETABLE, CARBOHYDRATE }
    
    public class Stock
    {
        public int Id { get; set; }

        public string Description { get; set ; }

        public DateTime ExpiryDate { get; set; } 

        public int Quantity { get; set; }
        
        //non food to be used to search for non food items in the enum
        public bool NonFood { get; set; } 

        //to also be used to search for only meet in the enum
        public bool Meat { get; set; } 

        public bool Vegetable { get; set; } 

        public bool Carbohydrate { get; set; } 

        //each Stock item will have only 1 Food Bank
        public int FoodBankId { get; set; }

        [JsonIgnore]
        //foreign key to the foodbank table
        public FoodBank FoodBank { get; set; }

        //Relationship M:N Stock - Recipes
        public IList<RecipeIngredients> RecipeIngredients { get; set; } = new List<RecipeIngredients>();
    }
}