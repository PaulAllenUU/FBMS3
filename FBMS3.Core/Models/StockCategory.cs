using System;

using System.ComponentModel.DataAnnotations;
using FBMS3.Core.Validators;

namespace FBMS3.Core.Models
{
    public class StockCategory
    {
        public int Id { get; set; }

        public string Category { get; set; }

        public Stock Stock { get; set; }

        public int StockId { get; set; }

    }
}

