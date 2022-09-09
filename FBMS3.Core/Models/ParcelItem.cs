namespace FBMS3.Core.Models
{
    //bridging class to normalise the many to many relationship between Parcel & Stock
    public class ParcelItem
    {
        //primary key - identity
        public int Id { get; set; }

        //navigation property to the stock table
        public Stock Item { get; set; }

        //foreign key to the stock table
        public int StockId { get; set; }

        //quantity of each stock item is determined by number in family of parcel.client.noofPeople
        public int Quantity => Quantify();

        //foreign key to the parcel table
        public int ParcelId { get; set; }

        //navigation property to the parcel table
        public Parcel Parcel { get; set; }

        //the family size of the client will determine the quantity of each parcel item served in the parcel
        private int Quantify()
        {
            //if there is 1 person they get 2 of each item
            if(Parcel.Client.NoOfPeople == 1)
            {
                return 2;
            }
            //else if there is 2 people they get 3 of each item in the parcel
            else if(Parcel.Client.NoOfPeople == 2)
            {
                return 3;
            }
                //if there is 3 people they get 5 of each item in the parcel
                else if(Parcel.Client.NoOfPeople == 3)
                {
                    return 5;
                }
                    //if there is more than 4 people they get 6 of item in each parcel
                    else
                    {
                        return 6;
                    }
        }
    }
}