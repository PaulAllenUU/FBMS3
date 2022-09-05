
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

using FBMS3.Core.Models;
using FBMS3.Core.Services;
using FBMS3.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using FBMS3.Core.Security;

namespace FBMS3.Web.Controllers
{
    [Authorize]
    public class ClientController : BaseController
    {
        private readonly IUserService svc;

        public ClientController(IUserService ss)
        {
            svc = ss;
        }

        //index page will allow search feature for clients
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult Index(ClientSearchViewModel c)
        {
            c.Clients = svc.SearchClients(c.Query);

            return View(c);
        }
        
        //return empty client registration form to the view for completion
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult Create()
        {
            var foodbanks = svc.GetFoodBanks();

            var ccvm = new ClientCreateViewModel
            {
                FoodBanks = new SelectList(foodbanks, "Id", "StreetName")
            };

            return View(ccvm);
        }

        [Authorize(Roles="admin,manager,staff")]
        public IActionResult Details(int id)
        {
            //retreive the client using the e mail address service method
            var c = svc.GetClientById(id);

            //if the object is null then the client does not exist
            if(c == null)
            {
                Alert($"Client with {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            //if found then return to the client to the view
            return View(c);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult Create([Bind("SecondName,PostCode,EmailAddress,NoOfPeople,FoodBankId")] ClientCreateViewModel c)
        {
            var foodbanks = svc.GetFoodBanks();

            var ccvm = new ClientCreateViewModel
            {
                FoodBanks = new SelectList(foodbanks, "Id", "StreetName")
            };

            if(!ModelState.IsValid)
            {
                return View(ccvm);
            }

            //add the client via the methods in the service
            var client = svc.AddClient(c.SecondName, c.PostCode, c.EmailAddress, c.NoOfPeople, c.FoodBankId);
            {
                if(client == null)
                {
                    Alert("There was a problem registering this client. Please try again", AlertType.warning);

                    return View(ccvm);
                }

                //call duplicated client method to see if the e mail addres is already in use
                if(svc.IsDuplicateClient(c.EmailAddress))
                {
                    Alert($"The email address {c.EmailAddress} is already in use");
                }

                Alert($"{client.SecondName} successfully added to {client.FoodBankStreetName}, please click next to view their food bank and parcel details");
                

                //change this to take the user to the assignment of the food bank and food parcel
                return RedirectToAction(nameof(Index));
            }
        }

        /*public IActionResult ParcelCreate([Bind("UserId")] ParcelCreateViewModel p)
        {
            var parcel = svc.GenerateParcelForClient(p.UserId, p.ClientId, p.FoodBankId);

            return View(parcel);


        }*/

        //GET - Edit by e mail address
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult Edit(int id)
        {
            //check the user exists through their e mail address
            var client = svc.GetClientById(id);

            //if null then cannot update so return null
            if(client == null)
            {
                Alert($"User with {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            //if client exists then pass to the view for editing
            return View(client);

        }

        //GET Client // e mail address
        [HttpPost]
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult Edit(int id, [Bind("Id,SecondName,PostCode, EmailAddress,NoOfPeople,FoodBankId")] Client c)
        {
            if(svc.IsDuplicateClient(c.EmailAddress))
            {
                ModelState.AddModelError(nameof(c.EmailAddress),
                                            $"{c.EmailAddress} is already in use for an existing client");
            }

            if(ModelState.IsValid)
            {
                svc.UpdateClient(c);
                Alert("Client details were updated successfully", AlertType.info);

                return RedirectToAction(nameof(Details), new { Id = c.Id } );
            }

            return View(c);
        }

        //GET - Client - email address
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult Delete(int id)
        {
            //use the service method to retrive the client by e mail address
            var c = svc.GetClientById(id);

            //if the email address does not exist then return error
            if (c == null)
            {
                Alert($"Client with {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            //pass the client to the view for deletion confirmation
            return View(c);
        
        }

        [HttpPost]
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult DeleteConfirm(int id)
        {
            var c = svc.DeleteClient(id);

            Alert("Client deleted successfully", AlertType.info);

            //redirect to the index view of all of the clients
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles="admin,manager,staff")]
        public IActionResult ParcelCreate(int id)
        {
            //retreive each of the entities required
            var c = svc.GetClientById(id);


            //if the client is null then return null
            if(c == null)
            {
                Alert("Could not find client, please try again", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult ParcelCreate([Bind("ParcelId,StockId,ClientId,Quantity")] ParcelItemViewModel pvm)
        {

            if(ModelState.IsValid)
            {
                svc.AddItemToParcel(pvm.ParcelId, pvm.StockId, pvm.Quantity);
                return RedirectToAction(nameof(Details));

            }

            //pass the view model back into the form in case of error
            return View(pvm);

        }

    }
}