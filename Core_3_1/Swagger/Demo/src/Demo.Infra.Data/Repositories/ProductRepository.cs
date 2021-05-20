using Demo.Domain.Entities;
using Demo.Infra.Data.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Demo.Infra.Data.Repositories
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

        public Product GetById(uint id)
        {
            if (_memoryProducts.Any(u => u.Id == id))
            {
                var Product = _memoryProducts.FirstOrDefault(u => u.Id == id);

                return Product;
            }
            else
            {
                return null;
            }
        }

        public Product Create(Product Product)
        {
            Product.Id = NewId();
            
            _memoryProducts.Add(Product);
            Product = _memoryProducts.FirstOrDefault(u => u.Id == Product.Id);

            return Product;
        }

        public Product Update(Product Product)
        {
            if (_memoryProducts.Any(u => u.Id == Product.Id))
            {
                var index = _memoryProducts.FindIndex(u => u.Id == Product.Id);
                _memoryProducts[index] = Product;
                Product = _memoryProducts.FirstOrDefault(u => u.Id == Product.Id);

                return Product;
            }
            else
            {
                return null;
            }
        }

        public bool DeleteById(uint id)
        {
            if(_memoryProducts.Any(x => x.Id == id))
            {
                _memoryProducts.RemoveAll(x => x.Id == id);
                var delete = !_memoryProducts.Any(x => x.Id == id);

                return delete;
            }
            else
            {
                return false;
            }
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