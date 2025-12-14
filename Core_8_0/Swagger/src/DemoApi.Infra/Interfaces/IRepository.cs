using DemoApi.Domain.Entities;

namespace DemoApi.Infra.Data.Interfaces
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        IList<TEntity> GetAll() => new List<TEntity>();

        TEntity Create(TEntity entity) => default;

        bool Update(TEntity entity) => default;
    }
}