using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkypointSocialBackend.Data;
using SkypointSocialBackend.DTOs;
using SkypointSocialBackend.Models;

namespace SkypointSocialBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostController(AppDbContext context)
        {
            _context = context;
        }

        // POST /post
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
                return NotFound("User not found.");

            var post = new Post
            {
                Content = dto.Content,
                AuthorId = dto.UserId
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                post.Id,
                post.Content,
                post.AuthorId,
                Author = post.Author.DisplayName,
                CreatedAt = DateTime.SpecifyKind(post.CreatedAt, DateTimeKind.Utc),
                CommentsCount = post.Comments.Count,
                VotesCount = post.Votes.Count,
                Votes = post.UpvoteCount - post.DownvoteCount,
                Comments = post.Comments.Select(c => new
                {
                    c.Id,
                    c.Content,
                    Author = c.Author.DisplayName,
                    CreatedAt = DateTime.SpecifyKind(c.CreatedAt, DateTimeKind.Utc),
                }).ToList(),
            });
        }

        // GET /post?userId=...
        [HttpGet]
        public IActionResult GetPosts([FromQuery] Guid? userId)
        {
            var query = _context.Posts
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new
                {
                    p.Id,
                    p.Content,
                    p.AuthorId,
                    AuthorName = p.Author.DisplayName,
                    p.CreatedAt,
                    Votes = p.Votes.Sum(v => v.Value),
                    VotesCount = p.Votes.Count,
                    CommentsCount = p.Comments.Count,
                    Comments = p.Comments.Select(c => new
                    {
                        c.Id,
                        c.Content,
                        Author = c.Author.DisplayName,
                        c.CreatedAt
                    }),
                    Followers = p.Author.Followers.Select(f => f.FollowerId)
                });

            if (userId.HasValue)
            {
                var followedIds = _context.Follows
                    .Where(f => f.FollowerId == userId.Value)
                    .Select(f => f.FollowedId)
                    .ToList();

                query = query
                    .OrderByDescending(p => followedIds.Contains(p.AuthorId))
                    .ThenByDescending(x => x.CreatedAt);
            }

            var posts = query
                .ToList()
                .Select(p => new
                {
                    p.Id,
                    p.Content,
                    p.AuthorId,
                    Author = p.AuthorName,
                    CreatedAt = DateTime.SpecifyKind(p.CreatedAt, DateTimeKind.Utc),
                    p.CommentsCount,
                    p.VotesCount,
                    p.Votes,
                    Comments = p.Comments.Select(c => new
                    {
                        c.Id,
                        c.Content,
                        c.Author,
                        CreatedAt = DateTime.SpecifyKind(c.CreatedAt, DateTimeKind.Utc)
                    }).ToList()
                });

            return Ok(posts);
        }
    }
}
