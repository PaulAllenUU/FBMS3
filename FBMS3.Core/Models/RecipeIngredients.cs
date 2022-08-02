namespace FBMS3.Core.Models
{
    //bridging class that removes the many to many relationships between stock and recipe
    public class RecipeIngredients
    {
        public int Id { get; set; }

        //detail about the relationship - quantity of that item of stock
        public int StockItemQuantity { get; set; }

        //foreign for the stock table
        public int StockId { get; set; }

        //navigation property for the related stock table
        public Stock Stock { get; set; }

        //foriegn key for the related recipe table
        public int RecipeId { get; set; }

        //navigation property for the related recipe table
        public Recipe Recipe { get; set; }
        
    }
}