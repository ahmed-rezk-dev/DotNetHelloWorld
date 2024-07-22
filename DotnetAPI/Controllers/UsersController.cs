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

        [HttpGet("{UserId}")]
        public User Find(string UserId)
        {
            string sql = @$"SELECT * FROM TutorialAppSchema.Users WHERE UserId = {UserId}";
            return _dapper.LoadDataSingle<User>(sql);
        }

        [HttpPost("")]
        public bool Create(User User)
        {
            string sql =
                @$"INSERT INTO TutorialAppSchema.Users ( 
                    FirstName, 
                    LastName, 
                    Email, 
                    Gender, 
                    Active
                )
                VALUES(
                    '{User.FirstName}',
                    '{User.LastName}',
                    '{User.Email}',
                    '{User.Gender}',
                    '{User.Active}'
                )";
            return _dapper.Execute(sql);
        }

        [HttpPut("{ UserId}")]
        public bool Update(string UserId, User User)
        {
            string sql =
                @$"UPDATE TutorialAppSchema.Users 
                SET 
                    FirstName='{User.FirstName}', 
                    LastName='{User.LastName}', 
                    Email='{User.Email}', 
                    Gender='{User.Gender}', 
                    Active='{User.Active}'
                WHERE UserId = {UserId}";
            return _dapper.Execute(sql);
        }
    }
}
