using DemoApi.Application.Models.Products;
using DemoApi.Application.Test.Builders.Products;
using DemoApi.Domain.Entities;
using FluentAssertions;
using Moq;

namespace DemoApi.Application.Test.Products
{
    public class UpdateProductTests : ProductTests
    {
        [Fact]
        public async Task Update_ShouldReturnTrue_WhenRepositoryUpdatesSuccessfully()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            var productFake = ProductBuilder.New().Build();
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
            result.Should().BeTrue();

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

            var productViewModel = _mapper.Map<ProductViewModel>(ProductBuilder.New().Build());

            productRepository
                .Setup(x => x.GetById(productViewModel.Id))
                .ReturnsAsync((Product?)null);


            // Act
            var result = await productApplication.Update(productViewModel);


            // Assert
            result.Should().BeFalse();

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
            result.Should().BeFalse();

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
        public async Task Update_ShouldReturnFalse_WhenRepositoryUpdateFails()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            var productFake = ProductBuilder.New().Build();
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
            result.Should().BeFalse();

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
    }
}