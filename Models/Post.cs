using System.Xml.Linq;

namespace SkypointSocialBackend.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid AuthorId { get; set; }
        public User Author { get; set; } = null!;

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public int UpvoteCount { get; set; }
        public int DownvoteCount { get; set; }
    }

}
