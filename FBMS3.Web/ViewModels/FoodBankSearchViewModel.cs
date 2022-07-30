using System;
using FBMS3.Core.Models;

namespace FBMS3.Web.ViewModels
{
    public class FoodBankSearchViewModel
    {
        public IList<FoodBank> FoodBanks { get; set; } = new List<FoodBank>();

        public string Query { get; set; } = "";
    }
}