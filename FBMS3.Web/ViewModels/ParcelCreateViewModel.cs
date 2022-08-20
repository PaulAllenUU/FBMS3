using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using FBMS3.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FBMS3.Web.ViewModels
{ //parcel controller inherits all of the properts from client create except for User Id
    public class ParcelCreateViewModel 
    {
        public int UserId { get; set;}

        public int ClientId { get; set; }

        public int FoodBankId { get; set; }

        public int NoOfPeople { get; set; }

        public IList<Stock> Stock { get; set; }

    }
}