using System;

namespace PMS.UserManage.Domain.Dtos
{
    public class InterestOuPutDto
    {
        /// <summary>
        /// 简称
        /// </summary>
        public string nickname { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string headImgUrl { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public string cityName { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        public string introduction { get; set; }
        /// <summary>
        /// 关注学段 0：幼儿园1：小学2：初中3：高中 |为多个连接符
        /// </summary>
        public string focus_grade { get; set; }
        /// <summary>
        /// 关注类型 0：公办1：民办2：国际 |为多个连接符
        /// </summary>
        public string focus_type { get; set; }
        /// <summary>
        /// 是否是达人员工
        /// </summary>
        public bool isTalentStaff { get; set; }
        /// <summary>
        /// 是否是达人
        /// </summary>
        public bool isTalent { get; set; }
        /// <summary>
        /// 达人Id
        /// </summary>
        public string talentId { get; set; }
        public Guid UserID { get; set; }
        /// <summary>
        /// 达人类型
        /// </summary>
        public int? talentType { get; set; }
        /// <summary>
        /// 达人认证称号预览
        /// </summary>
        public string talentCertificationPreview { get; set; }

        /// <summary>
        /// 关注住宿的制度 0：走读1：住宿 |为多个连接符
        /// </summary>
        public string focus_lodging { get; set; }
        public void initGrade(bool? Grade0, bool? Grade1, bool? Grade2, bool? Grade3) {
            focus_grade = ((Grade0 != null && Grade0==true) ? "0" : "") + ((Grade1 != null && Grade1 == true) ? "|1" : "") + ((Grade2 != null && Grade2 == true)  ? "|2" : "") + ((Grade3 != null && Grade3 == true)  ? "|3" : "");
        }
        public void initType (bool? Type0, bool? Type1, bool? Type2) {
            focus_type = ((Type0 != null && Type0 == true)  ? "0" : "") + ((Type1 != null && Type1 == true) ? "|1" : "") + ((Type2 != null && Type2 == true)   ? "|2" : "");
        }
        public void initLodging(bool? Lodging1, bool? Lodging2) {
            focus_lodging = ((Lodging1 != null && Lodging1 == true) ? "0" : "") + ((Lodging2 != null && Lodging2 == true)  ? "|1" : "");
        }

        public int? CityCode { get; set; }
    }
}
