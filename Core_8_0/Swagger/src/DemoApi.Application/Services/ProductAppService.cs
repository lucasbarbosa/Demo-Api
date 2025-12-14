using AutoMapper;
using DemoApi.Application.Interfaces;
using DemoApi.Application.Models;
using DemoApi.Domain.Entities;
using DemoApi.Domain.Interfaces;
using DemoApi.Infra.Data.Interfaces;

namespace DemoApi.Application.Services
{
    public class ProductAppService : BaseServices, IProductAppService
    {
        #region Properties

        private readonly IMapper _mapper;
        private readonly INotificatorHandler _notificator;
        private readonly IProductRepository _productRepository;

        #endregion

        #region Constructors

        public ProductAppService(IMapper mapper, INotificatorHandler notificator, IProductRepository productRepository)
        {
            _mapper = mapper;
            _notificator = notificator;
            _productRepository = productRepository;
        }

        #endregion

        #region Public Methods

        public IList<ProductViewModel> GetAll()
        {
            var response = _mapper.Map<IList<ProductViewModel>>(_productRepository.GetAll());

            return response;
        }

        public ProductViewModel GetById(uint id)
        {
            var response = _mapper.Map<ProductViewModel>(_productRepository.GetById(id));

            if (response == null)
                _notificator.AddError("Product was not found");

            return response;
        }

        public ProductViewModel Create(ProductViewModel product)
        {
            var response = _mapper.Map<ProductViewModel>(_productRepository.GetByName(product.Name));

            if (response != null)
                _notificator.AddError($"Product ({product.Name}) is already registered.");
            else
            {
                response = _mapper.Map<ProductViewModel>(_productRepository.Create(_mapper.Map<Product>(product)));

                if (response == null)
                    _notificator.AddError("Product could not be created");
            }

            return response;
        }

        public bool Update(ProductViewModel product)
        {
            if (_productRepository.GetById(product.Id) is null)
            {
                _notificator.AddError("Product was not found");
                return false;
            }

            var response = _productRepository.Update(_mapper.Map<Product>(product));

            if (!response)
                _notificator.AddError("Product could not be updated");

            return response;
        }

        public bool DeleteById(uint id)
        {
            bool response = false;

            if (_productRepository.GetById(id) is null)
            {
                _notificator.AddError("Product was not found");
                return response;
            }

            response = _productRepository.DeleteById(id);

            if (!response)
                _notificator.AddError("Product could not be deleted");

            return response;
        }

        #endregion
    }
}