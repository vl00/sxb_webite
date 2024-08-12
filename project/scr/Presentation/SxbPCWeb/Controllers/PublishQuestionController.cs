using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.School.Application.IServices;
using PMS.UserManage.Domain.Common;
using Sxb.PCWeb.RequestModel.Question;
using Sxb.PCWeb.Response;
using Sxb.PCWeb.Utils.CommentHelper;

namespace Sxb.PCWeb.Controllers
{
    [Authorize]
    public class PublishQuestionController : Controller
    {
        private readonly ISchoolService _schoolService;
        private readonly ISchoolInfoService _schoolInfoService;
        private readonly IQuestionInfoService _questionInfo;
        private readonly ISchoolImageService _imageService;
        private readonly IMapper _mapper;

        public PublishQuestionController(ISchoolService schoolService,
            ISchoolInfoService schoolInfoService,
            IQuestionInfoService questionInfo,
            ISchoolImageService imageService,
            IMapper mapper) 
        {
            _schoolService = schoolService;
            _schoolInfoService = schoolInfoService;
            _questionInfo = questionInfo;
            _imageService = imageService;
            _mapper = mapper;
        }

        /// <summary>
        /// 写提问页
        /// </summary>
        /// <param name="ExtId"></param>
        /// <param name="Sid"></param>
        /// <returns></returns>
        [Description("写提问页")]
        public async Task<IActionResult> Index(Guid ExtId, Guid Sid)
        {

            Guid UserId = Guid.Empty;
            //检测当前用户身份 （true：校方用户 |false：普通用户）
            ViewBag.IsSchoolRole = false;
            if (User.Identity.IsAuthenticated)
            {
                var UserInfo = User.Identity.GetUserInfo();
                //检测当前用户身份 （true：校方用户 |false：普通用户）
                ViewBag.IsSchoolRole = UserInfo.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);

                UserId = UserInfo.UserId;
            }

            //获取当前学校卡片信息
            var CurrentSchool = await _schoolInfoService.QuerySchoolInfo(ExtId);
            ViewBag.CurrentSchool = SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolQuestionCard(new List<SchoolInfoDto>() { CurrentSchool });

            //获取该校下其他分部信息
            //ViewBag.AllSchoolBranch = _schoolService.GetAllSchoolBranch(Sid).Where(x => x.Id != ExtId);

            //var adCodes = await _cityService.IntiAdCodes();

            //ViewBag.AllSchoolBranch = allSchoolBranch == null ? new List<SchoolSwitch>() : allSchoolBranch.Select(x => new SchoolSwitch()
            //{
            //    Id = x.Id,
            //    Name = x.Name,
            //    City = adCodes.SelectMany(c => c.CityCodes).FirstOrDefault(p => p.Id == x.City).Name
            //}).ToList();
            //ViewBag.SchoolId = SchoolId;

            //List<AreaDto> history = new List<AreaDto>();

            //if (userId != default(Guid))
            //{
            //    string Key = $"SearchCity:{userId}";
            //    var historyCity = await _easyRedisClient.SortedSetRangeByRankAsync<AreaDto>(Key, 0, 10, StackExchange.Redis.Order.Descending);
            //    history.AddRange(historyCity.ToList());
            //}

            //获取当前用户所在城市
            //var localCityCode = Request.GetLocalCity();

            ////将数据保存在前台
            //ViewBag.LocalCity = adCodes.SelectMany(p => p.CityCodes).FirstOrDefault(p => p.Id == localCityCode);

            //ViewBag.HotCity = JsonConvert.SerializeObject(await _cityService.GetHotCity());

            //ViewBag.History = JsonConvert.SerializeObject(history);

            //ViewBag.SchoolSectionId = SchoolId;
            ////城市编码
            //ViewBag.AllCity = JsonConvert.SerializeObject(adCodes);

            return View();
        }

        /// <summary>
        /// 提交问题
        /// </summary>
        /// <param name="question"></param>
        /// <param name="questionId"></param>
        /// <param name="questionImages"></param>
        /// <returns></returns>
        [Description("提交问题")]
        public ResponseResult CommitQuestion(QuestionWriterViewModel question, Guid questionId, List<string> questionImages)
        {
            try
            {
                var user = User.Identity.GetUserInfo();
                if (question.SchoolId == default(Guid))
                {
                    var SchoolInfo = _schoolService.GetSchoolByIds(question.SchoolSectionId.ToString()).FirstOrDefault();
                    question.SchoolId = SchoolInfo.SchoolId;
                }

                question.Id = questionId;

                List<SchoolImage> imgs = new List<SchoolImage>();
                //图片上传
                if (questionImages.Count() > 0)
                {
                    //标记该点评有上传过图片
                    question.IsHaveImagers = true;

                    //需要上传图片
                    imgs = new List<SchoolImage>();

                    for (int i = 0; i < questionImages.Count(); i++)
                    {
                        SchoolImage commentImg = new SchoolImage();
                        commentImg.DataSourcetId = question.Id;
                        commentImg.ImageType = ImageType.Question;
                        commentImg.ImageUrl = questionImages[i];

                        //图片上传成功，提交数据库
                        imgs.Add(commentImg);

                    }
                }
                question.UserId = user.UserId;
                QuestionInfo Question = _mapper.Map<QuestionWriterViewModel, QuestionInfo>(question);
                //检测提问用户是否拥有身份认证，无则为普通用户
                Question.PostUserRole = user.Role.Length > 0 ? (UserRole)user.Role[0] : UserRole.Member;
                _questionInfo.AddQuestion(Question);

                if (imgs.Count != 0)
                {
                    foreach (var img in imgs)
                    {
                        _imageService.AddSchoolImage(img);
                    }
                }

                return ResponseResult.Success(new {
                    DataId = question.Id,
                    SchoolId = question.SchoolSectionId
                });
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

    }
}