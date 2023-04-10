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
            //ASP.net core validation优化
            if (user.Type != UserType.BUYER && user.Type != UserType.SELLER)
            {
                return BadRequest("Type can only be 0 AS Buyer and 1 AS Seller.");
            }
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                return BadRequest("Password cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(user.Name))
            {
                return BadRequest("User name cannot be empty.");
            }
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