using DemoApi.Application.Models;

namespace DemoApi.Application.Interfaces
{
    public interface IProductAppService
    {
        IList<ProductViewModel> GetAll();

        ProductViewModel GetById(uint id);

        ProductViewModel Create(ProductViewModel product);

        bool Update(ProductViewModel product);

        bool DeleteById(uint id);
    }
}