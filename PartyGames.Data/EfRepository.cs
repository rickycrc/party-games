using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace PartyGames.Data
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _context;
        private IDbSet<T> _entities;

        public EfRepository(DbContext dbContext)
        {
            _context = dbContext;
        }

        protected IDbSet<T> Entities => _entities ?? (_entities = _context.Set<T>());

        public IQueryable<T> Table => Entities;

        public IQueryable<T> TableNoTracking => Entities.AsNoTracking();

        public int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return _context.Database.ExecuteSqlCommand(sql, parameters);
        }

        public void Add(T entity)
        {
            Entities.Add(entity);
            _context.SaveChanges();
        }

        public void Add(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
                Entities.Add(entity);
            _context.SaveChanges();
        }

        public void Delete(T entity)
        {
            Entities.Remove(entity);
            _context.SaveChanges();
        }

        public void Delete(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
                Entities.Remove(entity);
            _context.SaveChanges();
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return Table.Where(predicate);
        }

        public T GetSingle(Expression<Func<T, bool>> predicate)
        {
            return Table.FirstOrDefault(predicate);
        }

        public T GetById(object id)
        {
            return Entities.Find(id);
        }

        public void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Update(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
                _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
