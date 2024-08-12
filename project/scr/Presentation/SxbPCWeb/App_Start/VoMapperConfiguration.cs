using AutoMapper;
using PMS.School.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Framework.Foundation;
using PMS.CommentsManage.Domain.Entities;
using Sxb.PCWeb.Models.User;
using PMS.School.Application.ModelDto;
using PMS.CommentsManage.Application.ModelDto;
using Sxb.PCWeb.Models;
using Microsoft.Extensions.Options;
using Sxb.PCWeb.App_Start;
using Microsoft.Extensions.Logging.Abstractions;
using PMS.CommentsManage.Application.ModelDto.Reply;
using PMS.UserManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.Total;
using PMS.School.Infrastructure.Common;
using Sxb.PCWeb.ViewModels.Comment;
using Sxb.PCWeb.RequestModel.Comment;
using Sxb.PCWeb.RequestModel.Question;
using Sxb.Web.RequestModel.Question;
using Sxb.PCWeb.ViewModels.Sign;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using Sxb.PCWeb.ViewModels.CourseType;
using Sxb.PCWeb.ViewModels.School;
using Sxb.PCWeb.Utils;
using Sxb.PCWeb.ViewModels.SchoolRank;

namespace CommentApp.App_Start
{
    public class VoMapperConfiguration : Profile
    {
        public VoMapperConfiguration() 
        {
            //学校点评统计
            CreateMap<SchoolCommentTotal, SchoolCommentTotalViewModel>()
                .ForMember(s => s.TotalType, opt => opt.MapFrom(s => s.TotalType.Description()))
                .ForMember(s => s.TotalTypeNumber, opt => opt.MapFrom(s => (int)s.TotalType));

            //学校问答统计
            CreateMap<SchoolQuestionTotal, SchoolQuestionTotalViewModel>()
                .ForMember(s => s.TotalType, opt => opt.MapFrom(s => s.TotalType.Description()))
                .ForMember(s => s.TotalTypeNumber, opt => opt.MapFrom(s => (int)s.TotalType));

            //写点评
            CreateMap<CommentWriterViewModel, SchoolComment>()
                .ForMember(s => s.SchoolId, opt => opt.MapFrom(s => s.Sid))
                .ForMember(s => s.SchoolSectionId, opt => opt.MapFrom(s => s.Eid));

            CreateMap<CommentWriterViewModel, SchoolCommentScore>();


            //提问
            CreateMap<QuestionWriterViewModel, QuestionInfo>();

            //回答
            CreateMap<AnswerAdd, QuestionsAnswersInfo>();

            //聚合留资页 课程类型列表
            CreateMap<CourseTypeVo, PMS.School.Domain.Entities.CourseType>();

            //聚合留资页 用户留资
            CreateMap<LSRFLeveInfoVo, LSRFLeveInfo>()
                .ForMember(s => s.Type,opt => opt.MapFrom(s=>(LSRFLeveInfoType)s.Type));

            //聚合留资页 学校列表
            CreateMap<LSRFSchoolDetail, LSRFSchoolDetailViewModel>()
                .ForMember(s => s.Abroad, opt => opt.MapFrom(s => JsonToKV.ConvertObject(s.Abroad)))
                .ForMember(s => s.Courses, opt => opt.MapFrom(s => JsonToKV.ConvertObject(s.Courses)))
                .ForMember(s => s.Subjects, opt => opt.MapFrom(s => JsonToKV.ConvertObject(s.Subjects)))
                .ForMember(s => s.Abroad, opt => opt.MapFrom(s => JsonToKV.ConvertObject(s.Authentication)));

            //榜单
            CreateMap<SchoolRank, SchoolRankDetailViewModel>();
        }
    }
}
