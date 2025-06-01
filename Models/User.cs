using System.Xml.Linq;
using Microsoft.Extensions.Hosting;

namespace SkypointSocialBackend.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? DisplayName { get; set; }

        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public ICollection<Follow> Following { get; set; } = new List<Follow>();
        public ICollection<Follow> Followers { get; set; } = new List<Follow>();
    }

}
