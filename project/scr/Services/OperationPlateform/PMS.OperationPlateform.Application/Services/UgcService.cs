using PMS.OperationPlateform.Application.Dtos;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Services
{
    public class UgcService : IUgcService
    {
        private readonly IUgcLogRepository _ugcLogRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UgcService(IUgcLogRepository ugcLogRepository, OperationCommandDBContext unitOfWork)
        {
            _ugcLogRepository = ugcLogRepository;
            _unitOfWork = unitOfWork;
        }

        public AppServiceResultDto Feedback(Guid id)
        {
            var ugcLog = _ugcLogRepository.TakeFirst(" Id = @id ", new { id });
            if (ugcLog == null)
            {
                return AppServiceResultDto.Failure();
            }
            try
            {
                _unitOfWork.BeginTransaction();

                _ugcLogRepository.Update(ugcLog);
                _ugcLogRepository.UpdateUserAreaByExtId(ugcLog.ExtId, ugcLog.UserId, (Domain.Enums.AreaType)ugcLog.AreaType);

                _unitOfWork.Commit();
                return AppServiceResultDto.Success();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                return AppServiceResultDto.Failure();
            }
        }

    }
}
