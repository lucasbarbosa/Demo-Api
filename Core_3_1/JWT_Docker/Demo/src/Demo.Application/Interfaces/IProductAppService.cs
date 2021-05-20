using Demo.Application.ViewModels;
using System.Collections.Generic;

namespace Demo.Application.Interfaces
{
    public interface IProductAppService
    {
        IList<ProductViewModel> GetAll();

        ProductViewModel GetById(uint id);

        ProductViewModel Create(ProductViewModel Product);

        ProductViewModel Update(ProductViewModel Product);

        bool DeleteById(uint id);
    }
}