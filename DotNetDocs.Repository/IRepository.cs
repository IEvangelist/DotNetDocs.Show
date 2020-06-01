using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DotNetDocs.Repository
{
    public interface IRepository<T> where T : Document
    {
        ValueTask<T?> GetAsync(string id);

        ValueTask<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate);

        ValueTask<T> CreateAsync(T value);

        Task<T[]> CreateAsync(IEnumerable<T> values);

        ValueTask<T> UpdateAsync(T value);

        ValueTask<T> DeleteAsync(string id);
    }
}