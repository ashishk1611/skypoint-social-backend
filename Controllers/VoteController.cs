
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
    public class VoteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VoteController(AppDbContext context)
        {
            _context = context;
        }

        // POST /vote
        [HttpPost]
        public async Task<IActionResult> CastVote([FromBody] VoteDto request)
        {
            if (request.Value != 1 && request.Value != -1)
                return BadRequest("Vote must be +1 or -1.");

            var post = await _context.Posts.FindAsync(request.PostId);
            if (post == null)
                return NotFound("Post not found.");

            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                return NotFound("User not found.");

            var existingVote = await _context.Votes
                .FirstOrDefaultAsync(v => v.PostId == request.PostId && v.UserId == request.UserId);

            if (existingVote != null)
            {
                if (existingVote.Value == request.Value)
                {
                    return Ok(new
                    {
                        PostId = post.Id,
                        VotesCount = post.UpvoteCount + post.DownvoteCount,
                        Votes = post.UpvoteCount - post.DownvoteCount,
                    });
                }

                // Update vote and post counts
                if (existingVote.Value == 1)
                    post.UpvoteCount--;
                else if (existingVote.Value == -1)
                    post.DownvoteCount--;

                if (request.Value == 1)
                    post.UpvoteCount++;
                else
                    post.DownvoteCount++;

                existingVote.Value = request.Value;
                // Optionally: existingVote.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var vote = new Vote
                {
                    PostId = request.PostId,
                    UserId = request.UserId,
                    Value = request.Value
                };
                _context.Votes.Add(vote);

                if (request.Value == 1)
                    post.UpvoteCount++;
                else
                    post.DownvoteCount++;
            }

            await _context.SaveChangesAsync();
            return Ok(new
            {
                PostId = post.Id,
                VotesCount = post.UpvoteCount + post.DownvoteCount,
                Votes = post.UpvoteCount - post.DownvoteCount,
            });
        }
    }

}
