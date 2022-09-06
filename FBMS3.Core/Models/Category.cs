using System;

using System.ComponentModel.DataAnnotations;
using FBMS3.Core.Validators;

namespace FBMS3.Core.Models
{
    //category table used for the the distribution of varied and nutritionally dense parcels to clients
    public class Category
    {
        public int Id { get; set; }

        public string Description { get; set; }

        //each stock category has many items of stock
        public IList<Stock> Stock { get; set; } = new List<Stock>();

        //eacg category has a list of parcel items
        public IList<ParcelItem> Items { get; set; } = new List<ParcelItem>();

    }
}

