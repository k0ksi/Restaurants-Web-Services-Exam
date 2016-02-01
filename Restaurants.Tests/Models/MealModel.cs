namespace Restaurants.Tests.Models
{
    public class MealModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int RestaurantId { get; set; }

        public int TypeId { get; set; }
    }
}