using DemoApi.Application.Models;
using DemoApi.Application.Test.Builders.Products;
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

            var productFake = ProductBuilder.New().Build();

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

            var nonExistentProduct = ProductBuilder.New().WithId(999999).Build();

            productRepository
                .Setup(x => x.GetById(nonExistentProduct.Id))
                .ReturnsAsync((Product?)null);


            // Act
            var result = await productApplication.DeleteById(nonExistentProduct.Id);


            // Assert
            result.Should().BeFalse();

            productRepository.Verify(
                x => x.GetById(nonExistentProduct.Id),
                Times.Once
            );

            productRepository.Verify(
                x => x.DeleteById(nonExistentProduct.Id),
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

            var productFake = ProductBuilder.New().Build();

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