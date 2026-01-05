using AutoMapper;
using Bogus;
using DemoApi.Application.Automapper;
using DemoApi.Application.Services;
using DemoApi.Domain.Interfaces;
using Moq;

namespace DemoApi.Application.Test.Products
{
    public class ProductTests
    {
        #region Properties

        protected readonly IMapper _mapper;

        #endregion

        #region Constructors

        public ProductTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutomapperConfig());
            });

            config.AssertConfigurationIsValid();
            _mapper = config.CreateMapper();
            
            Randomizer.Seed = new Random(1234);
        }

        #endregion

        #region Protected Methods

        protected (Mock<INotificatorHandler>, Mock<IProductRepository>, ProductAppService) SetProductAppService()
        {
            var notificator = new Mock<INotificatorHandler>();
            var productRepository = new Mock<IProductRepository>();
            var productApplication = new ProductAppService(
                _mapper,
                notificator.Object,
                productRepository.Object
            );

            return (notificator, productRepository, productApplication);
        }

        #endregion
    }
}