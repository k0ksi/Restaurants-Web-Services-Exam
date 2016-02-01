using System;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Restaurants.Models;
using Restaurants.Services.Models.BindingModels;

namespace Restaurants.Services.Controllers
{
    public class OrdersController : BaseApiController
    {
        // POST api/meals/{id}/order
        [HttpPost]
        [Authorize]
        [Route("api/meals/{id}/order")]
        public IHttpActionResult CreateOrder(int id, [FromBody] OrderBindingModel model)
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
            var user = this.Data.Users.Find(loggedUserId);
            if (user == null)
            {
                return this.Unauthorized();
            }

            var order = new Order()
            {
                Meal = meal,
                MealId = id,
                CreatedOn = DateTime.Now,
                Quantity = model.Quantity,
                User = user,
                UserId = loggedUserId
            };

            this.Data.Orders.Add(order);
            this.Data.SaveChanges();

            return this.Ok();
        }

        // GET api/orders?startPage={start-page}&limit={page-size}&mealId={mealId}
        [HttpGet]
        [Authorize]
        [Route("api/orders")]
        public IHttpActionResult ViewPendingOrders([FromUri] int startPage = 0, [FromUri] int limit = 2,
            [FromUri] int? mealId = null)
        {
            var loggedUserId = this.User.Identity.GetUserId();
            var user = this.Data.Users.Find(loggedUserId);
            if (user == null)
            {
                return this.Unauthorized();
            }

            var orders = this.Data.Orders.All()
                .Where(o => o.UserId == loggedUserId && 
                    o.OrderStatus == OrderStatus.Pending);

            if (mealId != null)
            {
                orders = orders
                    .Where(o => o.MealId == mealId.Value);
            }

            var numberOfSkippedOrders = startPage * limit;
            orders = orders
                .OrderByDescending(o => o.CreatedOn)
                .Skip(numberOfSkippedOrders);

            var result = orders
                .Select(o => new
                {
                    id = o.Id,
                    meal = new
                    {
                        id = o.MealId,
                        name = o.Meal.Name,
                        price = o.Meal.Price,
                        type = o.Meal.Type.Name
                    },
                    quantity = o.Quantity,
                    status = o.OrderStatus,
                    createdOn = o.CreatedOn
                });

            return this.Ok(result);
        }
    }
}