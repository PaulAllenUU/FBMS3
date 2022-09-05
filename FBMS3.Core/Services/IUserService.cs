using FBMS3.Core.Models;
using System.Collections;

namespace FBMS3.Core.Services
{
    // This interface describes the operations that a UserService class implementation should provide
    public interface IUserService
    {
        // Initialise the repository - only to be used during development 
        void Initialise();

        // ---------------- User Management --------------

        //get list of all of the users currently on record in the database
        IList<User> GetUsers();

        //get a user by their id
        User GetUser(int id);

        //get user by their e mail address
        User GetUserByEmail(string email);

        User GetUserByFirstName(string firstName);

        //check if a users e mail address is available
        bool IsEmailAvailable(string email, int userId);

        bool IsClientEmailAvailable(string email, int userId);

        //add a user to the database
        User AddUser(string firstName, string secondName, string email, string password, int foodBankId, Role role);

        //update an existing user on the database
        User UpdateUser(User user);

        User EditUser(User user);

        //delete a user from the database
        bool DeleteUser(int id);

        //authenticate a user
        User Authenticate(string email, string password);

        IList<User> SearchUsers(string query);

        // ---------- End User Management Interface ------//

        //------- Queue Management Method---------//
        void AddClientToQueue(Client client);

        //-----Begin Food Bank Management Methods------??

        //Get all food banks that are in the database
        IList<FoodBank> GetFoodBanks();

        //Get list of food banks on certain street name
        IList<FoodBank> GetFoodBanksWithSameStreetName(string streetName);

        FoodBank GetFoodBankByStreetName(string streetName);

        //add food bank
        FoodBank AddFoodBank(int streetNumber, string streetName, string postCode);

        //update food bank
        FoodBank UpdateFoodBank(FoodBank foodbank);

        //delete food bank
        bool DeleteFoodBank(int id);

        bool IsDuplicateLocation(int id, int streetNumber, string postCode);

        //Get FoodBank By Id
        FoodBank GetFoodBankById(int id);

        FoodBank GetFoodBankByStreetNoAndPostCode(int streetNumber, string postCode);

        //Get FoodBank By StreetName & Number
        FoodBank GetFoodBankByStreetNameAndNumber(string streetName, int streetNumber);

        //Get FoodBank by PostCode
        FoodBank GetFoodBankByPostCode(string postCode);

        //Search food banks
        IList<FoodBank> SearchFoodBanks(string query);

        //-------End of Food Bank Management Methods ----//

        // ----Begin Stock Management Interface ----//
        IList <Stock> GetAllStock();

        //IList<Stock> SearchStock(StockRange range, string query);

        //Return stock by specific bool values - non-food, meat, vegetables, carbohydrates
        IList<Stock> GetAllNonFoodItems(bool nonFood);

        IList<Stock> GetAllMeatFoodItems(bool meat);

        IList<Stock> GetAllVegetablesFromStock (bool vegetable);

        IList<Stock> GetAllCarbohydratesFromStock (bool carbs);

        //search stock method based on input from the user
        IList<Stock> SearchStock(StockRange range, string query);

        //get stock by id - id passed in as parameter from the user
        Stock GetStockById(int id);

        IList<Stock> GetStockAtFoodBank(int foodBankId);

        //get stock available at foodbank
        IList<Stock> GetAvailableStockAtFoodBank(int id);

        //get stock by descrption passed in from the user
        Stock GetStockByDescription(string description);

        //get stock by expiry date
        Stock GetStockByExpiryDate(DateTime expiryDate);

        //add stock using all properties
        Stock AddStock(int foodBankId, string description, int quantity, DateTime expiryDate, int categoryId); 

        //update existing stock
        Stock UpdateStock(Stock updated);

        //delete an item of stock using id passed in as a parameter
        bool DeleteStockById(int id);

        //-----------End of Stock Management Methods ------//

        //-----Begin Stock Category Management Methods-----//
        Category AddCategory(string description);

        Category GetCategoryById(int id);

        Category GetCategoryByDescription(string description);

        //-------Begin client management methods----//
        
        //client added to the database by the food banks staff or volunteers at the food bank site
        Client AddClient(string secondName, string postCode, string email, int noOfPeople, int foodBankId);

        //method that will remove someone from the queue
        void RemoveClientFromTheQueue();

        IList<Client> GetAllClients();

        IList<Client> SearchClients (string query);

        Client GetClientById(int id);

        Client GetClientByEmailAddress(string email);

        Client UpdateClient(Client updated);

        bool DeleteClient(int id);

        bool IsDuplicateClient(string email);

        //--------Begin Parcel Management Methods---------///
        Parcel AddParcel(int clientId, int userId, int foodbankId);

        ParcelItem AddItemToParcel(int parcelId, int stockId, int quantity);

        ParcelItem GetParcelItemById(int id);

        IList<Stock> GetAvailableStockForParcel(int id);
    

        //-------Begin Category Management 
       IList<Category> GetAllCategorys();

       //----------Begin Parcel Management Methodds------//
       //Parcel GenerateParcelForClient(int userId, int clientId, int foodBankId);

       IList<Parcel> GetAllParcels();

       Parcel GetParcelById(int id);

        //--------Begin Recipe Management Methods------//

        //add recipe to the database
        /*Recipe AddRecipe(string title, int noOfIngredients, int cookingTime);

        //delete recipe
        bool DeleteRecipe(int id);

        //isrecipevegetarian - accepts a list of stock as parameter
        bool IsRecipeVegetarian(RecipeStock recipeStock);

        //check if recipe already exists when adding
        bool IsDuplicateRecipe(string description);

        //get recipe by id
        Recipe GetRecipeById(int id);

        //update recipe
        Recipe UpdateRecipe(Recipe updated);

        //get recipe by title
        Recipe GetRecipeByTitle(string title);

        //get list of all of the recipes in the database
        IList<Recipe> GetAllRecipes();

        //add ingredient to recipe
        RecipeStock AddStockItemToRecipe(int stockId, int recipeId, int stockItemQuantity);

        //get recipeingredient by Id
        //RecipeStock GetRecipeStockById(int id);

        //remove item from recipe
        bool RemoveStockItemFromRecipe(int stockId, int recipeId);

        //get available recipes for ingredient
        IList<Recipe> GetAvailableRecipesForStockItem(int id);

        //update recipeingredient stock item quantity
        RecipeStock UpdateStockItemQuantity(int stockId, int recipeId, int stockItemQuantity);*/

        bool checkFoodBankForStockItem(int FoodBankId, string description);

        bool CheckAllFoodBanksForStockItem(IList<FoodBank> foodbanks, string description);

        bool checkFoodBankForListOfStockItems(int FoodBankId, IList<Stock> s);

        bool checkAllFoodBanksForListOfStockItems(IList<FoodBank> f, IList<Stock> s);

        //-------End of Recipe Management Interface ------//

    }
    
}
