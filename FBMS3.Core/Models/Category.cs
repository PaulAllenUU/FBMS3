using System;

using System.ComponentModel.DataAnnotations;
using FBMS3.Core.Validators;

namespace FBMS3.Core.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Description { get; set; }

        //each stock category has many items of stock
        public IList<Stock> Stock { get; set; } = new List<Stock>();

    }
}

