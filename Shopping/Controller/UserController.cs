using Data.Exceptions;
using Data.Model;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace Shopping.Controller
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult> AddUser(User user)
        {
            try
            {
                var result = await _userService.AddUser(user);
                return CreatedAtAction(nameof(GetUserById), new { userId = user.Id }, result);
            }
            catch (DuplicateUserNameException exception)
            {
                return Conflict(exception.Message);
            }
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult> GetUserById(int userId)
        {
            try
            {
                var user = await _userService.GetUserById(userId);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}