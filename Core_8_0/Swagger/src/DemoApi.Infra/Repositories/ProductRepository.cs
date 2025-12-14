using DemoApi.Domain.Entities;
using DemoApi.Infra.Data.Interfaces;

namespace DemoApi.Infra.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        #region Properties

        private static readonly List<Product> _memoryProducts = new List<Product>();

        #endregion

        #region Pubic Methods

        public IList<Product> GetAll()
        {
            return _memoryProducts;
        }

        public Product? GetById(uint id)
        {
            return _memoryProducts.FirstOrDefault(p => p.Id == id);
        }

        public Product? GetByName(string name)
        {
            return _memoryProducts
                .FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public Product Create(Product product)
        {
            product.Id = NewId();
            _memoryProducts.Add(product);

            return product;
        }

        public bool Update(Product product)
        {
            var index = _memoryProducts.FindIndex(p => p.Id == product.Id);

            if (index < 0)
                return false;

            _memoryProducts[index] = product;
            return true;
        }

        public bool DeleteById(uint id)
        {
            var removedCount = _memoryProducts.RemoveAll(p => p.Id == id);
            return removedCount > 0;
        }

        #endregion

        #region Private Methods

        private uint NewId()
        {
            var id = _memoryProducts.Max(u => u?.Id) + 1;

            if (id == null)
                id = 1;

            return (uint)id;
        }

        #endregion
    }
}