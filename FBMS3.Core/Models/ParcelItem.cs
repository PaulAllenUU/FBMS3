namespace FBMS3.Core.Models
{
    //bridging class to normalise the many to many relationship between Parcel & Stock
    public class ParcelItem
    {
        //primary key - identity
        public int Id { get; set; }

        //navigation property to the stock table
        public Stock Item { get; set; }

        //foreign key to the stock table
        public int StockId { get; set; }

        //quantity of each stock item
        public int Quantity { get; set; }

        //foreign key to the parcel table
        public int ParcelId { get; set; }

        //navigation property to the parcel table
        public Parcel Parcel { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

    }
}