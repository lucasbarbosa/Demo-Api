using Demo.Api.Controllers;
using Demo.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.Api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : MainController
    {
        #region Public Methods

        [HttpGet("GetAll")]
        public async Task<ActionResult<IList<UserViewModel>>> GetAll()
        {
            var users = new List<UserViewModel>();

            return CustomResponse(users);
        }

        #endregion
    }
}