using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FBMS3.Core.Models
{   
    public class Parcel
    {
        public int Id { get; set; }

        //foreign key for client
        public int ClientId { get; set; }

        //navigation property for client
        public Client Client { get; set; }

        public string ClientSecondName => Client?.SecondName;

        public User User { get; set; }

        //each parcel as a User Id to identify which food bank worker distributed it
        //A parcel will have 1 user but a user can have many parcels
        public int UserId { get; set; }

        //give parcel access to the user first name property
        public string UserFirstName => User.FirstName;
        
        //navigation property to food bank
        public FoodBank FoodBank { get; set; }

        //relationahip - each parcel has 1 and only 1 food bank
        public int FoodBankId { get; set; }

        //give access to the food bank street name property
        public string FoodBankStreetName => FoodBank?.StreetName;
        
        //list of relationships to the bridging table
        public IList<ParcelItem> Items { get; set; } = new List<ParcelItem>();

    }

}