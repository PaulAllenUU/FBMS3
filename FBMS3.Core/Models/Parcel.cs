using System;
using System.Text.Json.Serialization;

namespace FBMS3.Core.Models
{   
    public class Parcel
    {
        public int Id { get; set; }

        //navigation property to the user table
        public User User { get; set; }

        //each parcel as a User Id to identify which food bank worker distributed it
        //A parcel will have 1 user but a user can have many parcels
        public int UserId { get; set; }

        //the date the parcel is created or given to the client - default to dateTime.now
        public DateTime Date { get; set; } = DateTime.Now;

        //the allocation of each item
        public int Quantity => SizeChanger();

        //navigation to the Client table
        public Client Client { get; set; }

        //each parcel will have one and only one client
        public int ClientId { get; set; }
        
        //navigation property to food bank
        public FoodBank FoodBank { get; set; }

        //relationahip - each parcel has 1 and only 1 food bank
        public int FoodBankId { get; set; }

        //no of people that the parcel is for
        public int NoOfPeople => Client.NoOfPeople;

        //every parcel will have a list of stock
        public IList<Stock> Items { get; set; } = new List<Stock>();

        private int SizeChanger()
        {
            if(NoOfPeople == 1)
            {
                return 5;
            }
            else if (NoOfPeople == 2)
            {
                return 10;
            }
                else if (NoOfPeople == 3)
                {
                    return 15;
                }
                else
                    return 20;

        }

    }

}