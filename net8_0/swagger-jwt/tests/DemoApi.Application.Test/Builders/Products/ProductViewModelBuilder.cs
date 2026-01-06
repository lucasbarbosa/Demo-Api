using Bogus;
using DemoApi.Application.Models.Products;

namespace DemoApi.Application.Test.Builders.Products
{
    public class ProductViewModelBuilder
    {
        #region Properties

        private uint _id = 0;
        private string _name;
        private double _weight;
        private static readonly Faker _faker = new();

        #endregion

        #region Constructors

        public ProductViewModelBuilder()
        {
            _name = _faker.Commerce.ProductName();
            _weight = Math.Round(_faker.Random.Double(0.1, 10.0), 2);
        }

        #endregion

        #region Public Methods

        public ProductViewModelBuilder WithId(uint id)
        {
            _id = id;
            return this;
        }

        public ProductViewModelBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public ProductViewModelBuilder WithWeight(double weight)
        {
            _weight = weight;
            return this;
        }

        public ProductViewModel Build()
        {
            return new ProductViewModel
            {
                Id = _id,
                Name = _name,
                Weight = _weight
            };
        }

        public static ProductViewModelBuilder New() => new();

        #endregion
    }
}