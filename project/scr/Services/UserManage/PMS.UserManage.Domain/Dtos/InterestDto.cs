using System;

namespace PMS.UserManage.Domain.Dtos
{
    public class InterestDto
    {
        public Guid? uuID { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid? userID { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string nickname { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string headImgUrl { get; set; }
        /// <summary>
        /// 城市编码
        /// </summary>
        public int? cityCode { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        public string introduction { get; set; }
        /// <summary>
        /// 关注学段 0：幼儿园1：小学2：初中3：高中
        /// </summary>
        public string focus_grade { get; set; }
        /// <summary>
        /// 关注类型 0：公办1：民办2：国际
        /// </summary>
        public string focus_type { get; set; }
        /// <summary>
        /// 关注住宿的制度 0：走读1：住宿
        /// </summary>
        public string focus_lodging { get; set; }

        public int? grade_1 { get { return (focus_grade+"").IndexOf("0") != -1 ? 1 : 0; } }
        public int? grade_2 { get { return (focus_grade + "").IndexOf("1") != -1 ? 1 : 0; } }
        public int? grade_3 { get { return (focus_grade + "").IndexOf("2") != -1 ? 1 : 0; } }
        public int? grade_4 { get { return (focus_grade + "").IndexOf("3") != -1 ? 1 : 0; } }
        public int? nature_1 { get { return (focus_type + "").IndexOf("0") != -1 ? 1 : 0; } }
        public int? nature_2 { get { return (focus_type + "").IndexOf("1") != -1 ? 1 : 0; } }
        public int? nature_3 { get { return (focus_type + "").IndexOf("2") != -1 ? 1 : 0; } }
        public int? lodging_0 { get { return (focus_lodging + "").IndexOf("0") != -1 ? 1 : 0; } }
        public int? lodging_1 { get { return (focus_lodging + "").IndexOf("1") != -1 ? 1 : 0; } }
    }

    public class MpUpdateUserDto
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string HeadImgUrl { get; set; }
    }
}
