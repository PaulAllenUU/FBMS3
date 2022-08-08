
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
            service.AddUser("guest", "phillips", "guest@mail.com", "guest", foodbank.Id, Role.guest);

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
        public void FoodBank_AddingFoodBank_WithSameAddress_ShouldFail()
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
        //End of Food Bank Service Management Tests

        [Fact]
        public static void Stock_GetAllStockWhenNone_ShouldReturnNone()
        {
            //arrange
            var list = service.GetAllStock();

            //assert
            Assert.Null(list):

        }

        [Fact]
        public void Stock_GetStockById_WhenExists_ShouldReturnOne()
        {
            //arrange
            var f = service.AddFoodBank(28, "Thorndale", "BT49 0ST");
            var stock = AddStock(f.FoodBankId, "Cereal", 3, new DateTime(2022, 04, 10));

            //act
            
        }

    }
}
