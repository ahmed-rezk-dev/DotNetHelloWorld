using DotNetAPI.Data;
using DotNetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostsController : ControllerBase
    {
        DataContextEF _dataContext;

        public PostsController(IConfiguration config)
        {
            _dataContext = new DataContextEF(config);
        }

        [HttpGet("")]
        public IActionResult Posts(Post inputs)
        {
            IEnumerable<Post> posts = _dataContext.Post.ToList();
            return Ok(posts);
        }

        // TODO: Get post by ID


        // TODO: Get all posts by user id.


        // TODO: Update post


        // TODO: Delete post


        // TODO: search by post title or content.
    }
}
