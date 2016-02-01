using System.Data.Entity;
using System.Linq;

namespace Restaurants.Data.Repositories
{
    public class GenericRepository<T> : IRepository<T>
        where T : class 
    {
        private DbContext dbContext;
        private IDbSet<T> entitySet;

        public GenericRepository(DbContext context)
        {
            this.dbContext = context;
            this.entitySet = dbContext.Set<T>();
        }

        public IDbSet<T> EntitySet
        {
            get { return this.entitySet; }
        } 

        public IQueryable<T> All()
        {
            return this.entitySet;
        }

        public T Find(object id)
        {
            return this.entitySet.Find(id);
        }

        public void Add(T entity)
        {
            this.ChangeState(entity, EntityState.Added);
        }

        public void Update(T entity)
        {
            this.ChangeState(entity, EntityState.Modified);
        }

        public void Remove(T entity)
        {
            this.ChangeState(entity, EntityState.Deleted);
        }

        public T Remove(object id)
        {
            var entity = this.Find(id);
            this.Remove(entity);
            return entity;
        }

        public void SaveChanges()
        {
            this.dbContext.SaveChanges();
        }

        private void ChangeState(T entity, EntityState state)
        {
            var entry = this.dbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.entitySet.Attach(entity);
            }

            entry.State = state;
        }
    }
}