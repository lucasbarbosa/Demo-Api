using Demo.Domain.Entities;

namespace Demo.Infra.Data.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Product GetById(uint id);

        bool DeleteById(uint id);
    }
}