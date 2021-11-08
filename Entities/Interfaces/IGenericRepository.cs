using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> All();
        Task<T> GetById(int id);
        Task<bool> Add(T entity);
        Task<bool> Delete(int id);
        Task<bool> Insert(T entity);
        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate);

        Task<T> FindOne(Expression<Func<T, bool>> predicate);
    }
}
