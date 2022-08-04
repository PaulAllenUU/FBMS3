using System;
namespace FBMS3.Core.Models
{
    //name given to those who need to use the food bank - added by the User
    //composite primary key required because second name , post code and no of people could all change causing potential anomalies
    public class Client
    {
        //convention of id entity framework to increment
        public int Id { get; set; }

        //second name of the household, get be a family or household of 1 person
        //second name would not be suitable key in case of change due to marriage or seperate
        public string SecondName { get; set; }

        //post code might change due to moving address
        public string PostCode { get; set; }

        //date of birth will never change
        public DateOnly DateOfBirth { get; set; }

        //no of people could change
        public int NoOfPeople { get; set; }

        //navigation property to food bank, each client has their preferred food bank
        public FoodBank FoodBank { get; set; }

        //foreign key from the food bank table, each client has 1 and only 1
        public int FoodBankId { get; set; }
    }
}