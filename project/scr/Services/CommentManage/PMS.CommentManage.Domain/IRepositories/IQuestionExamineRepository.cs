using PMS.CommentsManage.Domain.Entities;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.IRepositories
{
    /// <summary>
    /// 问题审核
    /// </summary>
    public interface IQuestionExamineRepository : IAppService<QuestionExamine>
    {
    }
}
