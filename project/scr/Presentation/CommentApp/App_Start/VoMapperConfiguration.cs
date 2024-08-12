using AutoMapper;
using CommentApp.Models.School;
using PMS.School.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Models.Comment;
using PMS.CommentsManage.Domain.Entities;
using Sxb.Web.Models.User;
using PMS.School.Application.ModelDto;
using PMS.CommentsManage.Application.ModelDto;
using Sxb.Web.Models;
using Microsoft.Extensions.Options;
using Sxb.Web.App_Start;
using Microsoft.Extensions.Logging.Abstractions;
using Sxb.Web.Models.Question;
using Sxb.Web.Models.Replay;
using PMS.CommentsManage.Application.ModelDto.Reply;
using Sxb.Web.Models.School;
using Sxb.Web.Models.Answer;
using PMS.UserManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.Total;
using Sxb.Web.ViewModels.Comment;
using Sxb.Web.ViewModels.Question;
using PMS.School.Infrastructure.Common;
using PMS.School.Domain.Dtos;
using Sxb.Web.ViewModels.School;
using PMS.UserManage.Application.ModelDto;
using Sxb.Web.ViewModels.Sign;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using Sxb.Web.ViewModels.FeedBack;
using Sxb.Web.ViewModels.TopicCircle;
using Sxb.Web.ViewModels.SchoolRank;

namespace CommentApp.App_Start
{
    public class VoMapperConfiguration : Profile
    {
        private readonly string _queryImage;
        public VoMapperConfiguration(string query)
        {
            _queryImage = query;

            CreateMap<SchoolDto, SchoolExtensionVo>()
                .ForMember(s => s.LodgingType, opt => opt.MapFrom(s => (int)s.LodgingType))
                .ForMember(s => s.LodgingReason, opt => opt.MapFrom(s => s.LodgingType.Description()));
            CreateMap<SchoolInfoDto, SchoolExtensionVo>()
                .ForMember(s => s.Id, opt => opt.MapFrom(s => s.SchoolSectionId))
                .ForMember(s => s.LodgingType, opt => opt.MapFrom(s => (int)s.LodgingType))
                .ForMember(s => s.LodgingReason, opt => opt.MapFrom(s => s.LodgingType.Description()));

            //.ForMember(source => source.SchoolGrade, opt => opt.MapFrom(s => s.SchoolGrade.Description()))
            //.ForMember(source => source.SchoolType, opt => opt.MapFrom(s => s.SchoolType.Description()))
            //.ForMember(source => source.Lodging, opt => opt.MapFrom(s => s.Lodging ? "寄宿" : ""));

            CreateMap<SchoolDto, SchoolListVo>()
                .ForMember(s => s.LodgingType, opt => opt.MapFrom(s => (int)s.LodgingType))
                .ForMember(s => s.LodgingReason, opt => opt.MapFrom(s => s.LodgingType.Description()));

            CreateMap<CommentWriterVo, SchoolComment>();
            CreateMap<CommentWriterVo, SchoolCommentScore>();

            CreateMap<SchoolCommentReply, SchoolCommentReplyVo>()
                .ForMember(s => s.Content, opt => opt.MapFrom(s => s.Content.Trim()))
                .ForMember(s => s.CreateTime, opt => opt.MapFrom(s => s.CreateTime.ConciseTime()));

            CreateMap<PMS.UserManage.Application.ModelDto.UserInfoDto, UserInfoVo>()
            .ForMember(source => source.HeadImager, opt => opt.MapFrom(s => s.HeadImgUrl));

            CreateMap<PMS.UserManage.Application.ModelDto.InviteUserInfoDto, InviteUserInfoVo>()
            .ForMember(source => source.HeadImager, opt => opt.MapFrom(s => s.HeadImgUrl))
            .ForMember(source => source.Role,opt => opt.MapFrom(s=>s.VerifyTypes));

            //CreateMap<UserInfo, UserInfoVo>()
            //.ForMember(source => source.HeadImager, opt => opt.MapFrom(s => _queryImage + s.HeadImgUrl));

            CreateMap<UserInfoDto, UserInfoVo>()
                .ForMember(source => source.Role, opt => opt.MapFrom(s => s.VerifyTypes))
                .ForMember(source => source.HeadImager, opt => opt.MapFrom(s => s.HeadImgUrl));

            CreateMap<SchoolCommentDto, CommentRootVo>();

            CreateMap<ReportType, ReportTypeVo>();

            CreateMap<CommentReportDto, SchoolCommentReport>();

            CreateMap<SchoolCommentDto, CommentList>()
                .ForMember(s => s.Content, opt => opt.MapFrom(s => s.Content.Trim()))
                .ForMember(s => s.CreateTime, opt => opt.MapFrom(s => s.CreateTime.ConciseTime()))
                .ForMember(s=>s.No,opt=>opt.MapFrom(s=> UrlShortIdUtil.Long2Base32(s.No).ToLower()));

            CreateMap<SchoolInfoDto, CommentList.SchoolInfoVo>()
                .ForMember(s => s.CommentTotal, opt => opt.MapFrom(s => s.SectionCommentTotal))
                .ForMember(s => s.LodgingType, opt => opt.MapFrom(s => (int)s.LodgingType))
                .ForMember(s => s.LodgingReason, opt => opt.MapFrom(s => s.LodgingType.Description()));

            CreateMap<QuestionDto, QuestionVo>()
                .ForMember(s=>s.QuestionContent,opt => opt.MapFrom(s=>s.QuestionContent.Trim()))
                .ForMember(s => s.QuestionCreateTime, opt => opt.MapFrom(s => s.QuestionCreateTime.ToString("yyyy年MM月dd日")))
                .ForMember(s=>s.No,opt=>opt.MapFrom(s=> UrlShortIdUtil.Long2Base32(s.No).ToLower()));


            CreateMap<SchoolInfoQaDto, QuestionVo.SchoolInfo>()
                .ForMember(s => s.LodgingType, opt => opt.MapFrom(s => (int)s.LodgingType))
                .ForMember(s => s.LodgingReason, opt => opt.MapFrom(s => s.LodgingType.Description()));


            CreateMap<ReplyDto, ReplayExhibition>();
            //.ForMember(s => s.AddTime, opt => opt.MapFrom(s => s.AddTime.ConciseTime()));

            CreateMap<SchoolCommentDto, CommentExhibition>();

            CreateMap<SchoolComment, SchoolCommentViewModel>()
                .ForMember(s => s.AddTime, opt => opt.MapFrom(s => s.AddTime.ConciseTime()))
                .ForMember(s => s.No, opt => opt.MapFrom(s => UrlShortIdUtil.Long2Base32(s.No).ToLower()));

            CreateMap<AnswerInfoDto, AnswerInfoVo>();
            //.ForMember(s => s.AddTime, opt => opt.MapFrom(s => s.AddTime.ToString("yyyy-MM-dd")));

            CreateMap<AnswerAdd, QuestionsAnswersInfo>();

            //学校点评统计
            CreateMap<SchoolCommentTotal, SchoolCommentTotalVo>()
                .ForMember(s => s.TotalType, opt => opt.MapFrom(s => s.TotalType.Description()))
                .ForMember(s => s.TotalTypeNumber, opt => opt.MapFrom(s => (int)s.TotalType));

            //学校问答统计
            CreateMap<SchoolQuestionTotal, SchoolQuesionTotalVo>()
                .ForMember(s => s.TotalType, opt => opt.MapFrom(s => s.TotalType.Description()))
                .ForMember(s => s.TotalTypeNumber, opt => opt.MapFrom(s => (int)s.TotalType));

            //纠错
            CreateMap<ExtraSchCorrect0Dto, ExtraSchCorrect0>()
                .AfterMap((s, t) =>
                {
                    t.Type = (byte)s.Type;
                    t.Img = JsonHelper.ObjectToJSON(s.Img);
                    t.SchType = s.SchType;
                    //if (s.SchType.IndexOf('.') < 0) t.SchType = iSchool.SchFType0.GetSchType(s.SchType).ToString();
                });

            //学校详情信息、分数、统计
            CreateMap<SchoolCollectionDto, SchoolCollectionVo>()
                .ForMember(s => s.LodgingType, opt => opt.MapFrom(s => (int)s.LodgingType))
                .ForMember(s => s.LodgingReason, opt => opt.MapFrom(s => s.LodgingType.Description()));

            //聚合留资页 课程类型列表
            CreateMap<CourseTypeVo, PMS.School.Domain.Entities.CourseType>();

            //聚合留资页 用户留资
            CreateMap<LSRFLeveInfoVo, LSRFLeveInfo>()
                .ForMember(s => s.Type, opt => opt.MapFrom(s => (LSRFLeveInfoType)s.Type));

            //聚合留资页 学校列表
            //CreateMap<LSRFSchoolDetail, LSRFSchoolDetailViewModel>()
            //    .ForMember(s => s.Abroad, opt => opt.MapFrom(s => JsonToKV.ConvertObject(s.Abroad)))
            //    .ForMember(s => s.Courses, opt => opt.MapFrom(s => JsonToKV.ConvertObject(s.Courses)))
            //    .ForMember(s => s.Subjects, opt => opt.MapFrom(s => JsonToKV.ConvertObject(s.Subjects)))
            //    .ForMember(s => s.Abroad, opt => opt.MapFrom(s => JsonToKV.ConvertObject(s.Authentication)));

            //学校纠错信息
            CreateMap<SchoolFeedback,SchoolFeedbackVo>();

            //话题圈搜索
            CreateMap<PMS.Search.Application.ModelDto.SearchCircleDto, CircleItemViewModel>();
            CreateMap<PMS.TopicCircle.Application.Dtos.SearchCircleDto, CircleItemViewModel>();

            //榜单
            CreateMap<SchoolRank, SchoolRankDetailViewModel>();
        }
    }
}
