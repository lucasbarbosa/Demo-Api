using AutoMapper;
using DemoApi.Application.Automapper;
using DemoApi.Application.Models;
using DemoApi.Application.Services;
using DemoApi.Domain.Entities;
using DemoApi.Domain.Interfaces;
using DemoApi.Infra.Data.Interfaces;
using Moq;

namespace DemoApi.Application.Test
{
    public class ProductTests
    {
        #region Properties

        private readonly IMapper _mapper;

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
        }

        #endregion

        #region Pubic Methods

        [Fact]
        public async Task Create_ShouldReturnProduct_WhenRepositoryCreatesSuccessfully()
        {
            // Arrange
            var notificator = new Mock<INotificatorHandler>();
            var productRepository = new Mock<IProductRepository>();
            var productApplication = new ProductAppService(
                _mapper,
                notificator.Object,
                productRepository.Object
            );

            var productFake = NewProduct();
            var productViewModel = _mapper.Map<ProductViewModel>(productFake);

            productRepository
                .Setup(x => x.Create(It.IsAny<Product>()))
                .ReturnsAsync(productFake);

            // Act
            var result = await productApplication.Create(productViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productFake.Name, result.Name);
            Assert.Equal(productFake.Weight, result.Weight);

            productRepository.Verify(
                x => x.Create(It.IsAny<Product>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Create_ShouldReturnNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            var notificator = new Mock<INotificatorHandler>();
            var productRepository = new Mock<IProductRepository>();
            var productApplication = new ProductAppService(
                _mapper,
                notificator.Object,
                productRepository.Object
            );

            var productFake = NewProduct();
            var productViewModel = _mapper.Map<ProductViewModel>(productFake);

            //productRepository
            //    .Setup(x => x.Create(It.IsAny<Product>()))
            //    .ReturnsAsync((Product)null);

            //productRepository?
            //    .Setup(x => x.Create(It.IsAny<Product>()))
            //    .ReturnsAsync((Product?)null);

            productRepository.Setup(x => x.Create(It.IsAny<Product>()))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await productApplication.Create(productViewModel);

            // Assert
            Assert.Null(result);
        }

        #endregion
        
        #region Private Methods

        private static Product NewProduct()
        {
            return new Product
            {
                Name = "Product Test",
                Weight = 2.3
            };
        }

        #endregion
    }
}