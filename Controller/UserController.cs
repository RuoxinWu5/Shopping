using Data.Model;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService=userService;
        }

        [HttpPost]
        public Task<ActionResult> AddUser(User user){
            throw new NotImplementedException();
        }
    }
}