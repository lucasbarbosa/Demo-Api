using AutoMapper;
using Demo.Api.Controllers;
using Demo.Api.ViewModels;
using Demo.Application.Interfaces;
using Demo.Domain.Entities;
using Demo.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Demo.Api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : MainApiController
    {
        #region Properties

        private readonly IMapper _mapper;
        private readonly IUserAppService _userApplication;

        #endregion

        #region Constructors

        public UserController(INotificator notificator, IMapper mapper, IUserAppService userApplication) : base(notificator)
        {
            _mapper = mapper;
            _userApplication = userApplication;
        }

        #endregion

        #region Public Methods

        [HttpGet("GetAll")]
        public ActionResult<IList<UserViewModel>> GetAll()
        {
            var response = _mapper.Map<IList<UserViewModel>>(_userApplication.GetAll());

            return CustomResponse(response);
        }

        [HttpGet("GetById/{Id}")]
        public ActionResult<UserViewModel> GetById(uint Id)
        {
            var response = _mapper.Map<UserViewModel>(_userApplication.GetById(Id));

            return CustomResponse(response);
        }

        [HttpPost("Create")]
        public ActionResult<UserViewModel> Create(UserViewModel user)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var response = _mapper.Map<UserViewModel>(_userApplication.Create(_mapper.Map<User>(user)));

            return CustomResponse(response);
        }

        [HttpPut("Update")]
        public ActionResult<UserViewModel> Update(UserViewModel user)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var response = _mapper.Map<UserViewModel>(_userApplication.Update(_mapper.Map<User>(user)));

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