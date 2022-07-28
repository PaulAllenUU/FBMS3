
using FBMS3.Core.Models;

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

        //add a user to the database
        User AddUser(string firstName, string secondName, string email, string password, string FoodBankStreetName, Role role);

        //update an existing user on the database
        User UpdateUser(User user);

        //delete a user from the database
        bool DeleteUser(int id);

        //authenticate a user
        User Authenticate(string email, string password);

        // ---------- End User Management Interface ------//

        //-----Begin Food Bank Management Methods------??

        //Get all food banks that are in the database
        IList<FoodBank> GetFoodBanks();

        //Get list of food banks on certain street name
        IList<FoodBank> GetFoodBanksWithSameStreetName(string streetName);

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

        //-------End of Food Bank Management Methods ----//

        // ----Begin Stock Management Interface ----//
        IList <Stock> GetAllStock();

        //get stock by id - id passed in as parameter from the user
        Stock GetStockById(int id);

        //get stock by descrption passed in from the user
        Stock GetStockByDescription(string description);

        //get stock by expiry date
        Stock GetStockByExpiryDate(DateTime expiryDate);

        //add stock using all properties
        Stock AddStock(int foodBankId, string description, int quantity, DateTime expiryDate); 

        //update existing stock
        Stock UpdateStock(Stock updated);

        //delete an item of stock using id passed in as a parameter
        bool DeleteStockById(int id);

        //-----------End of Stock Management Methods ------//
       
    }
    
}
