using System;

using System.ComponentModel.DataAnnotations;
using FBMS3.Core.Validators;

namespace FBMS3.Core.Models
{
    public class FoodBank
    {

        //set suitable properties for each FoodBank location
        public int Id { get; set; }

        [Required(ErrorMessage ="Street number is required")]
        public int StreetNumber { get; set; }

        [Required(ErrorMessage ="Street name is required")]
        public string StreetName { get; set; }

        [Required(ErrorMessage = "Post code is required")]
        public string PostCode { get; set; }

        //Many side of the relaton with users - 1 food bank will have many users
        public IList<User> Users { get ; set; } = new List <User>();

        //foodBanks will also have a list of stock - 1 food bank will have many stock items
        public IList<Stock> Stock { get; set; } = new List<Stock>();

        //foodbanks can have many clients
        public IList<Client> Clients { get; set; } = new List<Client>();

        //food banks will have many parcels
        public IList<Parcel> Parcels { get; set; } = new List<Parcel>();

    }
}