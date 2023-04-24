using Microsoft.AspNetCore.Mvc;
using Service;
using Data.RequestModel;

namespace Shopping.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginController(IUserService userService, IJwtTokenService jwtTokenService)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestModel request)
        {
            try
            {
                var user = await _userService.GetUserByUserNameAndPassword(request.Name, request.Password);
                var tokenString = _jwtTokenService.GenerateJwtToken(user);
                return Ok(tokenString);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
