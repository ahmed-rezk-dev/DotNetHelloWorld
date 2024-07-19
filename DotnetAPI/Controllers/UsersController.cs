using DotNetAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        DataContextDapper _dapper;

        public UsersController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("TestSql")]
        public DateTime TestSql()
        {
            return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
        }

        [HttpGet("GetUsers/{newUser}")]
        public string[] GetUsers(string newUser)
        {
            string[] usersList = new string[] { "Ahmed Rezk", "Jacob mac", newUser };
            return usersList;
        }
    }
}
