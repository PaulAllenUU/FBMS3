using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using FBMS3.Core.Models;

namespace FBMS3.Web.ViewModels
{
    public class ParcelItemViewModel
    {
        //each of the operties should have meaningful context for the use
        public int ParcelId { get; set; }

        public Parcel Parcel { get; set; }

        public int StockId { get; set; }

        public SelectList Items { get; set; }

        public int Quantity { get; set; } 

    }
}