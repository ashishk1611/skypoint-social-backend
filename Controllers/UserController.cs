using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkypointSocialBackend.Data;
using SkypointSocialBackend.DTOs;
using SkypointSocialBackend.Models;

namespace SkypointSocial.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // GET /User/{userId}/Following
        [HttpGet("{userId}/Following")]
        public async Task<IActionResult> GetFollowing(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            var following = await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Include(f => f.Followed)
                .Select(f => new {
                    f.FollowedId,
                    f.Followed.DisplayName,
                    f.Followed.Email
                })
                .ToListAsync();

            return Ok(following);
        }

        // POST /User/Follow
        [HttpPost("Follow")]
        public async Task<IActionResult> Follow([FromBody] FollowDto request)
        {
            if (request.FollowerId == request.FollowedId)
                return BadRequest("You cannot follow yourself.");

            var follower = await _context.Users.FindAsync(request.FollowerId);
            var followed = await _context.Users.FindAsync(request.FollowedId);

            if (follower == null || followed == null)
                return NotFound("User not found.");

            var existing = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == request.FollowerId && f.FollowedId == request.FollowedId);

            if (existing != null)
                return BadRequest("Already following.");

            var follow = new Follow
            {
                FollowerId = request.FollowerId,
                FollowedId = request.FollowedId
            };

            _context.Follows.Add(follow);
            await _context.SaveChangesAsync();

            return Ok("Followed successfully.");
        }

        // POST /User/Unfollow
        [HttpPost("Unfollow")]
        public async Task<IActionResult> Unfollow([FromBody] FollowDto request)
        {
            var follow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == request.FollowerId && f.FollowedId == request.FollowedId);

            if (follow == null)
                return NotFound("Follow relationship not found.");

            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();

            return Ok("Unfollowed successfully.");
        }
    }
}
