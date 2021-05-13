using Demo.Domain.Entities;
using System.Collections.Generic;

namespace Demo.Infra.Data.Interfaces
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        IList<TEntity> GetAll();

        TEntity Create(TEntity entity);

        TEntity Update(TEntity entity);
    }
}