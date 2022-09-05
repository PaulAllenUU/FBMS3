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

        //to also be used to search for only meat in the enumeration
        public bool Meat { get; set; } 

        public bool Vegetable { get; set; } 

        public bool Carbohydrate { get; set; } 

        //navigation property to stock category table
        public Category Category { get; set; }

        public int CategoryId { get; set; }

        private string GetCategoryDescription()
        {
            return Category?.Description;
        }

        //each Stock item will have only 1 Food Bank
        public int FoodBankId { get; set; }

        [JsonIgnore]
        //foreign key to the foodbank table
        public FoodBank FoodBank { get; set; }

        public string FoodBankStreetName => FoodBank?.StreetName;

        //list of relationships to the bridging table for parcel and item
        public IList<ParcelItem> Items { get; set; } = new List<ParcelItem>();
        
    }
}