using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Restaurants.Models;
using Restaurants.Services.Models.BindingModels;

namespace Restaurants.Services.Controllers
{
    public class RatesController : BaseApiController
    {
        // POST api/restaurants/{id}/rate
        [HttpPost]
        [Authorize]
        [Route("api/restaurants/{id}/rate")]
        public IHttpActionResult RateExistingRestaurant(int id, [FromBody]RateBindingModel model)
        {
            if (model == null)
            {
                return this.BadRequest("Model cannot be null (no data in request)");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Invalid rating. The rating value must be between 1 and 10.");
            }

            var restaurant = this.Data.Restaurants.Find(id);

            if (restaurant == null)
            {
                return this.NotFound();
            }

            var loggedUserId = this.User.Identity.GetUserId();
            var user = this.Data.Users.Find(loggedUserId);

            if (restaurant.OwnerId == loggedUserId)
            {
                return this.BadRequest("The owner cannot rate his own restaurant.");
            }

            if (user.GivenRatings
                .Any(r => r.RestaurantId == id))
            {
                var ratingFromDb = this.Data.Ratings.All()
                    .FirstOrDefault(r => r.RestaurantId == id && r.UserId == loggedUserId);

                if (ratingFromDb != null)
                {
                    ratingFromDb.Stars = model.Stars;
                }

                this.Data.SaveChanges();

                return this.Ok();
            };

            var rating = new Rating()
            {
                RestaurantId = id,
                Restaurant = restaurant,
                Stars = model.Stars,
                User = user,
                UserId = loggedUserId
            };

            this.Data.Ratings.Add(rating);
            this.Data.SaveChanges();

            return this.Ok();
        } 
    }
}