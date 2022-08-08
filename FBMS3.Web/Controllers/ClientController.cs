
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

using FBMS3.Core.Models;
using FBMS3.Core.Services;
using FBMS3.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using FBMS3.Core.Security;

namespace FBMS3.Web.Controllers
{
    public class ClientController : BaseController
    {
        private readonly IUserService svc;

        public ClientController(IUserService ss)
        {
            svc = ss;
        }

        //index page will allow search feature for clients
        public IActionResult Index(ClientSearchViewModel c)
        {
            c.Clients = svc.SearchClients(c.Query);

            return View(c);
        }
        
        //return empty client registration form to the view for completion
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Details(string email)
        {
            //retreive the client using the e mail address service method
            var c = svc.GetClientByEmailAddress(email);

            //if the object is null then the client does not exist
            if(c == null)
            {
                Alert($"Client with {email} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            //if found then return to the client to the view
            return View(c);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("SecondName,PostCode,EmailAddress,NoOfPeople")] ClientRegisterViewModel c)
        {
            if(!ModelState.IsValid)
            {
                return View(c);
            }

            //add the client via the methods in the service
            var client = svc.AddClient(c.SecondName, c.PostCode, c.EmailAddress, c.NoOfPeople);
            {
                if(client == null)
                {
                    Alert("There was a problem registering this client. Please try again", AlertType.warning);

                    return View(c);
                }

                //call duplicated client method to see if the e mail addres is already in use
                if(svc.IsDuplicateClient(c.EmailAddress))
                {
                    Alert($"The email address {c.EmailAddress} is already in use");
                }

                Alert("Client successfully added, please click next to view their food bank and parcel details");

                //change this to take the user to the assignment of the food bank and food parcel
                return RedirectToAction(nameof(Index));
            }
        }

        //GET - Edit by e mail address
        public IActionResult Edit(string email)
        {
            //check the user exists through their e mail address
            var client = svc.GetClientByEmailAddress(email);

            //if null then cannot update so return null
            if(client == null)
            {
                Alert($"User with {email} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            //if client exists then pass to the view for editing
            return View(client);

        }

        //GET Client // e mail address
        [HttpPost]
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
        public IActionResult Delete(string email)
        {
            //use the service method to retrive the client by e mail address
            var c = svc.GetClientByEmailAddress(email);

            //if the email address does not exist then return error
            if (c == null)
            {
                Alert($"Client with {email} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            //pass the client to the view for deletion confirmation
            return View(c);
        
        }

        [HttpPost]
        public IActionResult DeleteConfirm(string email)
        {
            var c = svc.DeleteClient(email);

            Alert("Client deleted successfully", AlertType.info);

            //redirect to the index view of all of the clients
            return RedirectToAction(nameof(Index));
        }
    }
}