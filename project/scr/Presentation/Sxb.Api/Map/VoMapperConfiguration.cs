using AutoMapper;
using PMS.CommentsManage.Application.ModelDto;
using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Domain.Entities;
using Sxb.Api.ViewDto;
using Sxb.Api.ViewDto.CommentVo;
using Sxb.Api.ViewDto.QuestionVo;
using Sxb.Api.ViewDto.UserInfoVo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Api.Map
{
    public class VoMapperConfiguration : Profile
    {
        private readonly string _queryImage;
        public VoMapperConfiguration(string query)
        {
            _queryImage = query;

            CreateMap<QuestionDto, QuestionModel>()
                .ForMember(x => x.QuestionId, opt => opt.MapFrom(s => s.Id))
                .ForMember(x => x.SchoolSectionId, opt => opt.MapFrom(s => s.SchoolSectionId))
                .ForMember(x => x.CurrentQuestionAnswerTotal, opt => opt.MapFrom(s => s.AnswerCount))
                .ForMember(x => x.QuestionContent, opt => opt.MapFrom(s => s.QuestionContent))
                //.ForMember(s => s.CreateTime, opt => opt.MapFrom(s => s.QuestionCreateTime.ToString("yyyy-MM-dd")))
                .ForMember(s => s.AnswerModels, opt => opt.MapFrom(s => s.answer));

            CreateMap<AnswerInfoDto, AnswerModel>()
                .ForMember(x => x.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(x => x.AnswerContent, opt => opt.MapFrom(s => s.AnswerContent))
                .ForMember(x => x.UserId, opt => opt.MapFrom(s => s.UserId));

                //.ForMember(x => x.UserName, opt => opt.MapFrom(s => s..NickName));

                   
        CreateMap<SchoolCommentDto, CommentModel>();
                //.ForMember(x => x.AddTime,opt=>opt.MapFrom(s=>s.CreateTime.ToString("yyyy-MM-dd")));

            CreateMap<UserInfo, UserInfoModel>();
            //.ForMember(x => x.HeadImager, opt => opt.MapFrom(s => _queryImage + s.HeadImager))
            //.ForMember(x=>x.Role,opt=>opt.MapFrom(s=>(int)s.Role));

            CreateMap<UserInfoDto, UserInfoModel>().
                ForMember(x=>x.HeadImager , opt => opt.MapFrom(x=>x.HeadImgUrl))
                .ForMember(x=>x.Id , opt=>opt.MapFrom(x=>x.Id))
                .ForMember(x=>x.NickName,opt=>opt.MapFrom(x=>x.NickName))
                .ForMember(x=>x.Role,opt=>opt.MapFrom(x=>x.VerifyTypes));
        }
    }
}
