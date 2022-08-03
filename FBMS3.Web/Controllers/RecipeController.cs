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
    public class RecipeController : BaseController
    {
        private IUserService service;

        public RecipeController(IUserService ss)
        {
            service =ss;
        }

        //GET - return a recipe by its id
        public IActionResult Details(int id)
        {
            var recipe = service.GetRecipeById(id);

            //if recipe is not there, ie null then alert the user
            if(recipe == null)
            {
                Alert("Recipe Not Found", AlertType.warning);
                return RedirectToAction(nameof(Index));
   
            }

            //otherwise return the recipe
            return View(recipe);
        }

        //GET - Display a blank for to create a recipe from stock and food bank
        
    }

}