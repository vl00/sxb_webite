using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.CommentsManage.Repository.Migrations
{
    public partial class UpdateDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SchoolCommentId",
                table: "SchoolCommentLikes",
                newName: "SourceId");

            migrationBuilder.AddColumn<int>(
                name: "LikeStatus",
                table: "SchoolCommentLikes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LikeType",
                table: "SchoolCommentLikes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "QuestionInfos",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LikeStatus",
                table: "SchoolCommentLikes");

            migrationBuilder.DropColumn(
                name: "LikeType",
                table: "SchoolCommentLikes");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "QuestionInfos");

            migrationBuilder.RenameColumn(
                name: "SourceId",
                table: "SchoolCommentLikes",
                newName: "SchoolCommentId");
        }
    }
}
