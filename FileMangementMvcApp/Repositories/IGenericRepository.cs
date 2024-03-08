using FileManagement.Domain;
using FileMangementMvcApp.Models;
using System.Linq.Expressions;

namespace FileMangementMvcApp.Repositories
{
    public interface IGenericRepository<T,C> where T : class where C : class
    {
        Task AddAsync(T item);

        Task UpdateAsync(Guid id, C item);
        Task DeleteAsync(Guid id);

        Task<List<T>> GetAllAsync();

        Task<T> GetItemAsync(Expression<Func<T, bool>> filter = null); // هذا البارامتر لوضع فلتر على البيانات
         
        Task Save();
    }
}
