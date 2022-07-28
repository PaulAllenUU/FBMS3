
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

            // add users
            svc.AddUser("Paul", "Allen", "admin@mail.com", "admin", "largy road", Role.admin);
            svc.AddUser("Manager", "Farwell", "manager@mail.com", "manager", "antrim road", Role.manager);
            svc.AddUser("Guest", "O'Hara", "guest@mail.com", "guest", "dublin road", Role.guest); 
            //end of dummy user data

            //add food bank locations
            svc.AddFoodBank(10, "Antrim Road", "BT49 0ST");
            svc.AddFoodBank(50, "Scroggy Road", "BT56 9QP");
            svc.AddFoodBank(100, "Springield Road", "BT12 9PF");   

            //add some dummy stock to the database
            svc.AddStock(1, "Orange", 4, new DateTime(2022, 10, 01));
            svc.AddStock(2, "Banana", 10, new DateTime(2023, 12, 03));
            svc.AddStock(3, "Potato", 20, new DateTime(2022, 12, 07));
        }
    }
}
