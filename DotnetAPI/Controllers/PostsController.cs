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
            post.PostContent = inputs.PostContent;
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
                    post.PostTitle.Contains(keywords) || post.PostContent.Contains(keywords)
                )
                .ToList();
            return Ok(posts);
        }

        //* Update post by {PostId}
        [HttpPut("{postId}")]
        public IActionResult Update(int postId, PostDto inputs)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());

            Post? post = _dataContext.Post.Where(post => post.PostId == postId).FirstOrDefault();

            if (post != null)
            {
                post.PostTitle = inputs.PostTitle;
                post.PostContent = inputs.PostContent;
                post.PostUpdated = DateTime.Now;
                if (_dataContext.SaveChanges() > 0)
                {
                    return Ok("Post successfully updated.");
                }
                return BadRequest("Failed to update post.");
            }
            return NotFound("Post Not Found.");
        }

        //* Delete post by {PostId}
        [HttpDelete("{postId}")]
        public IActionResult Delete(int postId)
        {
            Post? post = _dataContext.Post.Where(post => post.PostId == postId).FirstOrDefault();
            if (post != null)
            {
                _dataContext.Post.Remove(post);
                _dataContext.SaveChanges();
                return Ok("Post successfully deleted.");
            }
            return NotFound("Post Not Found.");
        }
    }
}
