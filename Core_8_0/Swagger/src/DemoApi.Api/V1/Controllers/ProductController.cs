using DemoApi.Api.Controllers;
using DemoApi.Application.Models;
using DemoApi.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace DemoApi.Api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/products")]
    [Produces("application/json")]
    public class ProductController : MainApiController
    {
        #region Constructors

        public ProductController(INotificatorHandler notificator) : base(notificator)
        {
        }

        #endregion

        #region Public Methods

        [HttpGet("{id}")]
        public IActionResult GetById(uint id)
        {
            var product = new ProductViewModel { Id = id, Name = "Product " + id, Weight = 1.5 };

            return CustomResponse(product);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = new List<ProductViewModel>
            {
                new ProductViewModel { Id = 1, Name = "Product 1", Weight = 1.5 },
                new ProductViewModel { Id = 2, Name = "Product 2", Weight = 2.0 }
            };

            return CustomResponse(products);
        }

        [HttpPost]
        public IActionResult Create([FromBody] ProductViewModel product)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            return CustomResponseCreate(product);
        }

        [HttpPut]
        public IActionResult Update([FromBody] ProductViewModel product)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            return CustomResponse(HttpStatusCode.OK, product);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(uint id)
        {
            return CustomResponse(HttpStatusCode.NoContent, true);
        }

        #endregion
    }
}