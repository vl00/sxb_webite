using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.CommentsManage.Repository.Migrations
{
    public partial class UpdatereportFor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchoolCommentReports_SchoolComments_CommentId",
                table: "SchoolCommentReports");

            migrationBuilder.DropIndex(
                name: "IX_SchoolCommentReports_CommentId",
                table: "SchoolCommentReports");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SchoolCommentReports_CommentId",
                table: "SchoolCommentReports",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolCommentReports_SchoolComments_CommentId",
                table: "SchoolCommentReports",
                column: "CommentId",
                principalTable: "SchoolComments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
