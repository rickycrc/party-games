using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PartyGames.Data
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Table { get; }
        IQueryable<T> TableNoTracking { get; }
        T GetSingle(Expression<Func<T, bool>> predicate);
        T GetById(object id);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        int ExecuteSqlCommand(string sql, params object[] parameters);
        void Add(T entity);
        void Add(IEnumerable<T> entities);
        void Delete(T entity);
        void Delete(IEnumerable<T> entities);
        void Update(T entity);
        void Update(IEnumerable<T> entities);
    }
}
