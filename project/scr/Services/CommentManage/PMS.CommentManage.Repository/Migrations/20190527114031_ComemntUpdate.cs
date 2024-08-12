using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.CommentsManage.Repository.Migrations
{
    public partial class ComemntUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTop",
                table: "SchoolCommentReplies",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTop",
                table: "SchoolCommentReplies");
        }
    }
}
