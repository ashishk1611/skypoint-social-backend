using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkypointSocialBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddVoteCountsToPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DownvoteCount",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UpvoteCount",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DownvoteCount",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "UpvoteCount",
                table: "Posts");
        }
    }
}
