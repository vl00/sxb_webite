using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.CommentsManage.Repository.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExaminerRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ExaminerType = table.Column<int>(nullable: false),
                    AdminId = table.Column<Guid>(nullable: false),
                    TargetId = table.Column<Guid>(nullable: false),
                    ChangeFirstStatus = table.Column<int>(nullable: false),
                    ChangeAfterStatus = table.Column<int>(nullable: false),
                    ExaminerTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExaminerRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartTimeJobAdmins",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Phone = table.Column<string>(maxLength: 11, nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Role = table.Column<int>(nullable: false),
                    Password = table.Column<string>(maxLength: 32, nullable: true),
                    ParentId = table.Column<Guid>(nullable: false),
                    InvitationCode = table.Column<string>(maxLength: 8, nullable: true),
                    Prohibit = table.Column<bool>(nullable: false),
                    SettlementType = table.Column<int>(nullable: false),
                    RegesitTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartTimeJobAdmins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TypeName = table.Column<string>(nullable: false),
                    Sort = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchoolCommentLikes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SchoolCommentId = table.Column<Guid>(nullable: false),
                    ReplyId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolCommentLikes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchoolImage",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DataSourcetId = table.Column<Guid>(nullable: false),
                    ImageType = table.Column<int>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolImage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Content = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    NickName = table.Column<string>(nullable: false),
                    Phone = table.Column<string>(nullable: false),
                    Role = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SettlementAmountMoneys",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PartTimeJobAdminId = table.Column<Guid>(nullable: false),
                    TotalSchoolCommentsSelected = table.Column<int>(nullable: false),
                    TotalAnswerSelected = table.Column<int>(nullable: false),
                    BeginTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    AddTime = table.Column<DateTime>(nullable: false),
                    SettlementAmount = table.Column<float>(nullable: false),
                    SettlementStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettlementAmountMoneys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SettlementAmountMoneys_PartTimeJobAdmins_PartTimeJobAdminId",
                        column: x => x.PartTimeJobAdminId,
                        principalTable: "PartTimeJobAdmins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    SchoolId = table.Column<Guid>(nullable: false),
                    SchoolSectionId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    Content = table.Column<string>(nullable: false),
                    IsAnony = table.Column<bool>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionInfos_UserInfo_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchoolComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SchoolId = table.Column<Guid>(nullable: false),
                    SchoolSectionId = table.Column<Guid>(nullable: false),
                    CommentUserId = table.Column<Guid>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    IsTop = table.Column<bool>(nullable: false),
                    ReplyCount = table.Column<int>(nullable: false),
                    LikeCount = table.Column<int>(nullable: false),
                    IsSettlement = table.Column<bool>(nullable: false),
                    IsAnony = table.Column<bool>(nullable: false),
                    RumorRefuting = table.Column<bool>(nullable: false),
                    AddTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolComments_UserInfo_CommentUserId",
                        column: x => x.CommentUserId,
                        principalTable: "UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionExamines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AdminId = table.Column<Guid>(nullable: false),
                    ExamineTime = table.Column<DateTime>(nullable: false),
                    QuestionInfoId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionExamines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionExamines_QuestionInfos_QuestionInfoId",
                        column: x => x.QuestionInfoId,
                        principalTable: "QuestionInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionsAnswersInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    QuestionInfoId = table.Column<Guid>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    IsSchoolPublish = table.Column<bool>(nullable: false),
                    IsAttend = table.Column<bool>(nullable: false),
                    IsAnony = table.Column<bool>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    LikeCount = table.Column<int>(nullable: false),
                    IsSettlement = table.Column<bool>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionsAnswersInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionsAnswersInfos_QuestionInfos_QuestionInfoId",
                        column: x => x.QuestionInfoId,
                        principalTable: "QuestionInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionsAnswersInfos_UserInfo_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SchoolCommentExamines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AdminId = table.Column<Guid>(nullable: false),
                    IsPartTimeJob = table.Column<bool>(nullable: false),
                    SchoolCommentId = table.Column<Guid>(nullable: false),
                    AddTime = table.Column<DateTime>(nullable: true),
                    UpdateTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolCommentExamines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolCommentExamines_SchoolComments_SchoolCommentId",
                        column: x => x.SchoolCommentId,
                        principalTable: "SchoolComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchoolCommentReplies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SchoolCommentId = table.Column<Guid>(nullable: false),
                    ReplyId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    IsSchoolPublish = table.Column<bool>(nullable: false),
                    IsAttend = table.Column<bool>(nullable: false),
                    IsAnony = table.Column<bool>(nullable: false),
                    RumorRefuting = table.Column<bool>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    ReplyCount = table.Column<int>(nullable: false),
                    LikeCount = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolCommentReplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolCommentReplies_SchoolComments_SchoolCommentId",
                        column: x => x.SchoolCommentId,
                        principalTable: "SchoolComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchoolCommentReplies_UserInfo_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchoolCommentReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ReportReasonType = table.Column<int>(nullable: false),
                    ReportUserId = table.Column<Guid>(nullable: false),
                    CommentId = table.Column<Guid>(nullable: false),
                    ReportDetail = table.Column<string>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ReportTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolCommentReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolCommentReports_SchoolComments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "SchoolComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchoolCommentReports_ReportTypes_ReportReasonType",
                        column: x => x.ReportReasonType,
                        principalTable: "ReportTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchoolCommentReports_UserInfo_ReportUserId",
                        column: x => x.ReportUserId,
                        principalTable: "UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SchoolCommentScores",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CommentId = table.Column<Guid>(nullable: false),
                    IsAttend = table.Column<bool>(nullable: false),
                    AggScore = table.Column<decimal>(nullable: false),
                    TeachScore = table.Column<decimal>(nullable: false),
                    HardScore = table.Column<decimal>(nullable: false),
                    EnvirScore = table.Column<decimal>(nullable: false),
                    ManageScore = table.Column<decimal>(nullable: false),
                    LifeScore = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolCommentScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolCommentScores_SchoolComments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "SchoolComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchoolTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TagId = table.Column<Guid>(nullable: false),
                    SchoolId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    SchoolCommentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolTags_SchoolComments_SchoolCommentId",
                        column: x => x.SchoolCommentId,
                        principalTable: "SchoolComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchoolTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchoolTags_UserInfo_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionsAnswerExamines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    QuestionsAnswersInfoId = table.Column<Guid>(nullable: false),
                    AdminId = table.Column<Guid>(nullable: false),
                    IsPartTimeJob = table.Column<bool>(nullable: false),
                    ExamineTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionsAnswerExamines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionsAnswerExamines_QuestionsAnswersInfos_QuestionsAnswersInfoId",
                        column: x => x.QuestionsAnswersInfoId,
                        principalTable: "QuestionsAnswersInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionsAnswersReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ReportReasonType = table.Column<int>(nullable: false),
                    ReportUserId = table.Column<Guid>(nullable: false),
                    QuestionsAnswersInfoId = table.Column<Guid>(nullable: false),
                    ReportDetail = table.Column<string>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ReportTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionsAnswersReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionsAnswersReports_QuestionsAnswersInfos_QuestionsAnswersInfoId",
                        column: x => x.QuestionsAnswersInfoId,
                        principalTable: "QuestionsAnswersInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionsAnswersReports_ReportTypes_ReportReasonType",
                        column: x => x.ReportReasonType,
                        principalTable: "ReportTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionsAnswersReports_UserInfo_ReportUserId",
                        column: x => x.ReportUserId,
                        principalTable: "UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommentReportReplys",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ReportId = table.Column<Guid>(nullable: false),
                    AdminId = table.Column<Guid>(nullable: false),
                    ReplyContent = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentReportReplys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentReportReplys_SchoolCommentReports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "SchoolCommentReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnswersReportReplys",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ReportId = table.Column<Guid>(nullable: false),
                    AdminId = table.Column<Guid>(nullable: false),
                    ReplyContent = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswersReportReplys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswersReportReplys_QuestionsAnswersReports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "QuestionsAnswersReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswersReportReplys_ReportId",
                table: "AnswersReportReplys",
                column: "ReportId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentReportReplys_ReportId",
                table: "CommentReportReplys",
                column: "ReportId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionExamines_QuestionInfoId",
                table: "QuestionExamines",
                column: "QuestionInfoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionInfos_UserId",
                table: "QuestionInfos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionsAnswerExamines_QuestionsAnswersInfoId",
                table: "QuestionsAnswerExamines",
                column: "QuestionsAnswersInfoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionsAnswersInfos_QuestionInfoId",
                table: "QuestionsAnswersInfos",
                column: "QuestionInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionsAnswersInfos_UserId",
                table: "QuestionsAnswersInfos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionsAnswersReports_QuestionsAnswersInfoId",
                table: "QuestionsAnswersReports",
                column: "QuestionsAnswersInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionsAnswersReports_ReportReasonType",
                table: "QuestionsAnswersReports",
                column: "ReportReasonType");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionsAnswersReports_ReportUserId",
                table: "QuestionsAnswersReports",
                column: "ReportUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolCommentExamines_SchoolCommentId",
                table: "SchoolCommentExamines",
                column: "SchoolCommentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchoolCommentReplies_SchoolCommentId",
                table: "SchoolCommentReplies",
                column: "SchoolCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolCommentReplies_UserId",
                table: "SchoolCommentReplies",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolCommentReports_CommentId",
                table: "SchoolCommentReports",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolCommentReports_ReportReasonType",
                table: "SchoolCommentReports",
                column: "ReportReasonType");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolCommentReports_ReportUserId",
                table: "SchoolCommentReports",
                column: "ReportUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolComments_CommentUserId",
                table: "SchoolComments",
                column: "CommentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolCommentScores_CommentId",
                table: "SchoolCommentScores",
                column: "CommentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchoolTags_SchoolCommentId",
                table: "SchoolTags",
                column: "SchoolCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolTags_TagId",
                table: "SchoolTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolTags_UserId",
                table: "SchoolTags",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SettlementAmountMoneys_PartTimeJobAdminId",
                table: "SettlementAmountMoneys",
                column: "PartTimeJobAdminId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswersReportReplys");

            migrationBuilder.DropTable(
                name: "CommentReportReplys");

            migrationBuilder.DropTable(
                name: "ExaminerRecords");

            migrationBuilder.DropTable(
                name: "QuestionExamines");

            migrationBuilder.DropTable(
                name: "QuestionsAnswerExamines");

            migrationBuilder.DropTable(
                name: "SchoolCommentExamines");

            migrationBuilder.DropTable(
                name: "SchoolCommentLikes");

            migrationBuilder.DropTable(
                name: "SchoolCommentReplies");

            migrationBuilder.DropTable(
                name: "SchoolCommentScores");

            migrationBuilder.DropTable(
                name: "SchoolImage");

            migrationBuilder.DropTable(
                name: "SchoolTags");

            migrationBuilder.DropTable(
                name: "SettlementAmountMoneys");

            migrationBuilder.DropTable(
                name: "QuestionsAnswersReports");

            migrationBuilder.DropTable(
                name: "SchoolCommentReports");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "PartTimeJobAdmins");

            migrationBuilder.DropTable(
                name: "QuestionsAnswersInfos");

            migrationBuilder.DropTable(
                name: "SchoolComments");

            migrationBuilder.DropTable(
                name: "ReportTypes");

            migrationBuilder.DropTable(
                name: "QuestionInfos");

            migrationBuilder.DropTable(
                name: "UserInfo");
        }
    }
}
