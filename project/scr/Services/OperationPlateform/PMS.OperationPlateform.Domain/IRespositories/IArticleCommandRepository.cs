using PMS.OperationPlateform.Domain.DTOs;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Domain.IRespositories
{
    public interface IArticleCommandRepository
    {
       
        bool AddViewCount(Guid articleId, int viewCount);

       
        /// <summary>
        /// 添加文章订阅配置记录
        /// </summary>
        /// <param name="article_SubscribePreference"></param>
        /// <returns></returns>
        bool AddArticleSubscribeInfo(Article_SubscribePreference article_SubscribePreference);

        /// <summary>
        /// 修改文章订阅配置记录
        /// </summary>
        /// <param name="article_SubscribePreference"></param>
        /// <returns></returns>
        bool UpdateArticleSubscribeInfo(Article_SubscribePreference article_SubscribePreference);
        
    }
}