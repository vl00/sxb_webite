using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PMS.OperationPlateform.Domain.Entitys;

namespace PMS.OperationPlateform.Domain.IRespositories
{
    public interface IHuaneAsksRepository
    {
        Task<List<HuaneAsksQuestion>> GetQuestionList(int pageIndex = 1, int pageSize = 10);
        Task<int> GetQuestionCount();
        Task<HuaneAsksQuestion> GetQuestion(int id);
        Task<List<HuaneAsksAnswer>> GetAnswerList(int qid, int pageIndex = 1, int pageSize = 20);
        Task<List<HuaneAsksQuestion>> GetRecommendQuestionList(int qid, int pageIndex = 1, int pageSize = 12);
    }
}
