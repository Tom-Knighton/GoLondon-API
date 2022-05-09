using System;
using System.Security.Authentication;
using GoLondonAPI.Domain.Entities;
using GoLondonAPI.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoLondonAPI.Controllers
{
    [Route("api/[controller]")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class AuthController : Controller
    {

        private readonly IAuthenticationService _authService;

        public AuthController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("Authenticate")]
        [Produces(typeof(User))]
        public async Task<IActionResult> AuthenticateUser([FromBody] AuthenticatingUser authDetails, bool needsTokens = true)
        {
            try
            {
                User user = await _authService.Authenticate(authDetails, needsTokens);
                if (user == null)
                    return NotFound("Invalid login attempt");
                return Ok(user);
            }
            catch (AuthenticationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("Register")]
        [Produces(typeof(User))]
        public async Task<IActionResult> RegisterUser([FromBody] RegistratingUser registrationDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid user registration details");
            }

            try
            {
                User user = await _authService.RegisterNewUser(registrationDetails);
                return Ok(user);
            }
            catch (AuthenticationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

