using DnsClient;
using PMS.CommentsManage.Application.ModelDto;
using PMS.School.Domain.Common;
using PMS.UserManage.Domain.Common;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;
using static Sxb.PCWeb.Areas.School.Models.AggSearchVM;

namespace Sxb.PCWeb.Areas.School.Models
{

    public class AggSearchQuestionVM
    {
        /// <summary>
        /// 问题Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 短链
        /// </summary>
        public string ShortNo { get; set; }

        /// <summary>
        /// 写入的用户id
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string HeadImgUrl { get; set; }

        /// <summary>
        /// 达人类型
        /// </summary>
        public TalentType? TalentType { get; set; }


        /// <summary>
        /// 问题内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 写入时间
        /// </summary>
        public string CreateTime { get; set; }

        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplyCount { get; set; }

        /// <summary>
        /// 学校问题总数
        /// </summary>
        public int SchoolQuestionCount { get; set; }

        /// <summary>
        /// 是否收藏
        /// </summary>
        public bool IsCollection { get; set; }

        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }

        /// <summary>
        /// <summary>
        /// 图片
        /// </summary>
        public IEnumerable<string> Images { get; set; }


        /// <summary>
        /// 当前问题的回答详情
        /// </summary>
        public AggSearchAnswerVM Answer { get; set; }

        public QuestionSchoolInfoVM School { get; set; }

        internal static AggSearchQuestionVM Convert(UserQuestionDto s)
        {
            var vm = new AggSearchQuestionVM()
            {
                Id = s.Id,
                ShortNo = UrlShortIdUtil.Long2Base32(s.No),
                UserId = s.UserId,
                NickName = s.User?.NickName,
                HeadImgUrl = s.User?.HeadImgUrl,
                TalentType = s.User?.Type,
                Content = s.Content,
                CreateTime = s.CreateTime.ConciseTime(),
                ReplyCount = s.ReplyCount,
                IsCollection = s.IsCollection,
                IsAnony = s.IsAnony,
                Images = s.Images,
                School = new QuestionSchoolInfoVM()
            };

            if (s.Answer != null)
            {
                vm.Answer = new AggSearchAnswerVM();
                vm.Answer.Id = s.Answer.Id;
                vm.Answer.UserId = s.Answer.User?.UserId ?? Guid.Empty;
                vm.Answer.Content = s.Answer.Content;
                vm.Answer.NickName = s.Answer.User?.NickName;
                vm.Answer.HeadImgUrl = s.Answer.User?.HeadImgUrl;
                vm.Answer.ReplyCount = s.Answer.ReplyCount;
                vm.Answer.CreateTime = s.Answer.FormatterCreateTime;
            }
            if (s.School != null)
            {
                vm.School.ShortNo = s.School.ShortSchoolNo;
                vm.School.SchoolName = s.School.SchoolName;
                vm.School.QuestionCount = s.SchoolQuestionCount;
                vm.School.LodgingType = s.School.LodgingType;
                vm.School.SchoolType = s.School.SchoolType;
            }
            return vm;
        }
    }
}
