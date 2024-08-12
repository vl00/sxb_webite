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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace PMS.SignalR.Clients.PaidQAClient.Models
{
    public class ResultMapperConfiguration : Profile
    {
        public ResultMapperConfiguration()
        {
            //_ = CreateMap<PMS.PaidQA.Domain.Entities.Order, OrderInfoResult>();
            _ = CreateMap<PMS.PaidQA.Domain.Entities.Message, MessageResult>();
            _ = CreateMap<UserInfoDto, UserInfoResult>();
            _ = CreateMap<Evaluate, EvaluateResult>();




        }
    }
}
