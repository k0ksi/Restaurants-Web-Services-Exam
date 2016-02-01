using System.ComponentModel.DataAnnotations;

namespace Restaurants.Services.Models.BindingModels
{
    public class MealBindingModel
    {
        [Required]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int TypeId { get; set; }

        public int RestaurantId { get; set; }
    }
}