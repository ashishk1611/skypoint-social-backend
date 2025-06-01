namespace SkypointSocialBackend.DTOs
{
    public class VoteDto
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public int Value { get; set; } // +1 or -1
    }

}
