using Data.Exceptions;
using Data.Model;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Net;

namespace Shopping.Controller
{
    [Route("api")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> AddUser(User user)
        {
            try
            {
                await _userService.AddUser(user);
                return CreatedAtAction(nameof(AddUser), new { id = user.Id }, "Registered successfully.");

            }
            catch (DuplicateUserNameException exception)
            {
                return Conflict(exception.Message);
            }
        }
    }
}