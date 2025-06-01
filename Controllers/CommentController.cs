using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkypointSocialBackend.Data;
using SkypointSocialBackend.DTOs;
using SkypointSocialBackend.Models;

namespace SkypointSocialBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentController(AppDbContext context)
        {
            _context = context;
        }

        // POST /comment
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CommentDto dto)
        {
            var post = await _context.Posts.FindAsync(dto.PostId);
            if (post == null)
                return NotFound("Post not found.");

            var user = await _context.Users.FindAsync(dto.AuthorId);
            if (user == null)
                return NotFound("User not found.");

            var comment = new Comment
            {
                Content = dto.Content,
                PostId = dto.PostId,
                AuthorId = dto.AuthorId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                comment.Id,
                comment.PostId,
                comment.Content,
                comment.CreatedAt,
                comment.AuthorId,
                Author = comment.Author.DisplayName
            });
        }
    }

}
