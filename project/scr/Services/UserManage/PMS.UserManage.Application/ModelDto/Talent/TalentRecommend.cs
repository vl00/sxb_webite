using iSchool;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace PMS.UserManage.Application.ModelDto.Talent
{
    public class TalentRecommend
    {
        /// <summary>
        /// data
        /// </summary>
        public List<Talent> Talents { get; set; }

        public int GroupIndex { get; set; } = 1;
        public int GroupSize { get; set; } = 50;

        #region search params

        public int CityCode { get; set; } = 440100;

        public Guid LoginUserId { get; set; }
        public Guid SchoolExtId { get; set; }
        #endregion

        /// <summary>
        /// 同城达人数量
        /// </summary>
        public long TheCityTalentSize { get; set; }

        /// <summary>
        /// 非同城达人数量
        /// </summary>
        public long OtherCityTalentSize { get; set; }

        /// <summary>
        /// 浏览过相同学校用户数量
        /// </summary>
        public long TheSchoolUserSize { get; set; }

        public class Talent
        {
            public Guid Id { get; set; }
            public string NickName { get; set; }
            public string HeadImgUrl { get; set; }
            public string headImager { get { return HeadImgUrl; } }

            /// <summary>
            /// 是否邀请过
            /// </summary>
            public bool IsInvite { get; set; }
            /// <summary>
            /// 是否是达人
            /// </summary>
            public bool IsTalent { get; set; }

            public int Score { get; set; }
        }
    }

}
