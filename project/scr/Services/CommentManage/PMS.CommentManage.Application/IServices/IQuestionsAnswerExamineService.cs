﻿using PMS.CommentsManage.Domain.Entities;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.IServices
{
    /// <summary>
    /// 问答审核
    /// </summary>
    public interface IQuestionsAnswerExamineService : IAppService<QuestionsAnswerExamine>
    {
        /// <summary>
        /// 问答审核
        /// </summary>
        /// <param name="PartTimeJobAdminId"></param>
        /// <param name="TargetId"></param>
        /// <param name="Status"></param>
        /// <param name="ExaminerType"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool ExecExaminer(Guid PartTimeJobAdminId, Guid TargetId, int Status, bool IsPartTimeJob = true);
        /// <summary>
        /// 根据审核者id，获取该审核下的审核记录
        /// </summary>
        /// <param name="AdminId"></param>
        /// <param name="Total"></param>
        /// <returns></returns>
        List<QuestionsAnswerExamine> GetAnswerInfoByAdminId(Guid AdminId, int PageIndex, int PageSize, List<Guid> Ids, out int Total);
    }
}
