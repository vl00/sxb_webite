using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Repository.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly UserDbContext _dbcontext;
        public FeedbackRepository(UserDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public bool AddFeedback(Feedback feedback)
        {
            return _dbcontext.Execute(@"insert into feedback (id, userID, type, text, time) 
            values (@id, @userID, @type, @text, @time)", feedback) > 0;
        }

        public bool AddFeedbackImg(List<Feedback_Img> imgInfo)
        {
            return _dbcontext.Execute(@"insert into feedback_img (id, feedback_id, url) values (@id, @feedback_id, @url);", imgInfo) > 0;
        }

        public bool AddfeedbackCorrect(Feedback_corrected feedback) 
        {
            return _dbcontext.Execute(@"insert into feedback_corrected(id,type,feedback_id,school_id,school_name,before_corrected,after_corrected) values(@id,@type,@feedback_id,@school_id,@school_name,@before_corrected,@after_corrected)", feedback) > 0;
        }

        public bool AddReport(Report reportInfo)
        {
            using (var transaction = _dbcontext.BeginTransaction())
            {
                bool rez = _dbcontext.Execute(@"insert into report (id, userID, dataID, dataType, reason, [description], [evidenceURL], time) 
values (@id, @userID, @dataID, @dataType, @reason, @description, @evidenceURL, @time)", reportInfo) > 0;

                if (rez)
                {

                    if (reportInfo.Report_Imgs.Count > 0)
                    {
                        bool addImager = AddReportImg(reportInfo.Report_Imgs);

                        if (addImager)
                        {
                            transaction.Commit();
                        }
                        else
                        {
                            transaction.Rollback();
                        }
                    }
                    else
                    {
                        transaction.Commit();
                    }
                }
                else
                {
                    transaction.Rollback();
                }
                return rez;
            }
        }

        public bool AddReportImg(List<Report_Img> imgInfo)
        {
            return _dbcontext.Execute(@"insert into report_img (id, report_id, url) values (@id, @report_id, @url);", imgInfo) > 0;
        }
    }
}
