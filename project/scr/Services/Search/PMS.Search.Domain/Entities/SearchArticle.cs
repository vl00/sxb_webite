using System;
using System.Collections.Generic;
using System.Linq;

namespace PMS.Search.Domain.Entities
{
    public class SearchArticle : SearchPUV
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public Guid? UserId { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime Time { get; set; }
        public DateTime UpdateTime { get; set; }

        public DateTime? TopTime { get; set; }
        public bool ToTop { get; set; }

        public int ArticleType { get; set; }
        public int ViewCount { get; set; }

        /// <summary>
        /// 文章标签
        /// </summary>
        public List<string> Tags { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// 文章关联的学校类型
        /// </summary>
        public List<string> SchTypes { get; set; }

        /// <summary>
        /// 文章投放区域
        /// </summary>
        public List<ArticleArea> Areas { get; set; }

        /// <summary>
        /// 文章投放城市(区域的简集)
        /// </summary>
        public List<int> CityCodes { get; set; }

        public class ArticleArea
        {
            public int Id { get; set; }

            public int ProvinceId { get; set; }

            public int CityId { get; set; }

            public int AreaId { get; set; }
        }
    }
}
