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

        public Queue<Client> TheQueue = new Queue<Client>(); 

        //public Queue<Client> TheQueue = new Queue<Client>();

        /*public UserServiceDb(Queue<Client> theQueue)
        {
            TheQueue = theQueue;
        }*/

        // ------------------ User Related Operations ------------------------

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
                    //include the stock and clients that are associated with that food bank
                     .Include(x => x.Stock)
                     .Include(x => x.Clients)
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

        public Stock AddStock(int foodBankId, string description, int quantity, DateTime expiryDate, int stockCategoryId)
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
                StockCategoryId = stockCategoryId,
            };

            if(DetermineEnumerationType(s.StockCategoryDescription, "Meat"))
            {
                s.Meat = true;
            }
                
            if(DetermineEnumerationType(s.StockCategoryDescription, "Vegetable"))
            {
                s.Vegetable = true;
            } 
            
            if(DetermineEnumerationType(s.StockCategoryDescription, "Carbohydrates"))
            {
                s.Carbohydrate = true;
            }
            IList<String> nonFoodList = new IList<>


            if(DetermineEnumerationTypeWithArray(s.StockCategoryDescription, {"Razor, Toileteries, "}))

            if(DetermineEnumerationType(s.StockCategoryDescription, "Razor", "")
            {
                s.NonFood = true;
            }
            else if(DetermineEnumerationType(s.StockCategoryDescription, "Toileteries"))
            {
                s.NonFood = true;
            }
                else if(DetermineEnumerationType(s.StockCategoryDescription, "Pet Food" || "Toileteries"))
                {
                    s.NonFood = true;
                }
                    else if(DetermineEnumerationType(s.StockCategoryDescription, "Kitchen Cleaning"))
                    {
                        s.NonFood = true;
                    }
                        else if(DetermineEnumerationType(s.StockCategoryDescription, "Logs"))
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
                                          //for the enumeration when the value of the boolean values are set they will come up
                                          (range == StockRange.NONFOOD && x.NonFood== true ||
                                           range == StockRange.MEAT && x.Meat == true ||
                                           range == StockRange.VEGETABLE && x.Vegetable == true ||
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

        //---------Stock Categiry Methods-----//
        public StockCategory AddStockCategory(string description)
        {
            var exists = GetStockCategoryByDescription(description);
            
            //if the category is not null, ie already exists then return null
            if(exists != null)
            {
                return null;
            }

            var scat = new StockCategory
            {
                Description = description
            };

            return scat;

        }

        public StockCategory GetStockCategoryById(int id)
        {
            return ctx.StockCategorys
                      .Include(x => x.Stock)
                      .FirstOrDefault(x => x.Id == id);
        }

        public StockCategory GetStockCategoryByDescription(string description)
        {
            return ctx.StockCategorys
                      .Include(x => x.Stock)
                      .FirstOrDefault(x => x.Description == description);
        }

        //add a recipe checking that is doesnt already exist using title
        public Recipe AddRecipe(string title, int noOfIngredients, int cookingTime)
        {
            var existing = GetRecipeByTitle(title);

            //if it does not null, already exists so return null
            if(existing != null)
            {
                return null;
            }

            var recipe = new Recipe
            {
                Title = title,
                NoOfIngredients = noOfIngredients,
                CookingTimeMins = cookingTime,
            };

            //return the recipe, add to database and save changes
            ctx.Add(recipe);
            ctx.SaveChanges();
            return recipe;
        }

        public bool DeleteRecipe(int id)
        {
            //check the recipe exists by loading it into memory using the service method previously created
            var recipe = GetRecipeById(id);

            //if it is null then it does not exist so cannot delete - therefore return false
            if (recipe == null)
            {
                return false;
            }

            ctx.Remove(recipe);
            ctx.SaveChanges();
            return true;
        }

        public bool IsRecipeVegetarian(RecipeStock recipeStock)
        {
            //check the recipe exists through the get recipe by title method
            var exists = GetRecipeStockById(recipeStock.Id);

            //if the recipe is null then cannot check so return false
            if(exists == null) { return false; };

            //use the list of possible meatTypes from previous method
            List<String> MeatTypes = new List<String>();
            MeatTypes.Add("Chicken");
            MeatTypes.Add("Pork");
            MeatTypes.Add("Beef");
            MeatTypes.Add("Fish");

            //if stock recipe contains any items with the description above then return false
            if (MeatTypes.Contains(exists.Stock.Description))
            {
                return false;
            }

            return true;
        }

        public Recipe GetRecipeById(int id)
        {
            return ctx.Recipes.FirstOrDefault( x => x.Id == id);
        }

        public Recipe UpdateRecipe(Recipe updated)
        {
            //check that recipe exists using the get recipe by id method
            var recipe = GetRecipeById(updated.Id);

            //if it is null, it does not exist therefore return null
            if(recipe == null)
            {
                return null;
            }

            //otherwise update all properties and then save changes
            recipe.Title = updated.Title;
            recipe.NoOfIngredients = updated.NoOfIngredients;
            recipe.CookingTimeMins = updated.CookingTimeMins;

            //save changes and return the recipe
            ctx.SaveChanges();
            return recipe;
        }

        public Recipe GetRecipeByTitle(string title)
        {
            return ctx.Recipes
                        //include the recipe stock associated with that recipe
                        //avoids eager loading
                      .Include( x => x.RecipeStock)
                      .FirstOrDefault( x => x.Title == title);
        }

        public IList<Recipe> GetAllRecipes()
        {
            //return all recipes but include recipestock
            return ctx.Recipes.ToList();
        }

        public RecipeStock AddStockItemToRecipe(int stockId, int recipeId, int stockItemQuantity)
        {
            //check that the recipe stock already exists and return null if found
            var rs = ctx.RecipeStock
                        .FirstOrDefault( x => x.StockId == stockId &&
                                              x.RecipeId == recipeId);

            //if these are not null then stock iteam already in recipe - return null
            if (rs != null) { return null; }

            //locate the stock item and the recipe
            var s = ctx.Stock.FirstOrDefault(s => s.Id == stockId);
            var r = ctx.Recipes.FirstOrDefault(r => r.Id == recipeId);

            //if either do not exist then cannot add so return null
            if(s == null || s == null) { return null; }

            //otherwise create the recipestock and add to the database
            var nrs = new RecipeStock { StockId = s.Id, RecipeId = r.Id, StockItemQuantity = stockItemQuantity };

            //save changes and return the recipe stock
            ctx.RecipeStock.Add(nrs);
            ctx.SaveChanges();
            return nrs;
        }

        public RecipeStock GetRecipeStockById(int id)
        {
            return ctx.RecipeStock.FirstOrDefault(x => x.Id == id);
        }

        public bool RemoveStockItemFromRecipe(int stockId, int recipeId)
        {
            //check that the stock item is already in the recipe before it can be removed
            var rs = ctx.RecipeStock.FirstOrDefault(
                s => s.StockId == stockId && s.RecipeId == recipeId
            );

            //if it is not already there then cannot remove to return false
            if (rs == null) { return false; }

            //remove the stock item from the recipe
            ctx.RecipeStock.Remove(rs);
            ctx.SaveChanges();
            return true;
        }

        public IList<Recipe> GetAvailableRecipesForStockItem(int id)
        {
            var stockitem = GetStockById(id);
            var rs = stockitem.RecipeStock.ToList();
            var recipes = ctx.Recipes.ToList();

            return recipes.Where(r => rs.Any( x => x.RecipeId != r.Id)).ToList();
        }

        public RecipeStock UpdateStockItemQuantity(int stockId, int recipeId, int stockItemQuantity)
        {
            var stockitem = GetStockById(stockId);

            //check that the stock item exists and if not, return null
            if(stockitem == null)
            {
                return null;
            }

            //check the recipe stock id exists
            var rs = stockitem.RecipeStock.FirstOrDefault(o => o.StockId == stockId);

            if(rs == null)
            {
                return null;
            }

            //update the stock item quantity
            rs.StockItemQuantity = stockItemQuantity;

            ctx.SaveChanges();
            return rs;
        }

        //method to determine if recipe already exists with the same title
        public bool IsDuplicateRecipe(string title)
        {
            //get all of the existing recipes using the service method
            var recipes = GetAllRecipes();

            //convert list to an array so it can be iterated through using for loop
            var recipesArray = recipes.ToArray();

            for(int i=0; i<recipesArray.Length; i++)
            {
                //check if the array index [i] equals the title passed in as parameter, if so it is a duplicate
                if(recipesArray[i].Title == title)
                {
                    return true;
                }
                
            }

            //if none of the elements in the array contain the same title then return false;
            return false;
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
            AddClientToQueue(client);

            //save changes
            ctx.SaveChanges();

            //return the client
            return client;

        }
       
        public Queue<Client> GetAllClients()
        {
            //get the clients from the DbSet
            var clients = ctx.Clients;

            //for each loop to add every client in to the queue
            foreach (var c in clients)
            {
                TheQueue.Enqueue(c);
            }

            //return the queue
            return TheQueue;
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

            //update the details with all propeties
            client.SecondName = updated.SecondName;
            client.PostCode = updated.PostCode;
            client.EmailAddress = updated.EmailAddress;
            client.NoOfPeople = updated.NoOfPeople;
            client.FoodBankId = updated.FoodBankId;

            ctx.SaveChanges();
            return client;
        
        }

        public bool DeleteClient(string email)
        {
            //get the client by id to ensure they are already there
            var client = GetClientByEmailAddress(email);
            
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

        /*public Parcel GenerateParcelForClient(Queue<Client> clients, int FoodBankId)
        {
            int QueueSize = clients.Count();

            for(int i=0; i<QueueSize; i++)
            {
                
            }

            
            
        }*/

        /*public bool checkFoodBankForStockItem(FoodBank foodbank, string stockitem)
        {
            foreach (var s in foodbank)
            {
                
            }
        }*/

        //using the built in methods to dequeue a client from the queue
        public void RemoveClientFromTheQueue()
        {
            //dequeue the client at the head of the queue and then save changes
            TheQueue.Dequeue();

            ctx.SaveChanges();
        }

        //using the built in DeQueue method we can add clients to the queue
        public void AddClientToQueue(Client client)
        {
            //pass in the client to be added to the queue
            TheQueue.Enqueue(client);

            //save changes
            ctx.SaveChanges();
        }

        /*public Parcel GenerateParcelFromStock(int userId, DateTime date, string item, int clientId, int foodBankId, int noOfPeople)
        {
            var Parcel = new Parcel
            {
                UserId = userId,
                Date = DateTime.Now,
                Item = item,
                ClientId = clientId,
                FoodBankId = foodBankId,
                NoOfPeople = noOfPeople,
            };
            
        }*/

        //method to check a specific food bank for specific item of stock
        public bool checkFoodBankForStockItem(int FoodBankId, string description)
        {
            //call the food bank by id into memory
            var foodbank = GetFoodBankById(FoodBankId);

            //call the stock item by description in to memory
            var stockitem = GetStockByDescription(description);

            //if either of the above are null then return false
            if(foodbank == null || stockitem == null)
            {
                return false;
            }

            //if the foodbank contains the stock item return true if 
            if(foodbank.Stock.Contains(stockitem))
            {
                return true;
            }

            return false;

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

        public bool checkFoodBankForListOfStockItems(int FoodBankId, IList<Stock> s)
        {
            var fb = GetFoodBankById(FoodBankId);
            var items = GetAllStock();
            bool found = false;

            //for loop to through all of the items
            for(int i =0 ; i < items.Count(); i++)
            {
                if(fb.Stock.Contains(items[i]))
                {
                    found = true;
                }
                found = false;
            }

            return found;
        }

        bool IUserService.checkAllFoodBanksForListOfStockItems(IList<FoodBank> f, IList<Stock> s)
        {
            throw new NotImplementedException();
        }

        public bool DetermineEnumerationType(string actual, string categorydescription)
        {
            bool found = false;
            if(actual.Contains(categorydescription))
            {
                found = true;
            }
            
            return found;
        }

        public bool DetermineEnumerationTypeWithArray(string actual, List <String> nonFoodCategories)
        {
            nonFoodCategories.ToArray();
            bool found = false;

            //for loop for traversing the array
            for (int i = 0; i < nonFoodCategories.Count ; i++)
            {
                if(actual == nonFoodCategories[i])
                {
                    found = true;
                }
                
            }

            return found;
        }



        /*public Parcel GenerateParcelFromStock(int userId, DateTime date, string item, int quantity, 
                                            string itemSize, int clientId, int foodBankId, int noOfPeople)
        {
            var foodBank = GetFoodBankById(foodBankId);

            var stock = 
            
        }*/

        /*public bool CheckFoodBanksForRecipeItem(int foodBankId, int stockId)
        {
            //check both the foodBank and and the stock item passed in both exist
            //if either of them returns not null then cannot find so return false
            var f = GetFoodBankById(foodBankId);
            var s = GetStockById(stockId);

            //if either of the above is null then return false
            if(f == null || s == null) { return false; }

            //otherwise check the recipestock stock id for the 
            



        }*/
    }
}