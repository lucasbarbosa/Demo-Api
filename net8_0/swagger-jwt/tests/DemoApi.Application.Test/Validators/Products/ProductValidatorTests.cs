using DemoApi.Application.Models.Products;
using DemoApi.Application.Validators.Products;
using FluentAssertions;
using FluentValidation.Results;

namespace DemoApi.Application.Test.Validators.Products
{
    public class ProductValidatorTests
    {
        #region Properties

        private readonly ProductValidator _validator;

        #endregion

        #region Constructors

        public ProductValidatorTests()
        {
            _validator = new ProductValidator();
        }

        #endregion

        #region Name Validation Tests

        [Fact]
        public void Validate_EmptyName_ReturnsValidationError()
        {
            // Arrange
            ProductViewModel model = new()
            { 
                Name = string.Empty, 
                Weight = 1.0 
            };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => 
                e.PropertyName == "Name" && 
                e.ErrorMessage == "Name is required");
        }

        [Fact]
        public void Validate_NullName_ReturnsValidationError()
        {
            // Arrange
            ProductViewModel model = new()
            { 
                Name = null!, 
                Weight = 1.0 
            };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => 
                e.PropertyName == "Name" && 
                e.ErrorMessage == "Name is required");
        }

        [Fact]
        public void Validate_WhitespaceName_ReturnsValidationError()
        {
            // Arrange
            ProductViewModel model = new() 
            { 
                Name = "   ", 
                Weight = 1.0 
            };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => 
                e.PropertyName == "Name" && 
                e.ErrorMessage == "Name is required");
        }

        [Fact]
        public void Validate_ValidName_ReturnsNoNameError()
        {
            // Arrange
            ProductViewModel model = new() 
            { 
                Name = "Valid Product Name", 
                Weight = 1.0 
            };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Name");
        }

        #endregion

        #region Weight Validation Tests

        [Fact]
        public void Validate_ZeroWeight_ReturnsValidationError()
        {
            // Arrange
            ProductViewModel model = new() 
            { 
                Name = "Test Product", 
                Weight = 0 
            };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => 
                e.PropertyName == "Weight" && 
                e.ErrorMessage == "Weight must be greater than 0");
        }

        [Fact]
        public void Validate_NegativeWeight_ReturnsValidationError()
        {
            // Arrange
            ProductViewModel model = new() 
            { 
                Name = "Test Product", 
                Weight = -1.5 
            };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => 
                e.PropertyName == "Weight" && 
                e.ErrorMessage == "Weight must be greater than 0");
        }

        [Theory]
        [InlineData(-100)]
        [InlineData(-10.5)]
        [InlineData(-0.01)]
        public void Validate_MultipleNegativeWeights_ReturnsValidationError(double weight)
        {
            // Arrange
            ProductViewModel model = new() 
            { 
                Name = "Test Product", 
                Weight = weight 
            };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => 
                e.PropertyName == "Weight" && 
                e.ErrorMessage == "Weight must be greater than 0");
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(1.0)]
        [InlineData(10.5)]
        [InlineData(100)]
        [InlineData(999.99)]
        public void Validate_PositiveWeight_ReturnsNoWeightError(double weight)
        {
            // Arrange
            ProductViewModel model = new() 
            { 
                Name = "Test Product", 
                Weight = weight 
            };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Weight");
        }

        #endregion

        #region Multiple Errors Tests

        [Fact]
        public void Validate_InvalidNameAndWeight_ReturnsMultipleValidationErrors()
        {
            // Arrange
            ProductViewModel model = new() 
            { 
                Name = string.Empty, 
                Weight = -1 
            };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
            result.Errors.Should().Contain(e => e.PropertyName == "Weight");
        }

        [Fact]
        public void Validate_AllValidFields_ReturnsNoErrors()
        {
            // Arrange
            ProductViewModel model = new() 
            { 
                Id = 0,
                Name = "Valid Product", 
                Weight = 10.5 
            };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        #endregion

        #region Boundary Tests

        [Fact]
        public void Validate_VerySmallPositiveWeight_ReturnsNoWeightError()
        {
            // Arrange
            ProductViewModel model = new() 
            { 
                Name = "Test Product", 
                Weight = double.Epsilon 
            };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Weight");
        }

        [Fact]
        public void Validate_MaximumWeight_ReturnsNoWeightError()
        {
            // Arrange
            ProductViewModel model = new() 
            { 
                Name = "Test Product", 
                Weight = double.MaxValue 
            };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Weight");
        }

        [Fact]
        public void Validate_MinimumWeight_ReturnsValidationError()
        {
            // Arrange
            ProductViewModel model = new() 
            { 
                Name = "Test Product", 
                Weight = double.MinValue 
            };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Weight");
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void Validate_NameWithOnlySpaces_ReturnsValidationError()
        {
            // Arrange
            ProductViewModel model = new() 
            { 
                Name = "     ", 
                Weight = 1.0 
            };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
        }

        [Fact]
        public void Validate_NameWithSpecialCharacters_ReturnsNoNameError()
        {
            // Arrange
            ProductViewModel model = new() 
            { 
                Name = "Product @#$% 123", 
                Weight = 1.0 
            };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Name");
        }

        [Fact]
        public void Validate_VeryLongName_ReturnsNoNameError()
        {
            // Arrange
            string longName = new('A', 1000);
            ProductViewModel model = new() 
            { 
                Name = longName, 
                Weight = 1.0 
            };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Name");
        }

        [Fact]
        public void Validate_NameWithUnicodeCharacters_ReturnsNoNameError()
        {
            // Arrange
            ProductViewModel model = new() 
            { 
                Name = "Produto ??? ?? ???????", 
                Weight = 1.0 
            };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Name");
        }

        [Fact]
        public void Validate_SingleCharacterName_ReturnsNoNameError()
        {
            // Arrange
            ProductViewModel model = new() 
            { 
                Name = "A", 
                Weight = 1.0 
            };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Name");
        }

        #endregion

        #region Validator Metadata Tests

        [Fact]
        public void CreateDescriptor_NameProperty_ReturnsValidationRules()
        {
            // Arrange & Act
            FluentValidation.IValidatorDescriptor descriptor = _validator.CreateDescriptor();
            var nameRules = descriptor.GetMembersWithValidators()
                .Where(m => m.Key == "Name")
                .SelectMany(m => m);

            // Assert
            nameRules.Should().NotBeEmpty();
        }

        [Fact]
        public void CreateDescriptor_WeightProperty_ReturnsValidationRules()
        {
            // Arrange & Act
            FluentValidation.IValidatorDescriptor descriptor = _validator.CreateDescriptor();
            var weightRules = descriptor.GetMembersWithValidators()
                .Where(m => m.Key == "Weight")
                .SelectMany(m => m);

            // Assert
            weightRules.Should().NotBeEmpty();
        }

        #endregion
    }
}