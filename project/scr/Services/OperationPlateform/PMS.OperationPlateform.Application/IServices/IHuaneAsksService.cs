using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PMS.OperationPlateform.Application.Dtos;

namespace PMS.OperationPlateform.Application.IServices
{
    public interface IHuaneAsksService
    {
        Task<(List<HuaneAskQuestionListDto>, int)> GetQuestionLsit(int pageIndex = 1, int pageSize = 10);
        Task<HuaneAskQuestionDetailDto> GetQuestionDatail(int id);
    }
}
