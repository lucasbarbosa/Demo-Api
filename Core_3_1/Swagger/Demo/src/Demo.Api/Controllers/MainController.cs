using Demo.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Demo.Api.Controllers
{
    [ApiController]
    public class MainController : Controller
    {
        #region Protected Methods

        protected ActionResult CustomResponse(object result = null)
        {
            if (!ValidOperation())
            {
                return BadRequest(new ResponseViewModel
                {
                    success = false,
                    errors = GetErrors()
                });
            }

            return Ok(new ResponseViewModel
            {
                success = true,
                data = result
            });
        }

        protected bool ValidOperation()
        {
            //return _notificator.GetErrors().Count > 0 ? false : true;
            return true;
        }

        protected IList<string> GetErrors()
        {
            //return _notificator.GetErrors().Select(x => x.Message).ToList();
            return new List<string>();
        }

        #endregion
    }
}