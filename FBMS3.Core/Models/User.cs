using System;
using System.ComponentModel.DataAnnotations;

namespace FBMS3.Core.Models
{
    // Add User roles relevant to your application
    public enum Role { admin, manager, staff }
    
    public class User
    {
        //set suitable properties for each User object
        public int Id { get; set; }
        
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string SecondName { get; set; }

        [EmailAddress(ErrorMessage ="Email is required")]
        [Required]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]

        public string Password { get; set; }

        // User role within application
        [Required]
        public Role Role { get; set; }

        //navigatin property between food banks and users
        public FoodBank FoodBank { get; set; }

        //1 - M relationship, each user will have 1 food bank
        [Required]
        public int FoodBankId { get; set; }

        //each user will have multiple parcels they allocate to clients
        public IList<Parcel> Parcels { get; set; } = new List<Parcel>();

    }
}
