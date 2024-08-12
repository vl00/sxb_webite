using System;
using System.Collections.Generic;

namespace PMS.School.Domain.Dtos
{
    [Serializable]
    public class RecommenderResponseDto
    {
        /// <summary>
        /// 0成功，1失败，失败时返回下面错误原因
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 错误时返回此字段
        /// </summary>
        public string ErrorDescription { get; set; }

        public List<RecommenderDto> Items { get; set; }

        public PageInfo PageInfo { get; set; }



    }



    [Serializable]
    public class RecommenderDto
    {
        public string ContentId { get; set; }

        public int BuType { get; set; }
    }
}
