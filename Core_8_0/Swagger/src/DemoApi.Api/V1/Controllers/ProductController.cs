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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(uint id)
        {
            var product = await _productApplication.GetById(id);

            return CustomResponse(product);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productApplication.GetAll();

            return CustomResponse(products);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ProductViewModel product)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var response = await _productApplication.Create(product);

            return CustomResponseCreate(response);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] ProductViewModel product)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            return await _productApplication.Update(product)
                ? CustomResponse(HttpStatusCode.NoContent, true)
                : CustomResponse();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(uint id)
        {
            return await _productApplication.DeleteById(id)
                ? CustomResponse(HttpStatusCode.NoContent, true)
                : CustomResponse();
        }

        #endregion
    }
}