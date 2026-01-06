using DemoApi.Application.Models.Products;
using DemoApi.Test.Builders.Products;
using FluentAssertions;

namespace DemoApi.Application.Test.Builders.Products
{
    public class ProductViewModelBuilderTests
    {
        [Fact]
        public void Build_ShouldCreateValidProduct_WhenUsingDefaultValues()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New().Build();

            // Assert
            product.Should().NotBeNull();
            product.Id.Should().Be(0);
            product.Name.Should().NotBeNullOrEmpty();
            product.Weight.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Build_ShouldCreateProductWithSpecificId_WhenUsingWithId()
        {
            // Arrange
            uint expectedId = 123;

            // Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithId(expectedId)
                .Build();

            // Assert
            product.Id.Should().Be(expectedId);
        }

        [Fact]
        public void Build_ShouldCreateProductWithIdZero_WhenUsingWithIdZero()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithIdZero()
                .Build();

            // Assert
            product.Id.Should().Be(0);
        }

        [Fact]
        public void Build_ShouldCreateProductWithNonExistentId_WhenUsingWithNonExistentId()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithNonExistentId()
                .Build();

            // Assert
            product.Id.Should().Be(999999);
        }

        [Fact]
        public void Build_ShouldCreateProductWithSpecificName_WhenUsingWithName()
        {
            // Arrange
            string expectedName = "Custom Product Name";

            // Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithName(expectedName)
                .Build();

            // Assert
            product.Name.Should().Be(expectedName);
        }

        [Fact]
        public void Build_ShouldCreateProductWithEmptyName_WhenUsingWithEmptyName()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithEmptyName()
                .Build();

            // Assert
            product.Name.Should().BeEmpty();
        }

        [Fact]
        public void Build_ShouldCreateProductWithNullName_WhenUsingWithNullName()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithNullName()
                .Build();

            // Assert
            product.Name.Should().BeNull();
        }

        [Fact]
        public void Build_ShouldCreateProductWithWhitespaceName_WhenUsingWithWhitespaceName()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithWhitespaceName()
                .Build();

            // Assert
            product.Name.Should().Be("   ");
        }

        [Fact]
        public void Build_ShouldCreateProductWithSingleCharacterName_WhenUsingWithSingleCharacterName()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithSingleCharacterName()
                .Build();

            // Assert
            product.Name.Should().Be("A");
            product.Name.Should().HaveLength(1);
        }

        [Fact]
        public void Build_ShouldCreateProductWithLongName_WhenUsingWithLongName()
        {
            // Arrange
            int expectedLength = 200;

            // Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithLongName(expectedLength)
                .Build();

            // Assert
            product.Name.Should().HaveLength(expectedLength);
            product.Name.Should().MatchRegex("^A+$");
        }

        [Fact]
        public void Build_ShouldCreateProductWithDefaultLongName_WhenUsingWithLongNameWithoutParameter()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithLongName()
                .Build();

            // Assert
            product.Name.Should().HaveLength(100);
        }

        [Fact]
        public void Build_ShouldCreateProductWithSpecialCharactersName_WhenUsingWithSpecialCharactersName()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithSpecialCharactersName()
                .Build();

            // Assert
            product.Name.Should().Be("Product @#$% 123");
        }

        [Fact]
        public void Build_ShouldCreateProductWithUnicodeName_WhenUsingWithUnicodeName()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithUnicodeName()
                .Build();

            // Assert
            product.Name.Should().Contain("ção");
            product.Name.Should().Contain("ãé");
        }

        [Fact]
        public void Build_ShouldCreateProductWithUniqueName_WhenUsingWithUniqueName()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithUniqueName()
                .Build();

            // Assert
            product.Name.Should().StartWith("Unique Product Name Test");
            product.Name.Should().MatchRegex(@"Unique Product Name Test [0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}");
        }

        [Fact]
        public void Build_ShouldCreateDifferentUniqueNames_WhenCalledMultipleTimes()
        {
            // Arrange & Act
            ProductViewModel product1 = ProductViewModelBuilder.New().WithUniqueName().Build();
            ProductViewModel product2 = ProductViewModelBuilder.New().WithUniqueName().Build();

            // Assert
            product1.Name.Should().NotBe(product2.Name);
            product1.Name.Should().StartWith("Unique Product Name Test");
            product2.Name.Should().StartWith("Unique Product Name Test");
        }

        [Fact]
        public void Build_ShouldCreateProductWithSpecificWeight_WhenUsingWithWeight()
        {
            // Arrange
            double expectedWeight = 15.75;

            // Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithWeight(expectedWeight)
                .Build();

            // Assert
            product.Weight.Should().Be(expectedWeight);
        }

        [Fact]
        public void Build_ShouldCreateProductWithZeroWeight_WhenUsingWithZeroWeight()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithZeroWeight()
                .Build();

            // Assert
            product.Weight.Should().Be(0);
        }

        [Fact]
        public void Build_ShouldCreateProductWithNegativeWeight_WhenUsingWithNegativeWeight()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithNegativeWeight()
                .Build();

            // Assert
            product.Weight.Should().BeNegative();
            product.Weight.Should().Be(-1.5);
        }

        [Fact]
        public void Build_ShouldCreateProductWithVerySmallPositiveWeight_WhenUsingWithVerySmallPositiveWeight()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithVerySmallPositiveWeight()
                .Build();

            // Assert
            product.Weight.Should().Be(double.Epsilon);
            product.Weight.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Build_ShouldCreateProductWithMaximumWeight_WhenUsingWithMaximumWeight()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithMaximumWeight()
                .Build();

            // Assert
            product.Weight.Should().Be(double.MaxValue);
        }

        [Fact]
        public void Build_ShouldCreateProductWithMinimumWeight_WhenUsingWithMinimumWeight()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithMinimumWeight()
                .Build();

            // Assert
            product.Weight.Should().Be(double.MinValue);
        }

        [Fact]
        public void Build_ShouldCreateProductWithLargeWeight_WhenUsingWithLargeWeight()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithLargeWeight()
                .Build();

            // Assert
            product.Weight.Should().Be(1000000.99);
        }

        [Fact]
        public void Build_ShouldCreateProductWithPreciseWeight_WhenUsingWithPreciseWeight()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithPreciseWeight()
                .Build();

            // Assert
            product.Weight.Should().Be(1.123456789);
        }

        [Fact]
        public void Build_ShouldSupportFluentInterface_WhenChainingMethods()
        {
            // Arrange
            uint expectedId = 123;
            string expectedName = "Custom Product";
            double expectedWeight = 5.5;

            // Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithId(expectedId)
                .WithName(expectedName)
                .WithWeight(expectedWeight)
                .Build();

            // Assert
            product.Id.Should().Be(expectedId);
            product.Name.Should().Be(expectedName);
            product.Weight.Should().Be(expectedWeight);
        }

        [Fact]
        public void Build_ShouldSupportFluentInterface_WhenChainingMultipleMethods()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithId(999)
                .WithName("Test Product")
                .WithWeight(10.5)
                .WithName("Updated Name")
                .WithWeight(20.75)
                .Build();

            // Assert
            product.Id.Should().Be(999);
            product.Name.Should().Be("Updated Name");
            product.Weight.Should().Be(20.75);
        }

        [Fact]
        public void Build_ShouldOverrideDefaultValues_WhenUsingCustomMethods()
        {
            // Arrange & Act
            ProductViewModel defaultProduct = ProductViewModelBuilder.New().Build();
            ProductViewModel customProduct = ProductViewModelBuilder.New()
                .WithId(500)
                .WithName("Override Test")
                .WithWeight(99.99)
                .Build();

            // Assert
            defaultProduct.Id.Should().Be(0);
            defaultProduct.Name.Should().NotBe("Override Test");
            defaultProduct.Weight.Should().NotBe(99.99);

            customProduct.Id.Should().Be(500);
            customProduct.Name.Should().Be("Override Test");
            customProduct.Weight.Should().Be(99.99);
        }

        [Fact]
        public void New_ShouldReturnNewInstance_WhenCalled()
        {
            // Arrange & Act
            ProductViewModelBuilder builder = ProductViewModelBuilder.New();

            // Assert
            builder.Should().NotBeNull();
            builder.Should().BeOfType<ProductViewModelBuilder>();
        }

        [Fact]
        public void New_ShouldReturnDifferentInstances_WhenCalledMultipleTimes()
        {
            // Arrange & Act
            ProductViewModelBuilder builder1 = ProductViewModelBuilder.New();
            ProductViewModelBuilder builder2 = ProductViewModelBuilder.New();

            // Assert
            builder1.Should().NotBeSameAs(builder2);
        }

        [Fact]
        public void Build_ShouldGenerateRandomName_WhenUsingDefaultConstructor()
        {
            // Arrange & Act
            ProductViewModel product1 = ProductViewModelBuilder.New().Build();
            ProductViewModel product2 = ProductViewModelBuilder.New().Build();

            // Assert
            product1.Name.Should().NotBeNullOrEmpty();
            product2.Name.Should().NotBeNullOrEmpty();
            product1.Name.Should().NotBe(product2.Name, "each builder should generate different random names");
        }

        [Fact]
        public void Build_ShouldGenerateWeightInValidRange_WhenUsingDefaultConstructor()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New().Build();

            // Assert
            product.Weight.Should().BeInRange(0.1, 10.0);
        }

        [Fact]
        public void Build_ShouldGenerateWeightWithTwoDecimalPlaces_WhenUsingDefaultConstructor()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New().Build();

            // Assert
            double roundedWeight = Math.Round(product.Weight, 2);
            product.Weight.Should().Be(roundedWeight);
        }
    }
}