using AutoMapper;
using Sxb.Web.Areas.Article.Models.SchoolActivity;
using Sxb.Web.Areas.PaidQA.Models.Talent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Sxb.Web.Areas.Article.Models
{
    public class ResultMapperConfiguration : Profile
    {
        public ResultMapperConfiguration()
        {
            _ = CreateMap<PMS.OperationPlateform.Application.Dtos.SchoolActivityDetailDto, SchoolActivityResult>();
            _ = CreateMap<PMS.OperationPlateform.Application.Dtos.SchoolActivityDetailDto.Image, ImageResult>();
            _ = CreateMap<PMS.OperationPlateform.Domain.Entitys.SchoolActivityProcess, SchoolActivityProcessResult>();
         
        }
    }
}
