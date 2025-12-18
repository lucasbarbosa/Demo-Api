using DemoApi.Application.Models;
using DemoApi.Domain.Entities;
using FluentAssertions;
using Moq;

namespace DemoApi.Application.Test.Products
{
    public class CreateProductTests : ProductTests
    {
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
            result.Should().NotBeNull();
            result.Name.Should().Be(productFake.Name);
            result.Weight.Should().Be(productFake.Weight);

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
            result.Should().BeNull();

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
            result.Should().BeNull();

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
        public async Task Create_ShouldReturnNull_WhenProductIsNull()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            ProductViewModel? nullProduct = null;


            // Act
            var result = await productApplication.Create(nullProduct!);


            // Assert
            result.Should().BeNull();

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
    }
}