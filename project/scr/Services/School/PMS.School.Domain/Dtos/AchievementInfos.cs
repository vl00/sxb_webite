using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.School.Domain.Dtos
{


    /// <summary>
    /// 榜单的结构
    /// </summary>
    public class AchievementInfos
    {

        /// <summary>
        /// 榜单名称
        /// </summary>
        public string RankName { get; set; }

        public string Alias { get; set; }

        public List<RankItems> List { get; set; }


        public class RankItems
        {

            //年份
            public int Year { get; set; }

            public List<RankItem> Items { get; set; }

            public class RankItem
            {

                /// <summary>
                /// 大学ID
                /// </summary>
                public Guid SchoolId { get; set; }

                /// <summary>
                /// 大学名称
                /// </summary>
                public string SchoolName { get; set; }

                /// <summary>
                /// 排序号
                /// </summary>
                public int Sort { get; set; }

                /// <summary>
                /// 录取人数
                /// </summary>
                public int Count { get; set; } = 0;
            }

        }
    }
    public class AchievementData
    {
        public int ErrCode { get; set; }

        public string Msg { get; set; }

        public List<AchievementInfos> Data { get; set; }
    }

}




