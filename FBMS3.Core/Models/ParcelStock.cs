using System;

namespace  FBMS3.Core.Models
{
    public class ParcelStock
    {
        public int Id { get; set;}

        public string Description { get; set; }

        public int Quantity { get; set; }

        public int StockId { get; set; }

        public Stock Stock { get; set; }

        public int ParcelId { get; set; }

        public Parcel Parcel { get; set; }
    }
    
}