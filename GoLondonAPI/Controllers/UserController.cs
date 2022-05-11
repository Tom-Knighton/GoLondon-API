using System;
using GoLondonAPI.Domain.Entities;
using GoLondonAPI.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoLondonAPI.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private IUserService _users;

        public UserController(IUserService userService)
        {
            _users = userService;
        }

        [HttpGet("{uuid}")]
        public async Task<IActionResult> GetUser(string uuid)
        {
            User user = await _users.GetUserAsync(uuid);
            return user == null ? NotFound("Invalid user uuid") : Ok(user);
        }

        [HttpPut("EditDetails/{uuid}")]
        public async Task<IActionResult> EditUserDetails(string uuid, [FromBody] RegistratingUser details)
        {
            User user = await _users.EditUserDetails(uuid, details);
            return user == null ? NotFound("Invalid user uuid") : Ok(user);
        }
    }
}

