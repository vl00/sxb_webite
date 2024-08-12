using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Math.EC.Rfc7748;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.Search.Application.IServices;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Entities;
using ProductManagement.API.Aliyun;
using ProductManagement.API.Http.Interface;
using Sxb.UserCenter.Models.CommonViewModel;
using Sxb.UserCenter.Models.QuestionViewModel;
using Sxb.UserCenter.Models.SchoolViewModel;
using Sxb.UserCenter.Response;
using Sxb.UserCenter.Utils;
using Sxb.UserCenter.Utils.DtoToViewModel;
using static PMS.UserManage.Domain.Common.EnumSet;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.UserCenter.Controllers
{
    public class ApiAnswerController : Base
    {
        private readonly IText _text;

        private readonly IMessageService _inviteStatus;
        private IQuestionsAnswersInfoService _questionsAnswers;


        private readonly ISchoolInfoService _schoolInfoService;
        private ICollectionService _collectionService;
        private readonly ImageSetting _setting;
        private readonly IUserService _userService;
        private readonly ISearchService _dataSearch;

        private readonly IQuestionInfoService _questionInfo;

        public ApiAnswerController(
            IMessageService inviteStatus,
            IQuestionsAnswersInfoService questionsAnswers,
            ICollectionService collectionService,
            IOptions<ImageSetting> set,
            IUserService userService,
            ISchoolInfoService schoolInfoService,
            ISearchService dataSearch,
            IQuestionInfoService questionInfo,
            IText text)
        {


            _inviteStatus = inviteStatus;
            _text = text;
            _questionsAnswers = questionsAnswers;
            _questionInfo = questionInfo;
            _setting = set.Value;
            _collectionService = collectionService;
            _userService = userService;
            _schoolInfoService = schoolInfoService;
            _dataSearch = dataSearch;
        }

        /// <summary>
        /// 个人列表页 回答列表
        /// </summary>
        /// <param name="search"></param>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ResponseResult HomeAnswer(string search = "", Guid userId = default(Guid), int page = 1, int size = 10)
        {
            List<QuestionAnswerViewModel> answerVos = new List<QuestionAnswerViewModel>();

            Guid loginUserId = userID;
            bool IsSelf = userId == default || userId == loginUserId;
            Guid searchUserId = IsSelf ? loginUserId : userId;

            List<AnswerInfoDto> answers = new List<AnswerInfoDto>();
            if (String.IsNullOrEmpty(search))
            {
                answers = _questionsAnswers.PageAnswerByUserId(searchUserId, loginUserId, IsSelf, page, size);
                if (!answers.Any())
                {
                    return ResponseResult.Success(answerVos);
                }
            }
            else
            {
                //ES 查询
                List<Guid> answerIds = new List<Guid>();
                if (!answerIds.Any())
                {
                    return ResponseResult.Success(answerVos);
                }
                answers = _questionsAnswers.PageAnswerByUserId(searchUserId, loginUserId, IsSelf, page, size, answerIds);
            }

            List<Guid> userIds = answers.Select(x => x.UserId).ToList();
            userIds.AddRange(answers.Select(x => x.questionDto.UserId).ToList());
            var Users = _userService.ListUserInfo(userIds);


            var SchoolExts = _schoolInfoService.GetSchoolStatuse(answers.Select(x => x.questionDto.SchoolSectionId).ToList());
            //检测提问是否关注
            var checkCollection = _collectionService.GetCollection(answers.Select(x => x.questionDto.Id).ToList(), searchUserId);

            var questions = QuestionDtoToVoHelper.QuestionDtoToVo(_setting.QueryImager, answers.Select(x => x.questionDto).ToList(), UserHelper.UserDtoToVo(Users), SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolQuestionCard(SchoolExts), checkCollection);

            if (IsSelf)
            {
                answers.ForEach(q => q.IsAnony = false);
            }

            answerVos = AnswerInfoDtoToVoHelper.AnswerInfoDtoToVo(answers, UserHelper.UserDtoToVo(Users), questions);

            return ResponseResult.Success(answerVos);
        }

        /// <summary>
        /// 提交回答
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult CommitAnswer([FromBody] AnswerWriterViewModel answer)
        {
            Guid UserId;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                UserId = user.UserId;
                var userInfo = _userService.GetUserInfo(UserId);
                if (string.IsNullOrWhiteSpace(userInfo.Mobile))
                {
                    return ResponseResult.Failed("该用户未绑定手机号 , 无法完成操作");
                }
            }
            else
            {
                return ResponseResult.Failed("未登录");
            }

            var contentCheck = _text.GarbageCheck(new ProductManagement.API.Aliyun.Model.GarbageCheckRequest()
            {
                scenes = new[] { "antispam" },
                tasks = new List<ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task>() {
                      new ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task(){
                       content=answer.Content,
                         dataId = Guid.NewGuid().ToString()
                      }
                      }
            }).Result;

            if (contentCheck == null || contentCheck.data == null)
            {
                return ResponseResult.Failed("无数据");
            }

            if (contentCheck.data.FirstOrDefault().results.FirstOrDefault()?.suggestion == "block")
            {
                return ResponseResult.Failed("您发布的内容包含敏感词，<br>请重新编辑后再发布。");
            }

            var question = _questionInfo.GetQuestionById(answer.QuestionId, UserId);
            if (question == null || question.Id == Guid.Empty)
                return ResponseResult.Failed("您回答的问题不存在。");

            var addanswer = new PMS.CommentsManage.Domain.Entities.QuestionsAnswersInfo()
            {
                Id = Guid.NewGuid(),
                UserId = UserId,
                Content = answer.Content,
                QuestionInfoId = answer.QuestionId,
                IsAnony = answer.IsAnony,
                IsAttend = answer.IsAttend
            };
            int res = _questionsAnswers.Insert(addanswer);

            if (res > 0)
            {
                //所有邀请我回答这个问题的消息, 全部设为已读,已处理
                _inviteStatus.UpdateMessageHandled(true, MessageType.InviteAnswer, MessageDataType.Question, UserId, default, new List<Guid>() { answer.QuestionId });
                string iSchoolAuth = HttpContext.Request.Cookies["iSchoolAuth"];

                if (question.UserId != UserId)
                {
                    bool states = _inviteStatus.AddMessage(new Message()
                    {
                        title = question.QuestionContent,
                        Content = answer.Content,
                        DataID = addanswer.Id,
                        DataType = (byte)MessageDataType.Answer,
                        EID = question.SchoolSectionId,
                        Type = (byte)MessageType.Reply,
                        userID = addanswer.UserId,
                        senderID = userID
                    });
                }
                return ResponseResult.Success(new { success = true });
            }
            else
            {
                return ResponseResult.Success(new { success = false });
            }
        }


    }
}
