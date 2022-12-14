using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using FBMS3.Core.Models;
using FBMS3.Web.ViewModels;
using FBMS3.Core.Services;
using FBMS3.Core.Security;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace FBMS3.Web.Controllers
{
    [Authorize]
    public class ParcelController : BaseController
    {
        private IUserService svc;

        //dependendy injection
        public ParcelController(IUserService ss)
        {
            svc = ss;
        }

        [Authorize(Roles="admin,manager,staff")]
        public IActionResult Index()
        {
            var parcels = svc.GetAllParcels();

            return View(parcels);
        }

        [Authorize(Roles="admin,manager,staff")]
        public IActionResult Details(int id)
        {
            var p = svc.GetParcelById(id);

            if(p == null)
            {
                //Alert($"Parcel {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            return View(p);
        }

        //pass in the empty form to generate the parcel
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult Create(int userId)
        {
            var clients = svc.GetAllClients();
            var users = svc.GetUsers();
            var foodbanks = svc.GetFoodBanks();

            var pcvm = new ParcelCreateViewModel
            {
               Clients = new SelectList(clients,"Id","SecondName"),
               Users = new SelectList(users,"Id","FirstName"),
               FoodBanks = new SelectList(foodbanks,"Id","StreetName")
            };

            return View(pcvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult Create([Bind("ClientId, UserId, FoodBankId")] ParcelCreateViewModel pcvm)
        {

            if(ModelState.IsValid)
            {
                svc.AddParcel(pcvm.ClientId, pcvm.UserId, pcvm.FoodBankId);

                Alert($"Parcel added successfully for client {pcvm.ClientId}");
                return RedirectToAction(nameof(Details));
            }

            var clients = svc.GetAllClients();
            var users = svc.GetUsers();
            var foodbanks = svc.GetFoodBanks();

            //return the view model to the form in case re editing is needed
            var pvm = new ParcelCreateViewModel
            {
               Clients = new SelectList(clients,"Id","SecondName"),
               Users = new SelectList(users,"Id","FirstName"),
               FoodBanks = new SelectList(foodbanks,"Id","StreetName")
            };


            return View(pvm);
        }

        public IActionResult ParcelFill(int id)
        {
            var items = svc.GetAllStock();
           
            var parcel = svc.GetParcelById(id);

            var pvm = new ParcelItemViewModel
            {
                Items = new SelectList(items,"Id","Description"),
                ParcelId = id
            };

            return View(pvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="admin,manager,staff")]
        public IActionResult ParcelFill([Bind("ParcelId,StockId,Quantity")] ParcelItemViewModel pivm)
        {
            if(ModelState.IsValid)
            {
               svc.AddItemToParcel(pivm.ParcelId, pivm.StockId, pivm.Quantity);
               svc.UpdateParcelItemQuantity(pivm.ParcelId, pivm.StockId, pivm.Quantity);
               Alert($"Item successfully added to parcel {pivm.ParcelId}", AlertType.info);

               return RedirectToAction(nameof(Details), new { Id = pivm.ParcelId });
            }

            var items = svc.GetAllStock();
            var categorys = svc.GetAllCategorys();
            var pvm = new ParcelItemViewModel
            {
                Items = new SelectList(items,"Id","Description"),
            };

            Alert("Something went wrong please try again");
            return View(pvm);
            
        }

        [Authorize(Roles = "admin,manager,staff")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult ItemRemove(int id, int parcelId)
        {
            //check the item is already inside the parcel
            var pi = svc.GetParcelItemById(id);
            if(pi == null)
            {
                Alert("Item not found in parcel", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }
            //delete the parcel item via the service method
            svc.RemoveItemFromParcel(pi.StockId, pi.ParcelId);
            //alert the user to the successful action
            Alert($"Item removed from parcel {pi.ParcelId}", AlertType.info);
            
            //return the user to the details of the parcel id
            return RedirectToAction(nameof(Details), new { Id = pi.ParcelId });

        }

        [Authorize(Roles = "admin,manager,staff")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult AutoPopulateParcel(int id, [Bind("ParcelId")] ParcelItemViewModel pivm)
        {
            var p = svc.GetParcelById(id);

            if(p == null)
            {
                Alert($"Parcel {id} not found", AlertType.info);
                return View(pivm);
            }

            svc.AutoPopulateParcel(id);
            Alert($"Parcel number {pivm.ParcelId} successfully populated. Please review", AlertType.success);

            return RedirectToAction(nameof(Details), new { Id = pivm.ParcelId });

        }
    }
}