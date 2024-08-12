using AutoMapper;
using iSchool;
using iSchool.Internal.API.OperationModule;
using iSchool.Internal.API.RankModule;
using Microsoft.Extensions.Configuration;
using PMS.School.Application.IServices;
using PMS.School.Application.ModelDto;
using PMS.School.Application.ModelDto.Query;
using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities;
using PMS.School.Domain.IRepositories;
using PMS.School.Infrastructure.Common;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PMS.School.Application.Services
{
    public class ToFenghuangtongService : IToFenghuangtongService
    {
        readonly IEasyRedisClient _easyRedisClient;
        readonly IToFenghuangtongRepository _toFenghuangtongRepository;

        public ToFenghuangtongService(IEasyRedisClient _easyRedisClient, IToFenghuangtongRepository toFenghuangtongRepository)
        {
            this._easyRedisClient = _easyRedisClient;
            this._toFenghuangtongRepository = toFenghuangtongRepository;
        }        
        

        public async Task<SchExtDto1> GetSchoolExtLatLongInfo(Guid eid = default, long eno = default)
        {
            if (eid == default)
            {
                eid = _toFenghuangtongRepository.GetSchoolEid(eno);
            }
            if (eid == default)
            {
                return null;
            }

            var dto = await _toFenghuangtongRepository.GetSchExtDto1(eid);

            return dto;
        }
    }
}
