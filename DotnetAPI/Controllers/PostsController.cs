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

        //* Get all posts.
        [HttpGet("")]
        public IActionResult Posts(Post inputs)
        {
            IEnumerable<Post> posts = _dataContext.Post.ToList();
            return Ok(posts);
        }

        //* Get post by ID
        [HttpGet("{PostId}")]
        public IActionResult Post(int PostId)
        {
            Post? post = _dataContext.Post.Where(post => post.PostId == PostId).FirstOrDefault();
            if (post == null)
                return NotFound("No post found with this id");
            return Ok(post);
        }

        //* Get all posts by user id.
        [HttpGet("user")]
        public IActionResult UserPosts()
        {
            string? userId = User.FindFirst("userId")?.Value;
            if (userId == null)
                return BadRequest("userId is required");
            int UserId = int.Parse(userId);

            IEnumerable<Post> userPosts = _dataContext
                .Post.Where(post => post.UserId == UserId)
                .ToList();
            return Ok(userPosts);
        }

        //* Crate a new post with user id.
        [HttpPost("")]
        public IActionResult add(PostDto inputs)
        {
            string? userId = User.FindFirst("userId")?.Value;
            if (userId == null)
                return BadRequest("userId is required");

            int UserId = int.Parse(userId);
            Post post = new Post();
            post.UserId = UserId;
            post.PostTitle = inputs.PostTitle;
            post.PostContect = inputs.PostContect;
            post.PostCreated = DateTime.Now;
            post.PostUpdated = DateTime.Now;
            _dataContext.Post.Add(post);
            _dataContext.SaveChanges();
            return Ok("Post Created Successfully.");
        }

        //* Search by post title or content.
        [HttpGet("search/{keywords}")]
        public IActionResult sarech(string keywords)
        {
            IEnumerable<Post> posts = _dataContext
                .Post.Where(post =>
                    post.PostTitle.Contains(keywords) || post.PostContect.Contains(keywords)
                )
                .ToList();
            return Ok(posts);
        }

        // TODO: Update post


        // TODO: Delete post
    }
}
