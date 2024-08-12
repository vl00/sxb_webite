using AutoMapper;
using PMS.CommentsManage.Application.IServices.IProcViewService;
using PMS.CommentsManage.Domain.Entities.ProcViewEntities;
using PMS.CommentsManage.Domain.IRepositories.IProcViewRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace PMS.CommentsManage.Application.Services.ProcView
{
    public class ProcFindAllJobEntityService : IProcFindAllJobEntityService
    {

        private readonly IFindAllJobEntityServiceRepository _repo;

        //private readonly IMapper _mapper;

        public ProcFindAllJobEntityService(IFindAllJobEntityServiceRepository repo)
        {
            _repo = repo;
            //_mapper = mapper;
        }

        public List<ProcFindAllJobEntityList> FindAllJobEntityByLeaderId(Guid Id, int PageIndex, int PageSize, string Phone, DateTime beginTime, DateTime endTime, out ProcFindAllJobEntityTotal procFindAllJobEntityTotal)
        {
            return  _repo.FindAllJobEntityByLeaderId(Id, PageIndex, PageSize, Phone,beginTime,endTime, out procFindAllJobEntityTotal);
        }

    }
}
