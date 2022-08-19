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

        //each item which included in the parcel
        public string Item { get; set; }

        //the allocation of each item
        private int Quantity => QuantityChanger();

        //the size of each item included in the parcel
        private string ItemSize => SizeChanger();

        //navigation to the Client table
        public Client Client { get; set; }

        //each parcel will have one and only one client
        public int ClientId { get; set; }
        
        //navigation property to food bank
        public FoodBank FoodBank { get; set; }

        //relationahip - each parcel has 1 and only 1 food bank
        public int FoodBankId { get; set; }

        //no of people that the parcel is for
        public int NoOfPeople { get; set; }

        //every parcel will have a list of stock
        public IList<Stock> Stock { get; set; } = new List<Stock>();

        //size and quantity determined by number of people in family that the parcel is for
        private string SizeChanger()
        {
            if(NoOfPeople == 1)
            {
                return "small";
            }
            else if(NoOfPeople == 2)
            {
                return "medium";
            }
                 else if(NoOfPeople == 3)
                 {
                    return "large";
                 }
                        else
                        {
                            return "extra large";
                        }
        }

        //larger family sizes will higher quantities of more stock
        private int QuantityChanger()
        {
            if(NoOfPeople == 1)
            {
                return 1;
            }
            else if(NoOfPeople == 2)
            {
                return 2;
            }
                else if(NoOfPeople == 3)
                {
                    return 3;
                }
                    else 
                    {
                        return 4;
                    }
        }
    }

}