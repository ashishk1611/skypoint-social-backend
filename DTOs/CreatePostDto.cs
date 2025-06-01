namespace SkypointSocialBackend.DTOs
{
    public class CreatePostDto
    {
        public Guid UserId { get; set; } // We'll replace this with auth later
        public string Content { get; set; } = string.Empty;
    }
}
