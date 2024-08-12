using AutoMapper;
using PMS.School.Application.IServices;
using PMS.School.Application.ModelDto;
using PMS.School.Application.ModelDto.Query;
using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities;
using PMS.School.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.School.Application.Services
{
    public class SchCorrectService : ISchCorrectService
    {
        ISchCorrectRepository schCorrectRepository;
        IMapper mapper;

        public SchCorrectService(ISchCorrectRepository schCorrectRepository, IMapper mapper)
        {
            this.schCorrectRepository = schCorrectRepository;
            this.mapper = mapper;
        }

        public SchSourceInfoDto GetSchSourceInfo(Guid eid)
        {
            return schCorrectRepository.GetSchSourceInfo(eid);
        }

        public bool Insert(ExtraSchCorrect0Dto dto)
        {
            var correct = mapper.Map(dto, new ExtraSchCorrect0());

            if (correct.Id == Guid.Empty) correct.Id = Guid.NewGuid();
            correct.Status = (byte)SchCorrectStatus.UnHandled;

            return schCorrectRepository.Insert(correct);
        }
    }
}
