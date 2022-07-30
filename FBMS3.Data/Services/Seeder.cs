
using FBMS3.Core.Models;
using FBMS3.Core.Services;

namespace FBMS3.Data.Services
{
    public static class Seeder
    {
        // use this class to seed the database with dummy 
        // test data using an IUserService 
         public static void Seed(IUserService svc)
        {
            svc.Initialise();

            //add food bank locations
            var f1 = svc.AddFoodBank(10, "Antrim Road", "BT49 0ST");
            var f2 = svc.AddFoodBank(50, "Scroggy Road", "BT56 9QP");
            var f3 = svc.AddFoodBank(100, "Springield Road", "BT12 9PF"); 

            // add users
            var u1 = svc.AddUser("Paul", "Allen", "admin@mail.com", "admin", f1.Id, Role.admin);
            var u2 = svc.AddUser("Manager", "Farwell", "manager@mail.com", "manager", f2.Id, Role.manager);
            var u3 = svc.AddUser("Guest", "O'Hara", "guest@mail.com", "guest", f2.Id, Role.guest); 
            //end of dummy user data  

            //add some dummy stock to the database
            var s1 = svc.AddStock(f1.Id, "Orange", 4, new DateTime(2022, 10, 01));
            var s2 = svc.AddStock(f2.Id, "Banana", 10, new DateTime(2023, 12, 03));
            var s3 = svc.AddStock(f3.Id, "Potato", 20, new DateTime(2022, 12, 07));
            var s4 = svc.AddStock(f1.Id, "Chicken", 6, new DateTime(2023, 08, 09));
            var s5 = svc.AddStock(f2.Id, "Toilet Paper", 10, new DateTime(2024, 10, 07));
            
            
        }
    }
}
