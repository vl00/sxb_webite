using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.UserManage.Application.IServices;
using ProductManagement.Infrastructure.Toolibrary;
using Sxb.Api.RequestModel.RequestOption;
using Sxb.Api.Response;
using Sxb.Api.ViewDto;
using Sxb.Api.ViewDto.QuestionVo;

namespace Sxb.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SchoolQuestionController : ControllerBase
    {
        private readonly IQuestionInfoService _questionInfo;
        private readonly IMapper _mapper;
        private readonly IQuestionsAnswersInfoService _questionsAnswersInfoService;
        private readonly IUserService _userService;

        public SchoolQuestionController(IMapper mapper,
            IQuestionInfoService questionInfo,
            IUserService userService,
            IQuestionsAnswersInfoService questionsAnswersInfoService)
        {
            _userService = userService;
            _questionInfo = questionInfo;
            _mapper = mapper;
            _questionsAnswersInfoService = questionsAnswersInfoService;
        }

        /// <summary>
        /// 根据【学校id | 点评内容】获取该学校下的问题列表，return：rows：数据集合，total：总页数
        /// </summary>
        /// <param name="SchoolId">学校分部id</param>
        /// <param name="Content">问题内容</param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize">默认值20条</param>
        /// <returns></returns>
        [HttpGet]
        public PageResult<List<QuestionModel>> GetQuestionAnswerBySchoolIdOrConente(Guid SchoolId, string Content = "", int PageIndex = 1, int PageSize = 20)
        {
            try
            {
                var searchResult = _questionInfo.GetQuestionAnswerBySchoolIdOrConente(SchoolId, Content, PageIndex, PageSize, out int total);
                return new PageResult<List<QuestionModel>>()
                {
                    rows = searchResult == null ? null : _mapper.Map<List<QuestionDto>, List<QuestionModel>>(searchResult),
                    StatusCode  = 200,
                    total = total
                };
            }
            catch (Exception ex)
            {
                return new PageResult<List<QuestionModel>>()
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 根据问题id获取该问题详情，以及该问题下的两条最新回复数据 return：rows：数据集合，total：该学校下总问题数量
        /// </summary>
        /// <param name="QuestionId">问题id</param>
        /// <returns></returns>
        [HttpGet]
        public PageResult<QuestionModel> GetQuestionInfoById(Guid QuestionId)
        {
            try
            {
                var rez = _questionInfo.GetQuestionInfoById(QuestionId);
                int CuurentQuestionTotal = rez.Item2;
                var r = rez.Item1;

                QuestionModel questionModel;
                if (rez.Item1 != null)
                {
                    questionModel = _mapper.Map<QuestionDto, QuestionModel>(r);
                    
                    if (questionModel.AnswerModels.Count() > 0)
                    {
                        questionModel.AnswerModels.ForEach(x => {
                            var user = (_userService.GetUserInfo(x.UserId));
                            x.UserName = user.NickName;
                        });
                    }
                }
                else
                {
                    questionModel = new QuestionModel();
                }

                    return new PageResult<QuestionModel>()
                    {
                        StatusCode = 200,
                        rows = questionModel,
                        total = CuurentQuestionTotal
                    };
            }
            catch (Exception ex)
            {
                return new PageResult<QuestionModel>() {
                    StatusCode = 500,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 根据问题id列表创建点评问题
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        [HttpGet]
        public PageResult<List<QuestionModel>> GetQuestionsByIds(List<Guid> Ids)
        {
            try
            {
                List<Guid> DistinctIds = Ids.Distinct()?.ToList();
                List<QuestionModel> comments = new List<QuestionModel>();
                foreach (var id in DistinctIds)
                {
                    var model = comments.Find(x => x.QuestionId == id);
                    if (model == null)
                    {
                        comments.Add(new QuestionModel() { QuestionId = id });
                    }
                    else
                    {
                        comments.Add(model);
                    }
                }

                return new PageResult<List<QuestionModel>>()
                {
                    StatusCode = 200,
                    rows = comments
                };
            }
            catch (Exception ex)
            {
                return new PageResult<List<QuestionModel>>()
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
            }
        }

        ///// <summary>
        ///// 一堆ID给你返回一个提问或者回答的list
        ///// </summary>
        ///// <param name="requestOptions"></param>
        ///// <returns></returns>
        //public PageResult<QuestionAndAnswer> GetQuestionOrAnswer(List<RequestOption> requestOptions)
        //{
        //    try
        //    {
        //        QuestionAndAnswer questionAndAnswer = new QuestionAndAnswer();
        //        string iSchoolAuth = HttpContext.Request.Cookies["iSchoolAuth"];

        //        if (iSchoolAuth == null)
        //        {
        //            return null;
        //        }

        //        var groups = requestOptions.GroupBy(x => x.dataIdType).ToList();

        //        for (int i = 0; i < groups.Count(); i++)
        //        {
        //            if (groups[i].Key == RequestModel.RequestEnum.dataIdType.Question)
        //            {
        //                var questions = _mapper.Map<List<QuestionDto>,List<QuestionModel>>(_questionInfo.GetQuestionByIds(groups[i].Select(x => x.dataId).ToList(), default(Guid)));

        //                questionAndAnswer.questionModels = questions;
        //            }
        //            else if (groups[i].Key == RequestModel.RequestEnum.dataIdType.Answer)
        //            {
        //                questionAndAnswer.answerModels = _mapper.Map<List<AnswerInfoDto>,List<AnswerModel>>(_questionsAnswersInfoService.GetAnswerInfoDtoByIds(groups[i].Select(x => x.dataId).ToList(), default(Guid)));
        //            }
        //        }


        //        return new PageResult<QuestionAndAnswer>()
        //        {
        //            StatusCode = 500,
        //            rows = questionAndAnswer
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new PageResult<QuestionAndAnswer>()
        //        {
        //            StatusCode = 500,
        //            Message = ex.Message
        //        };
        //    }
        //}

        ///// <summary>
        ///// 当前用户发布的问题或者答案
        ///// </summary>
        ///// <param name="requestOptions"></param>
        ///// <returns></returns>
        //public PageResult<QuestionAndAnswer> GetNewestQuestionOrAnswer(int PageIndex,int PageSize)
        //{
        //    try
        //    {
        //        QuestionAndAnswer questionAndAnswer = new QuestionAndAnswer();
        //        string iSchoolAuth = HttpContext.Request.Cookies["iSchoolAuth"];

        //        if (iSchoolAuth == null)
        //        {
        //            return null;
        //        }

        //        questionAndAnswer.answerModels = _mapper.Map<List<AnswerInfoDto>, List<AnswerModel>>(_questionsAnswersInfoService.GetNewestAnswerInfoDtoByUserId(PageIndex, PageSize, default(Guid)));

        //        var result = _questionInfo.GetNewestQuestion(PageIndex, PageSize, default(Guid));
        //        questionAndAnswer.questionModels = _mapper.Map<List<QuestionDto>, List<QuestionModel>>(result);
      
        //        return new PageResult<QuestionAndAnswer>()
        //        {
        //            StatusCode = 200,
        //            rows = questionAndAnswer
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new PageResult<QuestionAndAnswer>()
        //        {
        //            StatusCode = 500,
        //            Message = ex.Message
        //        };
        //    }
        //}

        ///// <summary>
        ///// 当前用户发布的问题或者答案
        ///// </summary>
        ///// <param name="requestOptions"></param>
        ///// <returns></returns>
        //public PageResult<List<AnswerModel>> GetCurrentUserNewestAnswer(int PageIndex, int PageSize)
        //{
        //    try
        //    {
        //        QuestionAndAnswer questionAndAnswer = new QuestionAndAnswer();
        //        string iSchoolAuth = HttpContext.Request.Cookies["iSchoolAuth"];

        //        if (iSchoolAuth == null)
        //        {
        //            return null;
        //        }

        //        var answerModels = _mapper.Map<List<AnswerInfoDto>, List<AnswerModel>>(_questionsAnswersInfoService.GetCurrentUserNewestAnswer(PageIndex, PageSize, default(Guid)));

        //        return new PageResult<List<AnswerModel>>()
        //        {
        //            StatusCode = 500,
        //            rows = answerModels
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new PageResult<List<AnswerModel>>()
        //        {
        //            StatusCode = 500,
        //            Message = ex.Message
        //        };
        //    }
        //}

    }
}
