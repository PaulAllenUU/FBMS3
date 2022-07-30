using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

using FBMS3.Core.Models;
using FBMS3.Web.ViewModels;
using FBMS3.Core.Services;
using FBMS3.Core.Security;

namespace FBMS3.Web.Controllers
{
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
        public IActionResult Index(FoodBankSearchViewModel f)
        {
            //call to the method will return any matching criteria for street number, 
            //street name or post code
            f.FoodBanks = svc.SearchFoodBanks(f.Query);

            return View (f);
        }

        public IActionResult Details(int id)
        {
            //retreive the foodbank with the specified id from the service methods
            var f = svc.GetFoodBankById(id);

            //if the foodbank is not and not found then cannot be retrieved
            if (f == null)
            {
                Alert($"Foodbank {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            //if the food bank is found the then return the food bank
            return View(f);

        }

        public IActionResult Create()
        {
            //display the a blank form to the view so user can create a food bank
            return View();
        }

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
        public IActionResult DeleteConfirm(int id)
        {
            svc.DeleteFoodBank(id);

            Alert("Food Bank deleted successfully", AlertType.info);

            //re direct to the index view of all the food banks
            return RedirectToAction(nameof(Index));
        }

    }
}