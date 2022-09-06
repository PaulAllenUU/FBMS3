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

        //-----Begin Food Bank Management Methods------??

        //Get all food banks that are in the database
        IList<FoodBank> GetFoodBanks();

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

        //Search food banks
        IList<FoodBank> SearchFoodBanks(string query);

        //-------End of Food Bank Management Methods ----//

        // ----Begin Stock Management Interface ----//
        IList <Stock> GetAllStock();

        //IList<Stock> SearchStock(StockRange range, string query);

        //search stock method based on input from the user
        IList<Stock> SearchStock(StockRange range, string query);

        //get stock by id - id passed in as parameter from the user
        Stock GetStockById(int id);

        //get stock by descrption passed in from the user
        Stock GetStockByDescription(string description);

        //add stock using all properties
        Stock AddStock(int foodBankId, string description, int quantity, DateTime expiryDate, int categoryId); 

        //update existing stock
        Stock UpdateStock(Stock updated);

        //delete an item of stock using id passed in as a parameter
        bool DeleteStockById(int id);

        //-----------End of Stock Management Methods ------//

        //-----Begin Stock Category Management Methods-----//
        Category AddCategory(string description);

        Category GetCategoryByDescription(string description);

        //-------Begin client management methods----//
        
        //client added to the database by the food banks staff or volunteers at the food bank site
        Client AddClient(string secondName, string postCode, string email, int noOfPeople, int foodBankId);

        IList<Client> GetAllClients();

        IList<Client> SearchClients (string query);

        Client GetClientById(int id);

        Client GetClientByEmailAddress(string email);

        Client UpdateClient(Client updated);

        bool DeleteClient(int id);

        bool IsDuplicateClient(string email);

        //--------Begin Parcel Management Methods---------///
        Parcel AddParcel(int clientId, int userId, int foodbankId);

        ParcelItem AddItemToParcel(int parcelId, int stockId, int categoryId, int quantity);

     
        IList<ParcelItem> PopulateParcel(int parcelId, int stockId, int categoryId, int quantity);

        ParcelItem UpdateParcelItemQuantity(int parcelId, int stockId, int categoryId, int quantity);

        ParcelItem GetParcelItemById(int id);

        IList<Stock> GetAvailableStockForParcel(int id);
    

        //-------Begin Category Management 
       IList<Category> GetAllCategorys();

       //----------Begin Parcel Management Methodds------//
       IList<Parcel> GetAllParcels();

       Parcel GetParcelById(int id);

        bool CheckAllFoodBanksForStockItem(IList<FoodBank> foodbanks, string description);

    }
    
}
