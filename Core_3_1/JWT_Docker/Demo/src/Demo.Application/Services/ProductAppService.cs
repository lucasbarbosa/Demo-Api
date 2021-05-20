using AutoMapper;
using Demo.Application.Interfaces;
using Demo.Application.ViewModels;
using Demo.Domain.Entities;
using Demo.Domain.Interfaces;
using Demo.Infra.Data.Interfaces;
using System.Collections.Generic;

namespace Demo.Application.Services
{
    public class ProductAppService : BaseServices, IProductAppService
    {
        #region Properties

        private readonly IMapper _mapper;
        private readonly INotificatorHandler _notificator;
        private readonly IProductRepository _ProductRepository;

        #endregion

        #region Constructors

        public ProductAppService(IMapper mapper, INotificatorHandler notificator, IProductRepository ProductRepository)
        {
            _mapper = mapper;
            _notificator = notificator;
            _ProductRepository = ProductRepository;
        }

        #endregion

        #region Public Methods

        public IList<ProductViewModel> GetAll()
        {
            var response = _mapper.Map<IList<ProductViewModel>>(_ProductRepository.GetAll());

            return response;
        }

        public ProductViewModel GetById(uint id)
        {
            var response = _mapper.Map<ProductViewModel>(_ProductRepository.GetById(id));

            if (response == null)
                _notificator.AddError("Usuário não encontrado");

            return response;
        }

        public ProductViewModel Create(ProductViewModel Product)
        {
            var response = _mapper.Map<ProductViewModel>(_ProductRepository.GetById(Product.Id));

            if (response != null)
            {
                _notificator.AddError($"Usuário (Id: {Product.Id}) já cadastrado.");
            }
            else
            {
                response = _mapper.Map<ProductViewModel>(_ProductRepository.Create(_mapper.Map<Product>(Product)));

                if (response == null)
                    _notificator.AddError("Usuário não cadastrado");
            }

            return response;
        }

        public ProductViewModel Update(ProductViewModel Product)
        {
            var response = _mapper.Map<ProductViewModel>(_ProductRepository.Update(_mapper.Map<Product>(Product)));

            if (response == null)
                _notificator.AddError("Usuário não encontrado");

            return response;
        }

        public bool DeleteById(uint id)
        {
            var response = _ProductRepository.DeleteById(id);

            if (!response)
                _notificator.AddError("Usuário não encontrado");

            return response;
        }

        #endregion
    }
}