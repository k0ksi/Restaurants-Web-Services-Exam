using System;
using System.Collections.Generic;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Restauranteur.Models;
using Restaurants.Data.Repositories;
using Restaurants.Models;

namespace Restaurants.Data.UnitOfWork
{
    public class RestaurantsData : IRestaurantsData
    {
        private readonly DbContext dbContext;

        private readonly IDictionary<Type, object> repositories;

        private IUserStore<ApplicationUser> userStore;

        public RestaurantsData()
            : this(new RestaurantsContext())
        {
        }

        public RestaurantsData(DbContext dbContext)
        {
            this.dbContext = dbContext;
            this.repositories = new Dictionary<Type, object>();
        }

        public IRepository<ApplicationUser> Users
        {
            get { return this.GetRepository<ApplicationUser>(); }
        }

        public IRepository<Meal> Meals
        {
            get { return this.GetRepository<Meal>(); }
        }

        public IRepository<MealType> MealTypes
        {
            get { return this.GetRepository<MealType>(); }
        }

        public IRepository<Order> Orders
        {
            get { return this.GetRepository<Order>(); }
        }

        public IRepository<Rating> Ratings
        {
            get { return this.GetRepository<Rating>(); }
        }

        public IRepository<Restaurant> Restaurants
        {
            get { return this.GetRepository<Restaurant>(); }
        }

        public IRepository<Town> Towns
        {
            get { return this.GetRepository<Town>(); }
        }

        public IUserStore<ApplicationUser> UserStore
        {
            get
            {
                if (this.userStore == null)
                {
                    this.userStore = new UserStore<ApplicationUser>(this.dbContext);
                }

                return this.userStore;
            }
        }
        public void SaveChanges()
        {
            this.dbContext.SaveChanges();
        }

        private IRepository<T> GetRepository<T>() where T : class
        {
            if (!this.repositories.ContainsKey(typeof(T)))
            {
                var type = typeof(GenericRepository<T>);
                this.repositories.Add(typeof(T),
                    Activator.CreateInstance(type, this.dbContext));
            }

            return (IRepository<T>)this.repositories[typeof(T)];
        }
    }
}