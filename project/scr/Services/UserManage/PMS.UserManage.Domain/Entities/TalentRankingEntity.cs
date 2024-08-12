using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.UserManage.Domain.Entities
{
    /// <summary>
    /// 达人榜
    /// </summary>
    /// <modify author="qzy" time="2020-10-15 16:47:50"></modify>
    [Table("talentranking")]
    public class TalentRankingEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public Guid ID { get; set; }
        /// <summary>
        /// 排行时间
        /// </summary>
        public DateTime RankDate { get; set; }
        /// <summary>
        /// 排行数据
        /// </summary>
        public string DataJson { get; set; }
    }

    public class TalentRankingItem
    {
        /// <summary>
        /// 达人USERID
        /// </summary>
        public Guid UserID { get; set; }
        /// <summary>
        /// 达人名称
        /// </summary>
        public string TalentName { get; set; }
        /// <summary>
        /// 排名序号
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 排名是否上升
        /// <para>Null为未变化</para>
        /// </summary>
        public bool? IsUp { get; set; }
        /// <summary>
        /// 距上一位粉丝差距
        /// </summary>
        public int Distence { get; set; }
        /// <summary>
        /// 粉丝数
        /// </summary>
        public int FollowCount { get; set; }
        /// <summary>
        /// 达人描述
        /// </summary>
        public string TalentTitle { get; set; }
    }
}
