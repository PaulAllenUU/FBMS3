using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

using FBMS3.Core.Models;
using FBMS3.Web.ViewModels;
using FBMS3.Core.Services;
using FBMS3.Core.Security;

namespace FBMS3.Web.Controllers
{
    [Authorize]
    public class FoodBankController : BaseController
    {
        //private readonly IConfiguration _config;
        private IUserService svc;

        //dependendy injection
        public FoodBankController(IUserService ss)
        {
            svc = ss;
        }

        //index page will allow a search feature to search for food banks
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult Index(FoodBankSearchViewModel f)
        {
            //call to the method will return any matching criteria for street number, 
            //street name or post code
            f.FoodBanks = svc.SearchFoodBanks(f.Query);

            return View (f);
        }

        [Authorize(Roles="admin,manager,staff")]
        public IActionResult Details(int id)
        {
            //retreive the foodbank with the specified id from the service methods
            var f = svc.GetFoodBankById(id);

            //if the foodbank is not and not found then cannot be retrieved
            if (f == null)
            {
                
                return RedirectToAction(nameof(Index));
            }

            //if the food bank is found the then return the food bank
            return View(f);

        }

        [Authorize(Roles="admin")]
        public IActionResult Create()
        {
            //display the a blank form to the view so user can create a food bank
            return View();
        }
        
        [Authorize(Roles="admin")]
        [HttpPost]
        public IActionResult Create([Bind("StreetNumber, StreetName, PostCode")] FoodBank f)
        {
            //check the street number and address is unique for this food bank
            if (svc.IsDuplicateLocation(f.Id, f.StreetNumber, f.PostCode))
            {
                ModelState.AddModelError(nameof(f.PostCode), 
                                            "This address is already in use");
            }

            if(ModelState.IsValid)
            {
                f = svc.AddFoodBank(f.StreetNumber, f.StreetName, f.PostCode);
                Alert($"Food Bank created successfully", AlertType.success);

                return RedirectToAction(nameof(Details), new { Id = f.Id });

            }

            return View(f);
        }

        //GET actions - FoodBank Id
        [Authorize(Roles="admin,manager")]
        public IActionResult Edit(int id)
        {

            //load the food bank into memory using the service methods
            var f = svc.GetFoodBankById(id);

            //check if f is not and if so alert the user
            if (f == null)
            {
                Alert($"Food Bank {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            //pass to the view for editing
            return View(f);

        }
         
        //POST - FoodBank/Edit/{id}
        [Authorize(Roles="admin,manager")]
        [HttpPost]
        public IActionResult Edit(int id, [Bind("Id, StreetNumber, StreetName, PostCode")] FoodBank f)
        {
            //check location (street no and post code) combined is unique for this food bank
            if(svc.IsDuplicateLocation(f.Id, f.StreetNumber, f.PostCode))
            {
                ModelState.AddModelError("Location", "The location is already in use");
            }

            if(ModelState.IsValid)
            {
                //pass the data to the service to perform the update
                svc.UpdateFoodBank(f);
                Alert("Food Bank updated successfully", AlertType.info);

                return RedirectToAction(nameof(Details), new { Id = f.Id });
            }

            return View(f);
        }

        //GET / FoodBank / Delete/ {id}
        [Authorize(Roles="admin,manager")]
        public IActionResult Delete(int id)
        {
            var f = svc.GetFoodBankById(id);

            if (f == null)
            {
                Alert($"Food Bank {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            //pass the food bank to the view for deletion confirmation
            return View(f);
        }

        //POST // FoodBank / Delte / {id}
        [Authorize(Roles="admin,manager")]
        [HttpPost]
        public IActionResult DeleteConfirm(int id)
        {
            var f = svc.DeleteFoodBank(id);

            Alert("Food Bank deleted successfully", AlertType.info);

            //re direct to the index view of all the food banks
            return RedirectToAction(nameof(Index));
        }

        //=====Food Bank Stock Management ========

        //GET - Stock Create from the Food bank details page
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult StockCreate(int id)
        {
            var f = svc.GetFoodBankById(id);
            var categorys = svc.GetAllCategorys();

            //check the returned food bank is not null
            if(f == null)
            {
                //if it is null then does not exist so return null
                Alert($"Foodbank {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

             
            //create the new stock item and set the FoodBank id - foreign key
            var s = new StockCreateViewModel
            { 
                FoodBankId = id,
                Categorys = new SelectList(categorys, "Id", "Description")
            
            };

            return View (s);
        }

        //POST - filling in the form to create the new item of stock
        [HttpPost]
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult StockCreate(int foodBankId, [Bind("FoodBankId, Description, Quantity, ExpiryDate, CategoryId")] StockCreateViewModel s)
        {
            if(ModelState.IsValid)
            {
                var stock = svc.AddStock(s.FoodBankId, s.Description, s.Quantity, s.ExpiryDate, s.CategoryId);
                Alert($"Stock item created successfully for food bank {s.FoodBankId}", AlertType.info);
                return RedirectToAction(nameof(Details), new { Id = stock.FoodBankId });
            }

            //in case of errors return the form for further editing
            return View(s);
        }

        //GET - Stock DeleteConfirm
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult StockDelete(int id)
        {
            //load the stock item using the service method
            var stockitem = svc.GetStockById(id);

            //check that it is not null and if it is then return null
            if(stockitem == null)
            {
                Alert($"Stock {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            //pass the stock item to the view for deletion confirmation
            return View(stockitem);
        }

        //POST - STOCK DeleteConfirm
        [HttpPost]
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult StockDeleteConfirm(int id, int foodbankId)
        {
            //delete the stock item via the service methods
            svc.DeleteStockById(id);
            Alert($"Stock item delete successfully for food bank {foodbankId}", AlertType.info);

            //re direct to the stock list index view
            return RedirectToAction(nameof(Details), new { Id = foodbankId });
        }

    }
}