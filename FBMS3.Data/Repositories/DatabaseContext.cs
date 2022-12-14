using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// import the Models (representing structure of tables in database)
using FBMS3.Core.Models; 

namespace FBMS3.Data.Repositories
{
    // The Context is How EntityFramework communicates with the database
    // We define DbSet properties for each table in the database
    public class DatabaseContext :DbContext
    {
         // authentication store
        public DbSet<User> Users { get; set; }

        //configure the db context for food banks of type food banks
        public DbSet<FoodBank> FoodBanks { get; set; }

        //configure the data base set for stock
        public DbSet<Stock> Stock { get ; set ; }

        //configure the Db set for Recipe Ingredients
        public DbSet<ParcelItem> ParcelItems { get; set; }

        //congfigure the dbSet for Clients - entity which are added when they come to the food bank
        public DbSet<Client> Clients { get; set; }

        //db set for Parcels - parcels are generated and will take food away from the food bank
        public DbSet<Parcel> Parcels { get; set; }

        //configure for stock category
        public DbSet<Category> Categorys { get; set; }

        // Configure the context to use Specified database. We are using 
        // Sqlite database as it does not require any additional installations.
        // FBMS3 configured to allow use of MySql and Postgres
        // ideally connections strings should be stored in appsettings.json

        //create the composite key for the parcelitem bridging table so that ParcelId and StocKId are both the foriegn key
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParcelItem>()
                        .HasKey( x => new { x.ParcelId, x.StockId });
        }
       
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder                  
                .UseSqlite("Filename=data.db")
                //.UseMySQL("server=localhost; port=3306; database=xxx; user=xxx; password=xxx")
                //.UseNpgsql("host=localhost; port=5432; database=xxx; username=xxx; password=xxx")
                .LogTo(Console.WriteLine, LogLevel.Information) // remove in production
                .EnableSensitiveDataLogging()                   // remove in production
                ;
        }

        // Convenience method to recreate the database thus ensuring the new database takes 
        // account of any changes to Models or DatabaseContext. ONLY to be used in development
        public void Initialise()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

    }
}
