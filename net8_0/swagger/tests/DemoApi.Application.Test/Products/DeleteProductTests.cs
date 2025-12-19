using DemoApi.Application.Models;
using DemoApi.Domain.Entities;
using FluentAssertions;
using Moq;

namespace DemoApi.Application.Test.Products
{
    public class DeleteProductTests : ProductTests
    {
        [Fact]
        public async Task DeleteById_ShouldReturnTrue_WhenRepositoryDeletesSuccessfully()
        {
            // Arrange
            var (notificator, productRepository, productApplication) = SetProductAppService();

            var productFake = NewProduct();

            productRepository
                .Setup(x => x.GetById(productFake.Id))
                .ReturnsAsync(productFake);

            productRepository
                .Setup(x => x.DeleteById(productFake.Id))
                .ReturnsAsync(true);


            // Act
            var result = await productApplication.DeleteById(productFake.Id);


            // Assert
            result.Should().BeTrue();

            productRepository.Verify(
                x => x.GetById(productFake.Id),
                Times.Once
            );

            productRepository.Verify(
                x => x.DeleteById(productFake.Id),
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
            result.Should().BeFalse();

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

            var productFake = NewProduct();

            productRepository
                .Setup(x => x.GetById(productFake.Id))
                .ReturnsAsync(productFake);

            productRepository
                .Setup(x => x.DeleteById(productFake.Id))
                .ReturnsAsync(false);


            // Act
            var result = await productApplication.DeleteById(productFake.Id);


            // Assert
            result.Should().BeFalse();

            productRepository.Verify(
                x => x.GetById(productFake.Id),
                Times.Once
            );

            productRepository.Verify(
                x => x.DeleteById(productFake.Id),
                Times.Once
            );

            notificator.Verify(
                x => x.AddError("Product could not be deleted"),
                Times.Once
            );
        }
    }
}