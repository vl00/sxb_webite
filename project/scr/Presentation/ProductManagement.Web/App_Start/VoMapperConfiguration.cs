using AutoMapper;
using ProductManagement.Web.Areas.PartTimeJob.Models;
using ProductManagement.Web.Areas.PartTimeJob.Models.ViewEntity;
using ProductManagement.Web.ModelVo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ProductManagement.Framework.Foundation;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.ProcViewEntities;

namespace ProductManagement.Web.App_Start
{
    public class VoMapperConfiguration : Profile
    {
        public VoMapperConfiguration() : base()
        {
            //ForMember(source=>source) 代表直接配置该实体，如果需要映射至该实体中某特定字段，需要直接指定字段
            CreateMap<PartTimeJobAdminVo, PartTimeJobAdmin>();
            CreateMap<PartTimeJobAdmin, PartTimeJobAdminVo>()
                .ForMember(source => source.Role, opt => opt.MapFrom(s => s.Role.Description()))
                .ForMember(source => source.Password, opt => opt.Ignore())
              // ForMember(source => source.Phone, opt => opt.MapFrom(s => Regex.Replace(s.Phone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2")))
                .ForMember(source => source.RegesitTime, opt => opt.MapFrom(x => x.RegesitTime.ToString("yyyy-MM-dd")));


            //（1：未阅，2：已阅，3：已加精，4：已屏蔽）

            //CreateMap<QuestionExamineVo, QuestionExamine>();
            //    CreateMap<QuestionExamine, QuestionExamineVo>()
            //        .ForMember(source => source.State, opt => opt.MapFrom(s => s.State == 1 ? "未阅" : s.State == 2 ? "已阅" : s.State == 3 ? "已加精" : "已屏蔽") );

            //    CreateMap<QuestionInfoVo, QuestionInfo>();
            //    CreateMap<QuestionInfo, QuestionInfoVo>();

            //    CreateMap<QuestionsAnswerExamineVo, QuestionsAnswerExamine>();
            //    CreateMap<QuestionsAnswerExamine, QuestionsAnswerExamineVo>()
            //         .ForMember(source => source.State, opt => opt.MapFrom(s => s.State == 1 ? "未阅" : s.State == 2 ? "已阅" : s.State == 3 ? "已加精" : "已屏蔽"));

            //    CreateMap<QuestionsAnswersInfoVo, QuestionsAnswersInfo>();
            //    CreateMap<QuestionsAnswersInfo, QuestionsAnswersInfoVo>();

            CreateMap<ProcGetAdminByRoleTypeEntity, ProcGetAdminByRoleTypeEntityVo>()
                .ForMember(source => source.SettlementType, opt => opt.MapFrom(s => s.SettlementType == 1 ? "微信现结" : "另结"));
            //.ForMember(source => source.Phone, opt => opt.MapFrom(s => Regex.Replace(s.Phone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2")));

            CreateMap<SettlementView, SettlementViewVo>()
                .ForMember(source => source.SettlementStatus, opt => opt.MapFrom(s => s.SettlementStatus.Description()))
                .ForMember(source => source.SettlementType, opt => opt.MapFrom(s => s.SettlementType.Description()))
                .ForMember(s => s.BeginTime, opt => opt.MapFrom(s => s.BeginTime.ToString("yyyy-MM-dd")))
                .ForMember(s => s.EndTime, opt => opt.MapFrom(s => s.EndTime.ToString("yyyy-MM-dd")));

        }
    }
}
