using DotNet.Models;
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

        [HttpGet("")]
        public IEnumerable<User> All()
        {
            string sql = @"SELECT * FROM TutorialAppSchema.Users";
            return _dapper.LoadData<User>(sql);
        }
    }
}
