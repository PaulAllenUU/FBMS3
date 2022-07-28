using System;
namespace FBMS3.Core.Models
{

    public enum StockRange { ALL, NONFOOD, MEAT, VEGETABLE, CARBOHYDRATE }
    public class Stock
    {
        public int Id { get; set; }

        public string Description { get; set ;}

        public DateTime ExpiryDate { get; set; }

        public int Quantity { get; set; }

        //each Stock item will have only 1 Food Bank
        public FoodBank FoodBank { get; set; }

        //foreign key to the foodbank table
        public int FoodBankId { get; set; }
    }
}