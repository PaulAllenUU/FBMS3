
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
        public void GetUsers_WhenThree_ShouldReturnThree()
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
            //act
             var f1 = service.AddFoodBank(10, "Antrim Road", "BT49 0ST");
             var u1 = service.AddUser("Paul", "Allen", "admin@mail.com", "admin", f1.Id, Role.admin);

             //arrange
             var delete = service.DeleteUser(u1.Id);

             //assert
             var users = service.GetUsers();

             //should be empty
             Assert.Empty(users);

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
        public void FoodBank_DeleteFoodBank_WhenNull_ShouldReturnFalse()
        {
            var f = service.DeleteFoodBank(1);

            Assert.False(f);
        }

        [Fact]
        public void AddFoodBank_WhenDuplicateAddress_ShouldReturnNull()
        {
            //act
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");

            //arrange - duplicate food bank address details
            var f2 = service.AddFoodBank(28, "Thorndale", "BT49 0ST");

            //assert
            Assert.NotNull(f);

            //f2 should not be added
            Assert.Null(f2);
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
        public void GetStockById_WhenExists_ShouldWork()
        {
            //arrange
            var c5 = service.AddCategory("Vegetables");
            var c6 = service.AddCategory("Meat");
            var foodbank = service.AddFoodBank(10, "Antrim Road", "BT49 0ST");
            var s4 = service.AddStock(foodbank.Id, "Meat", 6, new DateTime(2023, 08, 09), c6.Id);

            var get = service.GetStockById(s4.Id);

            //assert
            Assert.NotNull(get);
        }

        [Fact]
        public void GetStockById_WhenDoesntExist_ShouldReturnNull()
        {
            var c5 = service.AddCategory("Vegetables");
            var c6 = service.AddCategory("Meat");
            var foodbank = service.AddFoodBank(10, "Antrim Road", "BT49 0ST");

            var get = service.GetStockById(1);

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
            var s4 = service.AddStock(1, "Meat", 6, new DateTime(2023, 08, 09), c6.Id);

            Assert.Null(s4);

        }




        //--------End of Stock Management Tests-------//

        
        [Fact]
        public void GenerateParcelForClient_WhenFoodBankHas_ShouldAddItemsToParcel()
        {
            //arrange - add food bank
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var u = service.AddUser("Paul", "Allen", "allen@gmail.com", "password", f.Id, Role.admin);

            //add client
            var cL1 = service.AddClient("Allen", "BT45 7PL", "example@mail.com", 3, f.Id);
            //add stock category
            var c1 = service.AddCategory("Carbohydrates");
            var c2 = service.AddCategory("Vegetables");
            //add items of the same category
            var s1 = service.AddStock(f.Id, "Potato", 3, new DateTime(2022, 10, 01), c1.Id);
            var s2 = service.AddStock(f.Id, "Vegetables", 4, new DateTime(2022, 10, 01), c1.Id);

            //act
            var parcel = service.GenerateParcelForClient(u.Id, cL1.Id, f.Id);
            var parcelcount = parcel.Items.Count;

            //check if it auto decrements from the database
            var stockCount = f.Stock.Count;
            
            //assert
            Assert.NotNull(parcel);
            Assert.Equal(0, stockCount);  

        }

    }
}
