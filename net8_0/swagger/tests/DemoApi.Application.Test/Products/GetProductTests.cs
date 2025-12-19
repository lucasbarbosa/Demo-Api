using DemoApi.Application.Models.Products;
using DemoApi.Domain.Entities;
using FluentAssertions;
using Moq;

namespace DemoApi.Application.Test.Products
{
    public class GetProductTests : ProductTests
    {
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
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().AllBeOfType<ProductViewModel>();

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
            result.Should().NotBeNull()
                .And.BeEmpty();

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

            productRepository
                .Setup(x => x.GetById(productFake.Id))
                .ReturnsAsync(productFake);


            // Act
            var result = await productApplication.GetById(productFake.Id);


            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(productFake.Name);
            result.Weight.Should().Be(productFake.Weight);

            productRepository.Verify(
                x => x.GetById(productFake.Id),
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
            result.Should().BeNull();

            productRepository.Verify(
                x => x.GetById(productId),
                Times.Once
            );

            notificator.Verify(
                x => x.AddError("Product was not found"),
                Times.Once
            );
        }
    }
}