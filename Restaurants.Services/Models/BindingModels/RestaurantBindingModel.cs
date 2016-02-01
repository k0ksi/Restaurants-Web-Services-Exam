using System.ComponentModel.DataAnnotations;

namespace Restaurants.Services.Models.BindingModels
{
    public class RestaurantBindingModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int TownId { get; set; }
    }
}