using Demo.Api.Controllers;
using Demo.Application.ViewModels;
using Demo.Application.Interfaces;
using Demo.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Demo.Api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductController : MainApiController
    {
        #region Properties

        private readonly IProductAppService _ProductApplication;

        #endregion

        #region Constructors

        public ProductController(INotificatorHandler notificator, IProductAppService ProductApplication) : base(notificator)
        {
            _ProductApplication = ProductApplication;
        }

        #endregion

        #region Public Methods

        [HttpGet("GetAll")]
        public ActionResult<IList<ProductViewModel>> GetAll()
        {
            var response = _ProductApplication.GetAll();

            return CustomResponse(response);
        }

        [HttpGet("GetById/{Id}")]
        public ActionResult<ProductViewModel> GetById(uint Id)
        {
            var response = _ProductApplication.GetById(Id);

            return CustomResponse(response);
        }

        [HttpPost("Create")]
        public ActionResult<ProductViewModel> Create(ProductViewModel Product)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var response = _ProductApplication.Create(Product);

            return CustomResponse(response);
        }

        [HttpPut("Update")]
        public ActionResult<ProductViewModel> Update(ProductViewModel Product)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var response = _ProductApplication.Update(Product);

            return CustomResponse(response);
        }

        [HttpDelete("DeleteById/{Id}")]
        public ActionResult<bool> DeleteById(uint Id)
        {
            var response = _ProductApplication.DeleteById(Id);

            return CustomResponse(response);
        }

        #endregion
    }
}