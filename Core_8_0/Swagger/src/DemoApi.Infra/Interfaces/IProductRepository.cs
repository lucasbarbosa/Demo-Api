using DemoApi.Domain.Entities;

namespace DemoApi.Infra.Data.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Product GetById(uint id);

        Product GetByName(string name);

        bool DeleteById(uint id);
    }
}