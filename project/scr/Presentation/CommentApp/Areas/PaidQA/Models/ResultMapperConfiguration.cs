using AutoMapper;
using PMS.OperationPlateform.Domain.DTOs;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.EntityExtend;
using PMS.School.Application.ModelDto;
using PMS.School.Domain.Dtos;
using PMS.Search.Domain.Entities;
using PMS.TopicCircle.Domain.Dtos;
using PMS.TopicCircle.Domain.Entities;
using PMS.UserManage.Application.ModelDto;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Areas.Coupon.Models.Coupon;
using Sxb.Web.Areas.PaidQA.Models.Evaluate;
using Sxb.Web.Areas.PaidQA.Models.Order;
using Sxb.Web.Areas.PaidQA.Models.Question;
using Sxb.Web.Areas.PaidQA.Models.Talent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Sxb.Web.Areas.PaidQA.Models
{
    public class ResultMapperConfiguration : Profile
    {
        public ResultMapperConfiguration()
        {
            _ = CreateMap<PMS.PaidQA.Domain.Entities.Order, OrderInfoResult>();
            _ = CreateMap<PMS.PaidQA.Domain.Entities.Message, MessageResult>();
            _ = CreateMap<UserInfoDto, UserInfoResult>();
            _ = CreateMap<PMS.PaidQA.Domain.Entities.Evaluate, EvaluateResult>();
            _ = CreateMap<TalentDetailExtend, TalentInfoResult>()
                .ForMember(t => t.UserID, opt => opt.MapFrom(s => s.TalentUserID))
                .ForMember(t => t.NickName, opt => opt.MapFrom(s => s.NickName))
                .ForMember(t => t.AuthName, opt => opt.MapFrom(s => s.AuthName))
                .ForMember(t => t.LevelName, opt => opt.MapFrom(s => s.TalentLevelName))
                .ForMember(t => t.GradeNames, opt => opt.MapFrom(s => s.TalentGrades.Select(g => g.Name)))
                .ForMember(t => t.RegionTypeNames, opt => opt.MapFrom(s => s.TalentRegions.Select(r => r.Name)))
                .ForMember(t => t.ReplyPercent, opt => opt.MapFrom(s => s.SixHourReplyPercent))
                .ForMember(t => t.Score, opt => opt.MapFrom(s => s.AvgScore))
                .ForMember(t => t.Price, opt => opt.MapFrom(s => s.Price))
                .ForMember(t=>t.HeadImgUrl,opt=>opt.MapFrom(s=>s.HeadImgUrl));
            _ = CreateMap<TalentDetailExtend, TalentInfoResult_01>()
              .ForMember(t => t.UserID, opt => opt.MapFrom(s => s.TalentUserID))
              .ForMember(t => t.NickName, opt => opt.MapFrom(s => s.NickName))
              .ForMember(t => t.AuthName, opt => opt.MapFrom(s => s.AuthName))
              .ForMember(t => t.LevelName, opt => opt.MapFrom(s => s.TalentLevelName))
              .ForMember(t => t.GradeNames, opt => opt.MapFrom(s => s.TalentGrades.Select(g => g.Name)))
              .ForMember(t => t.RegionTypeNames, opt => opt.MapFrom(s => s.TalentRegions.Select(r => r.Name)))
              .ForMember(t => t.ReplyPercent, opt => opt.MapFrom(s => s.SixHourReplyPercent))
              .ForMember(t => t.Score, opt => opt.MapFrom(s => s.AvgScore))
              .ForMember(t => t.Price, opt => opt.MapFrom(s => s.Price))
              .ForMember(t => t.HeadImgUrl, opt => opt.MapFrom(s => s.HeadImgUrl));


            _ = CreateMap<SchoolExtFilterDto, SearchRenderSchoolResult>();
            _ = CreateMap<SchoolRank, SearchRenderSchoolRanklResult>();
            _ = CreateMap<ArticleDto, SearchRenderArticleResult>();
            _ = CreateMap<PMS.School.Domain.Dtos.SchoolExtDto, Talent.SchoolResult>();

        }
    }
}
