using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

using FBMS3.Core.Models;
using FBMS3.Web.ViewModels;
using FBMS3.Core.Services;
using FBMS3.Core.Security;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace FBMS3.Web.Controllers
{
    public class ParcelController : BaseController
    {
        private IUserService svc;

        //dependendy injection
        public ParcelController(IUserService ss)
        {
            svc = ss;
        }

        public IActionResult Index()
        {
            var parcels = svc.GetAllParcels();

            return View(parcels);
        }
            

        public IActionResult Details(int id)
        {
            var p = svc.GetParcelById(id);

            if(p == null)
            {
                Alert($"Parcel {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            return View(p);
        }

        //pass in the empty form to generate the parcel
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create([Bind("UserId, ClientId, FoodBankId, NoOfPeople")] Parcel p)
        {
            if(ModelState.IsValid)
            {
                p = svc.GenerateParcelForClient(p.UserId, p.ClientId, p.FoodBankId, p.NoOfPeople);
            }

            return View(p);

        }
    }
}