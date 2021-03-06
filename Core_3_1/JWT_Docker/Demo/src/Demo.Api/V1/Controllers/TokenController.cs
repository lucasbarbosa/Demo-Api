using Demo.Api.Extensions;
using Demo.Application.ViewModels;
using Demo.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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

        private readonly INotificatorHandler _notificator;
        private readonly AuthorizationSettings _authorizationSettings;

        #endregion

        #region Constructors

        public TokenController(INotificatorHandler notificator, IOptions<AuthorizationSettings> authorizationSettings) : base(notificator)
        {
            _notificator = notificator;
            _authorizationSettings = authorizationSettings.Value;
        }

        #endregion

        #region Public Methods

        [AllowAnonymous]
        [HttpGet("RequestToken")]
        public IActionResult RequestToken([FromHeader(Name = "SecurityKey")] string securityKey)
        {
            if (securityKey != _authorizationSettings.SecurityKey)
            {
                _notificator.AddError("Erro de autenticação. SecurityKey inválido!");
                return CustomResponse();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authorizationSettings.SecurityKey);
            var created = DateTime.UtcNow;
            var expires = DateTime.UtcNow.AddMinutes(_authorizationSettings.ExpirationMinutes);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor()
            {
                Issuer = _authorizationSettings.Sender,
                Audience = _authorizationSettings.ValidOn,
                NotBefore = created,
                Expires = expires,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            });
            var encodedToken = tokenHandler.WriteToken(token);

            return CustomResponse(new TokenViewModel
            {
                AccessToken = encodedToken,
                Created = created.ToString("yyyy-MM-dd HH:mm:ss"),
                ExpiresIn = expires.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }

        #endregion
    }
}