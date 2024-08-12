using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.School.Domain.IRepositories
{
    public interface ISchCorrectRepository
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
        bool Insert(ExtraSchCorrect0 correct);
    }
}
