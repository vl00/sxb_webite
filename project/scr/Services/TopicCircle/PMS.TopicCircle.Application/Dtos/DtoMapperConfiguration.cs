using AutoMapper;
using PMS.Search.Domain.Entities;
using PMS.TopicCircle.Domain.Dtos;
using PMS.TopicCircle.Domain.Entities;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.TopicCircle.Application.Dtos
{
    public class DtoMapperConfiguration : Profile
    {
        public DtoMapperConfiguration()
        {
            CreateMap<Topic, TopicDto>()
                .ForMember(t => t.IsOpen, opt => opt.MapFrom(s => s.OpenUserId == null))
                .ForMember(t => t.Time, opt => opt.MapFrom(s =>s.Time.ConciseTime("yyyy-MM-dd HH:mm")))
                .ForMember(t => t.LastReplyTime, opt => opt.MapFrom(s =>s.LastReplyTime.ConciseTime("yyyy-MM-dd HH:mm")))
                .ForMember(t => t.LastEditTime, opt => opt.MapFrom(s => s.LastEditTime.ConciseTime("yyyy-MM-dd HH:mm")))
                ;

            CreateMap<SimpleTopicDto, TopicDto>()
                .ForMember(t => t.IsOpen, opt => opt.MapFrom(s => s.OpenUserId == null))
                .ForMember(t => t.Time, opt => opt.MapFrom(s => s.Time.ConciseTime("yyyy-MM-dd HH:mm")))
                .ForMember(t => t.LastReplyTime, opt => opt.MapFrom(s => s.LastReplyTime.ConciseTime("yyyy-MM-dd HH:mm")))
                .ForMember(t => t.LastEditTime, opt => opt.MapFrom(s => s.LastEditTime.ConciseTime("yyyy-MM-dd HH:mm")))
                ;

            CreateMap<Topic, SearchTopic>()
                .ForMember(t => t.Tags, opt => opt.MapFrom(s => new List<SearchTopic.SearchTag>()))
                ;



            CreateMap<TopicReply, TopicReplyDto>()
                .ForMember(t => t.Time, opt => opt.MapFrom(s => s.CreateTime.ConciseTime("yyyy-MM-dd HH:mm")))
                ;

            CreateMap<TopicTag, SimpleTagDto>()
                .ForMember(t => t.Id, opt => opt.MapFrom(s => s.TagId))
                ;

            CreateMap<SimpleCircleDto, SearchCircleDto>()
                ;
        }
    }
}
