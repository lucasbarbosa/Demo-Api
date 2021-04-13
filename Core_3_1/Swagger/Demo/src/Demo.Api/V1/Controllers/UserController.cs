using AutoMapper;
using Demo.Api.Controllers;
using Demo.Api.ViewModels;
using Demo.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.Api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : MainApiController
    {
        #region Attributes

        private readonly IMapper _mapper;
        private readonly IUserApplication _userApplication;

        #endregion

        #region Constructors

        public UserController(IMapper mapper, IUserApplication userApplication)
        {
            _mapper = mapper;
            _userApplication = userApplication;
        }

        #endregion

        #region Public Methods

        [HttpGet("GetAll")]
        public async Task<ActionResult<IList<UserViewModel>>> GetAll()
        {
            var users = _mapper.Map<IList<UserViewModel>>(_userApplication.GetAll());

            return CustomResponse(users);
        }

        #endregion
    }
}