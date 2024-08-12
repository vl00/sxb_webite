using AutoMapper;
using PMS.CommentsManage.Application.ModelDto;
using PMS.TopicCircle.Application.Dtos;
using PMS.UserManage.Application.ModelDto.Message;
using PMS.UserManage.Domain.Entities;
using ProductManagement.Framework.Foundation;
using Sxb.UserCenter.Models.MessageViewModel;
using Sxb.UserCenter.Models.TopicCircle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sxb.UserCenter.Common
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration()
        {
            CreateMap<SchoolInfoDto, SchoolModel>()
                .ForMember(s => s.ExtId, opt => opt.MapFrom(s => s.SchoolSectionId))
                .ForMember(s => s.Sid, opt => opt.MapFrom(s => s.SchoolId))

                .ForMember(s => s.QuestionCount, opt => opt.MapFrom(s => s.SectionQuestionTotal))
                .ForMember(s => s.CommentCount, opt => opt.MapFrom(s => s.CommentTotal))
                .ForMember(s => s.Type, opt => opt.MapFrom(s => (int)s.SchoolType))
                .ForMember(s => s.LodgingType, opt => opt.MapFrom(s => (int)s.LodgingType))
                .ForMember(s => s.LodgingReason, opt => opt.MapFrom(s => s.LodgingType.Description()))
                ;

            CreateMap<SysMessageDetail, MessageDialogueViewModel>();


            CreateMap<TopicDto, CollectionTopicViewModel>()
                .ForMember(s => s.Image, opt => opt.MapFrom(s => s.Images.FirstOrDefault().Url))
                ;
        }
    }
}
