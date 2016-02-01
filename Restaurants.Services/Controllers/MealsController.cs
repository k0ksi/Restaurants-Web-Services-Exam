using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Restaurants.Models;
using Restaurants.Services.Models.BindingModels;

namespace Restaurants.Services.Controllers
{
    public class MealsController : BaseApiController
    {
        // GET api/restaurants/{id}/meals
        [HttpGet]
        [Route("api/restaurants/{id}/meals")]
        public IHttpActionResult GetRestaurantMeals(int id)
        {
            var restaurant = this.Data.Restaurants.Find(id);
            if (restaurant == null)
            {
                return this.NotFound();
            }

            var meals = this.Data.Meals.All()
                .Where(m => m.RestaurantId == id)
                .OrderBy(m => m.Type.Order)
                .ThenBy(m => m.Name)
                .Select(m => new
                {
                    id = m.Id,
                    name = m.Name,
                    price = m.Price,
                    type = m.Type.Name
                });

            return this.Ok(meals);
        }

        // POST api/meals
        [HttpPost]
        [Authorize]
        [Route("api/meals")]
        public IHttpActionResult CreateMeal([FromBody] MealBindingModel model)
        {
            if (model == null)
            {
                return this.BadRequest("Model cannot be null (no data in request)");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var restaurantId = model.RestaurantId;
            var restaurant = this.Data.Restaurants.Find(restaurantId);
            var typeId = model.TypeId;
            var mealType = this.Data.MealTypes.Find(typeId);
            if (restaurant == null || mealType == null)
            {
                return this.BadRequest("Both restaurant and type should exist in the database.");
            }

            var loggedUserId = this.User.Identity.GetUserId();
            var user = this.Data.Users.Find(loggedUserId);

            if (user == null)
            {
                return this.Unauthorized();
            }


            if (loggedUserId != restaurant.OwnerId)
            {
                return this.Unauthorized();
            }

            var meal = new Meal()
            {
                Name = model.Name,
                Price = model.Price,
                Restaurant = restaurant,
                RestaurantId = restaurantId,
                Type = mealType,
                TypeId = typeId
            };

            this.Data.Meals.Add(meal);
            this.Data.SaveChanges();

            return this.CreatedAtRoute(
                "DefaultApi",
                new {controller = "meals", id = meal.Id},
                new {id = meal.Id, name = meal.Name, price = meal.Price, type = meal.Type.Name});
        }

        // PUT api/meals/{id}
        [HttpPut]
        [Authorize]
        [Route("api/meals/{id}")]
        public IHttpActionResult EditExistingMeal(int id, [FromBody] MealBindingModel model)
        {
            if (model == null)
            {
                return this.BadRequest("Model cannot be null (no data in request)");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var meal = this.Data.Meals.Find(id);
            if (meal == null)
            {
                return this.NotFound();
            }

            var loggedUserId = this.User.Identity.GetUserId();
            if (meal.Restaurant.OwnerId != loggedUserId)
            {
                return this.Unauthorized();
            }

            var typeId = model.TypeId;
            var mealType = this.Data.MealTypes.Find(typeId);
            if (mealType == null)
            {
                return this.BadRequest("The meal type should exist in the database.");
            }

            meal.Name = model.Name;
            meal.Price = model.Price;
            meal.TypeId = model.TypeId;
            this.Data.SaveChanges();

            var result = this.Data.Meals.All()
                .Where(m => m.Id == meal.Id)
                .Select(m => new
                {
                    id = m.Id,
                    name = m.Name,
                    price = m.Price,
                    type = m.Type.Name
                });

            return this.Ok(result);
        }

        // DELETE api/meals/{id}
        [HttpDelete]
        [Authorize]
        [Route("api/meals/{id}")]
        public IHttpActionResult DeleteMealById(int id)
        {
            var meal = this.Data.Meals.Find(id);
            if (meal == null)
            {
                return this.NotFound();
            }

            var loggedUserId = this.User.Identity.GetUserId();
            if (meal.Restaurant.OwnerId != loggedUserId)
            {
                return this.Unauthorized();
            }

            this.Data.Meals.Remove(meal);
            this.Data.SaveChanges();

            return this.Ok(
                new
                {
                    message = "Meal #" + meal.Id + " deleted."
                });
        }
    }
}