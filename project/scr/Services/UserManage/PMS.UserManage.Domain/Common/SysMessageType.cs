using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Common
{
    /// <summary>
    /// 系统消息类型
    /// </summary>
    public enum SysMessageType
    {
        /// <summary>
        /// 认证成功
        /// </summary>
        AuthSuccess = 1,
        /// <summary>
        /// 认证失败
        /// </summary>
        AuthFail = 2,
        /// <summary>
        /// 邀请消息变更
        /// </summary>
        InviteChange = 3,
        /// <summary>
        /// 发布文章
        /// </summary>
        Article = 4,
        /// <summary>
        /// 发布直播
        /// </summary>
        Live = 5,
        /// <summary>
        /// 微课带有提问
        /// </summary>
        LiveQuestion = 6,
        /// <summary>
        /// 课堂审核通过
        /// </summary>
        LectureAuditPass = 10,//
        /// <summary>
        /// 收藏的课程开通通知
        /// </summary>
        CollectLectureOpenNotity = 11,//
        /// <summary>
        /// 讲师，助手开课提醒
        /// </summary>
        TeacherLectureOpenNotity = 12,//
        /// <summary>
        /// 课堂审核失败
        /// </summary>
        LectureAuditNo = 13,//
        /// <summary>
        /// 关注的讲师有新课
        /// </summary>
        FansLectureOpenSiteMsgNotity = 14,//

        /// <summary>
        /// 话题回复
        /// </summary>
        TopicReplyNotity = 15,
        /// <summary>
        /// 圈主信息提醒
        /// </summary>
        CircleUserNotity = 16,
        /// <summary>
        /// 圈子动态提醒
        /// </summary>
        CircleNewsNotity = 17,
        /// <summary>
        /// 加入圈子提醒
        /// </summary>
        JoinCircleNotity = 18,
    }
}
