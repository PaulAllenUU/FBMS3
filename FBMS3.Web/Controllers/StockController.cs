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
    [Authorize]
    public class StockController : BaseController
    {
        private IUserService service;

        public StockController(IUserService ss)
        {
            service = ss;
        }

        //Get - return an item of stock by its id
        [Authorize(Roles="admin,manager,staff")]
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
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult Create()
        {
            var foodbanks = service.GetFoodBanks();
            var categorys = service.GetAllCategorys();

            //populate viewmodel stock select list prperty
            var scvm = new StockCreateViewModel
            {
                FoodBanks = new SelectList(foodbanks,"Id","StreetName"),
                Categorys = new SelectList(categorys, "Id", "Description")
            };

            //render the blank form for stock to the view for adding all properties
            return View( scvm );
        }

        //POST - Create a stock item when it has been loaded
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult Create(StockCreateViewModel svm)
        {
            //view model properties need to be included in case they are not selected in the first form

            var foodbanks = service.GetFoodBanks();
            var categorys = service.GetAllCategorys();

            //populate viewmodel stock select list prperty
            var scvm = new StockCreateViewModel
            {
                FoodBanks = new SelectList(foodbanks,"Id","StreetName"),
                Categorys = new SelectList(categorys, "Id", "Description")
            };

            if(ModelState.IsValid)
            {
                service.AddStock(svm.FoodBankId, svm.Description, svm.Quantity, svm.ExpiryDate, svm.CategoryId);

                Alert($"{svm.Quantity} {svm.Description} added to food bank no. {svm.FoodBankId}");
                return RedirectToAction(nameof(Index));
            }

            //in case of error re display the form which includes the view model for re editing
            return View(scvm);
        }

        //GET - Display the index page with all of the stock currently
        //enumeration used to get either meat, vegetables, carbohydrates or non food items
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult Index(StockSearchViewModel s)
        {
            s.Stock = service.SearchStock(s.Range, s.Query);

            return View (s);
        }

        //GET - Edit/Update the stock item
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult Edit(int id)
        {
            //load the food banks and category in the view model as they are foreign keys
            var foodbanks = service.GetFoodBanks();
            var categorys = service.GetAllCategorys();
            //load the stock by using the service methods
            var s = service.GetStockById(id);

            //check if null and is so alert the user
            if (s == null)
            {
                Alert($"Stock item {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            var stockViewModel = new StockEditViewModel
            {
                FoodBanks = new SelectList(foodbanks,"Id","StreetName"),
                Categorys = new SelectList(categorys,"Id","Description"),
                Id = s.Id,
                FoodBankId = s.FoodBankId,
                Description = s.Description,
                Quantity = s.Quantity,
                ExpiryDate = s.ExpiryDate,
                CategoryId = s.CategoryId
            };

            //pass the stock item to the view for editing
            return View(stockViewModel);
        }

        //POST - Edit stock item
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult Edit ([Bind("Id,Description,Quantity,ExpiryDate,FoodBankId,CategoryId")] StockEditViewModel s)
        {
            var stock = service.GetStockById(s.Id);

            if(!ModelState.IsValid || stock == null)
            {
                //Alert the user to the error
                Alert("There was a problem editing the stock item. Please try again", AlertType.warning);
                //return the form for further editing
                return View(s);
            }
            
            stock.FoodBankId = s.FoodBankId;
            stock.Description = s.Description;
            stock.Quantity = s.Quantity;
            stock.ExpiryDate = s.ExpiryDate;
            stock.CategoryId = s.CategoryId;

            var updated = service.UpdateStock(stock);

            if(updated == null)
            {
                Alert("There was a problem updating the stock item. Please try again", AlertType.warning);
                return View(s);
            }
            
            Alert("Stock item updated successfully", AlertType.info);

            return RedirectToAction(nameof(Details), new { Id = s.Id } );
        }

        [Authorize(Roles="admin,manager,staff")]
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
        
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult DeleteConfirm(int id)
        {
            //delete the stock item via the service method
            service.DeleteStockById(id);

            Alert("Stock item deleted successfully", AlertType.info);

            return RedirectToAction(nameof(Index));
        }

    }
}

