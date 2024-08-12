using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto.ModelVo.CommentVo;
using ProductManagement.API.Http.Interface;
using Sxb.UserCenter.Models.ArticleViewModel;
using Sxb.UserCenter.Models.LiveViewModel;
using Sxb.UserCenter.Response;
using Sxb.UserCenter.Utils;
using Sxb.UserCenter.Utils.DtoToViewModel;
using ProductManagement.Framework.Foundation;
using PMS.UserManage.Application.ModelDto.ModelVo;
using PMS.UserManage.Domain.Common;
using PMS.School.Application.IServices;
using PMS.School.Application.ModelDto;
using static PMS.UserManage.Domain.Common.EnumSet;
using PMS.TopicCircle.Application.Services;
using PMS.TopicCircle.Application.Dtos;
using AutoMapper;
using Sxb.UserCenter.Models.TopicCircle;
using Microsoft.VisualBasic;

namespace Sxb.UserCenter.Controllers
{
    /// <summary>
    /// 我的搜藏
    /// </summary>
    public class ApiCollectionController : Base
    {
        private readonly IMapper _mapper;
        private ISchoolService _schoolService { get; set; }

        private readonly ICollectionService _collection;
        private readonly IArticleService _articleService;
        private readonly ILiveServiceClient _liveServiceClient;
        private readonly ITopicService _topicService;
        readonly IAccountService _AccountService;

        public ApiCollectionController(ICollectionService collection, IArticleService articleService,
            ISchoolService schoolService, ILiveServiceClient liveServiceClient, ITopicService topicService, IMapper mapper, IAccountService accountService)
        {
            _collection = collection;
            _articleService = articleService;
            _liveServiceClient = liveServiceClient;
            _schoolService = schoolService;
            _topicService = topicService;
            _mapper = mapper;
            _AccountService = accountService;
        }

        /// <summary>
        /// 收藏点评列表
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ResponseResult Comment(int page = 1)
        {
            string cookieStr = Request.Headers["Cookie"];
            var data = _collection.GetCommentCollection(userID, cookieStr, page: page);

            var comment = JsonConvert.DeserializeObject<CommentReplyData>(data.data.ToString());
            if (!comment.commentModels.Any())
            {
                return ResponseResult.Success(new List<CommentReplyData>());
            }
            else
            {
                foreach (var item in comment.commentModels.Where(p => p.UserInfo != null))
                {
                    item.UserHeadImage = item.UserInfo.HeadImager;
                    item.UserName = item.UserInfo.NickName;
                }
                return ResponseResult.Success(comment);
            }
        }

        /// <summary>
        /// 收藏问答列表
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ResponseResult QA(int page = 1)
        {
            string cookieStr = Request.Headers["Cookie"];
            var data = _collection.GetQACollection(userID, cookieStr, page: page);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 收藏学校列表
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<ResponseResult> School(int page = 1)
        {
            var ids = _collection.GetUserCollection(userID, 1, page) ?? default;

            var result = ids.Count > 0 ? await _schoolService.ListExtSchoolByBranchIds(ids, latitude ?? 0, longitude ?? 0) : new List<SchoolExtFilterDto>();

            var school = result.Select(q => new SchoolModel
            {
                Sid = q.Sid,
                ExtId = q.ExtId,
                SchoolName = q.Name,
                City = q.CityCode,
                Area = q.AreaCode,
                CityName = q.City,
                AreaName = q.Area,
                Distance = q.Distance.ToString(),
                LodgingType = (int)q.LodgingType,
                LodgingReason = q.LodgingType.Description(),
                Tuition = q.Tuition,
                Tags = q.Tags,
                Score = q.Score,
                Grade = (byte)q.Grade,
                Type = (EnumSet.SchoolType)q.Type,
                CommentCount = q.CommentCount
            }).ToList();

            return ResponseResult.Success(school);
        }

        /// <summary>
        /// 收藏文章列表
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ResponseResult Article(int page = 1)
        {
            var ids = _collection.GetUserCollection(userID, 0, page) ?? default;
            if (!ids.Any())
            {
                return ResponseResult.Success(new List<DataViewModel>());
            }
            List<article> dataDtos = _articleService.GetByIds(ids.ToArray(), readCovers: true).ToList();
            List<DataViewModel> datas = ArticleDataToVoHelper.ArticleToViewModelHelper(dataDtos);
            return ResponseResult.Success(datas);
        }

        /// <summary>
        /// 收藏直播列表
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<ResponseResult> Live(int page = 1)
        {
            List<LiveViewModel> liveViews = new List<LiveViewModel>();

            var result = await _liveServiceClient.MyCollections(HttpContext.Request.GetAllCookies(), page);
            if (result.Items != null && result.Items.Any())
            {
                liveViews = result.Items.Select(q => new LiveViewModel
                {
                    Id = q.Lecture.Id,
                    Title = q.Lecture.Subject,
                    FrontCover = q.Lecture.HomeCover,
                    Time = ((DateTime)q.Time).ConciseTime(),
                    ViewCount = q.Lecture.Onlinecount,
                    LectorId = q.Lecture.lector?.Id ?? Guid.Empty,
                    LectorName = q.Lecture.lector?.Name,
                    LectorHeadImg = q.Lecture.lector?.Headimgurl,
                }).ToList();
            }
            return ResponseResult.Success(liveViews);
        }

        /// <summary>
        /// 收藏话题分页列表
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ResponseResult GetTopicPagination(int page = 1)
        {
            //收藏的数据Id
            var ids = _collection.GetUserCollection(userID, (byte)CollectionDataType.Topic, page) ?? default;

            //话题信息
            var dataDtos = _topicService.GetByIds(ids.ToArray(), userID).ToList();
            //按受收藏排序
            var sortDataDtos = ids.Select(id => dataDtos.FirstOrDefault(s => s.Id == id)).Where(s => s != null);

            //to model
            var data = _mapper.Map<List<CollectionTopicViewModel>>(sortDataDtos);
            return ResponseResult.Success(data);
        }

        [Authorize]
        public ResponseResult Add(CollectionDataType dataType, Guid dataID)
        {
            if (dataType == CollectionDataType.School) //关注学校
            {
                if (!_AccountService.IsBindPhone(userID)) 
                    return ResponseResult.Failed("该用户未绑定手机号 , 无法完成操作", new { ErrorCode = 300001 });
            }

            if (dataID.Equals(Guid.Empty) || !_collection.AddCollection(userID, dataID, (byte)dataType))
            {
                return ResponseResult.Failed("添加失败");
            }
            return ResponseResult.Success();
        }

        [Authorize]
        public ResponseResult Remove(CollectionDataType dataType, Guid dataID)
        {
            if (!_collection.RemoveCollection(userID, dataID))
            {
                return ResponseResult.Failed("删除失败");
            }
            return ResponseResult.Success();
        }

        public IActionResult AddCollection(Guid dataID, byte dataType, string callback)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Jsonp(new
                {
                    status = 1,
                    errorDescription = "请先登录",
                }, callback);
            }

            if (dataType == 1) //关注学校
            {
                if (!_AccountService.IsBindPhone(userID)) return Jsonp(ResponseResult.Failed("该用户未绑定手机号 , 无法完成操作", new { ErrorCode = 300001 }), callback);
            }

            PMS.UserManage.Domain.Common.RootModel json = new PMS.UserManage.Domain.Common.RootModel();
            if (dataID.Equals(Guid.Empty) || !_collection.AddCollection(userID, dataID, dataType))
            {
                json.status = 1;
                json.errorDescription = "添加失败";
            }
            return Jsonp(json, callback);
        }


        public IActionResult RemoveCollection(Guid dataID, string callback)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Jsonp(new
                {
                    status = 1,
                    errorDescription = "请先登录",
                }, callback);
            }

            PMS.UserManage.Domain.Common.RootModel json = new PMS.UserManage.Domain.Common.RootModel();
            if (!_collection.RemoveCollection(userID, dataID))
            {
                json.status = 1;
                json.errorDescription = "删除失败";
            }
            return Jsonp(json, callback);
        }

        [AllowAnonymous]
        public IActionResult IsCollected(Guid dataID, string callback, Guid userID = new Guid())
        {
            userID = userID == Guid.Empty ? base.userID : userID;
            if (string.IsNullOrEmpty(callback))
            {
                return Json(new { status = 0, iscollected = _collection.IsCollected(userID, dataID), userID });
            }
            else
            {
                return Jsonp(new { status = 0, iscollected = _collection.IsCollected(userID, dataID), userID }, callback);
            }
        }
        public IActionResult GetSchoolCollectionID()
        {
            return Json(_collection.GetSchoolCollectionID(userID));
        }
    }
}