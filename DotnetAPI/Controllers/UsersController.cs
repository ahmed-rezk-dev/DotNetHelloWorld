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
        public IActionResult Create(UserDto user)
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
                    '{user.FirstName}',
                    '{user.LastName}',
                    '{user.Email}',
                    '{user.Gender}',
                    '{user.Active}'
                )";
            if (_dapper.Execute(sql))
            {
                return Ok("User successfully created.");
            }
            throw new Exception("Failed to create user.");
        }

        [HttpPut("{UserId}")]
        public IActionResult Update(string UserId, UserDto user)
        {
            string sql =
                @$"UPDATE TutorialAppSchema.Users 
                SET 
                    FirstName='{user.FirstName}', 
                    LastName='{user.LastName}', 
                    Email='{user.Email}', 
                    Gender='{user.Gender}', 
                    Active='{user.Active}'
                WHERE UserId = {UserId}";
            if (_dapper.Execute(sql))
            {
                return Ok("User successfully updated.");
            }
            throw new Exception("Failed to update user.");
        }

        [HttpDelete("{UserId}")]
        public IActionResult Delete(string UserId)
        {
            string sql = @$"DELETE FROM TutorialAppSchema.Users WHERE UserId = {UserId}";
            if (_dapper.Execute(sql))
            {
                return Ok("User successfully deleted.");
            }
            throw new Exception("Failed to delete user.");
        }
    }
}
