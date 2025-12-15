using DemoApi.Application.Models;

namespace DemoApi.Application.Interfaces
{
    public interface IProductAppService
    {
        Task<IList<ProductViewModel>> GetAll();

        Task<ProductViewModel?> GetById(uint id);

        Task<ProductViewModel?> Create(ProductViewModel product);

        Task<bool> Update(ProductViewModel product);

        Task<bool> DeleteById(uint id);
    }
}