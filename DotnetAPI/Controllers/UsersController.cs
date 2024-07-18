using Microsoft.AspNetCore.Mvc;

namespace DotNetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        public UsersController() { }

        [HttpGet("GetUsers/{newUser}")]
        public string[] GetUsers(string newUser)
        {
            string[] usersList = new string[] { "Ahmed Rezk", "Jacob mac", newUser };
            return usersList;
        }
    }
}
