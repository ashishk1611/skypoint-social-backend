namespace SkypointSocialBackend.DTOs
{
    public class CommentDto
    {
        public Guid PostId { get; set; }
        public Guid AuthorId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
