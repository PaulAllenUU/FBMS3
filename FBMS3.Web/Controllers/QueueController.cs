
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
    public class QueueController : BaseController
    {
        private readonly IUserService svc;

        public QueueController(IUserService ss)
        {
            svc = ss;
        }

        public IActionResult Index()
        {
            var queue = svc.GetAllClients();

            var count = queue.Count();

            Alert($"There are currently {count} clients in the queue", AlertType.info);

            return View(queue);
        }

    }
}