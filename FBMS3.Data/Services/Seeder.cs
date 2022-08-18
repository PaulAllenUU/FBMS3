
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

            // add users
            var u1 = svc.AddUser("Paul", "Allen", "admin@mail.com", "admin", f1.Id, Role.admin);
            var u2 = svc.AddUser("Manager", "Farwell", "manager@mail.com", "manager", f2.Id, Role.manager);
            var u3 = svc.AddUser("Guest", "O'Hara", "guest@mail.com", "guest", f2.Id, Role.guest); 
            
            //end of dummy user data

            //add dummy food category data
            var c1 = svc.AddStockCategory("Cereal");
            var c2 = svc.AddStockCategory("Soup");
            var c3 = svc.AddStockCategory("Baked Beans");
            var c4 = svc.AddStockCategory("Tomatoes");
            var c5 = svc.AddStockCategory("Vegetables");
            var c6 = svc.AddStockCategory("Meat");
            var c7 = svc.AddStockCategory("Veggie Option");
            var c8 = svc.AddStockCategory("Fish");
            var c9 = svc.AddStockCategory("Fruit");
            var c10 = svc.AddStockCategory("Pudding");
            var c11 = svc.AddStockCategory("Carbohydrates");
            var c12 = svc.AddStockCategory("Hot Beverage");
            var c13 = svc.AddStockCategory("Juice");
            var c14 = svc.AddStockCategory("Milk UHT");
            var c15 = svc.AddStockCategory("Sauces");
            var c16 = svc.AddStockCategory("Treats");
            var c17 = svc.AddStockCategory("Spreads");
            var c18 = svc.AddStockCategory("Toileteries");
            var c19 = svc.AddStockCategory("Pet Food");
            var c20 = svc.AddStockCategory("Kitchen Cleaning");
            var c21 = svc.AddStockCategory("Logs");  

            //add some dummy stock to the database
            var s1 = svc.AddStock(f1.Id, "Orange", 4, new DateTime(2022, 10, 01), c9.Id);
            var s2 = svc.AddStock(f2.Id, "Banana", 10, new DateTime(2023, 12, 03), c9.Id);
            var s3 = svc.AddStock(f3.Id, "Potato", 20, new DateTime(2022, 12, 07), c11.Id);
            var s4 = svc.AddStock(f1.Id, "Chicken", 6, new DateTime(2023, 08, 09), c6.Id);
            var s11 = svc.AddStock(f3.Id, "Carrots", 10, new DateTime(2023, 10, 01), c5.Id);
            var s5 = svc.AddStock(f2.Id, "Toilet Paper", 10, new DateTime(2024, 10, 07), c18.Id);
            var s6 = svc.AddStock(f2.Id, "Fish", 8, new DateTime(2022, 11, 18), c8.Id);
            var s7 = svc.AddStock(f3.Id, "Tinned Tomatoes", 4, new DateTime(2024,01,01), c4.Id);
            var s8 = svc.AddStock(f5.Id, "Beef", 2, new DateTime(2022, 08, 08), c6.Id);
            var s9 = svc.AddStock(f5.Id, "Sweet Potato", 10, new DateTime(2022, 09, 01), c11.Id);
            var s10 = svc.AddStock(f1.Id, "Tomatoes", 12, new DateTime(2022, 08, 30), c4.Id);
            var s12 = svc.AddStock(f2.Id, "Frozen Vegetables Packet", 8, new DateTime(2023, 01, 01), c5.Id);
            var s13 = svc.AddStock(f6.Id, "Onions", 6, new DateTime(2022, 02,02), c5.Id);
            var s14 = svc.AddStock(f6.Id, "Peppers", 5, new DateTime(2022, 09, 04), c5.Id);

            //add some dummy client data
            var cL1 = svc.AddClient("Allen", "BT45 7PL", "example@mail.com", 3, f1.Id);
            var cL2 = svc.AddClient("McLaughlin", "BT65 9LU", "mcl@gmx.com", 4, f2.Id);
            var cL3 = svc.AddClient("Johnston", "BT72 9PO", "jjopl@yahoo.co.uk", 1, f3.Id);
            var cL4 = svc.AddClient("O'Neill", "BT31 0NM", "oneill@hotmail.co.uk", 2, f3.Id);

            //add some dummy recipe data
            /*var r1 = svc.AddRecipe("Carrot & Tomato Soup", 2, 30);
            var r2 = svc.AddRecipe("Vegetable Stir Fry", 4, 20);
            var r3 = svc.AddRecipe("Chilli Con Carne", 4, 20);
            var r4 = svc.AddRecipe("Chicken Curry", 5, 30);*/

            //add the stock to each recipe

            //for recipe 1 you use carrots and tomatoes, add those to the recipe along with the quantity of each
        

            //add some
            
            
        }
    }
}
