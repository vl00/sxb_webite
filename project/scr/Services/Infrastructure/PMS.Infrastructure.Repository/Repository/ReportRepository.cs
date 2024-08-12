using PMS.Infrastructure.Domain.Entities;
using PMS.Infrastructure.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Infrastructure.Repository.Repository
{
    public class ReportRepository : IFeedbackRepository
    {
        private JcDbContext _dbContext;
        public ReportRepository(JcDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool AddFeedback(Feedback feedback)
        {
                        return _dbContext.Execute(@"insert into feedback (id, userID, type, text, time) 
            values (@id, @userID, @type, @text, @time)", feedback) > 0;
        }

        public bool AddFeedbackImg(List<Feedback_Img> imgInfo)
        {
            return _dbContext.Execute(@"insert into feedback_img (id, feedback_id, url) values (@id, @feedback_id, @url);", imgInfo) > 0;
        }

        public bool AddReport(Report reportInfo)
        {
            using (var transaction = _dbContext.BeginTransaction())
            {
                bool rez = _dbContext.Execute(@"insert into report (id, userID, dataID, dataType, reason, [description], [evidenceURL], time) 
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
            return _dbContext.Execute(@"insert into report_img (id, report_id, url) values (@id, @report_id, @url);", imgInfo) > 0;
        }
    }
}
