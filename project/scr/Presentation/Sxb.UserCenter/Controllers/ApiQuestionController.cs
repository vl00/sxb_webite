using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.Search.Application.IServices;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Entities;
using ProductManagement.API.Aliyun;
using Sxb.UserCenter.Authentication.Attributes;
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
    public class ApiQuestionController : Base
    {

        private readonly IMessageService _inviteStatus;
        private readonly IQuestionInfoService _questionInfo;

        private readonly ISchoolInfoService _schoolInfoService;
        private ICollectionService _collectionService;
        private readonly ImageSetting _setting;
        private readonly IUserService _userService;
        private readonly ISearchService _dataSearch;
        private readonly IText _text;

        public ApiQuestionController(
            IMessageService inviteStatus,
            IQuestionInfoService questionInfo,
            ICollectionService collectionService,
            IOptions<ImageSetting> set,
            IUserService userService,
            ISchoolInfoService schoolInfoService,
            IText text,
            ISearchService dataSearch)
        {

            _inviteStatus = inviteStatus;
            _questionInfo = questionInfo;
            _setting = set.Value;
            _collectionService = collectionService;
            _userService = userService;
            _schoolInfoService = schoolInfoService;
            _dataSearch = dataSearch;

            _text = text;
        }

        /// <summary>
        /// 个人动态 提问列表
        /// </summary>
        /// <param name="search"></param>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ResponseResult HomeQuestion(string search = "", Guid userId = default(Guid), int page = 1, int size = 10) 
        {
            List<QuestionInfoViewModel> questions = new List<QuestionInfoViewModel>();

            Guid loginUserId = userID;
            bool IsSelf = userId == default || userId == loginUserId;
            Guid searchUserId = IsSelf ? loginUserId : userId;

            List<QuestionDto> questionDtos = new List<QuestionDto>();
            if (string.IsNullOrEmpty(search))
            {
                questionDtos = _questionInfo.PageQuestionByUserId(searchUserId, loginUserId, IsSelf, page, size);
                if (!questionDtos.Any())
                {
                    return ResponseResult.Success(questions);
                }
            }
            else 
            {
                //调用ES 返回问题id
                List<Guid> Ids = new List<Guid>();
                if (!Ids.Any()) 
                {
                    return ResponseResult.Success(questions);
                }
                questionDtos = _questionInfo.PageQuestionByQuestionIds(loginUserId, Ids, IsSelf);
            }
            var Users = _userService.ListUserInfo(questionDtos.Select(x => x.UserId).ToList());
            var SchoolExts = _schoolInfoService.GetSchoolStatuse(questionDtos.Select(x => x.SchoolSectionId).ToList());
            //检测提问是否关注
            var checkCollection = _collectionService.GetCollection(questionDtos.Select(x => x.Id).ToList(), searchUserId);

            if (IsSelf)
            {
                questionDtos.ForEach(q => q.IsAnony = false);
            }

            //提问领域实体转视图实体
            questions = QuestionDtoToVoHelper.QuestionDtoToVo(_setting.QueryImager, questionDtos, UserHelper.UserDtoToVo(Users), SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolQuestionCard(SchoolExts), checkCollection);

            return ResponseResult.Success(questions);
        }

        /// <summary>
        /// 提问
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [BindMobile]
        public ResponseResult CommitQuestion([FromBody]QuestionWriterViewModel questionInfo) 
        {
            Guid UserId = Guid.Empty;

            if (questionInfo == null) 
            {
                return ResponseResult.Success();
            }

            var contentCheck = _text.GarbageCheck(new ProductManagement.API.Aliyun.Model.GarbageCheckRequest()
            {
                scenes = new[] { "antispam" },
                tasks = new List<ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task>() {
                      new ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task(){
                       content=questionInfo.Content,
                         dataId = Guid.NewGuid().ToString()
                      }
                      }
            }).Result;


            if (contentCheck.data.FirstOrDefault().results.FirstOrDefault().suggestion == "block")
            {
                return ResponseResult.Failed("您发布的内容包含敏感词，<br>请重新编辑后再发布。");
            }

            var school = _schoolInfoService.GetSchoolSectionByIds(new List<Guid>() { questionInfo.Eid }).FirstOrDefault();

            bool res = _questionInfo.AddQuestion(new PMS.CommentsManage.Domain.Entities.QuestionInfo()
            {
                Id = Guid.NewGuid(),
                UserId = UserId,
                IsAnony = questionInfo.IsAnony,
                Content = questionInfo.Content,
                SchoolSectionId = school.SchoolSectionId,
                SchoolId = school.SchoolId
            });


            //所有邀请我提问这个学校的消息, true,已处理
            _inviteStatus.UpdateMessageHandled(true, MessageType.InviteQuestion, MessageDataType.School, UserId, default, new List<Guid>() { questionInfo.Eid });

            return ResponseResult.Success(new { success = res });
        }


    }
}
