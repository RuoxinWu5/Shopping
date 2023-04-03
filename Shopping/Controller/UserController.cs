using Data.Model;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Net;

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
            if (user.type != UserType.BUYER && user.type != UserType.SELLER)
            {
                return BadRequest("Type can only be 0 AS Buyer and 1 AS Seller.");
            }
            if (string.IsNullOrWhiteSpace(user.password))
            {
                return BadRequest("Password cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(user.name))
            {
                return BadRequest("User name cannot be empty.");
            }
            var result = await _userService.AddUser(user);
            if (result.StatusCode == HttpStatusCode.Created)
            {
                return CreatedAtAction(nameof(AddUser), new { id = user.id }, "Registered successfully.");
            }
            return Conflict(await result.Content.ReadAsStringAsync());
        }
    }
}