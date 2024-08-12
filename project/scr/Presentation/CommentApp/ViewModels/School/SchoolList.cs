using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Models.School
{
    public class SchoolListVo
    {
        /// <summary>
        /// 学校分部id，每个学校必定对应一间分部
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 学校id
        /// </summary>
        public Guid SchoolId { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 学校年级
        /// </summary>
        public int SchoolGrade { get; set; }
        /// <summary>
        /// 学校类型
        /// </summary>
        public int SchoolType { get; set; }
        /// <summary>
        /// 寄宿类型
        /// </summary>
        public int LodgingType { get; set; }
        /// <summary>
        /// 寄宿类型描述
        /// </summary>
        public string LodgingReason { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public float Longitude { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public float Latitude { get; set; }
        /// <summary>
        /// 口碑评级分
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// 星星
        /// </summary>
        public int StartTotal { get; set; }
        /// <summary>
        /// 学校点评总条数
        /// </summary>
        public int SchoolCommentTotal { get; set; }
        /// <summary>
        /// 是否上学帮认证
        /// </summary>
        public bool IsAuth { get; set; }
        /// <summary>
        /// 点评Id
        /// </summary>
        public Guid CommentId { get; set; }
        /// <summary>
        /// 点评
        /// </summary>
        public string SelectedCommentContent { get; set; }
        /// <summary>
        /// 回复总数
        /// </summary>
        public int SelectedReplyTotal { get; set; }
        /// <summary>
        /// 学费
        /// </summary>
        public decimal Tuition { get; set; }
    }
}
