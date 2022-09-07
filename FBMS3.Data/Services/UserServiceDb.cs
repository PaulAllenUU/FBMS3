using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

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

        // retrieve list of Users
        public IList<User> GetUsers()
        {
            return ctx.Users.ToList();
        }

        // Retrive User by Id 
        public User GetUser(int id)
        {
            return ctx.Users
                      .Include(x => x.FoodBank)
                      .FirstOrDefault(s => s.Id == id);
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

        public User EditUser(User updated)
        {
            var user = GetUser(updated.Id);

            if(user == null)
            {
                return null;
            }

            if(!IsEmailAvailable(updated.Email, updated.Id))
            {
                return null;
            }

            //update the details
            user.Id = updated.Id;
            user.FirstName = updated.FirstName;
            user.SecondName = updated.SecondName;
            user.Email = updated.Email;
            user.Role = updated.Role;
            user.FoodBankId = updated.FoodBankId;

            ctx.SaveChanges();
            
            return user;
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

        public bool IsClientEmailAvailable(string email, int clientId)
        {
            return ctx.Clients.FirstOrDefault(u => u.EmailAddress == email && u.Id != clientId) == null;
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

        public IList<User> SearchUsers(string query)
        {
            //ensure that the query for the user accounts is not null
            query = query == null ? "" : query.ToLower();

            var results = ctx.Users
                             .Include(x => x.FoodBank)
                             .Where( x => (x.FirstName.ToLower().Contains(query) ||
                                           x.SecondName.ToLower().Contains(query) ||
                                           x.FoodBank.StreetName.ToLower().Contains(query) ||
                                           //x.Role.ToString().ToLower().Contains(query) ||
                                           x.Email.ToLower().Contains(query))
                                    ).ToList();

            return results;

        }

        // End of User Management Methods

        //Begin Food Bank Management Methods///

        public IList<FoodBank> GetFoodBanks()
        {
            return ctx.FoodBanks.ToList();
        }

        public FoodBank GetFoodBankById(int id)
        {
            return ctx.FoodBanks
                    //include the stock and clients that are associated with that food bank
                     .Include(x => x.Clients)
                     //include the stock that is currently at that food bank
                     .Include(x => x.Stock)
                     //include the category that the stock at the food bank belongs to 
                     .ThenInclude(x => x.Category)
                     //first or default will find the first value or defauled
                     .FirstOrDefault( x=> x.Id == id);
        }

        public FoodBank GetFoodBankByStreetNameAndNumber(string streetName, int streetNumber)
        {
            return ctx.FoodBanks.FirstOrDefault( x => x.StreetNumber == streetNumber
                                                && x.StreetName == streetName);
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


        //------End of Food Bank Management Methods -----//

        //------Begin Stock Management Methods-------//
        public IList<Stock> GetAllStock()
        {
            return ctx.Stock
                      .Include(s => s.FoodBank)
                      .Include(s => s.Category)
                      .ToList();
        }

        //Get one item of stock by id
        public Stock GetStockById(int id)
        {
            return ctx.Stock
                        //include the food bank that the item of stock is associated with
                      .Include(x => x.FoodBank)
                      .Include(x => x.Category)
                      .FirstOrDefault( x => x.Id == id);
        }

        public Stock GetStockByDescription(string description)
        {
            return ctx.Stock.FirstOrDefault ( x => x.Description.Equals(description));
        }

        public Category AddCategory(string description)
        {
            var exists = GetCategoryByDescription(description);
            
            //if the category is not null, ie already exists then return null
            if(exists != null)
            {
                return null;
            }

            var cat = new Category
            {
                Description = description
            };

            ctx.Categorys.Add(cat);
            ctx.SaveChanges();
            return cat;

        }

        public Stock AddStock(int foodBankId, string description, int quantity, DateTime expiryDate, int categoryId)
        {
            //the food bank id is the foreign key so need to check it exists
            var foodbank = GetFoodBankById(foodBankId);
            var category = GetCategoryById(categoryId);

            //if the food bank is null - does not exist then return null
            if (foodbank == null || category == null) return null;

            //otherwise create the new item of stock
            var s = new Stock
            {
                FoodBankId = foodBankId,
                Description = description,
                Quantity = quantity,
                ExpiryDate = expiryDate,
                CategoryId = categoryId
            };


            //determine enumeration types from the object just create
            if(s.Description == "Meat")
            {
                s.Meat = true;
            }

            //use for the enum for vegetables
            if(s.Description == "Vegetables")
            {
                s.Vegetable = true;
            }

            //use in the enum for carbohydrates
            if(s.Description == "Carbohydrates")
            {
                s.Carbohydrate = true;
            }

            //use in the enum for non food items
            String [] NonFoodItems = new String[]{ "Toileteries", "Pet Food", "Logs", "Razors", "Kitchen Cleaning"};

            //for loop to traverse the array
            for(int i = 0 ; i < NonFoodItems.Length ; i++)
            {

                if(s.Description == NonFoodItems[i])
                {
                    s.NonFood = true;
                }
                
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
            s.CategoryId = updated.CategoryId;

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

        public IList <Stock> SearchStock(StockRange range, string query)
        {

            //ensure that the query is not null
            query = query == null ? "" : query.ToLower();

            var results = ctx.Stock
                             .Include(x => x.FoodBank)
                             .Where(x => (x.Description.ToLower().Contains(query) ||
                                          x.Quantity.ToString().Contains(query) ||
                                          x.FoodBank.StreetName.ToLower().Contains(query)

                                          ) &&
                                          //for the enumeration when the value of the boolean values are set they will come up
                                          (range == StockRange.NONFOOD && x.NonFood == true ||
                                           range == StockRange.MEAT && x.Meat == true ||
                                           range == StockRange.VEGETABLE && x.Vegetable == true ||
                                           range == StockRange.CARBOHYDRATE && x.Carbohydrate == true ||
                                           range == StockRange.ALL
                                          )
                             ).ToList();
            return results;
        }

        //---------Stock Categiry Methods-----//

        public Category GetCategoryById(int id)
        {
            return ctx.Categorys
                      .Include(x => x.Stock)
                      .FirstOrDefault(x => x.Id == id);
        }

        public Category GetCategoryByDescription(string description)
        {
            return ctx.Categorys
                      .Include(x => x.Stock)
                      .FirstOrDefault(x => x.Description == description);
        }

        public IList<Stock> GetAvailableStockAtFoodBank(int id)
        {
            var foodbank = GetFoodBankById(id);
            var stockavailable = foodbank.Stock.ToList();

            return stockavailable;
        }

        //----Begin Parcel Management Methods
        public Parcel AddParcel(int clientId, int userId, int foodbankId)
        {
            //use user defined methods to retrieve the entities
            var u = GetUser(userId);
            
            var c = GetClientById(clientId);
            //check the food bank is not null
            var f = GetFoodBankById(foodbankId);
            //if any are null then return return null

            if(c == null || u == null || f == null)
            {
                return null;
            }

            //create the empty parcel with the foreign key properties set
            var p = new Parcel
            {
                UserId = userId,
                ClientId = clientId,
                FoodBankId = foodbankId
            };
            
            //add the new parcel item to the database
            ctx.Parcels.Add(p);
            //save the changes
            ctx.SaveChanges();
            //return the newly created parcel
            return p;

        }

        public ParcelItem AddItemToParcel(int parcelId, int stockId, int quantity)
        {
            //check the parcel does not already contain stock with the stock id passed in
            var pi = ctx.ParcelItems
                        .FirstOrDefault(x => x.ParcelId == parcelId &&
                                             x.StockId == stockId);

                                             
            //if these are not null then return null 
            if(pi != null) { return null; }

            //locate the parcel and the stock item
            var p = ctx.Parcels.FirstOrDefault(p => p.Id == parcelId);
            var s = ctx.Stock.FirstOrDefault(s => s.Id == stockId);
           

            //if either are null then return null
            if(p == null || s == null) { return null ;}
            //create the parcel item and add to database
            var npi = new ParcelItem { 
                                        ParcelId = parcelId,
                                        StockId = stockId,
                                        Quantity = quantity
                                     };

            
            //add the new parcel item to the database
            ctx.ParcelItems.Add(npi);
            
            //save changes and return the new parcel item
            ctx.SaveChanges();
            return npi;

        }

        /*public IList<ParcelItem> PopulateParcel(int parcelId, int stockId, int categoryId, int quantity)
        {
             //check the parcel does not already contain stock with the stock id passed in
            var pi = ctx.ParcelItems
                        .FirstOrDefault(x => x.ParcelId == parcelId &&
                                             x.StockId == stockId &&
                                             x.CategoryId == categoryId);

           
            //if these are not null then return null 
            if(pi != null) { return null; }

            //locate the parcel and the stock item
            var p = ctx.Parcels.FirstOrDefault(p => p.Id == parcelId);
            var s = ctx.Stock.FirstOrDefault(s => s.Id == stockId);
            var c = ctx.Categorys.FirstOrDefault(c => c.Id == categoryId);

             //if either are null then return null
            if (p == null || s == null || c == null) { return null ; }

            //get categorie ids
            IList<int> categories = ctx.Categorys.Select(x => x.Id).ToList();
            IList<int> stockids = ctx.Stock.Select(x => x.Id).ToList();

            //list of stock items to be populated
            IList <ParcelItem> npiList = new List<ParcelItem>(categories.Count);

            for(int i = 0; i < npiList.Count; i++)
            {
                npiList.Add( new ParcelItem
                                    {
                                        ParcelId = parcelId,
                                        CategoryId = categoryId,
                                        StockId = stockId,
                                        Quantity = quantity
                                    });

            };

            ctx.ParcelItems.AddRange(npiList);
            ctx.SaveChanges();
       
            return npiList;

        }*/

        public ParcelItem UpdateParcelItemQuantity(int parcelId, int stockId, int quantity)
        {
            var parcel = GetParcelById(parcelId);
            //check the parcel exists
            if(parcel == null)
            {
                return null;
            }
            var pi = parcel.Items.FirstOrDefault(s => s.StockId == stockId);

            if(pi == null)
            {
                return null;
            }

            pi.Quantity = quantity;

            ctx.SaveChanges();
            return pi;
        }

        public IList<String> CategoryDescriptions()
        {
            var descriptions = ctx.Categorys.Select(x => x.Description).ToList();

            return descriptions;
        }

        public IList<Parcel> GetAllParcels()
        {

            var parcels = ctx.Parcels
                             .Include(p => p.User)
                             .Include(p => p.FoodBank)
                             .Include(p => p.Client)
                             //get the parcelitems
                             .Include(p => p.Items)
                             //for each parcel item get the item
                             .ThenInclude(pi => pi.Item);
            
            return parcels.ToList();
        }

        public Parcel GetParcelById(int id)
        {
            return ctx.Parcels
                      .Include(p => p.Client)
                      .Include(p => p.FoodBank)
                      .Include(p => p.User)
                      .Include(p => p.Items)
                      .ThenInclude(pi => pi.Item)
                      .FirstOrDefault(x => x.Id == id);
        }

       
        public Client AddClient(string secondName, string postCode, string email, int noOfPeople, int foodBankId)
        {
            //check that the client does not exist already using email address
            var c = GetClientByEmailAddress(email);

            //var theQueue = new Queue<Client>();
            

            //if c is not null then the client already exists so return null
            if(c != null) 
            { 
                return null; 
            
            }

            var client = new Client
            {
                SecondName = secondName,
                PostCode = postCode,
                EmailAddress = email,
                NoOfPeople = noOfPeople,
                FoodBankId = foodBankId,

            };

            ctx.Add(client);

            //call the add client to queue method from below
            //AddClientToQueue(client);

            //save changes
            ctx.SaveChanges();

            //return the client
            return client;

        }
       
        public IList<Client> GetAllClients()
        {
            return ctx.Clients.ToList();

        }

        public Client GetClientById(int id)
        {
            return ctx.Clients
                      .Include(x => x.FoodBank)
                      .FirstOrDefault(x => x.Id == id);
        }

        public Client GetClientByEmailAddress(string email)
        {
            var c = ctx.Clients
                       .Include(x => x.FoodBank)
                       .FirstOrDefault(x => x.EmailAddress == email);

            return c;
        }

        public Client UpdateClient(Client updated)
        {
            //check that the updating client exists from a previous service method
            var client = GetClientById(updated.Id);
            
            if(client == null) { return null; }

            if (!IsClientEmailAvailable(updated.EmailAddress, updated.Id))
            {
                return null;
            }

            //update the details with all propeties
            client.SecondName = updated.SecondName;
            client.PostCode = updated.PostCode;
            client.EmailAddress = updated.EmailAddress;
            client.NoOfPeople = updated.NoOfPeople;
            client.FoodBankId = updated.FoodBankId;

            ctx.SaveChanges();
            return client;
        
        }

        public bool DeleteClient(int id)
        {
            //get the client by id to ensure they are already there
            var client = GetClientById(id);
            
            if(client == null)
            {
                return false;
            }

            ctx.Remove(client);
            ctx.SaveChanges();
            return true;

        }

        public IList<Client> SearchClients(string query)
        {
            //ensure that the query is not null
            query = query == null ? "" : query.ToLower();

            var results = ctx.Clients
                             .Include(x => x.FoodBank)
                             .Where (x => (x.SecondName.ToLower().Contains(query) ||
                                          x.FoodBank.StreetName.ToLower().Contains(query) ||
                                          x.PostCode.ToLower().Contains(query) ||
                                          x.NoOfPeople.ToString().Contains(query) ||
                                          x.EmailAddress.Contains(query) ||
                                          x.FoodBankId.ToString().Contains(query)
                                         )
                             ).ToList();
            return results;
        }

        public bool IsDuplicateClient(string email)
        {
            //get lists of all clients
            var clients = GetAllClients();

            //change to array
            var clientsArray = clients.ToArray();

            //for loop to check all of the clients e mail
            for(int i = 0; i<clientsArray.Length; i++)
            {
                //if the e mail address in any indexes of the array equals the e mail passed in then diplicate so return true
                if(clientsArray[i].EmailAddress == email)
                {
                    return true;
                }

            }

            return false;

        }

        public IList<Category> GetAllCategorys()
        {
            return ctx.Categorys.ToList();
        }

        public IList<String> GetAllCategoryDescriptions()
        {
            var categoryDescriptions = ctx.Categorys
                                         .Select(d => d.Description).ToList();
            
            return categoryDescriptions;
        }


        
        public bool CheckAllFoodBanksForStockItem(IList<FoodBank> f, string description)
        {
            //load all of the food banks in to memory using one of the above methods
            var foodbanks = GetFoodBanks();

            //load the stock item in to memoery using the method above
            var stockitem = GetStockByDescription(description);

            bool found = false;
            //use for loop to iterate through each food bank
            for (int i = 0; i < foodbanks.Count(); i++)
            {
                if(foodbanks[i].Stock.Contains(stockitem))
                {
                    found = true;
                }
            }
            return found;
        }


        public ParcelItem GetParcelItemById(int id)
        {
            return ctx.ParcelItems.FirstOrDefault(pi => pi.Id == id);
        }

        public IList<Stock> GetAvailableStockForParcel(int id)
        {
            var parcel = GetParcelById(id);
            var pi = parcel.Items.ToList();
            var stock = ctx.Stock.ToList();

            return stock.Where(s => pi.Any(x => x.StockId != s.Id)).ToList();
        }

    }
}