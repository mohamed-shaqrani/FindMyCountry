using Main.Entities;
using Main.Repository;

namespace Main.Common.Repository;
public interface IUnitOfWork
{
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity;
    Task<int> SaveChangesAsync();
}
