using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;

namespace PMS.PaidQA.Domain.EntityExtend
{
    public class TalentDetailExtend : TalentSetting
    {
        /// <summary>
        /// 擅长领域
        /// </summary>
        public IEnumerable<RegionType> TalentRegions { get; set; }
        /// <summary>
        /// 学段
        /// </summary>
        public IEnumerable<Grade> TalentGrades { get; set; }
        /// <summary>
        /// 达人昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 头像URL
        /// </summary>
        public string HeadImgUrl { get; set; }
        /// <summary>
        /// 达人认证名称
        /// </summary>
        public string AuthName { get; set; }
        /// <summary>
        /// 达人介绍
        /// </summary>
        public string TelentIntroduction { get; set; }
        /// <summary>
        /// 认证等级名称
        /// </summary>
        public string TalentLevelName { get; set; }
        /// <summary>
        /// 平均评分
        /// </summary>
        public double AvgScore { get; set; }
        /// <summary>
        /// 六小时回复率
        /// </summary>
        public double SixHourReplyPercent { get; set; }
        /// <summary>
        /// 是否开启上学问
        /// </summary>
        public bool IsEnable { get; set; }


        /// <summary>
        /// 指定用户是否关注该达人
        /// </summary>
        public bool IsFollowed { get; set; }
        /// <summary>
        /// 达人类型(0.个人 | 1.机构)
        /// </summary>
        public int TalentType { get; set; }

        /// <summary>
        /// 详情背景图
        /// </summary>
        public List<string> Covers
        {
            get
            {

                List<string> defaultCovers = new List<string>();
                try{
                    if (!string.IsNullOrEmpty(this.JA_Covers))
                    {
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(this.JA_Covers); ;
                    }
                }
                catch {
                
                }
                return defaultCovers;
            }
        }

        public Guid? SchoolExtId { get; set; }
    }
}