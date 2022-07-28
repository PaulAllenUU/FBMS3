using FBMS3.Core.Models;

namespace FBMS3.Web.ViewModels
{
    public class StockSearchViewModel
    {
        //will return an IList of IStock called Stock
        public IList<Stock> Stock { get; set;}

        //search options for looking for specific stock items
        public string Query { get; set; }

        //when further down development add in enum for meat, veg or carbs
        public StockRange Range { get; set; }
    }
}