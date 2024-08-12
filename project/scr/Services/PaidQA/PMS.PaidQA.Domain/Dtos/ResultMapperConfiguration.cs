using AutoMapper;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.EntityExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace PMS.PaidQA.Domain.Dtos
{
    public class ResultMapperConfiguration : Profile
    {
        public ResultMapperConfiguration()
        {
            _ = CreateMap<PMS.PaidQA.Domain.Entities.Message, MessageDto>();


        }
    }
}
