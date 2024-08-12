using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.UserManage.Application.IServices;
using ProductManagement.Infrastructure.Toolibrary;
using Sxb.Api.ViewDto.CommentVo;
using Sxb.Api.ViewDto.UserInfoVo;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Application.ModelDto;
using Sxb.Api.RequestModel.RequestOption;
using PMS.CommentsManage.Application.ModelDto.Reply;

namespace Sxb.Api.Controllers
{
    /// <summary>
    /// 点评接口
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SchoolCommentController : ControllerBase
    {
        private ISchoolCommentService _schoolCommentService;
        private IMapper _mapper;
        private IUserService _userService;
        private ISchoolCommentReplyService _schoolCommentReplyService;

        public SchoolCommentController(IMapper mapper, 
            ISchoolCommentService schoolCommentService, 
            IUserService userService,
            ISchoolCommentReplyService schoolCommentReplyService)
        {
            _schoolCommentService = schoolCommentService;
            _mapper = mapper;
            _userService = userService;
            _schoolCommentReplyService = schoolCommentReplyService;
        }

        /// <summary>
        /// 根据点评id获取点评详情
        /// </summary>
        /// <param name="CommentId"></param>
        /// <returns></returns>
        [HttpGet]
        public PageResult<CommentModel> GetCommentInfoById(Guid CommentId)
        {
            try
            {
                var comment =  _mapper.Map<SchoolCommentDto, CommentModel>(_schoolCommentService.QueryComment(CommentId));
                comment.UserInfoModel = _mapper.Map<UserInfoDto, UserInfoModel>(_userService.GetUserInfo(comment.UserId));

                return new PageResult<CommentModel>()
                {
                    rows = comment,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new PageResult<CommentModel>()
                {
                    rows = null,
                    StatusCode = 500,
                    Message = ex.Message
                };
            }
        }


        /// <summary>
        /// 根据点评id列表获取点评详情列表
        /// </summary>
        /// <param name="CommentIds">点评id列表</param>
        /// <returns></returns>
        [HttpPost]
        public PageResult<List<CommentModel>> GetCommentInfoByIds(List<Guid> CommentIds)
        {
            try
            {
                var comment = _mapper.Map<List<SchoolCommentDto>, List<CommentModel>>(_schoolCommentService.QueryCommentByIds(CommentIds,Guid.Empty));

                comment.ForEach(x => {
                    x.UserInfoModel = _mapper.Map<UserInfoDto, UserInfoModel>(_userService.GetUserInfo(x.UserId));
                });
                //comment.UserInfoModel = _mapper.Map<UserInfo, UserInfoModel>(_userService.GetUserInfo(comment.UserId));

                return new PageResult<List<CommentModel>>()
                {
                    rows = comment,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new PageResult<List<CommentModel>>()
                {
                    rows = null,
                    StatusCode = 500,
                    Message = ex.Message
                };
            }
        }


        /// <summary>
        /// 根据分部id，获取该分部下最新的前20条点数据，且可以模糊指定匹配
        /// </summary>
        /// <param name="SchoolSectionId">学校分部Id</param>
        /// <param name="Conente">点评内容</param>
        /// <returns></returns>
        [HttpGet]
        public PageResult<List<CommentModel>> GetCommentInfoBySchoolSectionIdOrConente(Guid SchoolSectionId,string Conente)
        {
            try
            {
                var comment = _mapper.Map<List<SchoolCommentDto>, List<CommentModel>>(_schoolCommentService.GetSchoolCommentBySchoolIdOrConente(SchoolSectionId, Conente));
                comment.ForEach(x => {
                    var user = (_userService.GetUserInfo(x.UserId));
                    x.UserInfoModel = new UserInfoModel() { Id = user.Id, HeadImager = user.HeadImgUrl,NickName = user.NickName,Role = user.VerifyTypes?.ToList() };
                });
                //comment.UserInfoModel = _mapper.Map<UserInfo, UserInfoModel>(_userService.GetUserInfo(comment.UserId));

                return new PageResult<List<CommentModel>>()
                {
                    rows = comment,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new PageResult<List<CommentModel>>()
                {
                    rows = null,
                    StatusCode = 500,
                    Message = ex.Message
                };
            }
        }


        /// <summary>
        /// 根据点评id列表创建点评实体
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        [HttpGet]
        public PageResult<List<CommentModel>> GetCommentsByIds(List<Guid> Ids)
        {
            try
            {
                List<Guid> DistinctIds = Ids.Distinct()?.ToList();
                List<CommentModel> comments = new List<CommentModel>();
                foreach (var id in DistinctIds)
                {
                    var model = comments.Find(x => x.Id == id);
                    if (model == null)
                    {
                        comments.Add(new CommentModel() { Id = id });
                    }
                    else
                    {
                        comments.Add(model);
                    }
                }

                return new PageResult<List<CommentModel>>()
                {
                    StatusCode = 200,
                    rows = comments
                };
            }
            catch (Exception ex)
            {
                return new PageResult<List<CommentModel>>()
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
            }
        }

        ///// <summary>
        ///// 第1个是我传一堆ID给你返回一个评论或者回复的list
        ///// </summary>
        //public PageResult<CommentAndCommentReply> GetSchooCommentOrReply(List<RequestOption> requestOptions)
        //{
        //    try
        //    {
        //        string iSchoolAuth = HttpContext.Request.Cookies["iSchoolAuth"];
                
        //        if(iSchoolAuth == null)
        //        {
        //            return null;
        //        }

        //        CommentAndCommentReply commentAndCommentReply = new CommentAndCommentReply();
        //        var groups = requestOptions.GroupBy(x => x.dataIdType).ToList();

        //        for (int i = 0; i < groups.Count(); i++)
        //        {
        //            if(groups[i].Key == RequestModel.RequestEnum.dataIdType.SchooComment)
        //            {
        //                var comment = _mapper.Map<List<SchoolCommentDto>, List<CommentModel>>(_schoolCommentService.QueryCommentByIds(groups[i].Select(x => x.dataId).ToList(), default(Guid)));
        //                comment.ForEach(x => {
        //                    x.UserInfoModel = _mapper.Map<UserInfoDto, UserInfoModel>(_userService.GetUserInfo(x.UserId));
        //                });
        //                commentAndCommentReply.commentModels = comment;
        //            }
        //            else if(groups[i].Key == RequestModel.RequestEnum.dataIdType.CommentReply)
        //            {
        //                commentAndCommentReply.commentReplyModels = _mapper.Map<List<ReplyDto>,List<CommentReplyModel>>(_schoolCommentReplyService.GetCommentReplyByIds(groups[i].Select(x => x.dataId).ToList(), default(Guid)));
        //            }
        //        }

        //        return new PageResult<CommentAndCommentReply>()
        //        {
        //            StatusCode = 200,
        //            rows = commentAndCommentReply
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new PageResult<CommentAndCommentReply>()
        //        {
        //            StatusCode = 500,
        //            Message = ex.Message
        //        };
        //    }
        //}

        ///// <summary>
        ///// 当前用户发布的评论或者回复
        ///// </summary>
        ///// <param name="CommentId"></param>
        ///// <param name="UserId"></param>
        ///// <param name="PageIndex"></param>
        ///// <param name="PageSize"></param>
        ///// <returns></returns>
        //public PageResult<CommentAndCommentReply> GetCommentReplyByCommetId(int PageIndex, int PageSize)
        //{
        //    try
        //    {   
        //        CommentAndCommentReply commentAndCommentReply = new CommentAndCommentReply();

        //        var comment = _mapper.Map<List<SchoolCommentDto>, List<CommentModel>>(_schoolCommentService.QueryNewestCommentByIds(PageIndex, PageSize, default(Guid)));
        //        comment.ForEach(x => {
        //            x.UserInfoModel = _mapper.Map<UserInfoDto, UserInfoModel>(_userService.GetUserInfo(x.UserId));
        //        });
        //        commentAndCommentReply.commentModels = comment;

        //        commentAndCommentReply.commentReplyModels = _mapper.Map<List<ReplyDto>, List<CommentReplyModel>>(_schoolCommentReplyService.GetNewestCommentReplyByUserId(PageIndex,PageSize,default(Guid)));

        //        return new PageResult<CommentAndCommentReply>()
        //        {
        //            StatusCode = 200,
        //            rows = commentAndCommentReply
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new PageResult<CommentAndCommentReply>()
        //        {
        //            StatusCode = 500,
        //            Message = ex.Message
        //        };
        //    }
        //}

        ///// <summary>
        ///// 其他用户回复当前用户的评论
        ///// </summary>
        ///// <param name="PageIndex"></param>
        ///// <param name="PageSize"></param>
        ///// <returns></returns>
        //public PageResult<List<CommentReplyModel>> GetCurretUserNewestReply(int PageIndex, int PageSize)
        //{
        //    try
        //    {
        //        var commentReplyModels = _mapper.Map<List<ReplyDto>, List<CommentReplyModel>>(_schoolCommentReplyService.GetCurretUserNewestReply(PageIndex, PageSize, default(Guid)));

        //        return new PageResult<List<CommentReplyModel>>()
        //        {
        //            StatusCode = 200,
        //            rows = commentReplyModels
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new PageResult<List<CommentReplyModel>>()
        //        {
        //            StatusCode = 500,
        //            Message = ex.Message
        //        };
        //    }
        //}


    }

}