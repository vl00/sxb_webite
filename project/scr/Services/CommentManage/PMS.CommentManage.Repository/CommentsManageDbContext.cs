using Microsoft.EntityFrameworkCore;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Repository.Interface;

namespace PMS.CommentsManage.Repository
{
    public class CommentsManageDbContext : DbContext, ICommentsManageDbContext
    {
        public CommentsManageDbContext() 
        {
        
        }
        public CommentsManageDbContext(DbContextOptions<CommentsManageDbContext> options)
            : base(options)
        {
        }


        public virtual DbSet<QuestionsAnswersReportReply> AnswersReportReplys { get; set; }
        public virtual DbSet<SchoolCommentReportReply> CommentReportReplys { get; set; }
        public virtual DbSet<ExaminerRecord> ExaminerRecords { get; set; }
        //public virtual DbSet<JobStateChangeRecord> JobStateChangeRecords { get; set; }
        //public virtual DbSet<PartJobTimeSettlementRe> PartJobTimeSettlementRes { get; set; }
        public virtual DbSet<PartTimeJobAdmin> PartTimeJobAdmins { get; set; }
        public virtual DbSet<PartTimeJobAdminRole> PartTimeJobAdminRoles { get; set; }
        public virtual DbSet<QuestionExamine> QuestionExamines { get; set; }
        public virtual DbSet<QuestionInfo> QuestionInfos { get; set; }
        public virtual DbSet<QuestionsAnswerExamine> QuestionsAnswerExamines { get; set; }
        public virtual DbSet<QuestionsAnswersInfo> QuestionsAnswersInfos { get; set; }
        public virtual DbSet<QuestionsAnswersReport> AnswerReports { get; set; }
        public virtual DbSet<ReportType> ReportTypes { get; set; }
        public virtual DbSet<SchoolComment> SchoolComments { get; set; }
        public virtual DbSet<SchoolCommentExamine> SchoolCommentExamines { get; set; }
        public virtual DbSet<GiveLike> SchoolCommentLikes { get; set; }
        public virtual DbSet<SchoolCommentReply> SchoolCommentReplies { get; set; }
        public virtual DbSet<SchoolCommentReport> SchoolCommentReports { get; set; }
        public virtual DbSet<SchoolCommentScore> SchoolCommentScores { get; set; }
        public virtual DbSet<SchoolImage> SchoolCommentImages { get; set; }
        public virtual DbSet<SchoolScore> SchoolScores { get; set; }
        public virtual DbSet<SchoolTag> SchoolCommentTags { get; set; }
        public virtual DbSet<SettlementAmountMoney> SettlementAmountMoneys { get; set; }
        public virtual DbSet<CommentTag> Tags { get; set; }
        public virtual DbSet<UserGrantAuth> UserGrantAuths { get; set; }
        public virtual DbSet<UserInfoEx> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuestionsAnswersReportReply>(entity =>
            {
                entity.HasIndex(e => e.ReportId)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ReplyContent).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.QuestionsAnswersReport)
                    .WithOne(p => p.QuestionsAnswersReportReply)
                    .HasForeignKey<QuestionsAnswersReportReply>(d => d.ReportId);
            });

            modelBuilder.Entity<SchoolCommentReportReply>(entity =>
            {
                entity.HasIndex(e => e.ReportId)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreateTime).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.SchoolCommentReport)
                    .WithOne(p => p.SchoolCommentReportReply)
                    .HasForeignKey<SchoolCommentReportReply>(d => d.ReportId);
            });

            modelBuilder.Entity<ExaminerRecord>(entity =>
            {
                entity.HasIndex(e => e.AdminId)
                    .HasName("AdminId");

                entity.HasIndex(e => e.TargetId)
                    .HasName("TargetId");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ExaminerTime).HasDefaultValueSql("(getdate())");
            });

           

            modelBuilder.Entity<PartTimeJobAdmin>(entity =>
            {
                entity.HasIndex(e => e.Phone)
                    .HasName("AdminPhone");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.InvitationCode).HasMaxLength(8);

                entity.Property(e => e.Password).HasMaxLength(32);

                entity.Property(e => e.Phone).HasMaxLength(11);
            });

            modelBuilder.Entity<PartTimeJobAdminRole>(entity =>
            {
                entity.HasIndex(e => e.AdminId)
                    .HasName("AdminId");

                entity.HasIndex(e => e.Role)
                    .HasName("AdminRole");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Shield).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.PartTimeJobAdmin)
                    .WithMany(p => p.PartTimeJobAdminRoles)
                    .HasForeignKey(d => d.AdminId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PartTimeJ__Admin__65CCCF36");
            });

            modelBuilder.Entity<QuestionExamine>(entity =>
            {
                entity.HasIndex(e => e.AdminId)
                    .HasName("lX_AdminId");

                entity.HasIndex(e => e.QuestionInfoId)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.QuestionInfo)
                    .WithOne(p => p.QuestionExamine)
                    .HasForeignKey<QuestionExamine>(d => d.QuestionInfoId);
            });

            modelBuilder.Entity<QuestionInfo>(entity =>
            {
                entity.HasIndex(e => e.Id)
                    .HasName("QuestionInfos_Id")
                    .IsUnique();

                entity.HasIndex(e => e.SchoolId)
                    .HasName("NonClustered_SchoolId");

                entity.HasIndex(e => e.SchoolSectionId)
                    .HasName("NonClusteredIndex-QuestionInfos-SchoolSectionId");

                entity.HasIndex(e => e.UserId)
                    .HasName("NonClusteredIndex-QuestionInfos-userID");

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(800);

                entity.Property(e => e.CreateTime).HasDefaultValueSql("(getdate())");

            });

            modelBuilder.Entity<QuestionsAnswerExamine>(entity =>
            {
                entity.HasIndex(e => e.QuestionsAnswersInfoId)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ExamineTime).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.QuestionsAnswersInfo)
                    .WithOne(p => p.QuestionsAnswerExamine)
                    .HasForeignKey<QuestionsAnswerExamine>(d => d.QuestionsAnswersInfoId);
            });

            modelBuilder.Entity<QuestionsAnswersInfo>(entity =>
            {
                entity.HasIndex(e => e.ParentId);

                entity.HasIndex(e => e.QuestionInfoId);

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreateTime).HasDefaultValueSql("(getdate())");

              
                entity.HasOne(d => d.QuestionInfo)
                    .WithMany(p => p.QuestionsAnswersInfos)
                    .HasForeignKey(d => d.QuestionInfoId);

            });

            modelBuilder.Entity<QuestionsAnswersReport>(entity =>
            {
                entity.HasIndex(e => e.ReportReasonType);

                entity.HasIndex(e => e.ReportUserId);

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ReportDetail).IsRequired();

                entity.Property(e => e.ReportTime).HasDefaultValueSql("(getdate())");


            });

            modelBuilder.Entity<ReportType>(entity =>
            {
                entity.Property(e => e.TypeName).IsRequired();
            });

            modelBuilder.Entity<SchoolComment>(entity =>
            {
                entity.HasIndex(e => e.CommentUserId)
                    .HasName("Comment_CommentUserId");

                entity.HasIndex(e => e.Id)
                    .HasName("SchoolComment_Id")
                    .IsUnique();

                entity.HasIndex(e => e.SchoolId)
                    .HasName("Comment_SchoolId");

                entity.HasIndex(e => e.SchoolSectionId)
                    .HasName("Comment_SchoolSectionId");

                entity.HasIndex(e => e.State)
                    .HasName("Comment_State");

                entity.HasIndex(e => e.CommentUserId);

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AddTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Content).HasMaxLength(800);


            });

            modelBuilder.Entity<SchoolCommentExamine>(entity =>
            {
                entity.HasIndex(e => e.SchoolCommentId)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AddTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdateTime).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.SchoolComment)
                    .WithOne(p => p.SchoolCommentExamine)
                    .HasForeignKey<SchoolCommentExamine>(d => d.SchoolCommentId);
            });

            modelBuilder.Entity<GiveLike>(entity =>
            {
                entity.Property(e => e.CreateTime).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SchoolCommentReply>(entity =>
            {
                entity.HasIndex(e => e.ReplyId);

                entity.HasIndex(e => e.SchoolCommentId);

                entity.HasIndex(e => e.UserId)
                    .HasName("NonClusteredIndex-SchoolCommentReplies-userID");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Content).HasMaxLength(800);

                entity.Property(e => e.CreateTime).HasDefaultValueSql("(getdate())");


                entity.HasOne(d => d.SchoolComment)
                    .WithMany(p => p.SchoolCommentReplys)
                    .HasForeignKey(d => d.SchoolCommentId);
            });

            modelBuilder.Entity<SchoolCommentReport>(entity =>
            {
                entity.HasIndex(e => e.ReportReasonType);

                entity.HasIndex(e => e.ReportUserId);

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ReportDetail).IsRequired();

                entity.Property(e => e.ReportTime).HasDefaultValueSql("(getdate())");

            });

            modelBuilder.Entity<SchoolCommentScore>(entity =>
            {
                entity.HasIndex(e => e.CommentId)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AggScore).HasColumnType("decimal(18, 5)");

                entity.Property(e => e.EnvirScore).HasColumnType("decimal(18, 5)");

                entity.Property(e => e.HardScore).HasColumnType("decimal(18, 5)");

                entity.Property(e => e.LifeScore).HasColumnType("decimal(18, 5)");

                entity.Property(e => e.ManageScore).HasColumnType("decimal(18, 5)");

                entity.Property(e => e.TeachScore).HasColumnType("decimal(18, 5)");

                entity.HasOne(d => d.SchoolComment)
                    .WithOne(p => p.SchoolCommentScore)
                    .HasForeignKey<SchoolCommentScore>(d => d.CommentId);
            });

            modelBuilder.Entity<SchoolImage>(entity =>
            {
                entity.ToTable("SchoolImage");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AddTime).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SchoolScore>(entity =>
            {
                entity.HasKey(e => e.SchoolSectionId);

                entity.HasIndex(e => e.SchoolId)
                    .HasName("SchoolScore_SchoolId");

                entity.HasIndex(e => e.SchoolSectionId)
                    .HasName("SchoolScore_SchoolSectionId")
                    .IsUnique();

                entity.Property(e => e.SchoolSectionId).ValueGeneratedNever();

                entity.Property(e => e.AggScore).HasColumnType("decimal(18, 5)");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EnvirScore).HasColumnType("decimal(18, 5)");

                entity.Property(e => e.HardScore).HasColumnType("decimal(18, 5)");

                entity.Property(e => e.LastCommentTime).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.LastQuestionTime).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.LifeScore).HasColumnType("decimal(18, 5)");

                entity.Property(e => e.ManageScore).HasColumnType("decimal(18, 5)");

                entity.Property(e => e.TeachScore).HasColumnType("decimal(18, 5)");

                entity.Property(e => e.UpdateTime).HasDefaultValueSql("(getdate())");
            });


            modelBuilder.Entity<SchoolTag>(entity =>
            {
                entity.HasIndex(e => e.SchoolCommentId);

                entity.HasIndex(e => e.TagId);

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.SchoolComment)
                    .WithMany(p => p.SchoolCommentTags)
                    .HasForeignKey(d => d.SchoolCommentId);

            });

            modelBuilder.Entity<SettlementAmountMoney>(entity =>
            {
                entity.HasIndex(e => e.PartTimeJobAdminId);

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AddTime).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.PartTimeJobAdmin)
                    .WithMany(p => p.SettlementAmountMoneys)
                    .HasForeignKey(d => d.PartTimeJobAdminId);
            });

            modelBuilder.Entity<CommentTag>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<UserGrantAuth>(entity =>
            {
                entity.Property(e => e.GrantAuthTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<UserInfoEx>(entity =>
            {
                entity.ToTable("UserInfo");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.NickName).IsRequired();

                entity.Property(e => e.Phone).IsRequired();
            });


            base.OnModelCreating(modelBuilder);
            //加载程序集加载所有的继承自EntityTypeConfiguration 为模型配置类
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(CommentsManageDbContext).Assembly);

            //modelBuilder.Entity<QuestionsAnswersInfo>().HasOne(x => x.User).WithMany(x => x.QuestionsAnswersInfos).OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<SchoolComment>().HasOne(x => x.User).WithMany(x => x.SchoolComment).OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<SchoolCommentReport>().HasOne(x => x.User).WithMany(x => x.SchoolCommentReport).OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<QuestionsAnswersReport>().HasOne(x => x.User).WithMany(x => x.QuestionsAnswersReports).OnDelete(DeleteBehavior.Restrict);
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseLazyLoadingProxies().UseSqlServer("Server=mssql.8mb.xyz;Database=Test2;User ID=sa;Password=abc123..");
        //}
    }

}
