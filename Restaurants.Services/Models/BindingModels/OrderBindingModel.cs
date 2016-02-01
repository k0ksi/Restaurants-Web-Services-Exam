using System.ComponentModel.DataAnnotations;

namespace Restaurants.Services.Models.BindingModels
{
    public class OrderBindingModel
    {
        [Required]
        public int Quantity { get; set; } 
    }
}