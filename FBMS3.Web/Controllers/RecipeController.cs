/*using Microsoft.AspNetCore.Mvc;
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
        public IActionResult RecipeDetails(int id)
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
        public IActionResult RecipeCreate()
        {
            //display the blank form for creating a recipe so that a user can fill out
            return View();
        }

        [HttpPost]
        public IActionResult RecipeCreate([Bind("Title, NoOfIngredients, CookingTimeMins")] Recipe r)
        {
            //if the service method for duplicate recipe returns true
            if(service.IsDuplicateRecipe(r.Title))
            {
                //flag a model error to the user and tell them that the title is already in use
                ModelState.AddModelError(nameof(r.Title), 
                                            "This title is already in use for a recipe");
            }

            //otherwise create the recipe and set all properties
            if(ModelState.IsValid)
            {
                r = service.AddRecipe(r.Title, r.NoOfIngredients, r.CookingTimeMins);
                Alert($"Recipe created successfully", AlertType.success);

                return RedirectToAction(nameof(RecipeDetails), new { Id = r.Id} );
            }

            return View(r);

        }

        public IActionResult RecipeEdit(int id)
        {
            //get the recipe by id to make sure it exists
            var r = service.GetRecipeById(id);

            //if it does not exist then cannot edit so return null
            if (r == null)
            {
                Alert($"Recipe {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            //return the recipe back to the view for further editing
            return View(r);

        }

        [HttpPost]
        public IActionResult RecipeEdit(int id, [Bind("Id, Title, NoOfIngredients, CookingTimeMins")] Recipe r)
        {
            //check that the title is not a duplicate using the previous method
            if(service.IsDuplicateRecipe(r.Title))
            {
                //if the service method return true flag to the user that the title already exists
                ModelState.AddModelError(nameof(r.Title), 
                                            "This title is already in use for a recipe");
            }

            //if the above method returns false then update the recipe
            if(ModelState.IsValid)
            {
                service.UpdateRecipe(r);
                Alert("Recipe updated successfully", AlertType.info);

                return RedirectToAction(nameof(RecipeDetails), new { Id = r.Id });
            }

            return View(r);

        }


    }
}*/