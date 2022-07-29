using System;
namespace FBMS3.Core.Models
{
    // Add User roles relevant to your application
    public enum Role { admin, manager, guest }
    
    public class User
    {
        //set suitable properties for each User object
        public int Id { get; set; }
        public string FirstName { get; set; }

        public string SecondName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        // User role within application
        public Role Role { get; set; }

        //navigatin property between food banks and users
        public FoodBank FoodBank { get; set; }

        //1 - M relationship, each user will have 1 food bank
        public int FoodBankId { get; set; }

    }
}
