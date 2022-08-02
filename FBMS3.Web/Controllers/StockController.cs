using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

using FBMS3.Core.Models;
using FBMS3.Core.Services;
using FBMS3.Core.Security;
using FBMS3.Web.ViewModels;
using FBMS3.Data.Services;
using FBMS3.Data.Repositories;

namespace FBMS3.Web.Controllers
{
    //Stock controller inherits from Base Controller
    public class StockController : BaseController
    {
        private IUserService service;

        public StockController(IUserService ss)
        {
            service = ss;
        }

        //GET - return a list of all of the stock 
        /*public IActionResult Index()
        {
            var stock = service.GetAllStock();

            return View(stock);
        }*/

        //Get - return an item of stock by its id
        public IActionResult Details(int id)
        {
            var stockitem = service.GetStockById(id);
            if(stockitem == null)
            {
                Alert ("Stock Item Not Found", AlertType.warning);
                return RedirectToAction(nameof(Index)); 
            }

            return View (stockitem);

        }

        //GET - display blank form to create an item of stock for user
        public IActionResult Create()
        {
            var foodbanks = service.GetFoodBanks();

            //populate viewmodel stock select list prperty
            var scvm = new StockCreateViewModel
            {
                FoodBanks = new SelectList(foodbanks,"Id","StreetName")
            };

            //render the blank form for stock to the view for adding all properties
            return View( scvm );
        }

        //POST - Create a stock item when it has been loaded
        [HttpPost]
        public IActionResult Create(StockCreateViewModel svm)
        {
            if(ModelState.IsValid)
            {
                service.AddStock(svm.FoodBankId, svm.Description, svm.Quantity, svm.ExpiryDate);

                Alert($"{svm.Quantity} of {svm.Description} added to food bank no.1 {svm.FoodBankId}");
                return RedirectToAction(nameof(Index));
            }

            //in case of error re display the form for editing
            return View(svm);
        }

        //GET - Display the index page with all of the stock currently
        //enumeration used to get either meat, vegetables, carbohydrates or non food items
        public IActionResult Index(StockSearchViewModel s)
        {
            s.Stock = service.SearchStock(s.Range, s.Query);

            return View (s);
        }

        //GET - Edit/Update the stock item
        public IActionResult Edit(int id)
        {
            //load the stock by using the service methods
            var s = service.GetStockById(id);

            //check if null and is so alert the user
            if (s == null)
            {
                Alert($"Stock item {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            //pass the stock item to the view for editing
            return View(s);
        }

        //POST - Edit stock item
        [HttpPost]
        public IActionResult Edit (int id, [Bind("Id, Description, Quantity, ExpiryDate")] Stock s)
        {
            if(ModelState.IsValid)
            {
                service.UpdateStock(s);
                Alert("Stock item update successfully", AlertType.info);

                return RedirectToAction(nameof(Details), new { Id = s.Id});
            }

            //model state not valid return the form for editing to correct the errors
            return View(s);
        }

        public IActionResult Delete(int id)
        {
            var s = service.GetStockById(id);

            //check if the stock item is null and if so, return null
            if(s == null)
            {
                Alert($"Stock {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            //pass the item on to the view for deletion confirmation
            return View(s);
        }

        public IActionResult DeleteConfirm(int id)
        {
            //delete the stock item via the service method
            service.DeleteStockById(id);

            Alert("Stock item deleted successfully", AlertType.info);

            return RedirectToAction(nameof(Index));
        }


    }
}

