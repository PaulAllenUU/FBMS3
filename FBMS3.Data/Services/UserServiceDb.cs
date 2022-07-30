using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using FBMS3.Core.Models;
using FBMS3.Core.Services;
using FBMS3.Core.Security;
using FBMS3.Data.Repositories;
namespace FBMS3.Data.Services
{
    public class UserServiceDb : IUserService
    {
        private readonly DatabaseContext  ctx;

        public UserServiceDb()
        {
            ctx = new DatabaseContext(); 
        }

        public void Initialise()
        {
           ctx.Initialise(); 
        }

        // ------------------ User Related Operations ------------------------

        // retrieve list of Users
        public IList<User> GetUsers()
        {
            return ctx.Users.ToList();
        }

        // Retrive User by Id 
        public User GetUser(int id)
        {
            return ctx.Users.FirstOrDefault(s => s.Id == id);
        }

        // Add a new User checking a User with same email does not exist
        public User AddUser(string firstName, string secondName, string email, string password, int foodBankId, Role role)
        {     
            var existing = GetUserByEmail(email);
            if (existing != null)
            {
                return null;
            } 

            var user = new User
            {            
                FirstName = firstName,
                SecondName = secondName,
                Email = email,
                Password = Hasher.CalculateHash(password), // can hash if require
                FoodBankId = foodBankId, 
                Role = role              
            };
            ctx.Users.Add(user);
            ctx.SaveChanges();
            return user; // return newly added User
        }

        // Delete the User identified by Id returning true if deleted and false if not found
        public bool DeleteUser(int id)
        {
            var s = GetUser(id);
            if (s == null)
            {
                return false;
            }
            ctx.Users.Remove(s);
            ctx.SaveChanges();
            return true;
        }

        // Update the User with the details in updated 
        public User UpdateUser(User updated)
        {
            // verify the User exists
            var User = GetUser(updated.Id);
            if (User == null)
            {
                return null;
            }
            // verify email address is registered or available to this user
            if (!IsEmailAvailable(updated.Email, updated.Id))
            {
                return null;
            }
            // update the details of the User retrieved and save
            User.FirstName = updated.FirstName;
            User.SecondName = updated.SecondName;
            User.Email = updated.Email;
            User.Password = Hasher.CalculateHash(updated.Password);
            User.FoodBankId = updated.FoodBankId; 
            User.Role = updated.Role; 

            ctx.SaveChanges();          
            return User;
        }

        // Find a user with specified email address
        public User GetUserByEmail(string email)
        {
            return ctx.Users.FirstOrDefault(u => u.Email == email);
        }

        // Verify if email is available or registered to specified user
        public bool IsEmailAvailable(string email, int userId)
        {
            return ctx.Users.FirstOrDefault(u => u.Email == email && u.Id != userId) == null;
        }

        public IList<User> GetUsersQuery(Func<User,bool> q)
        {
            return ctx.Users.Where(q).ToList();
        }

        public User Authenticate(string email, string password)
        {
            // retrieve the user based on the EmailAddress (assumes EmailAddress is unique)
            var user = GetUserByEmail(email);

            // Verify the user exists and Hashed User password matches the password provided
            return (user != null && Hasher.ValidateHash(user.Password, password)) ? user : null;
            //return (user != null && user.Password == password ) ? user: null;
        }

        // End of User Management Methods

        //Begin Food Bank Management Methods///

        public IList<FoodBank> GetFoodBanks()
        {
            return ctx.FoodBanks.ToList();
        }

        public IList<FoodBank> GetFoodBanksWithSameStreetName(string streetName)
        {
            return ctx.FoodBanks.Where(x => x.StreetName == streetName).ToList();
        }

        public FoodBank GetFoodBankById(int id)
        {
            return ctx.FoodBanks
                    //include the stock that is associated with that food bank
                     .Include(x => x.Stock)
                     .FirstOrDefault( x=> x.Id == id);
        }

        public FoodBank GetFoodBankByStreetNameAndNumber(string streetName, int streetNumber)
        {
            return ctx.FoodBanks.FirstOrDefault( x => x.StreetNumber == streetNumber
                                                && x.StreetName == streetName);
        }

        public FoodBank GetFoodBankByPostCode(string postCode)
        {
            return ctx.FoodBanks.FirstOrDefault ( x => x.PostCode == postCode);
        }

        public bool IsDuplicateLocation(int id, int streetNumber, string postCode)
        {
            var existing = GetFoodBankByStreetNoAndPostCode(streetNumber, postCode);

            //if a FoodBank with the street number and post code exists and id doesnt match
            //cannot use this address
            return existing != null && existing.Id != id && existing.PostCode != postCode;
        }

        public FoodBank AddFoodBank(int streetNumber, string streetName, string postCode)
        {
            //check that food bank with same street number and post code doesnt exist already
            var exists = GetFoodBankByStreetNameAndNumber(streetName, streetNumber);

            //if it does exist then cannot add so return null
            if (exists != null) { return null; }

            var foodbank = new FoodBank
            {
                StreetNumber = streetNumber,
                StreetName = streetName,
                PostCode = postCode
            };

            //add the new food bank, save changes and return the food bank
            ctx.Add(foodbank);
            ctx.SaveChanges();
            return foodbank;
        }

        public FoodBank UpdateFoodBank(FoodBank updated)
        {
            //verify the food bank exists before it can be updated
            var foodbank = GetFoodBankById(updated.Id);

            if(foodbank == null) 
            { 
                return null; 
            
            }

            //verify this address registed or available to this user
            /*if (IsDuplicateLocation(updated.Id, updated.StreetNumber, updated.PostCode)) 
            {
                return null;
            }*/

            foodbank.StreetNumber = updated.StreetNumber;
            foodbank.Id = updated.Id;
            foodbank.PostCode = updated.PostCode;

            //save changes and return the food bank
            ctx.SaveChanges();
            return foodbank;
        }

        public bool DeleteFoodBank(int id)
        {
            //check the food bank exists before it can be deleted
            var foodbank = GetFoodBankById(id);

            if (foodbank == null)
            {
                return false;
            }

            //if it not null then remove the food bank, save and return 
            ctx.Remove(foodbank);
            ctx.SaveChanges();
            return true;
        }

        public FoodBank GetFoodBankByStreetNoAndPostCode(int streetNumber, string postCode)
        {
            return ctx.FoodBanks.FirstOrDefault(x => x.StreetNumber == streetNumber && 
                                                     x.PostCode == postCode);
        }

        //------End of Food Bank Management Methods -----//

        //------Begin Stock Management Methods-------//
        public IList<Stock> GetAllStock()
        {
            return ctx.Stock
                      .Include(s => s.FoodBank)
                      .ToList();
        }

        //Get one item of stock by id
        public Stock GetStockById(int id)
        {
            return ctx.Stock
                        //include the food bank that the item of stock is associated with
                      .Include(x => x.FoodBank)
                      .FirstOrDefault( x => x.Id == id);
        }

        public Stock GetStockByDescription(string description)
        {
            return ctx.Stock.FirstOrDefault ( x => x.Description.Equals(description));
        }

        public Stock GetStockByExpiryDate(DateTime expiryDate)
        {
            return ctx.Stock.FirstOrDefault ( x => x.ExpiryDate == expiryDate);
        }

        public Stock AddStock(int foodBankId, string description, int quantity, DateTime expiryDate)
        {
            //the food bank id is the foreign key so need to check it exists
            var foodbank = GetFoodBankById(foodBankId);

            //if the food bank is null - does not exist then return null
            if (foodbank == null) return null;

            //otherwise create the new item of stock
            var s = new Stock
            {
                FoodBankId = foodBankId,
                Description = description,
                Quantity = quantity,
                ExpiryDate = expiryDate,

            };

            //create a set of all meat types and add them all
            List<String> MeatTypes = new List<String>();
            MeatTypes.Add("Chicken");
            MeatTypes.Add("Pork");
            MeatTypes.Add("Beef");
            MeatTypes.Add("Fish");

            //check if the description added by the user contains any of the items in the list
            if(MeatTypes.Contains(description))
            {
                s.Meat = true;
            }

            //do the same for checking for vegetable
            List<String> Vegetables = new List<String>();
            Vegetables.Add("Carrots");
            Vegetables.Add("Peppers");
            Vegetables.Add("Cauliflower");
            Vegetables.Add("Onions");

            //check if the descrption added by the user contains any items included in the created list
            if(Vegetables.Contains(description))
            {
                s.Vegetable = true;
            }

            //do the same for carbohydrate items such as bread, pasta, rice and potatoes
            List<String> Carbohydrates = new List<String>();
            Carbohydrates.Add("Potato");
            Carbohydrates.Add("Pasta");
            Carbohydrates.Add("Rice");
            Carbohydrates.Add("Spagehtti");

            //check if the descrption contains any of these items
            if(Carbohydrates.Contains(description))
            {
                s.Carbohydrate = true;
            }

            //last do the same for non food items
            List<String> NonFoodItems = new List<String>();
            NonFoodItems.Add("Toilet Paper");
            NonFoodItems.Add("Kitchen Roll");
            NonFoodItems.Add("Razors");
            NonFoodItems.Add("Pet Food");
            NonFoodItems.Add("Shower Gel");

            //final if logic for containing non food items
            if(NonFoodItems.Contains(description))
            {
                s.NonFood = true;
            }

            //add the newly created stock item to the database and save changes
            ctx.Stock.Add(s);
            ctx.SaveChanges();
            return s;
        }


        public Stock UpdateStock(Stock updated)
        {
            //check item of stock exists before it can be updated
            var s = GetStockById(updated.Id);

            //if it dooes not exist then return null
            if (s == null) return null;

            //otherwise update the item of stock
            s.Description = updated.Description;
            s.FoodBankId = updated.FoodBankId;
            s.Quantity = updated.Quantity;

            s.NonFood = updated.NonFood;
            s.ExpiryDate = updated.ExpiryDate;

            //save changes and return the stock item just updated
            ctx.SaveChanges();
            return s;
        }

        public bool DeleteStockById(int id)
        {
            //check that the item of stock marked for deletion exists
            var s = GetStockById(id);

            //if null then does not exist so return false
            if (s == null) return false;

            //otherwise remove stock and save changes
            ctx.Stock.Remove(s);
            ctx.SaveChanges();
            return true;
        }

        public IList<Stock> GetAllNonFoodItems(bool nonFood)
        {
            return ctx.Stock.Where(x => x.NonFood == true).ToList();
        }

        public IList<Stock> GetAllMeatFoodItems(bool meat)
        {
            return ctx.Stock.Where(x => x.Meat == true).ToList();
        }

        public IList<Stock> GetAllVegetablesFromStock(bool vegetable)
        {
            return ctx.Stock.Where( x => x.Vegetable == true).ToList();
        }

        public IList<Stock> GetAllCarbohydratesFromStock(bool carbs)
        {
            return ctx.Stock.Where( x => x.Carbohydrate == true).ToList();
        }

        public IList <Stock> SearchStock(StockRange range, string query)
        {

            //ensure that the query is not null
            query = query == null ? "" : query.ToLower();

            var results = ctx.Stock
                             .Include(x => x.FoodBank)
                             .Where(x => (x.Description.ToLower().Contains(query) ||
                                          x.FoodBank.StreetName.ToLower().Contains(query)
                                          ) &&
                                          (range == StockRange.NONFOOD && x.NonFood== true ||
                                           range == StockRange.MEAT && x.Meat == true ||
                                           range == StockRange.CARBOHYDRATE && x.Carbohydrate == true ||
                                           range == StockRange.ALL
                                          )
                             ).ToList();
            return results;
        }

        public IList<FoodBank> SearchFoodBanks(string query)
        {
            //ensure that the query is not null
             query = query == null ? "" : query.ToLower();

             //enable user to search food banks by street number, street name or post code
             var results = ctx.FoodBanks
                              .Include(x => x.Stock)
                              .Where(x => (x.StreetName.ToLower().Contains(query) ||
                                    
                                    //street number must be cast to string
                                     x.StreetNumber.ToString().Contains(query) ||
                                     x.PostCode.ToLower().Contains(query))
                              ).ToList();

            return results;
        }
    }
}