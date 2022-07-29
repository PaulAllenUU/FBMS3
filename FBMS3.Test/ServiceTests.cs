
using Xunit;
using FBMS3.Core.Models;
using FBMS3.Core.Services;


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
            
        }

    }
}
