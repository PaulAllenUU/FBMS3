
using Xunit;
using FBMS3.Core.Models;
using FBMS3.Core.Services;
using System;

using FBMS3.Data.Services;

namespace FBMS3.Test
{
    public class ServiceTests
    {
        private IUserService service;

        public ServiceTests()
        {
            service = new UserServiceDb();
            service.Initialise();
        }

        //Begin - service tests for user methods

        [Fact]
        public void EmptyDbShouldReturnNoUsers()
        {
            // act
            var users = service.GetUsers();

            // assert
            Assert.Equal(0, users.Count);
        }
        
        [Fact]
        public void AddingUsersShouldWork()
        {
            var foodbank = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            // arrange
            service.AddUser("admin", "scholefield", "admin@mail.com", "admin", foodbank.Id, Role.admin );
            service.AddUser("guest", "phillips", "guest@mail.com", "guest", foodbank.Id, Role.staff);

            // act
            var users = service.GetUsers();

            // assert
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public void UpdatingUserShouldWork()
        {
            // arrange
            var foodbank = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var user = service.AddUser("admin", "o'neill", "admin@mail.com", "admin", foodbank.Id, Role.admin );
            
            // act
            user.FirstName = "administrator";
            user.SecondName = "o'neill";
            user.Email = "admin@mail.com";          
            user.FoodBankId = foodbank.Id;  
            var updatedUser = service.UpdateUser(user);

            // assert
            Assert.Equal("administrator", user.FirstName);
            Assert.Equal("admin@mail.com", user.Email);
        }

        [Fact]
        public void LoginWithValidCredentialsShouldWork()
        {
            // arrange
            var foodbank = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            service.AddUser("admin", "o'neill", "admin@mail.com", "admin", foodbank.Id, Role.admin );
            
            // act            
            var user = service.Authenticate("admin@mail.com","admin");

            // assert
            Assert.NotNull(user);
           
        }

        [Fact]
        public void LoginWithInvalidCredentialsShouldNotWork()
        {
            // arrange
            var foodbank = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            service.AddUser("admin", "o'neill", "admin@mail.com", "admin", foodbank.Id, Role.admin );

            // act      
            var user = service.Authenticate("admin@mail.com","xxx");

            // assert
            Assert.Null(user);
           
        }

        [Fact]
        public void GetUsers_WhenThreeExist_ShouldReturnThree()
        {
            //arrange
            var f1 = service.AddFoodBank(10, "Antrim Road", "BT49 0ST");
            var f2 = service.AddFoodBank(50, "Scroggy Road", "BT56 9QP");
            var f3 = service.AddFoodBank(100, "Springfield Road", "BT12 9PF");

            var u1 = service.AddUser("Paul", "Allen", "admin@mail.com", "admin", f1.Id, Role.admin);
            var u2 = service.AddUser("Manager", "Farwell", "manager@mail.com", "manager", f2.Id, Role.manager);
            var u3 = service.AddUser("Guest", "O'Hara", "guest@mail.com", "guest", f2.Id, Role.staff); 

            //act
            var users = service.GetUsers();
            var count = users.Count;

            //assert
            Assert.NotNull(u1);
            Assert.NotNull(u2);
            Assert.NotNull(u3);
            
            Assert.Equal(3, count);
        }

        [Fact]
        public void DeleteUserShouldWork()
        {
            //arrange
             var f1 = service.AddFoodBank(10, "Antrim Road", "BT49 0ST");
             var u1 = service.AddUser("Paul", "Allen", "admin@mail.com", "admin", f1.Id, Role.admin);

             //
             var delete = service.DeleteUser(u1.Id);

             //act
             var users = service.GetUsers();

             //assert - list will show but should be empty
             Assert.Empty(users);

        }

        [Fact]
        public void DeleteUser_WhenDoesntExist_ShouldReturnFalse()
        {
            //arrange
            var f1 = service.AddFoodBank(10, "Antrim Road", "BT49 0ST");
            var u1 = service.AddUser("Paul", "Allen", "admin@mail.com", "admin", f1.Id, Role.admin);

            //act
            var delete = service.DeleteUser(2);

            //assert - var delete should be null
            Assert.False(delete);

        }

        [Fact]
        public void GetUser_WhenNone_ShouldReturnEmptyList()
        {
            //aarange & act
            var users = service.GetUsers();

            //asset - list will be returned but will be empty
            Assert.Empty(users);
        }

        [Fact]
        public void GetUserByEmail_WhenExists_ShouldWork()
        {
            //arrange
            var f1 = service.AddFoodBank(10, "Antrim Road", "BT49 0ST");
            var u1 = service.AddUser("Paul", "Allen", "admin@mail.com", "admin", f1.Id, Role.admin);

            //act
            var get = service.GetUserByEmail("admin@mail.com");

            //assert
            Assert.Contains(u1.Email, get.Email);

        }

        [Fact]
        public void AddUser_WithSameEmail_ShouldReturnNull()
        {
            //arrange
            var f1 = service.AddFoodBank(10, "Antrim Road", "BT49 0ST");
            var f2 = service.AddFoodBank(50, "Scroggy Road", "BT56 9QP");
            var f3 = service.AddFoodBank(100, "Springfield Road", "BT12 9PF");

            var u1 = service.AddUser("Paul", "Allen", "admin@mail.com", "admin", f1.Id, Role.admin);
            var u2 = service.AddUser("Manager", "Farwell", "admin@mail.com", "manager", f2.Id, Role.manager);

            Assert.NotNull(u1);
            //u2 should be null because duplicate e mail address is not allowed
            Assert.Null(u2);
        }

        [Fact]
        public void IsEmailAvailable_WhenNot_ShouldReturnFalse()
        {
            //arrange
            var f1 = service.AddFoodBank(10, "Antrim Road", "BT49 0ST");
            var f2 = service.AddFoodBank(50, "Scroggy Road", "BT56 9QP");
            var f3 = service.AddFoodBank(100, "Springfield Road", "BT12 9PF");

            var u1 = service.AddUser("Paul", "Allen", "admin@mail.com", "admin", f1.Id, Role.admin);

            var emailcheck =  service.IsEmailAvailable("admin@mail.com", 1);

            //should retrn true
            Assert.True(emailcheck);
        }

        //End of User Management Unit Tests

        //Begin Food Bank Management Unit Tests
        [Fact]
        public void FoodBank_AddingFoodBankShouldWork()
        {
            //arrange
            var foodbank = service.AddFoodBank(28, "Thorndale", "BT49 0ST");

            //assert
            Assert.NotNull(foodbank);
            
        }

        [Fact]
        public void GetFoodBankList_When2Exist_ShouldReturn2()
        {
            //arrange
            var f1 = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var f2 = service.AddFoodBank(36, "Taylor Park", "BT48 7KL");

            //act
            var both = service.GetFoodBanks();
            var count = both.Count;

            //assert
            Assert.Equal(2, count);

        }

        [Fact]
        public void GetFoodBankById_WhenExists_ShouldReturnFoodBank()
        {
            //arrange
            var foodbank = service.AddFoodBank(28, "Thorndale", "BT49 0ST");

            //act
            var get = service.GetFoodBankById(foodbank.Id);
            var count = service.GetFoodBanks().Count;

            //assert
            Assert.NotNull(get);
            Assert.Equal(1, count);
        }

        [Fact]
        public void GetFoodBankById_ShouldAlsoIncludeStockAndClients()
        {
            //arrange
            var foodbank = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var cat1 = service.AddCategory("Vegetables");
            var cat2 = service.AddCategory("Meat");
            
            var stock = service.AddStock(foodbank.Id, "Chicken", 3, new DateTime(2023, 04, 01), cat1.Id);
            var client  = service.AddClient("Allen", "BT49 0ST", "admin@mail.com", 4, foodbank.Id);

            //assert
            Assert.Contains(stock, foodbank.Stock);
            Assert.Contains(client, foodbank.Clients);

        }

        [Fact]
        public void FoodBank_AddingFoodBank_WithSameAddress_ShouldReturnNull()
        {
            var foodbank = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var foodbank2 = service.AddFoodBank(28, "Thorndale", "BT49 0ST");

            //assert
            Assert.NotNull(foodbank);
            Assert.Null(foodbank2);

        }

        [Fact]
        public void FoodBank_AddFoodBank_WhenNone_ShouldSetAllProperties()
        {
            //arrange
            var added = service.AddFoodBank(28, "Thorndale", "BT49 0ST");

            var f = service.GetFoodBankById(added.Id);

            //assert that the food bank is not null
            Assert.NotNull(f);

            //assert that each property was set properly
            Assert.Equal(f.Id, f.Id);
            Assert.Equal(28, f.StreetNumber);
            Assert.Equal("Thorndale", f.StreetName);
            Assert.Equal("BT49 0ST", f.PostCode);
        }

        [Fact]
        public void FoodBank_UpdateFoodBankThatExist_ShouldUpdateAllProperties()
        {
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");

            var get = service.GetFoodBankById(1);

            //update the student
            var u = service.UpdateFoodBank(f);
            {
                f.StreetNumber = 30;
                f.StreetName = "Meadowvale";
                f.PostCode = "BT41 2FL";
            };

            //assert the the properties have now changed after the update method
            Assert.Equal(30, f.StreetNumber);
            Assert.Equal("Meadowvale", f.StreetName);
            Assert.Equal("BT41 2FL", f.PostCode);
        }

        [Fact]
        public void FoodBank_GetAllFoodBanks_WhenNone_ShouldReturn0()
        {
            //act
            var foodbanks = service.GetFoodBanks();
            var count = foodbanks.Count;

            //assert
            Assert.Equal(0, count);
        }

        [Fact]
        public void FoodBank_GetAllFoodBanks_WhenOne_ShouldReturn1()
        {
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var list = service.GetFoodBanks();

            //count
            var count = list.Count;

            Assert.Equal(1, count);
        }

        [Fact]
        public void FoodBank_GetFoodBankById_WhenNull_ShouldReturnNull()
        {
            //act
            var f = service.GetFoodBankById(1); //non existent food bank

            //assert
            Assert.Null(f);
        }

        [Fact]
        public void FoodBank_DeleteFoodBank_WhenDoesntExist_ShouldReturnFalse()
        {
            var f = service.DeleteFoodBank(1);

            Assert.False(f);
        }

        [Fact]
        public void FoodBank_RemoveFoodBankThatExists_ShouldReturnTrue()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            
            //act
            var remove = service.DeleteFoodBank(1);

            //assert
            Assert.True(remove);
            
        }

        [Fact]
        public void CheckAllFoodBanksForStockItem_WhenHas_ShouldReturnTrue()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var f2 = service.AddFoodBank(36, "Meadowvale", "bt61 7LK");
            var list = service.GetFoodBanks();
            var category = service.AddCategory("Fruit");
            var s = service.AddStock(f.Id, "Apple", 4, new DateTime(2023, 04, 01), category.Id);

            //act
            var check = service.CheckAllFoodBanksForStockItem(list, "Apple");

            Assert.True(check);

        }

        [Fact]
        public void CheckAllFoodBanksForStockItem_WhenDoesntHave_ShouldReturnFalse()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var f2 = service.AddFoodBank(36, "Meadowvale", "bt61 7LK");
            var list = service.GetFoodBanks();
            var category = service.AddCategory("Fruit");
            var s = service.AddStock(f.Id, "Apple", 4, new DateTime(2023, 04, 01), category.Id);

            //act
            var check = service.CheckAllFoodBanksForStockItem(list, "Orange");

            Assert.False(check);

        }

        [Fact]
        public void SearchFoodBank_WhenSearchStringExists_ShouldReturnResult()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var f2 = service.AddFoodBank(36, "Meadowvale", "bt61 7LK");

            //act
            var search = service.SearchFoodBanks("Thorndale");

            //assert
            Assert.Contains(f, search);

        }

         [Fact]
        public void SearchFood_WhenSearchStringDoesntExist_ShouldReturnEmpty()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var f2 = service.AddFoodBank(36, "Meadowvale", "bt61 7LK");

            //act
            var search = service.SearchFoodBanks("Taylor Park");

            //assert
            Assert.Empty(search);

        }
        //End of Food Bank Service Management Tests

        //Begin Stock Management Service Tests
        [Fact]
        public void Stock_GetAllStockWhenNone_ShouldReturnEmptyList()
        {
            //arrange
            var list = service.GetAllStock();

            //assert
            Assert.Empty(list);

        }

        [Fact]
        public void Stock_GetAllStock_WhenOneExists_ShouldReturnOne()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var category = service.AddCategory("Fruit");
            var s = service.AddStock(f.Id, "Apple", 4, new DateTime(2023, 04, 01), category.Id);

            //act
            var get = service.GetAllStock();
            var count = get.Count;

            //assert
            Assert.Equal(1, count);
        }

        [Fact]
        public void GetAllStock_WhenThreeExist_ShouldReturnThree()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var category = service.AddCategory("Fruit");
            var s1 = service.AddStock(f.Id, "Apple", 4, new DateTime(2023, 04, 01), category.Id);
            var s2 = service.AddStock(f.Id, "Banana", 10, new DateTime(2023,10,09), category.Id);
            var s3 = service.AddStock(f.Id, "Orange", 6, new DateTime(2023, 04, 05), category.Id);

            //act
            var get = service.GetAllStock();
            var count = get.Count;

            //assert
            Assert.Equal(3, count);

        }

        [Fact]
        public void AddNonFoodItem_ShouldFilterBoolean()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var category = service.AddCategory("NonFood");

            //act
            var s1 = service.AddStock(f.Id, "Toileteries", 3, new DateTime(2023,01,01), category.Id);

            //assert
            Assert.True(s1.NonFood == true);

        }

        [Fact]
        public void GetStockById_WhenExists_ShouldWork()
        {
            //arrange
            var c5 = service.AddCategory("Vegetables");
            var c6 = service.AddCategory("Meat");
            var foodbank = service.AddFoodBank(10, "Antrim Road", "BT49 0ST");
            var s4 = service.AddStock(foodbank.Id, "Meat", 6, new DateTime(2023, 08, 09), c6.Id);

            //act
            var get = service.GetStockById(s4.Id);

            //assert
            Assert.NotNull(get);
        }

        [Fact]
        public void GetStockById_WhenDoesntExist_ShouldReturnNull()
        {
            //arrange
            var c5 = service.AddCategory("Vegetables");
            var c6 = service.AddCategory("Meat");
            var foodbank = service.AddFoodBank(10, "Antrim Road", "BT49 0ST");

            //act
            var get = service.GetStockById(1);

            //assert
            Assert.Null(get);

        }

        [Fact]
        public void AddStock_WhenFoodBankExists_ShouldWork()
        {
            //arrange
            var c5 = service.AddCategory("Vegetables");
            var c6 = service.AddCategory("Meat");
            var foodbank = service.AddFoodBank(10, "Antrim Road", "BT49 0ST");
            var s4 = service.AddStock(foodbank.Id, "Meat", 6, new DateTime(2023, 08, 09), c6.Id);

            //act
            Assert.Contains(s4, foodbank.Stock);

        }

        [Fact]
        public void AddStock_WhenFoodBankDoesntExist_ShouldReturnNull()
        {
            //arrange
            var c5 = service.AddCategory("Vegetables");
            var c6 = service.AddCategory("Meat");

            //act
            var s4 = service.AddStock(1, "Meat", 6, new DateTime(2023, 08, 09), c6.Id);

            //assert
            Assert.Null(s4);

        }

        [Fact]
        public void UpdateStock_WhenExists_ShouldSetProperties()
        {
            //arrange
            var c5 = service.AddCategory("Vegetables");
            var c6 = service.AddCategory("Meat");
            var foodbank = service.AddFoodBank(10, "Antrim Road", "BT49 0ST");
            var s4 = service.AddStock(foodbank.Id, "Meat", 6, new DateTime(2023, 08, 09), c6.Id);

            //act
            var update = service.UpdateStock(s4);
            {
                s4.Description = "Beef";
                s4.Quantity = 10;
            }

            //assert
            Assert.Equal("Beef", s4.Description);
            Assert.Equal(10, s4.Quantity);

        }
        //--------End of Stock Management Tests-------//

        //-----Begin Client Management Tests-----//
        [Fact]
        public void GetClients_WhenNone_ShouldReturnEmptyList()
        {
            //arrange and act
            var clients = service.GetAllClients();

            //assert
            Assert.Empty(clients);
        }

        [Fact]
        public void GetClients_WhenOneExists_ShouldReturnOne()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var c = service.AddClient("Allen", "BT46 8KM", "paul@yahoo.co.uk", 3, f.Id);

            //act
            var getAll  = service.GetAllClients();
            var count = getAll.Count;

            //assert
            Assert.Equal(1, count);
        }

        [Fact]
        public void GetClientById_WhenExists_ShouldReturnClient()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var c = service.AddClient("Allen", "BT46 8KM", "paul@yahoo.co.uk", 3, f.Id);

            //act
            var getClient = service.GetClientById(f.Id);

            //assert
            Assert.NotNull(getClient);
            Assert.Equal(c, getClient);

        }

        [Fact]
        public void GetClientById_WhenDoesntExist_ShouldReturnNull()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");

            //act
            var get = service.GetClientById(1);

            //assert
            Assert.Null(get);

        }

        [Fact]
        public void AddClient_WhenFoodBankExists_ShouldAddClient()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var c = service.AddClient("Allen", "BT46 8KM", "paul@yahoo.co.uk", 3, f.Id);

            //act
            var get = service.GetClientById(c.Id);

            //assert
            Assert.Equal(c, get);
        }

        [Fact]
        public void UpdateClient_WhenExists_ShouldUpdateProperties()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var c = service.AddClient("Allen", "BT46 8KM", "paul@yahoo.co.uk", 3, f.Id);

            //act
            var update = service.UpdateClient(c);
            {
                c.SecondName = "O'Neill";
                c.NoOfPeople = 5;
            }
            
            //assert
            Assert.True(c.SecondName == "O'Neill");
            Assert.True(c.NoOfPeople == 5);

        }

        [Fact]
        public void DeleteClient_WhenExists_ShouldReturnTrue()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var c = service.AddClient("Allen", "BT46 8KM", "paul@yahoo.co.uk", 3, f.Id);

            //act
            var delete = service.DeleteClient(c.Id);
            
            //assert
            Assert.True(delete);

        }

        [Fact]
        public void DeleteClient_WhenDoesntExist_ShouldReturnFalse()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var c = service.AddClient("Allen", "BT46 8KM", "paul@yahoo.co.uk", 3, f.Id);

            //act
            var delete = service.DeleteClient(2);

            //assert
            Assert.False(delete);

        }

        [Fact]
        public void SearchClients_WhenStringContains_ShouldReturnClients()
        {
             //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var c = service.AddClient("Allen", "BT46 8KM", "paul@yahoo.co.uk", 3, f.Id);
            var c2 = service.AddClient("O'Neill", "bt56 9LM", "example@yahoo.com", 4, f.Id);

            //act
            var search = service.SearchClients("Thorndale");

            //assert
            Assert.Contains(c, search);
            Assert.Contains(c2, search);

        }

        [Fact]
        public void AddParcelToClient_ParcelIdShouldContainClientId()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var cL1 = service.AddClient("Allen", "BT45 7PL", "example@mail.com", 3, f.Id);
            var c1 = service.AddCategory("Carbohydrates");
            var user = service.AddUser("Joanne", "McCracken", "jo@mail.com", "1234", f.Id, Role.admin);
            var s1 = service.AddStock(f.Id, "Carbohydrates", 3, new DateTime(2022, 10, 01), c1.Id);

            //act
            var p = service.AddParcel(cL1.Id, user.Id, f.Id);

            //assert
            Assert.NotNull(p);
            Assert.Equal(c1.Id, p.ClientId);
        }

        [Fact]
        public void AddParcelToUser_ParcelShouldContainUserId()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var cL1 = service.AddClient("Allen", "BT45 7PL", "example@mail.com", 3, f.Id);
            var c1 = service.AddCategory("Carbohydrates");
            var user = service.AddUser("Joanne", "McCracken", "jo@mail.com", "1234", f.Id, Role.admin);
            var s1 = service.AddStock(f.Id, "Carbohydrates", 3, new DateTime(2022, 10, 01), c1.Id);

            //act
            var p = service.AddParcel(cL1.Id, user.Id, f.Id);

            //assert
            Assert.NotNull(p);
            Assert.Equal(user.Id, p.UserId);

        }

        [Fact]
        public void AddParcelToFoodBank_ParcelShouldContainFoodBankId()
        {
              //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var cL1 = service.AddClient("Allen", "BT45 7PL", "example@mail.com", 3, f.Id);
            var c1 = service.AddCategory("Carbohydrates");
            var user = service.AddUser("Joanne", "McCracken", "jo@mail.com", "1234", f.Id, Role.admin);
            var s1 = service.AddStock(f.Id, "Carbohydrates", 3, new DateTime(2022, 10, 01), c1.Id);

            //act
            var p = service.AddParcel(cL1.Id, user.Id, f.Id);

            //assert
            Assert.NotNull(p);
            Assert.Equal(f.Id, p.FoodBankId);

        }

        [Fact]
        public void DeleteParcel_WhenExists_ShouldReturnTrue()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var cL1 = service.AddClient("Allen", "BT45 7PL", "example@mail.com", 3, f.Id);
            var c1 = service.AddCategory("Carbohydrates");
            var user = service.AddUser("Joanne", "McCracken", "jo@mail.com", "1234", f.Id, Role.admin);
            var s1 = service.AddStock(f.Id, "Carbohydrates", 3, new DateTime(2022, 10, 01), c1.Id);

            //act
            var p = service.AddParcel(cL1.Id, user.Id, f.Id);
            var delete = service.DeleteParcel(p.Id);

            //assert
            Assert.True(delete);

        }

        [Fact]
        public void DeleteParcel_WhenDoesnExist_ShouldReturnFalse()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var cL1 = service.AddClient("Allen", "BT45 7PL", "example@mail.com", 3, f.Id);
            var c1 = service.AddCategory("Carbohydrates");
            var user = service.AddUser("Joanne", "McCracken", "jo@mail.com", "1234", f.Id, Role.admin);
            var s1 = service.AddStock(f.Id, "Carbohydrates", 3, new DateTime(2022, 10, 01), c1.Id);

            //act
            var delete = service.DeleteParcel(1);

            //assert
            Assert.False(delete);

        }

        [Fact]
        public void RemoveItemFromParcel_WhenItemInParcel_ShouldReturnTrue()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var cL1 = service.AddClient("Allen", "BT45 7PL", "example@mail.com", 3, f.Id);
            var c1 = service.AddCategory("Carbohydrates");
            var user = service.AddUser("Joanne", "McCracken", "jo@mail.com", "1234", f.Id, Role.admin);
            var s1 = service.AddStock(f.Id, "Carbohydrates", 3, new DateTime(2022, 10, 01), c1.Id);

            //act
            var p = service.AddParcel(cL1.Id, user.Id, f.Id);
            var pi = service.AddItemToParcel(p.Id, s1.Id);
            var remove = service.RemoveItemFromParcel(s1.Id, p.Id);

            //assert
            Assert.True(remove);
        }

        [Fact]
        public void RemoveItemFromParcel_WhenItemNotInParcel_ShouldReturnFalse()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var cL1 = service.AddClient("Allen", "BT45 7PL", "example@mail.com", 3, f.Id);
            var c1 = service.AddCategory("Carbohydrates");
            var user = service.AddUser("Joanne", "McCracken", "jo@mail.com", "1234", f.Id, Role.admin);
            var s1 = service.AddStock(f.Id, "Carbohydrates", 3, new DateTime(2022, 10, 01), c1.Id);

            //act
            var p = service.AddParcel(cL1.Id, user.Id, f.Id);
            var remove = service.RemoveItemFromParcel(s1.Id, p.Id);

            //assert
            Assert.False(remove);
        }

        

        [Fact]
        public void ParcelItem_AddParcelItem_ShouldAddItemToParcel()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var cL1 = service.AddClient("Allen", "BT45 7PL", "example@mail.com", 3, f.Id);
            var c1 = service.AddCategory("Carbohydrates");
            var user = service.AddUser("Joanne", "McCracken", "jo@mail.com", "1234", f.Id, Role.admin);
            var s1 = service.AddStock(f.Id, "Carbohydrates", 3, new DateTime(2022, 10, 01), c1.Id);

            //act
            var p = service.AddParcel(cL1.Id, user.Id, f.Id);
            var pi = service.AddItemToParcel(p.Id, s1.Id);

            //assert
            Assert.NotNull(pi);
            Assert.Equal(s1.Id, pi.StockId);
            Assert.Equal(p.Id, pi.ParcelId);
            Assert.Equal(3, pi.Quantity);
            Assert.Equal(s1.Description, pi.Item.Description);
            
        }

        [Fact]
        public void AutoPopulateParcelWith2Item_ShouldWork()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var cL1 = service.AddClient("Allen", "BT45 7PL", "example@mail.com", 1, f.Id);
            var c1 = service.AddCategory("Carbohydrates");
            var c2 = service.AddCategory("Meat");
            var user = service.AddUser("Joanne", "McCracken", "jo@mail.com", "1234", f.Id, Role.admin);
            var s1 = service.AddStock(f.Id, "Carbohydrates", 3, new DateTime(2022, 10, 01), c1.Id);
            var s2 = service.AddStock(f.Id, "Meat", 3, new DateTime(2023,02,02), c2.Id);

            //act
            var p = service.AddParcel(cL1.Id, user.Id, f.Id);
            var pi = service.AutoPopulateParcel(p.Id);

            //assert - pi is not null
            Assert.NotNull(pi);

            //should contain 2 items
            Assert.Equal(2, pi.Count);

        }

        [Fact]
        public void AutoPopulateWith10Items_ShouldWork()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var cL1 = service.AddClient("Allen", "BT45 7PL", "example@mail.com", 1, f.Id);
            var cat1 = service.AddCategory("Cereal");
            var cat2 = service.AddCategory("Soup");
            var cat3 = service.AddCategory("Baked Beans");
            var cat4 = service.AddCategory("Tomatoes");
            var cat5 = service.AddCategory("Vegetables");
            var cat6 = service.AddCategory("Meat");
            var cat7 = service.AddCategory("Veggie Option");
            var cat8 = service.AddCategory("Fish");
            var cat9 = service.AddCategory("Fruit");
            var cat10 = service.AddCategory("Pudding");
            var user = service.AddUser("Joanne", "McCracken", "jo@mail.com", "1234", f.Id, Role.admin);
            var s1 = service.AddStock(f.Id, "Corn Flakes", 100, new DateTime(2022, 10, 01), cat1.Id);
            var s2 = service.AddStock(f.Id, "Vegetable Broth", 100, new DateTime(2023,01,01), cat2.Id);
            var s3 = service.AddStock(f.Id, "Baked Beans", 100, new DateTime(2023,01,01), cat3.Id);
            var s4 = service.AddStock(f.Id, "Tinned Tomastoes", 100, new DateTime(2022,01,01), cat4.Id);
            var s5 = service.AddStock(f.Id, "Peppers", 100, new DateTime(2022,01,02), cat5.Id);
            var s6 = service.AddStock(f.Id, "Chicken", 100, new DateTime(2022, 09,09), cat6.Id);
            var s7 = service.AddStock(f.Id, "Kidney Beans", 100, new DateTime(2022,10,12), cat7.Id);
            var s8 = service.AddStock(f.Id, "Frozen Cod", 100, new DateTime(2022,10,30), cat8.Id);
            var s9 = service.AddStock(f.Id, "Banana", 100, new DateTime(2022, 09, 01), cat9.Id);
            var s10 = service.AddStock(f.Id, "Angel Delight", 100, new DateTime(2022, 05, 07), cat10.Id);

            
            //act
            var p = service.AddParcel(cL1.Id, user.Id, f.Id);
            var pi = service.AutoPopulateParcel(p.Id);

            //assert
            Assert.NotNull(pi);
            Assert.Equal(10, pi.Count);

        }
       
    }
}
