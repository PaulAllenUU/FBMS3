using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using FBMS3.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FBMS3.Web.ViewModels
{ //parcel controller inherits all of the properts from client create except for User Id
    public class ParcelStockListViewModel
    {
        public int UserFirstName { get; set; }
        
        public IList <Parcel> Parcels { get; set; } = new List <Parcel>();

        public IList<Stock> Stock { get; set; } = new List<Stock>();

        public int FoodBankId { get; set; }

        public string ClientSecondName { get; set; }

    }
}