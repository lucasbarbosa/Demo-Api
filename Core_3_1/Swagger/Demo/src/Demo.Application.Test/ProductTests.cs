using AutoMapper;
using Demo.Application.Automapper;
using Demo.Application.Services;
using Demo.Application.ViewModels;
using Demo.Domain.Entities;
using Demo.Domain.Interfaces;
using Demo.Infra.Data.Interfaces;
using Moq;
using Xunit;

namespace Demo.Application.Test
{
    public class ProductTests
    {
        #region Properties

        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public ProductTests()
        {
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutomapperConfig());
            });
            _mapper = mockMapper.CreateMapper();
        }

        #endregion

        #region Pubic Methods

        [Fact]
        public void Product_Create_OK()
        {
            // Arrange
            var notificator = new Mock<INotificatorHandler>();
            var ProductRepository = new Mock<IProductRepository>();
            var ProductApplication = new ProductAppService(_mapper, notificator.Object, ProductRepository.Object);
            var ProductFake = NewProduct();
            var ProductMapperView = _mapper.Map<ProductViewModel>(ProductFake);
            ProductRepository.Setup(x => x.Create(It.IsAny<Product>())).Returns(ProductFake);

            // Act
            var Product = ProductApplication.Create(ProductMapperView);

            // Asset
            Assert.NotNull(Product);
        }

        [Fact]
        public void Product_Create_Null()
        {
            // Arrange
            var notificator = new Mock<INotificatorHandler>();
            var ProductRepository = new Mock<IProductRepository>();
            var ProductApplication = new ProductAppService(_mapper, notificator.Object, ProductRepository.Object);
            var ProductFake = NewProduct();
            var ProductMapperView = _mapper.Map<ProductViewModel>(ProductFake);

            // Act
            var Product = ProductApplication.Create(ProductMapperView);

            // Asset
            Assert.Null(Product);
        }

        [Fact]
        public void Product_Update_OK()
        {
            // Arrange
            var notificator = new Mock<INotificatorHandler>();
            var ProductRepository = new Mock<IProductRepository>();
            var ProductApplication = new ProductAppService(_mapper, notificator.Object, ProductRepository.Object);
            var ProductFake = NewProduct();
            var ProductMapperView = _mapper.Map<ProductViewModel>(ProductFake);
            ProductRepository.Setup(x => x.Update(It.IsAny<Product>())).Returns(ProductFake);

            // Act
            var Product = ProductApplication.Update(ProductMapperView);

            // Asset
            Assert.NotNull(Product);
        }

        [Fact]
        public void Product_Update_Null()
        {
            // Arrange
            var notificator = new Mock<INotificatorHandler>();
            var ProductRepository = new Mock<IProductRepository>();
            var ProductApplication = new ProductAppService(_mapper, notificator.Object, ProductRepository.Object);
            var ProductFake = NewProduct();
            var ProductMapperView = _mapper.Map<ProductViewModel>(ProductFake);

            // Act
            var Product = ProductApplication.Update(ProductMapperView);

            // Asset
            Assert.Null(Product);
        }

        [Fact]
        public void Product_Delete_Ok()
        {
            // Arrange
            var notificator = new Mock<INotificatorHandler>();
            var ProductRepository = new Mock<IProductRepository>();
            var ProductApplication = new ProductAppService(_mapper, notificator.Object, ProductRepository.Object);
            uint ProductIdDelete = 1;
            ProductRepository.Setup(x => x.DeleteById(ProductIdDelete)).Returns(true);

            // Act
            var response = ProductApplication.DeleteById(ProductIdDelete);

            // Asset
            Assert.True(response);
        }

        [Fact]
        public void Product_Delete_Nok()
        {
            // Arrange
            var notificator = new Mock<INotificatorHandler>();
            var ProductRepository = new Mock<IProductRepository>();
            var ProductApplication = new ProductAppService(_mapper, notificator.Object, ProductRepository.Object);
            uint ProductIdDelete = 1;
            ProductRepository.Setup(x => x.DeleteById(ProductIdDelete)).Returns(false);

            // Act
            var response = ProductApplication.DeleteById(ProductIdDelete);

            // Asset
            Assert.False(response);
        }

        #endregion

        #region Private  Methods

        private Product NewProduct()
        {
            return new Product { Id = 1, Name = "Teste Nome", Weight = 2.3 };
        }

        #endregion
    }
}