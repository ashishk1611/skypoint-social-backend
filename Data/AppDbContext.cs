namespace SkypointSocialBackend.Data
{
    using Microsoft.EntityFrameworkCore;
    using SkypointSocialBackend.Models;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Vote> Votes => Set<Vote>();
        public DbSet<Follow> Follows => Set<Follow>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Comments.Author → User (NO CASCADE)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Posts.Author → User (NO CASCADE)
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Votes.User → User (NO CASCADE)
            //modelBuilder.Entity<Vote>()
            //    .HasOne(v => v.User)
            //    .WithMany(u => u.Votes)
            //    .HasForeignKey(v => v.UserId)
            //    .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Post)
                .WithMany(p => p.Votes)
                .HasForeignKey(v => v.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Follows.Follower → User (NO CASCADE)
            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Follows.Followed → User (NO CASCADE)
            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Followed)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FollowedId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
                .Property(p => p.UpvoteCount)
                .HasDefaultValue(0);

            modelBuilder.Entity<Post>()
                .Property(p => p.DownvoteCount)
                .HasDefaultValue(0);


        }
    }

}
