using System;

using System.ComponentModel.DataAnnotations;
using FBMS3.Core.Validators;

namespace FBMS3.Core.Models
{
    public class FoodBank
    {

        //set suitable properties for each FoodBank location
        public int Id { get; set; }

        public int StreetNumber { get; set; }

        public string StreetName { get; set; }

        public string PostCode { get; set; }

        //Many side of the relaton with users - 1 food bank will have many users
        public IList<User> Users { get ; set; } = new List <User>();

        //foodBanks will also have a list of stock - 1 food bank will have many stock items
        public IList<Stock> Stock { get; set; } = new List<Stock>();

        //foodbanks can have many clients
        public IList<Client> Clients { get; set; } = new List<Client>();

    }
}