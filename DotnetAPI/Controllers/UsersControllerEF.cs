using AutoMapper;
using DotNet.Models;
using DotNetAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersControllerEF : ControllerBase
    {
        DataContextEF _entityFramework;
        IMapper _mapper;

        public UsersControllerEF(IConfiguration config)
        {
            _entityFramework = new DataContextEF(config);
            _mapper = new Mapper(
                new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<UserDto, User>();
                })
            );
        }

        [HttpGet("")]
        public IEnumerable<User> All()
        {
            IEnumerable<User> users = _entityFramework.Users.ToList<User>();
            Console.WriteLine(users);
            return users;
        }

        [HttpGet("{UserId}")]
        public ActionResult<User> Find(int UserId)
        {
            User? findUser = _entityFramework
                .Users.Where(User => User.UserId == UserId)
                .FirstOrDefault();
            if (findUser != null)
            {
                return findUser;
            }
            return NotFound();
        }

        [HttpPost("")]
        public IActionResult Create(UserDto user)
        {
            User newUser = _mapper.Map<User>(user);

            _entityFramework.Add(newUser);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok("User successfully created.");
            }
            return BadRequest("Failed to create user.");
        }

        [HttpPut("{UserId}")]
        public IActionResult Update(int UserId, UserDto user)
        {
            User? findUser = _entityFramework
                .Users.Where(User => User.UserId == UserId)
                .FirstOrDefault();

            if (findUser != null)
            {
                findUser.FirstName = user.FirstName;
                findUser.LastName = user.LastName;
                findUser.Email = user.Email;
                findUser.Active = user.Active;
                findUser.Gender = user.Gender;
                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok("User successfully updated.");
                }
                return BadRequest("Failed to update user.");
            }
            return NotFound("User Not Found.");
        }

        [HttpDelete("{UserId}")]
        public IActionResult Delete(int UserId)
        {
            User? findUser = _entityFramework
                .Users.Where(User => User.UserId == UserId)
                .FirstOrDefault();

            if (findUser != null)
            {
                _entityFramework.Remove(findUser);
                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok("User successfully deleted.");
                }
                throw new Exception("Failed to delete user.");
            }
            return NotFound("User Not Found.");
        }
    }
}
