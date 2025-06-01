namespace SkypointSocialBackend.Models
{
    public class Follow
    {
        public Guid Id { get; set; }

        public Guid FollowerId { get; set; }
        public User Follower { get; set; } = null!;

        public Guid FollowedId { get; set; }
        public User Followed { get; set; } = null!;
    }

}
