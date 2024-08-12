using System;
using System.Linq;
using System.Collections.Generic;
using PMS.OperationPlateform.Application.Dtos;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.IRespositories;
using System.Threading.Tasks;
using ProductManagement.Framework.Foundation;

namespace PMS.OperationPlateform.Application.Services
{
    public class HuaneAsksService: IHuaneAsksService
    {
        private readonly IHuaneAsksRepository _huaneAsksRepository;
        public HuaneAsksService(IHuaneAsksRepository huaneAsksRepository)
        {
            _huaneAsksRepository = huaneAsksRepository;
        }

        public async Task<(List<HuaneAskQuestionListDto>,int)> GetQuestionLsit(int pageIndex = 1, int pageSize = 10)
        {
            var list = await _huaneAsksRepository.GetQuestionList(pageIndex, pageSize);
            var count = await _huaneAsksRepository.GetQuestionCount();
            return (list.Select(q => new HuaneAskQuestionListDto
            {
                Id = q.Id,
                Question = q.Question,
                Answer = q.Answer.Substring(0, q.Answer.Length < 300 ? q.Answer.Length : 300),
                Click = q.ClickTimes,
                CreateTime = q.CreateTime.ConciseTime()
            }).ToList(), count);
        }


        public async Task<HuaneAskQuestionDetailDto> GetQuestionDatail(int id)
        {
            var result = await _huaneAsksRepository.GetQuestion(id);

            if(result==null)
            {
                return null;
            }

            var answerList = await _huaneAsksRepository.GetAnswerList(id);
            var recommentList = await _huaneAsksRepository.GetRecommendQuestionList(id);

            return new HuaneAskQuestionDetailDto
            {
                Id = result.Id,
                Question = result.Question,
                Click = result.ClickTimes,
                CreateTime = result.CreateTime.ConciseTime(),
                Answers = answerList.Select(q => new HuaneAskAnswerDto
                {
                    Id = q.Id,
                    Content = q.Content,
                    FloorNumber = q.FloorNumber,
                    Likes = q.Likes,
                    Dislikes = q.Dislikes,
                    CreateTime = q.CreateTime.ConciseTime()
                }).ToList(),
                RecommentQuestions = recommentList.Select(q => new HuaneAskQuestionListDto
                {
                    Id = q.Id,
                    Question = q.Question,
                    Answer = q.Answer.Substring(0, q.Answer.Length < 300 ? q.Answer.Length : 300),
                    Click = q.ClickTimes,
                    CreateTime = q.CreateTime.ToString("yyyy-MM-dd")
                }).ToList()
            };
        }
    }
}
