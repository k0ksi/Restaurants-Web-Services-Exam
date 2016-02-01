using System.Web.Http;
using Restaurants.Data;
using Restaurants.Data.UnitOfWork;

namespace Restaurants.Services.Controllers
{
    public class BaseApiController : ApiController
    {
        private IRestaurantsData data;

        protected IRestaurantsData Data { get; private set; }

        public BaseApiController()
            : this(new RestaurantsData(new RestaurantsContext()))
        {
        }

        public BaseApiController(IRestaurantsData data)
        {
            this.Data = data;
        }
    }
}