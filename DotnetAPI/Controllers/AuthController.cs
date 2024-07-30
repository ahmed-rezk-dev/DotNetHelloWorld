using System.Data;
using System.Text;
using DotNet.Models;
using DotNetAPI.Data;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotNetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;
        private readonly string? _pepper = "";
        private readonly int _iteration = 3;

        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
            _pepper = config.GetConnectionString("AppSettings:PasswordKey");
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register([FromBody] UserRegistrationDto inputs)
        {
            if (inputs.Password == inputs.PasswordConfirmation)
            {
                string sqlCheckUserExists =
                    "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" + inputs.Email + "'";

                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
                if (existingUsers.Count() == 0)
                {
                    byte[] PasswordSaltByte = _authHelper.GenerateSalt();
                    string PasswordSalt = Convert.ToBase64String(PasswordSaltByte);
                    string PasswordHash = _authHelper.ComputeHash(
                        inputs.Password,
                        PasswordSalt,
                        _pepper,
                        _iteration
                    );

                    string sqlAddAuth =
                        @"
                        INSERT INTO TutorialAppSchema.Auth  ([Email],
                        [PasswordHash],
                        [PasswordSalt]) VALUES ('"
                        + inputs.Email
                        + "', @PasswordHash, @PasswordSalt)";

                    List<SqlParameter> sqlParameters = new List<SqlParameter>();

                    SqlParameter passwordSaltParameter = new SqlParameter(
                        "@PasswordSalt",
                        SqlDbType.Char
                    );
                    passwordSaltParameter.Value = PasswordSalt;

                    SqlParameter passwordHashParameter = new SqlParameter(
                        "@PasswordHash",
                        SqlDbType.Char
                    );
                    passwordHashParameter.Value = PasswordHash;

                    sqlParameters.Add(passwordSaltParameter);
                    sqlParameters.Add(passwordHashParameter);

                    if (_dapper.ExecuteWithParametersList(sqlAddAuth, sqlParameters))
                    {
                        string sqlAddUser =
                            @"
                            INSERT INTO TutorialAppSchema.Users(
                                [FirstName],
                                [LastName],
                                [Email],
                                [Gender],
                                [Active]
                            ) VALUES ("
                            + "'"
                            + inputs.FirstName
                            + "', '"
                            + inputs.LastName
                            + "', '"
                            + inputs.Email
                            + "', '"
                            + inputs.Gender
                            + "', 1)";
                        if (_dapper.Execute(sqlAddUser))
                        {
                            return Ok();
                        }
                        throw new Exception("Failed to add user.");
                    }
                    throw new Exception("Failed to register user.");
                }
                else
                {
                    return BadRequest("Email is already exists.");
                }
            }
            return BadRequest("Password and PasswordConfirmation are not equal.");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserLoginDto inputs)
        {
            string sqlForHashAndSalt =
                @"SELECT 
                [PasswordHash],
                [PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email = '"
                + inputs.Email
                + "'";
            UserAuth user = _dapper.LoadDataSingle<UserAuth>(sqlForHashAndSalt);

            var passwordHash = _authHelper.ComputeHash(
                inputs.Password,
                user.PasswordSalt,
                _pepper,
                _iteration
            );
            if (user.PasswordHash != passwordHash)
                return StatusCode(401, "Username or password did not match.");

            string userIdSql =
                @"
                SELECT UserId FROM TutorialAppSchema.Users WHERE Email = '"
                + inputs.Email
                + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(
                new Dictionary<string, string> { { "token", _authHelper.CreateToken(userId) } }
            );
        }
    }
}
