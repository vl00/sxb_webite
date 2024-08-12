using PMS.UserManage.Application.ModelDto.Info;
using Sxb.UserCenter.Models.MessageViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Models.UserInfoViewModel
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string HeadImage { get; set; }
        /// <summary>
        /// 用户角色 【个人、企业】
        /// </summary>
        public int? Role { get; set; }
        /// <summary>
        /// 是否认证
        /// </summary>
        public bool IsAuth { get; set; }
        /// <summary>
        /// 认证title
        /// </summary>
        public string AuthTitle { get; set; }
        /// <summary>
        /// 用户简介
        /// </summary>
        public string Introduction { get; set; }
    }

    /// <summary>
    /// 用户个人详细信息
    /// </summary>
    public class UserInfoDetailModel : UserViewModel
    {
        /// <summary>
        /// 是否关注
        /// </summary>
        public bool IsFollow { get; set; }
        /// <summary>
        /// 认证title
        /// </summary>
        public string AuthTitle { get; set; }
        /// <summary>
        /// 认证简介
        /// </summary>
        public string AuthIntroduce { get; set; }
        /// <summary>
        /// 是否是员工
        /// </summary>
        public bool IsTalentStaff { get; set; }
        /// <summary>
        /// 员工对应的机构userid
        /// </summary>
        public Guid? ParentUserId { get; set; }

        /// <summary>
        /// 关注数
        /// </summary>
        public int FollowTotal { get; set; }
        /// <summary>
        /// 粉丝数
        /// </summary>
        public int FansTotal { get; set; }
        /// <summary>
        /// 我的关键词
        /// </summary>
        public Interest Interest { get; set; }
        /// <summary>
        /// 机构名
        /// </summary>
        public string Organization_name { get; set; }
        /// <summary>
        /// 学部id
        /// </summary>
        public Guid Eid { get; set; }
    }   

    /// <summary>
    /// 我的动态数
    /// </summary>
    public class MydynamicDetail : UserViewModel
    {
        /// <summary>
        /// 是否登录
        /// </summary>
        public bool IsLogin { get; set; }
        /// <summary>
        /// 是否绑定手机号
        /// </summary>
        public bool IsBindPhone { get; set; }
        /// <summary>
        /// 关注数
        /// </summary>
        public int Follow { get; set; }
        /// <summary>
        /// 动态数
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 是否有新系统消息
        /// </summary>
        public bool HasSysMeesage { get; set; }
        /// <summary>
        /// 最近浏览数
        /// </summary>
        public int History { get; set; }
        /// <summary>
        /// 邀请我的
        /// </summary>
        public List<InviteMessageViewModel> Invites { get; set; }
        /// <summary>
        /// 关注的直播
        /// </summary>
        public List<FollowLive> FollowLive { get; set; }
    }
}
