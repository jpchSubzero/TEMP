using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UTPL.AuthenticationADFS.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Auth")]
    public class AuthController : ControllerBase
    {

        [HttpGet]
        [Route("private")]
        public IActionResult GetPrivate()
        {
            var name = User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("name"))?.Value.ToString();
            var email = User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn"))?.Value.ToString();

            string message = string.Empty;

            if (User.Identity.IsAuthenticated)
            {
                message = $"Información privada: Usuario: {name}, E-mail: {email}";
            }

            return Ok(new
            {
                message = message
            });
        }

        [HttpGet]
        [Route("public")]
        [AllowAnonymous]
        public IActionResult GetPublic()
        {
            return Ok(new
            {
                message = "Información pública para todo usuario"
            });
        }

    }
}