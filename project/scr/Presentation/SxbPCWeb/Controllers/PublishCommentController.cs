using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.Infrastructure.Application.IService;
using PMS.School.Application.IServices;
using PMS.UserManage.Domain.Common;
using ProductManagement.Framework.Cache.Redis;
using Sxb.PCWeb.Models;
using Sxb.PCWeb.Response;
using Sxb.PCWeb.Utils.CommentHelper;
using PMS.CommentsManage.Application.ModelDto;
using Sxb.PCWeb.RequestModel.Comment;
using System.ComponentModel;

namespace Sxb.PCWeb.Controllers
{
    [Authorize]
    public class PublishCommentController : Controller
    {
        ////配置文件内容
        //private readonly ImageSetting _setting;
        //private readonly ISchoolService _schoolService;
        //private readonly ISchoolCommentService _commentService;
        //private readonly ISchoolCommentScoreService _commentScoreService;
        //private readonly ISchoolTagService _schoolTagService;
        //private readonly ITagService _tagService;
        //private readonly ISchoolImageService _imageService;

        //private readonly IEasyRedisClient _easyRedisClient;
        //readonly ICityInfoService _cityService;

        //private readonly IMapper _mapper;
        //readonly ISchoolInfoService _schoolInfoService;

        //public PublishCommentController(IEasyRedisClient easyRedisClient, ICityInfoService cityService, IOptions<ImageSetting> set, ISchoolInfoService schoolInfoService, ISchoolService schoolService, ISchoolCommentService schoolComment, ISchoolCommentScoreService commentScoreService, ISchoolTagService schoolTagService, ITagService tagService, ISchoolImageService imageService, IMapper mapper) 
        //{
        //    _easyRedisClient = easyRedisClient;
        //    _cityService = cityService;

        //    _schoolService = schoolService;
        //    _commentService = schoolComment;
        //    _commentScoreService = commentScoreService;
        //    _schoolTagService = schoolTagService;
        //    _tagService = tagService;
        //    _setting = set.Value;
        //    _imageService = imageService;
        //    _mapper = mapper;
        //    _schoolInfoService = schoolInfoService;
        //}

        ///// <summary>
        ///// 写点评页
        ///// </summary>
        ///// <param name="ExtId"></param>
        ///// <param name="Sid"></param>
        ///// <returns></returns>
        //[Description("写点评页")]
        //public async Task<IActionResult> Index(Guid ExtId, Guid Sid)
        //{
        //    Guid UserId = Guid.Empty;

        //    //检测当前用户身份 （true：校方用户 |false：普通用户）
        //    ViewBag.IsSchoolRole = false;
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        var UserInfo = User.Identity.GetUserInfo();
        //        //检测当前用户身份 （true：校方用户 |false：普通用户）
        //        ViewBag.IsSchoolRole = UserInfo.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);

        //        UserId = UserInfo.UserId;
        //    }

        //    //获取当前学校卡片信息
        //    var CurrentSchool = await _schoolInfoService.QuerySchoolInfo(ExtId);
        //    ViewBag.CurrentSchool = SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolCommentCard(new List<SchoolInfoDto>() { CurrentSchool });
        //    //获取该校下点评标签
        //    //ViewBag.Tag = _schoolTagService.GetSchoolTagBySchoolId(ExtId).Select(x => x.Tag).ToList();
        //    //获取该校下其他分部信息
        //    ViewBag.AllSchoolBranch = _schoolService.GetAllSchoolBranch(Sid).Where(x => x.Id != ExtId);

        //    //List<AreaDto> history = new List<AreaDto>();
        //    //if (userId != default(Guid))
        //    //{
        //    //    string Key = $"SearchCity:{userId}";
        //    //    var historyCity = await _easyRedisClient.SortedSetRangeByRankAsync<AreaDto>(Key, 0, 10, StackExchange.Redis.Order.Descending);
        //    //    history.AddRange(historyCity.ToList());
        //    //}

        //    //ViewBag.HotCity = JsonConvert.SerializeObject(await _cityService.GetHotCity());

        //    //ViewBag.History = JsonConvert.SerializeObject(history);

        //    //var adCodes = await _cityService.IntiAdCodes();

        //    ////获取当前用户所在城市
        //    //var localCityCode = Request.GetLocalCity();

        //    ////将数据保存在前台
        //    //ViewBag.LocalCity = adCodes.SelectMany(p => p.CityCodes).FirstOrDefault(p => p.Id == localCityCode);

        //    ////城市编码
        //    //ViewBag.AllCity = JsonConvert.SerializeObject(adCodes);

        //    return View();
        //}

        ///// <summary>
        ///// 点评提交
        ///// </summary>
        ///// <param name="comment"></param>
        ///// <param name="commentId"></param>
        ///// <param name="imgUrls"></param>
        ///// <param name="Tags"></param>
        ///// <returns></returns>
        //[Description("点评提交")]
        //public ResponseResult CommitComment(CommentWriterViewModel comment, Guid commentId, List<string> imgUrls, List<CommentTag> Tags)
        //{
        //    try
        //    {
        //        Guid userId = Guid.Empty;
        //        UserRole userRole = new UserRole();
        //        if (User.Identity.IsAuthenticated)
        //        {
        //            var user = User.Identity.GetUserInfo();
        //            userId = user.UserId;
        //            userRole = user.Role.Length > 0 ? (UserRole)user.Role[0] : UserRole.Member;
        //        }
        //        //获取前端post过来的点评对象
        //        //CommentWriterViewModel comment = JsonConvert.DeserializeObject<CommentWriterViewModel>(HttpContext.Request.Form["comment"]);

        //        //根据学校id得到该学校详情信息
        //        var extension = _schoolService.GetSchoolExtension(comment.SchoolId);

        //        //首先创建点评对象，得到该点评id，进行拼接点评图片路径
        //        SchoolComment schoolComment = _mapper.Map<CommentWriterViewModel, SchoolComment>(comment);
        //        schoolComment.Id = commentId;
        //        schoolComment.SchoolId = extension.SchoolId;
        //        schoolComment.SchoolSectionId = extension.Id;
        //        schoolComment.CommentUserId = userId;
        //        schoolComment.PostUserRole = userRole;

        //        List<SchoolImage> imgs = new List<SchoolImage>();
        //        //上传图片
        //        if (imgUrls.Count() > 0)
        //        {
        //            //标记该点评有上传过图片
        //            schoolComment.IsHaveImagers = true;

        //            //需要上传图片
        //            imgs = new List<SchoolImage>();

        //            for (int i = 0; i < imgUrls.Count(); i++)
        //            {
        //                SchoolImage commentImg = new SchoolImage
        //                {
        //                    DataSourcetId = schoolComment.Id,
        //                    ImageType = ImageType.Comment,
        //                    ImageUrl = imgUrls[i]
        //                };

        //                //图片上传成功，提交数据库
        //                imgs.Add(commentImg);
        //            }
        //        }

        //        //获取该用户所选，与新添加的标签
        //        //List<CommentTag> Tags = JsonConvert.DeserializeObject<List<CommentTag>>(HttpContext.Request.Form["Tags"]);
        //        _commentService.AddSchoolComment(schoolComment);

        //        SchoolCommentScore commentScore = _mapper.Map<CommentWriterViewModel, SchoolCommentScore>(comment);
        //        commentScore.CommentId = schoolComment.Id;
        //        //已入读，录入该点评本次平均分
        //        if (commentScore.IsAttend)
        //        {
        //            //总分录入平均分
        //            commentScore.AggScore = Convert.ToInt32(commentScore.AggScore) / 5;
        //        }

        //        _commentScoreService.AddSchoolComment(commentScore);

        //        foreach (CommentTag tag in Tags)
        //        {
        //            if (tag.Id == default(Guid))
        //            {
        //                var newTag = _tagService.CheckTagIsExists(tag.Content);
        //                //检测该标签是否存在
        //                if (newTag == null)
        //                {
        //                    //标签不存在
        //                    tag.Id = Guid.NewGuid();
        //                    _tagService.Add(tag);

        //                    SchoolTag schoolTag = new SchoolTag
        //                    {
        //                        TagId = tag.Id,
        //                        SchoolCommentId = schoolComment.Id,
        //                        UserId = userId,
        //                        SchoolId = schoolComment.SchoolSectionId
        //                    };
        //                    _schoolTagService.Add(schoolTag);
        //                }
        //                else
        //                {
        //                    //标签已存在，添加关系
        //                    SchoolTag schoolTag = new SchoolTag
        //                    {
        //                        TagId = newTag.Id,
        //                        SchoolCommentId = schoolComment.Id,
        //                        UserId = userId,
        //                        SchoolId = schoolComment.SchoolSectionId
        //                    };
        //                    _schoolTagService.Add(schoolTag);
        //                }
        //            }
        //            else
        //            {
        //                SchoolTag schoolTag = new SchoolTag
        //                {
        //                    TagId = tag.Id,
        //                    SchoolCommentId = schoolComment.Id,
        //                    UserId = userId,
        //                    SchoolId = schoolComment.SchoolSectionId
        //                };
        //                _schoolTagService.Add(schoolTag);
        //            }
        //        }

        //        if (imgs.Count != 0)
        //        {
        //            foreach (var img in imgs)
        //            {
        //                _imageService.AddSchoolImage(img);
        //            }
        //        }

        //        return ResponseResult.Success(new {
        //            DataId = schoolComment.Id,
        //            SchoolId = schoolComment.SchoolSectionId
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return ResponseResult.Failed(ex.Message);
        //    }
        //}
    }
}