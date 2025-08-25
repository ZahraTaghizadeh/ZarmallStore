using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ZarmallStore.Data.Context;
using ZarmallStore.Data.Entities.Common;

namespace ZarmallStore.Data.Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<TEntity> _dbset;
        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            this._dbset = _dbContext.Set<TEntity>();
        }
        public async ValueTask DisposeAsync()
        {
            if (_dbContext != null)
            {
                await _dbContext.DisposeAsync();
            }
        }
        public IQueryable<TEntity> GetQuery()
        {
            return _dbset.AsQueryable();
        }
        public async Task<TEntity> GetEntityById(long id)
        {
            return await _dbset.SingleOrDefaultAsync(d => d.Id == id);
        }
        public async Task AddEntity(TEntity entity)
        {
            entity.CreatDate = DateTime.Now;
            entity.LastUpdateDate = DateTime.Now;
            await _dbset.AddAsync(entity);
        }
        public async Task AddRangeEntities(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreatDate = DateTime.Now;
                entity.LastUpdateDate = DateTime.Now;
                await _dbset.AddAsync(entity);
            }
        }
        public async void EditEntity(TEntity entity)
        {
            entity.LastUpdateDate = DateTime.Now;
            _dbset.Update(entity);
        }
        public async void DeleteEntity(TEntity entity)
        {
            entity.IsDeleted = true;
            entity.LastUpdateDate = DateTime.Now;
            _dbset.Update(entity);
        }
        public async void DeleteEntity(List<TEntity> entities)
        {
            foreach (var item in entities)
            {
                item.IsDeleted = true;
                item.LastUpdateDate = DateTime.Now;
                _dbset.Update(item);

            }
        }
        public async Task DeletePermanent(TEntity entity)
        {
            _dbset.Remove(entity);
        }
        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
