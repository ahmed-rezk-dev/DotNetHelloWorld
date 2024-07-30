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

        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            string userIdSql =
                @"
                SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = '"
                + User.FindFirst("userId")?.Value
                + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return _authHelper.CreateToken(userId);
        }

        [AllowAnonymous]
        [HttpPost("RegisterV2")]
        public IActionResult RegisterV2(UserRegistrationDto inputs)
        {
            string Email = inputs.Password;
            string password = inputs.Password;
            string passwordConfirmation = inputs.PasswordConfirmation;

            if (password != passwordConfirmation)
                return Ok("Password And Confirm Password Are Not Matched");

            // Check if any users are already registered with the same Email.
            string sqlCheckUserExists =
                "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" + Email + "'";

            IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);

            if (existingUsers.Count() > 0)
                return BadRequest("User Is Already Registered.");

            byte[] saltBytes = _authHelper.GenerateSalt();
            // Hash the password with the salt
            string PasswordHash = _authHelper.HashPassword(password, saltBytes);
            string PasswordSalt = Convert.ToBase64String(saltBytes);

            string sqlCreatUserString =
                @"INSERT INTO TutorialAppSchema.Auth (Email, PasswordHash, PasswordSalt) VALUES (@Email, @PasswordHash, @PasswordSalt)";
            _dapper.ExecuteWithParameters(
                sqlCreatUserString,
                new
                {
                    Email,
                    PasswordHash,
                    PasswordSalt
                }
            );

            return Ok("User Resgistered successfully");
        }

        [AllowAnonymous]
        [HttpPost("LoginV2")]
        public IActionResult LoginV2(UserLoginDto inputs)
        {
            string sqlCheckUserExists =
                "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" + inputs.Email + "'";

            IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);

            if (existingUsers.Count() == 0)
                return NotFound("User not found");

            // In a real scenario, you would retrieve these values from your database
            string sqlUserAuth =
                @"SELECT PasswordHash, PasswordSalt FROM TutorialAppSchema.Auth WHERE Email = '"
                + inputs.Email
                + "'";

            UserAuth? user = _dapper.LoadDataSingle<UserAuth>(sqlUserAuth);

            string storedHashedPassword = user.PasswordHash; // "hashed_password_from_database";
            string storedSalt = user.PasswordSalt; //"salt_from_database";
            // byte[] storedSaltBytes = user.PasswordSalt;
            string enteredPassword = inputs.Password; //"user_entered_password";

            // Convert the stored salt and entered password to byte arrays
            byte[] storedSaltBytes = Convert.FromBase64String(user.PasswordSalt);
            byte[] enteredPasswordBytes = Encoding.UTF8.GetBytes(enteredPassword);
            //
            // // Concatenate entered password and stored salt
            byte[] saltedPassword = new byte[enteredPasswordBytes.Length + storedSaltBytes.Length];
            Buffer.BlockCopy(
                enteredPasswordBytes,
                0,
                saltedPassword,
                0,
                enteredPasswordBytes.Length
            );
            Buffer.BlockCopy(
                storedSaltBytes,
                0,
                saltedPassword,
                enteredPasswordBytes.Length,
                storedSaltBytes.Length
            );
            //
            // // Hash the concatenated value
            string enteredPasswordHash = _authHelper.HashPassword(enteredPassword, storedSaltBytes);

            // // Compare the entered password hash with the stored hash
            if (enteredPasswordHash == storedHashedPassword)
            {
                return Ok("Password is correct.");
            }
            else
            {
                return Ok("Password is incorrect.");
            }
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            return Ok("Logout success");
        }
    }
}
