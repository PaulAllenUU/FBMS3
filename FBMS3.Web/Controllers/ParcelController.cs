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
                Alert($"Parcel {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            return View(p);
        }

        //pass in the empty form to generate the parcel
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles="admin,manager,staff")]
        public IActionResult Create([Bind("UserId, ClientId, FoodBankId, NoOfPeople")] Parcel p)
        {
            if(ModelState.IsValid)
            {
                p = svc.GenerateParcelForClient(p.UserId, p.ClientId, p.FoodBankId);
            }

            return View(p);

        }
    }
}