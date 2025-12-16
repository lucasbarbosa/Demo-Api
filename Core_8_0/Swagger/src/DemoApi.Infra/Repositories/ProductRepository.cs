using DemoApi.Domain.Entities;
using DemoApi.Infra.Data.Interfaces;

namespace DemoApi.Infra.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        #region Properties

        private static readonly List<Product> _memoryProducts = new List<Product>();

        #endregion

        #region Public Methods

        public async Task<IList<Product>> GetAll()
        {
            return await Task.FromResult(_memoryProducts);
        }

        public async Task<Product?> GetById(uint id)
        {
            return await Task.FromResult(_memoryProducts.FirstOrDefault(p => p.Id == id));
        }

        public async Task<Product?> GetByName(string name)
        {
            return await Task.FromResult(_memoryProducts
                .FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<Product> Create(Product product)
        {
            product.Id = NewId();
            _memoryProducts.Add(product);

            return await Task.FromResult(product);
        }

        public async Task<bool> Update(Product product)
        {
            var index = _memoryProducts.FindIndex(p => p.Id == product.Id);

            if (index < 0)
                return await Task.FromResult(false);

            _memoryProducts[index] = product;
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteById(uint id)
        {
            var removedCount = _memoryProducts.RemoveAll(p => p.Id == id);
            return await Task.FromResult(removedCount > 0);
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