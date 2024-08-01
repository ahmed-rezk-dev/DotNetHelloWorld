
namespace DotNetAPI.Models
{
    public partial class Post
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string PostTitle { get; set; } = "";
        public string PostContect { get; set; } = "";
        public DateTime PostCreated { get; set; }
        public DateTime PostUpdated { get; set; }
    }

    public partial class PostDto
    {
        public required string PostTitle { get; set; }
        public required string PostContect { get; set; }
    }
}
