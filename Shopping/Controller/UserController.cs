using Data.Model;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace Shopping.Controller
{
    [Route("api/[controller]")]
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
            var result = await _userService.AddUser(user);
            if (result == "Registered successfully"){
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}