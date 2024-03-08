using FileManagement.Data;
using FileManagement.Domain;
using FileMangementMvcApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FileMangementMvcApp.Repositories
{

    // T من أجل نوع الكلاس الاصلي 
    // C من اجل نوع الموديل الذي يُستخدم للتعديل

    public class GenericRepository<T,C> : IGenericRepository<T, C> where T : class where C : class
    {
        private readonly FileManagementDBContext context;

        public GenericRepository(FileManagementDBContext context)
        {
            this.context = context;
        }
        public async Task AddAsync(T item)
        {
            await context .Set<T>().AddAsync(item);

            await Save();
        }

        public   async Task DeleteAsync(Guid id)
        {
            var item = await context.Set<T>().FindAsync(id);

            if (item != null)
                context.Set<T>().Remove(item);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await context.Set<T>().ToListAsync();

        }


        public async Task<T> GetItemAsync(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = context.Set<T>();
           
            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }



        public async Task Save()
        {
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, C item)
        {
            var existingFileUpload = await context.Set<T>().FindAsync(id);

            if (existingFileUpload != null)
            {
                context.Entry(existingFileUpload).CurrentValues.SetValues(item);
                await context.SaveChangesAsync();
            }
        }
    }
}
