using AutoMapper;
using PMS.School.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Framework.Foundation;
using PMS.CommentsManage.Domain.Entities;
using PMS.School.Application.ModelDto;
using PMS.CommentsManage.Application.ModelDto;
using Sxb.Inside.Models;
using Microsoft.Extensions.Options;
using Sxb.Inside.App_Start;
using Microsoft.Extensions.Logging.Abstractions;
using PMS.CommentsManage.Application.ModelDto.Reply;
using PMS.UserManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.Total;
using PMS.School.Infrastructure.Common;
using PMS.School.Domain.Dtos;

namespace CommentApp.App_Start
{
    public class VoMapperConfiguration : Profile
    {
        private readonly string _queryImage;
        public VoMapperConfiguration(string query) 
        {
            _queryImage = query;

        }
    }
}
