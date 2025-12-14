using DemoApi.Api.Controllers;
using DemoApi.Application.Interfaces;
using DemoApi.Application.Models;
using DemoApi.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DemoApi.Api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/products")]
    [Produces("application/json")]
    public class ProductController : MainApiController
    {
        #region Properties

        private readonly IProductAppService _productApplication;

        #endregion

        #region Constructors

        public ProductController(INotificatorHandler notificator, IProductAppService productApplication) : base(notificator)
        {
            _productApplication = productApplication;
        }

        #endregion

        #region Public Methods

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProductViewModel), StatusCodes.Status200OK)]
        public IActionResult GetById(uint id)
        {
            var product = _productApplication.GetById(id);

            return CustomResponse(product);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<ProductViewModel>), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            var products = _productApplication.GetAll();

            return CustomResponse(products);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status400BadRequest)]
        public IActionResult Create([FromBody] ProductViewModel product)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var response = _productApplication.Create(product);

            return CustomResponseCreate(response);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status404NotFound)]
        public IActionResult Update([FromBody] ProductViewModel product)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            return _productApplication.Update(product)
                ? CustomResponse(HttpStatusCode.NoContent)
                : CustomResponse();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status404NotFound)]
        public IActionResult Delete(uint id)
        {
            return _productApplication.DeleteById(id)
                ? CustomResponse(HttpStatusCode.NoContent)
                : CustomResponse();
        }

        #endregion
    }
}