using AutoMapper;
using Bogus;
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
            
            Randomizer.Seed = new Random(1234);
        }

        #endregion

        #region Public Methods

        [Fact]
        public async Task GetAll_ShouldReturnAllProducts_WhenRepositoryHasProducts()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            var productsFake = new List<Product>
            {
                NewProduct(),
                NewProduct(),
                NewProduct()
            };

            productRepository
                .Setup(x => x.GetAll())
                .ReturnsAsync(productsFake);

            // Act
            var result = await productApplication.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.All(result, item => Assert.IsType<ProductViewModel>(item));

            productRepository.Verify(
                x => x.GetAll(),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmptyList_WhenRepositoryHasNoProducts()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            var productsFake = new List<Product>();

            productRepository
                .Setup(x => x.GetAll())
                .ReturnsAsync(productsFake);

            // Act
            var result = await productApplication.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            productRepository.Verify(
                x => x.GetAll(),
                Times.Once
            );
        }

        [Fact]
        public async Task GetById_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            var productFake = NewProduct();
            uint productId = 1;

            productRepository
                .Setup(x => x.GetById(productId))
                .ReturnsAsync(productFake);

            // Act
            var result = await productApplication.GetById(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productFake.Name, result.Name);
            Assert.Equal(productFake.Weight, result.Weight);

            productRepository.Verify(
                x => x.GetById(productId),
                Times.Once
            );

            notificator.Verify(
                x => x.AddError(It.IsAny<string>()),
                Times.Never
            );
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            uint productId = 99999;

            productRepository
                .Setup(x => x.GetById(productId))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await productApplication.GetById(productId);

            // Assert
            Assert.Null(result);

            productRepository.Verify(
                x => x.GetById(productId),
                Times.Once
            );

            notificator.Verify(
                x => x.AddError("Product was not found"),
                Times.Once
            );
        }

        [Fact]
        public async Task Create_ShouldReturnProduct_WhenRepositoryCreatesSuccessfully()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            var productFake = NewProduct();
            var productViewModel = _mapper.Map<ProductViewModel>(productFake);

            productRepository
                .Setup(x => x.GetByName(productViewModel.Name))
                .ReturnsAsync((Product?)null);

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
                x => x.GetByName(productViewModel.Name),
                Times.Once
            );

            productRepository.Verify(
                x => x.Create(It.IsAny<Product>()),
                Times.Once
            );

            notificator.Verify(
                x => x.AddError(It.IsAny<string>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Create_ShouldReturnNull_WhenProductAlreadyExists()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            var productFake = NewProduct();
            var productViewModel = _mapper.Map<ProductViewModel>(productFake);

            productRepository
                .Setup(x => x.GetByName(productViewModel.Name))
                .ReturnsAsync(productFake);

            // Act
            var result = await productApplication.Create(productViewModel);

            // Assert
            Assert.Null(result);

            productRepository.Verify(
                x => x.GetByName(productViewModel.Name),
                Times.Once
            );

            productRepository.Verify(
                x => x.Create(It.IsAny<Product>()),
                Times.Never
            );

            notificator.Verify(
                x => x.AddError($"Product ({productViewModel.Name}) is already registered"),
                Times.Once
            );
        }

        [Fact]
        public async Task Create_ShouldReturnNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            var productFake = NewProduct();
            var productViewModel = _mapper.Map<ProductViewModel>(productFake);

            productRepository
                .Setup(x => x.GetByName(productViewModel.Name))
                .ReturnsAsync((Product?)null);

            productRepository.Setup(x => x.Create(It.IsAny<Product>())).ReturnsAsync(default(Product)!);

            // Act
            var result = await productApplication.Create(productViewModel);

            // Assert
            Assert.Null(result);

            productRepository.Verify(
                x => x.GetByName(productViewModel.Name),
                Times.Once
            );

            productRepository.Verify(
                x => x.Create(It.IsAny<Product>()),
                Times.Once
            );

            notificator.Verify(
                x => x.AddError("Product could not be created"),
                Times.Once
            );
        }

        [Fact]
        public async Task Create_ShouldReturnNull_WhenProductNameIsEmpty()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            var productViewModelWithoutName = new ProductViewModel
            {
                Name = string.Empty,
                Weight = 2.5
            };

            // Act
            var result = await productApplication.Create(productViewModelWithoutName);

            // Assert
            Assert.Null(result);

            productRepository.Verify(
                x => x.GetByName(It.IsAny<string>()),
                Times.Never
            );

            productRepository.Verify(
                x => x.Create(It.IsAny<Product>()),
                Times.Never
            );

            notificator.Verify(
                x => x.AddError("Product name is required"),
                Times.Once
            );
        }

        [Fact]
        public async Task Create_ShouldReturnNull_WhenProductNameIsNull()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            var productViewModelWithNullName = new ProductViewModel
            {
                Name = null!,
                Weight = 2.5
            };

            // Act
            var result = await productApplication.Create(productViewModelWithNullName);

            // Assert
            Assert.Null(result);

            productRepository.Verify(
                x => x.GetByName(It.IsAny<string>()),
                Times.Never
            );

            productRepository.Verify(
                x => x.Create(It.IsAny<Product>()),
                Times.Never
            );

            notificator.Verify(
                x => x.AddError("Product name is required"),
                Times.Once
            );
        }

        [Fact]
        public async Task Create_ShouldReturnNull_WhenProductIsNull()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            ProductViewModel? nullProduct = null;

            // Act
            var result = await productApplication.Create(nullProduct!);

            // Assert
            Assert.Null(result);

            productRepository.Verify(
                x => x.GetByName(It.IsAny<string>()),
                Times.Never
            );

            productRepository.Verify(
                x => x.Create(It.IsAny<Product>()),
                Times.Never
            );

            notificator.Verify(
                x => x.AddError("Product could not be created"),
                Times.Once
            );
        }

        [Fact]
        public async Task Update_ShouldReturnTrue_WhenRepositoryUpdatesSuccessfully()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            var productFake = NewProduct();
            var productViewModel = _mapper.Map<ProductViewModel>(productFake);

            productRepository
                .Setup(x => x.GetById(productViewModel.Id))
                .ReturnsAsync(productFake);

            productRepository
                .Setup(x => x.Update(It.IsAny<Product>()))
                .ReturnsAsync(true);

            // Act
            var result = await productApplication.Update(productViewModel);

            // Assert
            Assert.True(result);

            productRepository.Verify(
                x => x.GetById(productViewModel.Id),
                Times.Once
            );

            productRepository.Verify(
                x => x.Update(It.IsAny<Product>()),
                Times.Once
            );

            notificator.Verify(
                x => x.AddError(It.IsAny<string>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Update_ShouldReturnFalse_WhenProductDoesNotExist()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            var productViewModel = _mapper.Map<ProductViewModel>(NewProduct());

            productRepository
                .Setup(x => x.GetById(productViewModel.Id))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await productApplication.Update(productViewModel);

            // Assert
            Assert.False(result);

            productRepository.Verify(
                x => x.GetById(productViewModel.Id),
                Times.Once
            );

            productRepository.Verify(
                x => x.Update(It.IsAny<Product>()),
                Times.Never
            );

            notificator.Verify(
                x => x.AddError("Product was not found"),
                Times.Once
            );
        }

        [Fact]
        public async Task Update_ShouldReturnFalse_WhenProductIsNull()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            ProductViewModel? nullProduct = null;

            // Act
            var result = await productApplication.Update(nullProduct!);

            // Assert
            Assert.False(result);

            productRepository.Verify(
                x => x.GetById(It.IsAny<uint>()),
                Times.Never
            );

            productRepository.Verify(
                x => x.Update(It.IsAny<Product>()),
                Times.Never
            );

            notificator.Verify(
                x => x.AddError("Product could not be updated"),
                Times.Once
            );
        }

        [Fact]
        public async Task Update_ShouldReturnFalse_WhenProductNameIsEmpty()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            var productViewModelWithoutName = new ProductViewModel
            {
                Name = string.Empty,
                Weight = 2.5
            };

            // Act
            var result = await productApplication.Update(productViewModelWithoutName);

            // Assert
            Assert.False(result);

            productRepository.Verify(
                x => x.GetByName(It.IsAny<string>()),
                Times.Never
            );

            productRepository.Verify(
                x => x.Create(It.IsAny<Product>()),
                Times.Never
            );

            notificator.Verify(
                x => x.AddError("Product name is required"),
                Times.Once
            );
        }

        [Fact]
        public async Task Update_ShouldReturnFalse_WhenProductNameIsNull()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            var productViewModelWithNullName = new ProductViewModel
            {
                Name = null!,
                Weight = 2.5
            };

            // Act
            var result = await productApplication.Update(productViewModelWithNullName);

            // Assert
            Assert.False(result);

            productRepository.Verify(
                x => x.GetById(It.IsAny<uint>()),
                Times.Never
            );

            productRepository.Verify(
                x => x.Update(It.IsAny<Product>()),
                Times.Never
            );

            notificator.Verify(
                x => x.AddError("Product name is required"),
                Times.Once
            );
        }

        [Fact]
        public async Task Update_ShouldReturnFalse_WhenRepositoryUpdateFails()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            var productFake = NewProduct();
            var productViewModel = _mapper.Map<ProductViewModel>(productFake);

            productRepository
                .Setup(x => x.GetById(productViewModel.Id))
                .ReturnsAsync(productFake);

            productRepository
                .Setup(x => x.Update(It.IsAny<Product>()))
                .ReturnsAsync(false);

            // Act
            var result = await productApplication.Update(productViewModel);

            // Assert
            Assert.False(result);

            productRepository.Verify(
                x => x.GetById(productViewModel.Id),
                Times.Once
            );

            productRepository.Verify(
                x => x.Update(It.IsAny<Product>()),
                Times.Once
            );

            notificator.Verify(
                x => x.AddError("Product could not be updated"),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteById_ShouldReturnTrue_WhenRepositoryDeletesSuccessfully()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            uint productId = 1;
            var productFake = NewProduct();

            productRepository
                .Setup(x => x.GetById(productId))
                .ReturnsAsync(productFake);

            productRepository
                .Setup(x => x.DeleteById(productId))
                .ReturnsAsync(true);

            // Act
            var result = await productApplication.DeleteById(productId);

            // Assert
            Assert.True(result);

            productRepository.Verify(
                x => x.GetById(productId),
                Times.Once
            );

            productRepository.Verify(
                x => x.DeleteById(productId),
                Times.Once
            );

            notificator.Verify(
                x => x.AddError(It.IsAny<string>()),
                Times.Never
            );
        }

        [Fact]
        public async Task DeleteById_ShouldReturnFalse_WhenProductDoesNotExist()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            uint productId = 999999;

            productRepository
                .Setup(x => x.GetById(productId))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await productApplication.DeleteById(productId);

            // Assert
            Assert.False(result);

            productRepository.Verify(
                x => x.GetById(productId),
                Times.Once
            );

            productRepository.Verify(
                x => x.DeleteById(productId),
                Times.Never
            );

            notificator.Verify(
                x => x.AddError("Product was not found"),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteById_ShouldReturnFalse_WhenRepositoryDeleteFails()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            uint productId = 1;
            var productFake = NewProduct();

            productRepository
                .Setup(x => x.GetById(productId))
                .ReturnsAsync(productFake);

            productRepository
                .Setup(x => x.DeleteById(productId))
                .ReturnsAsync(false);

            // Act
            var result = await productApplication.DeleteById(productId);

            // Assert
            Assert.False(result);

            productRepository.Verify(
                x => x.GetById(productId),
                Times.Once
            );

            productRepository.Verify(
                x => x.DeleteById(productId),
                Times.Once
            );

            notificator.Verify(
                x => x.AddError("Product could not be deleted"),
                Times.Once
            );
        }

        #endregion

        #region Private Methods

        private (Mock<INotificatorHandler>, Mock<IProductRepository>, ProductAppService) SetProductAppService()
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

        private static Product NewProduct()
        {
            var faker = new Faker<Product>()
                .RuleFor(p => p.Id, f => 0u)
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Weight, f => Math.Round(f.Random.Double(0.1, 10.0), 2));

            return faker.Generate();
        }

        #endregion
    }
}