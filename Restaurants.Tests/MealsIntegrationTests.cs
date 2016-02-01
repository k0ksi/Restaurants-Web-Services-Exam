using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Http;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Owin;
using Restaurants.Data;
using Restaurants.Models;
using Restaurants.Services;

namespace Restaurants.Tests
{
    [TestClass]
    public class MealsIntegrationTests
    {
        [TestMethod]
        public void EditMeal_NonExistingMeal_ShouldNotReturnMealForEditing()
        {
            using (var dbContext = new RestaurantsContext())
            {
                var httpTestServer = TestServer.Create(appBuilder =>
                {
                    var config = new HttpConfiguration();
                    WebApiConfig.Register(config);
                    appBuilder.UseWebApi(config);
                });

                // Arrange

                // Clean Database
                CleanDatabase();

                // Create restaurant

                var restaurant = new Restaurant()
                {
                    Id = 1,
                    Name = "Gurlata",
                    TownId = 1
                };

                // Create meal
                var meal = new Meal()
                {
                    Id = 1,
                    Name = "Shish kebab",
                    Price = 18.2m,
                    TypeId = 3,
                    RestaurantId = 1
                };

                // Act -> add restaurant and meal to database

                dbContext.Restaurants.Add(restaurant);
                dbContext.Meals.Add(meal);
                dbContext.SaveChanges();

                var mealWithNonExistingId = dbContext.Meals
                    .Where(m => m.Id == meal.Id + 1);

                // Assert -> check if non existing meal returns not found

                var fakeMealId = meal.Id + 1;

                var fakeMeal = dbContext.Meals.Find(fakeMealId);

                Assert.IsNull(fakeMeal);

                CleanDatabase();
            }
        }

        [ExpectedException(typeof(DbEntityValidationException))]
        [TestMethod]
        public void EditMeal_WithEmptyMealName_ShouldNotAllowMealEdition()
        {
            using (var dbContext = new RestaurantsContext())
            {
                var httpTestServer = TestServer.Create(appBuilder =>
                {
                    var config = new HttpConfiguration();
                    WebApiConfig.Register(config);
                    appBuilder.UseWebApi(config);
                });

                // Arrange

                // Clean Database
                CleanDatabase();

                // Create restaurant

                var restaurant = new Restaurant()
                {
                    Id = 1,
                    Name = "Gurlata",
                    TownId = 1
                };

                // Create meal
                var meal = new Meal()
                {
                    Id = 1,
                    Name = "Gozba",
                    Price = 18.2m,
                    TypeId = 3,
                    RestaurantId = 1
                };

                // Act -> add restaurant and meal to database

                dbContext.Restaurants.Add(restaurant);
                dbContext.Meals.Add(meal);
                dbContext.SaveChanges();

                var mealFromDb = dbContext.Meals.Find(meal.Id);

                mealFromDb.Name = "";
                dbContext.SaveChanges();

                CleanDatabase();
            }
        }

        [TestMethod]
        public void EditMeal_WithCorrectData_ShouldAllowMealEdition()
        {
            using (var dbContext = new RestaurantsContext())
            {
                var httpTestServer = TestServer.Create(appBuilder =>
                {
                    var config = new HttpConfiguration();
                    WebApiConfig.Register(config);
                    appBuilder.UseWebApi(config);
                });

                // Arrange

                // Clean Database
                CleanDatabase();

                // Create restaurant

                var restaurant = new Restaurant()
                {
                    Id = 1,
                    Name = "Gurlata",
                    TownId = 1
                };

                // Create meal
                var meal = new Meal()
                {
                    Id = 1,
                    Name = "Kibap4ita",
                    Price = 18.2m,
                    TypeId = 3,
                    RestaurantId = 1
                };

                // Act -> add restaurant and meal to database

                dbContext.Restaurants.Add(restaurant);
                dbContext.Meals.Add(meal);
                dbContext.SaveChanges();

                var mealFromDb = dbContext.Meals.Find(meal.Id);

                mealFromDb.Name = "Kufteta";
                dbContext.SaveChanges();

                var editedMealFromBd = dbContext.Meals.Find(mealFromDb.Id);

                Assert.AreEqual(mealFromDb.Name, editedMealFromBd.Name);

                CleanDatabase();
            }
        }

        public static void CleanDatabase()
        {
            using (var dbContext = new RestaurantsContext())
            {
                foreach (var meal in dbContext.Meals)
                {
                    dbContext.Meals.Remove(meal);
                }

                foreach (var order in dbContext.Orders)
                {
                    dbContext.Orders.Remove(order);
                }

                foreach (var rating in dbContext.Ratings)
                {
                    dbContext.Ratings.Remove(rating);
                }

                foreach (var restaurant in dbContext.Restaurants)
                {
                    dbContext.Restaurants.Remove(restaurant);
                }
                dbContext.SaveChanges();
            }
        }
    }
}