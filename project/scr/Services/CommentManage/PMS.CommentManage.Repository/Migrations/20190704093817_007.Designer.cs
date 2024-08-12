﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PMS.CommentsManage.Repository;

namespace PMS.CommentsManage.Repository.Migrations
{
    [DbContext(typeof(CommentsManageDbContext))]
    [Migration("20190704093817_007")]
    partial class _007
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.ExaminerRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AdminId");

                    b.Property<int>("ChangeAfterStatus");

                    b.Property<int>("ChangeFirstStatus");

                    b.Property<DateTime?>("ExaminerTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("ExaminerType");

                    b.Property<bool>("IsPartTimeJob");

                    b.Property<Guid>("TargetId");

                    b.HasKey("Id");

                    b.ToTable("ExaminerRecords");
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.GiveLike", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("LikeStatus");

                    b.Property<int>("LikeType");

                    b.Property<Guid>("ReplyId");

                    b.Property<Guid>("SourceId");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.ToTable("SchoolCommentLikes");
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.PartTimeJobAdmin", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("InvitationCode")
                        .HasMaxLength(8);

                    b.Property<string>("Name");

                    b.Property<Guid>("ParentId");

                    b.Property<string>("Password")
                        .HasMaxLength(32);

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(11);

                    b.Property<bool>("Prohibit");

                    b.Property<DateTime>("RegesitTime");

                    b.Property<int>("Role");

                    b.Property<int>("SettlementType");

                    b.HasKey("Id");

                    b.ToTable("PartTimeJobAdmins");
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.QuestionExamine", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AdminId");

                    b.Property<DateTime>("ExamineTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("QuestionInfoId");

                    b.HasKey("Id");

                    b.HasIndex("QuestionInfoId")
                        .IsUnique();

                    b.ToTable("QuestionExamines");
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.QuestionInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content")
                        .IsRequired();

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<bool>("IsAnony");

                    b.Property<bool>("IsHaveImagers");

                    b.Property<bool>("IsTop");

                    b.Property<int>("LikeCount");

                    b.Property<int>("PostUserRole");

                    b.Property<int>("ReplyCount");

                    b.Property<Guid>("SchoolId");

                    b.Property<Guid>("SchoolSectionId");

                    b.Property<int>("State");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.ToTable("QuestionInfos");
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.QuestionsAnswerExamine", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AdminId");

                    b.Property<DateTime?>("ExamineTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<bool>("IsPartTimeJob");

                    b.Property<Guid>("QuestionsAnswersInfoId");

                    b.HasKey("Id");

                    b.HasIndex("QuestionsAnswersInfoId")
                        .IsUnique();

                    b.ToTable("QuestionsAnswerExamines");
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.QuestionsAnswersInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<bool>("IsAnony");

                    b.Property<bool>("IsAttend");

                    b.Property<bool>("IsSchoolPublish");

                    b.Property<bool>("IsSettlement");

                    b.Property<bool>("IsTop");

                    b.Property<int>("LikeCount");

                    b.Property<Guid?>("ParentId");

                    b.Property<int>("PostUserRole");

                    b.Property<Guid>("QuestionInfoId");

                    b.Property<int>("ReplyCount");

                    b.Property<int>("State");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.HasIndex("QuestionInfoId");

                    b.ToTable("QuestionsAnswersInfos");
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.QuestionsAnswersReport", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("AnswersReplyId");

                    b.Property<Guid>("QuestionId");

                    b.Property<Guid?>("QuestionsAnswersInfoId");

                    b.Property<int>("ReportDataType");

                    b.Property<string>("ReportDetail")
                        .IsRequired();

                    b.Property<int>("ReportReasonType");

                    b.Property<DateTime>("ReportTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("ReportUserId");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.HasIndex("ReportReasonType");

                    b.ToTable("QuestionsAnswersReports");
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.QuestionsAnswersReportReply", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AdminId");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("ReplyContent");

                    b.Property<Guid>("ReportId");

                    b.HasKey("Id");

                    b.HasIndex("ReportId")
                        .IsUnique();

                    b.ToTable("AnswersReportReplys");
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.ReportType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Sort");

                    b.Property<string>("TypeName")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("ReportTypes");
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.SchoolComment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("CommentUserId");

                    b.Property<string>("Content");

                    b.Property<bool>("IsAnony");

                    b.Property<bool>("IsHaveImagers");

                    b.Property<bool>("IsSettlement");

                    b.Property<bool>("IsTop");

                    b.Property<int>("LikeCount");

                    b.Property<int>("PostUserRole");

                    b.Property<int>("ReplyCount");

                    b.Property<bool>("RumorRefuting");

                    b.Property<Guid>("SchoolId");

                    b.Property<Guid>("SchoolSectionId");

                    b.Property<int>("State");

                    b.HasKey("Id");

                    b.ToTable("SchoolComments");
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.SchoolCommentExamine", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("AddTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("AdminId");

                    b.Property<bool>("IsPartTimeJob");

                    b.Property<Guid>("SchoolCommentId");

                    b.Property<DateTime?>("UpdateTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.HasIndex("SchoolCommentId")
                        .IsUnique();

                    b.ToTable("SchoolCommentExamines");
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.SchoolCommentReply", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<bool>("IsAnony");

                    b.Property<bool>("IsAttend");

                    b.Property<bool>("IsSchoolPublish");

                    b.Property<bool>("IsTop");

                    b.Property<int>("LikeCount");

                    b.Property<int>("PostUserRole");

                    b.Property<int>("ReplyCount");

                    b.Property<Guid?>("ReplyId");

                    b.Property<bool>("RumorRefuting");

                    b.Property<Guid>("SchoolCommentId");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ReplyId");

                    b.HasIndex("SchoolCommentId");

                    b.ToTable("SchoolCommentReplies");
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.SchoolCommentReport", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CommentId");

                    b.Property<Guid?>("ReplayId");

                    b.Property<int>("ReportDataType");

                    b.Property<string>("ReportDetail")
                        .IsRequired();

                    b.Property<int>("ReportReasonType");

                    b.Property<DateTime>("ReportTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("ReportUserId");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("CommentId");

                    b.HasIndex("ReportReasonType");

                    b.ToTable("SchoolCommentReports");
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.SchoolCommentReportReply", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AdminId");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("ReplyContent");

                    b.Property<Guid>("ReportId");

                    b.HasKey("Id");

                    b.HasIndex("ReportId")
                        .IsUnique();

                    b.ToTable("CommentReportReplys");
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.SchoolCommentScore", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("AggScore");

                    b.Property<Guid>("CommentId");

                    b.Property<decimal>("EnvirScore");

                    b.Property<decimal>("HardScore");

                    b.Property<bool>("IsAttend");

                    b.Property<decimal>("LifeScore");

                    b.Property<decimal>("ManageScore");

                    b.Property<decimal>("TeachScore");

                    b.HasKey("Id");

                    b.HasIndex("CommentId")
                        .IsUnique();

                    b.ToTable("SchoolCommentScores");
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.SchoolImage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("DataSourcetId");

                    b.Property<int>("ImageType");

                    b.Property<string>("ImageUrl");

                    b.HasKey("Id");

                    b.ToTable("SchoolImage");
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.SchoolScore", b =>
                {
                    b.Property<Guid>("SchoolSectionId")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("AggScore");

                    b.Property<int>("AttendCommentCount");

                    b.Property<int>("CommentCount");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<decimal>("EnvirScore");

                    b.Property<decimal>("HardScore");

                    b.Property<DateTime>("LastCommentTime");

                    b.Property<decimal>("LifeScore");

                    b.Property<decimal>("ManageScore");

                    b.Property<Guid>("SchoolId");

                    b.Property<decimal>("TeachScore");

                    b.Property<DateTime>("UpdateTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("SchoolSectionId");

                    b.ToTable("SchoolScores");
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.SchoolTag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("SchoolCommentId");

                    b.Property<Guid>("SchoolId");

                    b.Property<Guid>("TagId");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("SchoolCommentId");

                    b.HasIndex("TagId");

                    b.ToTable("SchoolTags");
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.SettlementAmountMoney", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("BeginTime");

                    b.Property<DateTime>("EndTime");

                    b.Property<Guid>("PartTimeJobAdminId");

                    b.Property<float>("SettlementAmount");

                    b.Property<int>("SettlementStatus");

                    b.Property<int>("TotalAnswerSelected");

                    b.Property<int>("TotalSchoolCommentsSelected");

                    b.HasKey("Id");

                    b.HasIndex("PartTimeJobAdminId");

                    b.ToTable("SettlementAmountMoneys");
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.QuestionExamine", b =>
                {
                    b.HasOne("PMS.CommentsManage.Domain.Entities.QuestionInfo", "QuestionInfo")
                        .WithOne("QuestionExamine")
                        .HasForeignKey("PMS.CommentsManage.Domain.Entities.QuestionExamine", "QuestionInfoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.QuestionsAnswerExamine", b =>
                {
                    b.HasOne("PMS.CommentsManage.Domain.Entities.QuestionsAnswersInfo", "QuestionsAnswersInfo")
                        .WithOne("QuestionsAnswerExamine")
                        .HasForeignKey("PMS.CommentsManage.Domain.Entities.QuestionsAnswerExamine", "QuestionsAnswersInfoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.QuestionsAnswersInfo", b =>
                {
                    b.HasOne("PMS.CommentsManage.Domain.Entities.QuestionsAnswersInfo", "ParentAnswerInfo")
                        .WithMany()
                        .HasForeignKey("ParentId");

                    b.HasOne("PMS.CommentsManage.Domain.Entities.QuestionInfo", "QuestionInfo")
                        .WithMany("QuestionsAnswersInfos")
                        .HasForeignKey("QuestionInfoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.QuestionsAnswersReport", b =>
                {
                    b.HasOne("PMS.CommentsManage.Domain.Entities.QuestionInfo", "QuestionInfos")
                        .WithMany("QuestionsAnswersReports")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PMS.CommentsManage.Domain.Entities.ReportType", "ReportType")
                        .WithMany()
                        .HasForeignKey("ReportReasonType")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.QuestionsAnswersReportReply", b =>
                {
                    b.HasOne("PMS.CommentsManage.Domain.Entities.QuestionsAnswersReport", "QuestionsAnswersReport")
                        .WithOne("QuestionsAnswersReportReply")
                        .HasForeignKey("PMS.CommentsManage.Domain.Entities.QuestionsAnswersReportReply", "ReportId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.SchoolCommentExamine", b =>
                {
                    b.HasOne("PMS.CommentsManage.Domain.Entities.SchoolComment", "SchoolComment")
                        .WithOne("SchoolCommentExamine")
                        .HasForeignKey("PMS.CommentsManage.Domain.Entities.SchoolCommentExamine", "SchoolCommentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.SchoolCommentReply", b =>
                {
                    b.HasOne("PMS.CommentsManage.Domain.Entities.SchoolCommentReply", "ParentReplyInfo")
                        .WithMany()
                        .HasForeignKey("ReplyId");

                    b.HasOne("PMS.CommentsManage.Domain.Entities.SchoolComment", "SchoolComment")
                        .WithMany("SchoolCommentReplys")
                        .HasForeignKey("SchoolCommentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.SchoolCommentReport", b =>
                {
                    b.HasOne("PMS.CommentsManage.Domain.Entities.SchoolComment", "SchoolComments")
                        .WithMany("SchoolCommentReports")
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PMS.CommentsManage.Domain.Entities.ReportType", "ReportType")
                        .WithMany()
                        .HasForeignKey("ReportReasonType")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.SchoolCommentReportReply", b =>
                {
                    b.HasOne("PMS.CommentsManage.Domain.Entities.SchoolCommentReport", "SchoolCommentReport")
                        .WithOne("SchoolCommentReportReply")
                        .HasForeignKey("PMS.CommentsManage.Domain.Entities.SchoolCommentReportReply", "ReportId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.SchoolCommentScore", b =>
                {
                    b.HasOne("PMS.CommentsManage.Domain.Entities.SchoolComment", "SchoolComment")
                        .WithOne("SchoolCommentScore")
                        .HasForeignKey("PMS.CommentsManage.Domain.Entities.SchoolCommentScore", "CommentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.SchoolTag", b =>
                {
                    b.HasOne("PMS.CommentsManage.Domain.Entities.SchoolComment", "SchoolComment")
                        .WithMany("SchoolCommentTags")
                        .HasForeignKey("SchoolCommentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PMS.CommentsManage.Domain.Entities.Tag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PMS.CommentsManage.Domain.Entities.SettlementAmountMoney", b =>
                {
                    b.HasOne("PMS.CommentsManage.Domain.Entities.PartTimeJobAdmin", "PartTimeJobAdmin")
                        .WithMany("SettlementAmountMoneys")
                        .HasForeignKey("PartTimeJobAdminId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
