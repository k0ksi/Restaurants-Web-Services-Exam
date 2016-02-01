using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Restaurants.Models;
using Restaurants.Services.Models.BindingModels;

namespace Restaurants.Services.Controllers
{
    [RoutePrefix("api/restaurants")]
    public class RestaurantsController : BaseApiController
    {
        // GET api/restaurants?townId={townId}
        [HttpGet]
        public IHttpActionResult GetRestaurantsByTown([FromUri] int townId)
        {
            var data = this.Data.Restaurants.All();

            var restaurantsByTown = data
                .Where(r => r.TownId == townId)
                .OrderByDescending(r => r.Ratings.Average(rate => rate.Stars))
                .ThenBy(r => r.Name)
                .Select(r => new
                {
                    id = r.Id,
                    name = r.Name,
                    rating = r.Ratings.Average(rate => rate.Stars),
                    town = new
                    {
                        id = r.TownId,
                        name = r.Town.Name
                    }
                });

            return this.Ok(restaurantsByTown);
        }

        // POST api/restaurants
        [HttpPost]
        [Authorize]
        public IHttpActionResult CreateRestaurant([FromBody]RestaurantBindingModel model)
        {
            if (model == null)
            {
                return this.BadRequest("Model cannot be null (no data in request)");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var loggedUserId = this.User.Identity.GetUserId();
            var user = this.Data.Users.Find(loggedUserId);

            if (user == null)
            {
                return this.Unauthorized();
            }

            var town = this.Data.Towns.Find(model.TownId);

            var restaurant = new Restaurant()
            {
                Name = model.Name,
                Owner = user,
                OwnerId = loggedUserId,
                TownId = model.TownId,
                Town = town
            };

            this.Data.Restaurants.Add(restaurant);
            this.Data.SaveChanges();

            return this.CreatedAtRoute(
                "DefaultApi",
                new {controller = "restaurans", id = restaurant.Id},
                new
                {
                    id = restaurant.Id,
                    name = restaurant.Name,
                    rating = (restaurant.Ratings.Any()) ?restaurant.Ratings.Average(r => r.Stars) : null,
                    town = new {id = restaurant.TownId, name = restaurant.Town.Name}
                });
        }
    }
}