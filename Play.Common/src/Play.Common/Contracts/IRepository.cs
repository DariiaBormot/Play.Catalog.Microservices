using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Play.Common.Contracts
{
    public interface IRepository<T> where T : IEntity
    {
        Task CreateAsync(T entity);
        Task<IReadOnlyCollection<T>> GetAllAsync();
        Task<IReadOnlyCollection<T>> FilterAllAsync(Expression<Func<T, bool>> filter);
        Task<T> GetByIdAsync(Guid id);
        Task<T> FilterOneAsync(Expression<Func<T, bool>> filter); 
        Task RemoveAsync(Guid id);
        Task UpdateAsync(T entity);
    }
}
