using PMS.School.Application.ModelDto;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.School.Application.IServices
{
    public interface ISchCorrectService
    {
        /// <summary>
        /// 获取学校原信息
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        SchSourceInfoDto GetSchSourceInfo(Guid eid);
        /// <summary>
        /// 插入纠错信息
        /// </summary>
        /// <returns></returns>
        bool Insert(ExtraSchCorrect0Dto correct);
    }
}
