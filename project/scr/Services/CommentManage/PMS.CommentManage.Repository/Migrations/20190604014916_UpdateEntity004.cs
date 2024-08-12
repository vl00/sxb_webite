using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.CommentsManage.Repository.Migrations
{
    public partial class UpdateEntity004 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "SchoolCommentReplies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "SchoolCommentReplies");
        }
    }
}
