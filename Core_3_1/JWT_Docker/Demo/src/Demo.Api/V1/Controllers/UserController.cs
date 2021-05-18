using Demo.Api.Controllers;
using Demo.Application.ViewModels;
using Demo.Application.Interfaces;
using Demo.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Demo.Api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : MainApiController
    {
        #region Properties

        private readonly IUserAppService _userApplication;

        #endregion

        #region Constructors

        public UserController(INotificatorHandler notificator, IUserAppService userApplication) : base(notificator)
        {
            _userApplication = userApplication;
        }

        #endregion

        #region Public Methods

        [HttpGet("GetAll")]
        public ActionResult<IList<UserViewModel>> GetAll()
        {
            var response = _userApplication.GetAll();

            return CustomResponse(response);
        }

        [HttpGet("GetById/{Id}")]
        public ActionResult<UserViewModel> GetById(uint Id)
        {
            var response = _userApplication.GetById(Id);

            return CustomResponse(response);
        }

        [HttpPost("Create")]
        public ActionResult<UserViewModel> Create(UserViewModel user)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var response = _userApplication.Create(user);

            return CustomResponse(response);
        }

        [HttpPut("Update")]
        public ActionResult<UserViewModel> Update(UserViewModel user)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var response = _userApplication.Update(user);

            return CustomResponse(response);
        }

        [HttpDelete("DeleteById/{Id}")]
        public ActionResult<bool> DeleteById(uint Id)
        {
            var response = _userApplication.DeleteById(Id);

            return CustomResponse(response);
        }

        #endregion
    }
}