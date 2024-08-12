using AutoMapper;
using PMS.OperationPlateform.Domain.DTOs;
using PMS.OperationPlateform.Domain.Entitys;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace PMS.OperationPlateform.Domain
{
    public class ResultMapperConfiguration : Profile
    {
        public ResultMapperConfiguration()
        {
            _ = CreateMap<article, ArticleDto>()
                .ForMember(d => d.Id, (opt) => opt.MapFrom(s =>s.id))
                .ForMember(d=>d.No,(opt)=> opt.MapFrom(s=> UrlShortIdUtil.Long2Base32(s.No).ToLower()))
                .ForMember(d => d.Layout, (opt) => opt.MapFrom(s => s.layout))
                .ForMember(d => d.Type, (opt) => opt.MapFrom(s => string.IsNullOrEmpty(s.type) ? 1 : int.Parse(s.type)))
                .ForMember(d => d.Title, (opt) => opt.MapFrom(s => s.title))
                .ForMember(d => d.ViewCount, (opt) => opt.MapFrom(s =>s.VirualViewCount))
                .ForMember(d => d.CreateTime, (opt) => opt.MapFrom(s => s.createTime))
                .ForMember(d => d.Time, (opt) => opt.MapFrom(s => s.time.GetValueOrDefault().ConciseTime("yyyy年MM月dd日")))
                .ForMember(d => d._Time, (opt) => opt.MapFrom(s => s.time))
                .ForMember(d => d.Html, (opt) => opt.MapFrom(s => s.html))
                .ForMember(d => d.Covers, (opt) => opt.MapFrom(s =>s.Covers.Select(c => c.ToString())));

    }
    }
}
