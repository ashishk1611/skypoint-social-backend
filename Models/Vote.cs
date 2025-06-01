namespace SkypointSocialBackend.Models
{
    public class Vote
    {
        public Guid Id { get; set; }
        public int Value { get; set; } // +1 or -1
        public Guid PostId { get; set; }
        public Post Post { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
