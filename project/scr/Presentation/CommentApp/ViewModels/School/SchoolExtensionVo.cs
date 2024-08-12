using PMS.CommentsManage.Domain.Entities;
using PMS.School.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Framework.Foundation;
using MongoDB.Bson;

namespace CommentApp.Models.School
{
    public class SchoolExtensionVo
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
        public decimal Score { get; set; }
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
        public SchoolComment SchoolComment { get; set; }
        public decimal SchoolCommentAvgScore { get; set; }

        public string SchoolSectionName => SchoolName.Split('-')[1];

        public int SchoolNo { get; set; }
        public string ShortSchoolNo => UrlShortIdUtil.Long2Base32(SchoolNo).ToLower();
    }

    public class AmbientModel
    {
        //public ObjectId _id { get; set; }
        public string gdid { get; set; }
        public string schoolname { get; set; }
        public double bookmarket { get; set; }
        public double poiinfo { get; set; }
        public double bus { get; set; }
        public double hospital { get; set; }
        public double library { get; set; }
        public double market { get; set; }
        public double metro { get; set; }
        public double museum { get; set; }
        public double play { get; set; }
        public double police { get; set; }
        public double river { get; set; }
        public double rubbish { get; set; }
        public double shoppinginfo { get; set; }
        public double subway { get; set; }
        public double traininfo { get; set; }
        public double view { get; set; }
        public double buildingprice { get; set; }
        public string _class { get; set; }
        public string eid { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        //public List<Poiinfo> poiinfos { get; set; }
    }

    public class Poiinfo
    {
        public string name { get; set; }
        public string address { get; set; }
        public string location { get; set; }
        public string distance { get; set; }
    }
}
