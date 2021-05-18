using Demo.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Demo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TokenController : MainApiController
    {
        #region Properties

        private readonly IConfiguration _configuration;
        private readonly INotificatorHandler _notificator;

        #endregion

        public TokenController(INotificatorHandler notificator, IConfiguration configuration) : base(notificator)
        {
            _configuration = configuration;
            _notificator = notificator;
        }

        [AllowAnonymous]
        [HttpGet("RequestToken")]
        public IActionResult RequestToken([FromHeader(Name = "SecurityKey")] string securityKey)
        {
            if (securityKey != _configuration["Authorization:SecurityKey"])
            {
                _notificator.AddError("Erro de autenticação. SecurityKey inválido!");
                return CustomResponse();
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Authorization:SecurityKey"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "Demo.Api",
                audience: "Demo.Api",
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return CustomResponse(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}