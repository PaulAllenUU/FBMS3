
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
            var f3 = svc.AddFoodBank(100, "Springfield Road", "BT12 9PF");
            var f4 = svc.AddFoodBank(90, "Strand Road", "BT48 0AE");
            var f5 = svc.AddFoodBank(98, "Malone Avenue", "BT9 6ES");
            var f6 = svc.AddFoodBank(30, "Evergreen Terrace", "TW78 2PL"); 
            var f7 = svc.AddFoodBank(45, "Hillside Way", "BT78 3FG");
            var f8 = svc.AddFoodBank(89, "University Street", "BT41 4JL");
            var f9 = svc.AddFoodBank(60, "Limavady Road", "BT123 8YT");
            var f10 = svc.AddFoodBank(82, "Main Street", "BT89 3OP");

            // add users
            var u1 = svc.AddUser("Paul", "Allen", "admin@mail.com", "admin", f1.Id, Role.admin);
            var u2 = svc.AddUser("Manager", "Farwell", "manager@mail.com", "manager", f2.Id, Role.manager);
            var u3 = svc.AddUser("Guest", "O'Hara", "guest@mail.com", "guest", f2.Id, Role.staff); 
            var u4 = svc.AddUser("Max", "O'Neill", "max@hotmail.co.uk", "123", f2.Id, Role.staff);
            
            //end of dummy user data

            //add dummy food category data
            var c1 = svc.AddCategory("Cereal");
            var c2 = svc.AddCategory("Soup");
            var c3 = svc.AddCategory("Baked Beans");
            var c4 = svc.AddCategory("Tomatoes");
            var c5 = svc.AddCategory("Vegetables");
            var c6 = svc.AddCategory("Meat");
            var c7 = svc.AddCategory("Veggie Option");
            var c8 = svc.AddCategory("Fish");
            var c9 = svc.AddCategory("Fruit");
            var c10 = svc.AddCategory("Pudding");
            var c11 = svc.AddCategory("Carbohydrates");
            var c12 = svc.AddCategory("Hot Beverage");
            var c13 = svc.AddCategory("Juice");
            var c14 = svc.AddCategory("Milk UHT");
            var c15 = svc.AddCategory("Sauces");
            var c16 = svc.AddCategory("Treats");
            var c17 = svc.AddCategory("Spreads");
            var c18 = svc.AddCategory("Toileteries");
            var c19 = svc.AddCategory("Pet Food");
            var c20 = svc.AddCategory("Kitchen Cleaning");
            var c21 = svc.AddCategory("Logs");  

            //add some dummy stock to the database
            var s1 = svc.AddStock(f1.Id, "Corn Flakes", 100, new DateTime(2022, 10, 01), c9.Id);
            var s2 = svc.AddStock(f1.Id, "Vegetable Broth", 100, new DateTime(2023,01,01), c2.Id);
            var s3 = svc.AddStock(f1.Id, "Baked Beans", 100, new DateTime(2023,01,01), c3.Id);
            var s4 = svc.AddStock(f1.Id, "Tinned Tomastoes", 100, new DateTime(2022,01,01), c4.Id);
            var s5 = svc.AddStock(f1.Id, "Peppers", 100, new DateTime(2022,01,02), c5.Id);
            var s6 = svc.AddStock(f1.Id, "Chicken", 100, new DateTime(2022, 09,09), c6.Id);
            var s7 = svc.AddStock(f1.Id, "Kidney Beans", 100, new DateTime(2022,10,12), c7.Id);
            var s8 = svc.AddStock(f1.Id, "Frozen Cod", 100, new DateTime(2022,10,30), c8.Id);
            var s9 = svc.AddStock(f1.Id, "Banana", 100, new DateTime(2022, 09, 01), c9.Id);
            var s10 = svc.AddStock(f1.Id, "Angel Delight", 100, new DateTime(2022, 05, 07), c10.Id);
            var s11 = svc.AddStock(f1.Id, "Pasta", 100, new DateTime(2023, 02,03), c11.Id);
            var s12 = svc.AddStock(f1.Id, "Coffee Beans", 100, new DateTime(2023,04,05), c12.Id);
            var s13 = svc.AddStock(f1.Id, "Apple Juice", 100, new DateTime(2022,12,12), c13.Id);
            var s14 = svc.AddStock(f1.Id, "Milk", 100, new DateTime(2023,04,05), c14.Id);
            var s15 = svc.AddStock(f1.Id, "Ketchup", 100, new DateTime(2023, 04, 06), c15.Id);
            var s16 = svc.AddStock(f1.Id, "Almond Slices", 100, new DateTime(2022, 10, 01), c16.Id);
            var s17 = svc.AddStock(f1.Id, "Margarine", 100, new DateTime(2024,01,01), c17.Id);
            var s18 = svc.AddStock(f1.Id, "Toilet Paper", 100, new DateTime(2024,05,06), c18.Id);
            var s19 = svc.AddStock(f1.Id, "Dog Food", 100, new DateTime(2023, 02, 03), c19.Id);
            var s20 = svc.AddStock(f1.Id, "Kitchen Towels", 100, new DateTime(2025,01,01), c20.Id);
            var s21 = svc.AddStock(f1.Id, "Logs", 100, new DateTime(2023, 04, 08), c21.Id);
            var s22 = svc.AddStock(f2.Id, "Banana", 10, new DateTime(2023, 12, 03), c9.Id);
            var s23 = svc.AddStock(f3.Id, "Carbohydrates", 20, new DateTime(2022, 12, 07), c11.Id);
            var s24 = svc.AddStock(f1.Id, "Meat", 6, new DateTime(2023, 08, 09), c6.Id);
            var s25 = svc.AddStock(f3.Id, "Vegetables", 10, new DateTime(2023, 10, 01), c5.Id);
            var s26 = svc.AddStock(f2.Id, "Toileteries", 10, new DateTime(2024, 10, 07), c18.Id);
            var s27 = svc.AddStock(f2.Id, "Fish", 8, new DateTime(2022, 11, 18), c8.Id);
            var s28 = svc.AddStock(f3.Id, "Tinned Tomatoes", 4, new DateTime(2024,01,01), c4.Id);
            var s29 = svc.AddStock(f5.Id, "Meat", 2, new DateTime(2022, 08, 08), c6.Id);
            var s30 = svc.AddStock(f5.Id, "Carbohydrates", 10, new DateTime(2022, 09, 01), c11.Id);
            var s31 = svc.AddStock(f1.Id, "Vegetables", 12, new DateTime(2022, 08, 30), c4.Id);
            var s32 = svc.AddStock(f2.Id, "Vegetables", 8, new DateTime(2023, 01, 01), c5.Id);
            var s33 = svc.AddStock(f6.Id, "Vegetables", 6, new DateTime(2022, 02,02), c5.Id);
            var s34 = svc.AddStock(f6.Id, "Vegetables", 5, new DateTime(2022, 09, 04), c5.Id);

            //add some dummy client data
            var cL1 = svc.AddClient("Allen", "BT45 7PL", "example@mail.com", 3, f1.Id);
            var cL2 = svc.AddClient("McLaughlin", "BT65 9LU", "mcl@gmx.com", 4, f2.Id);
            var cL3 = svc.AddClient("Johnston", "BT72 9PO", "jjopl@yahoo.co.uk", 1, f3.Id);
            var cL4 = svc.AddClient("O'Neill", "BT31 0NM", "oneill@hotmail.co.uk", 2, f3.Id);
            var cL5 = svc.AddClient("Campbell", "BT56 7KM", "cmp@hotmail.co.uk", 4, f2.Id);
            var cL6 = svc.AddClient("Marquess", "BT68 7KL", "nm@outlook.com", 3, f2.Id);
            var cL7 = svc.AddClient("Brownlow", "BT73 5QW", "wolyaj@btinternet.com", 4, f1.Id);

            //create some empty parcels
            var parcel1 = svc.AddParcel(cL1.Id, u1.Id, f1.Id);

            //var populate = svc.PopulateParcel(parcel1.Id, s1.Id, c1.Id, 2);
            
            
        }
    }
}
