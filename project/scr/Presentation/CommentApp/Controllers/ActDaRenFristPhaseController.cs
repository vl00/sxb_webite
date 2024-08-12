using Microsoft.AspNetCore.Mvc;

namespace Sxb.Web.Controllers
{
    using PMS.OperationPlateform.Application.IServices;
    using PMS.OperationPlateform.Domain;
    using PMS.UserManage.Application.IServices;
    using PMS.CommentsManage.Application.IServices;
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using PMS.OperationPlateform.Domain.Entitys;
    using PMS.UserManage.Application.ModelDto;
    using AutoMapper;
    using PMS.CommentsManage.Application.ModelDto;
    using Sxb.Web.Models.Comment;
    using Sxb.Web.Models;
    using Microsoft.Extensions.Options;
    using Sxb.Web.ViewModels.Article;
    using ProductManagement.Framework.Foundation;
    using NSwag.Annotations;

    [OpenApiIgnore]
    public class ActDaRenFristPhaseController : Controller
    {
        private IActDaRenFristPhaseService _service;

        private IArticleService _articleService;

        private IUserService _userService;

        private ISchoolCommentService _schoolCommentService;

        private IMapper _mapper;

        private IArticleCoverService _articleCoverService;

        private readonly ImageSetting _setting;

        public ActDaRenFristPhaseController(
            IActDaRenFristPhaseService service, IArticleService articleService,
            IUserService userService, ISchoolCommentService schoolCommentService,
            IMapper mapper, IOptions<ImageSetting> set,
            IArticleCoverService articleCoverService)
        {
            this._service = service;
            this._articleService = articleService;
            this._userService = userService;
            this._schoolCommentService = schoolCommentService;
            this._mapper = mapper;
            this._setting = set.Value;
            this._articleCoverService = articleCoverService;
        }

        public IActionResult Index()
        {
            var config = this._service.GetTheLast();
            //取文章
            var keys = config.ArticleIds.Split(';', StringSplitOptions.RemoveEmptyEntries).Skip(0).Take(3);
            if ((keys?.Any()).GetValueOrDefault())
            {
                var split_id_no = AidToIdOrNo(keys);
                var articles = this._articleService.GetEffectiveBy(split_id_no.ids, split_id_no.nos, false);
                if ((articles?.Any()).GetValueOrDefault())
                {
                    var covers = this._articleCoverService.GetCoversByIds(articles.Select(a => a.id).ToArray());
                    articles = articles.Select(a =>
                    {
                        a.Covers = covers.Where(c => a.id == c.articleID).ToList();
                        return a;
                    });
                    ViewBag.Articles = articles;
                }
            }

            //取达人信息
            var userInfo = this._userService.GetUserInfo(config.DaRenUserId.GetValueOrDefault());

            //取学校点评
            var scIds = config.CommentIDs.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(sid => Guid.Parse(sid)).Skip(0).Take(3);
            var comment = this._schoolCommentService.GetSchoolCommentByCommentId(scIds.ToList());
            var Exhibtion = _mapper.Map<List<SchoolCommentDto>, List<CommentList>>(comment);
            if (comment.Any())
            {
                var UserIds = new List<Guid>();
                UserIds.AddRange(comment.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
                var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

                for (int i = 0; i < Exhibtion.Count(); i++)
                {
                    var user = Users.FirstOrDefault(q => q.Id == comment[i].UserId);

                    for (int j = 0; j < Exhibtion[i].Images.Count; j++)
                    {
                        string tempImage = Exhibtion[i].Images[j];

                        Exhibtion[i].Images[j] = _setting.QueryImager + tempImage;
                    }

                    if (Exhibtion[i].IsAnony)
                    {
                        Exhibtion[i].UserInfo = new Models.User.UserInfoVo { NickName = "匿名用户", HeadImager = "https://cdn.sxkid.com//images/AppComment/defaultUserHeadImage.png" };
                    }
                    else
                    {
                        Exhibtion[i].UserInfo = _mapper.Map<UserInfoDto, Models.User.UserInfoVo>(user);
                    }
                }
                ViewBag.Comments = Exhibtion;

                //var schooinfos = _schoolInfoService.GetSchoolStatuse(Exhibtion.GroupBy(x => x.SchoolSectionId).Select(x => x.Key).ToList());
                //Exhibtion.ForEach(x =>
                //{
                //    var schoolTemp = schooinfos.Where(s => s.SchoolSectionId == x.SchoolSectionId).FirstOrDefault();
                //    x.School = _mapper.Map<SchoolInfoDto, CommentList.SchoolInfo>(schoolTemp);
                //});
            }

            //return Json(new { config = config, articles = articles, schoolComments, userInfo, config.DaRenIntro, config.DaRenQRCode });

            ViewBag.UserInfo = userInfo;
            ViewBag.Introduce = config.DaRenIntro;
            ViewBag.QRCode = config.DaRenQRCode;

            return View();
        }

        public IActionResult CommentItemsPartialView(int offset = 0, int limit = 3)
        {
            var config = this._service.GetTheLast();
            //取学校点评
            var scIds = config.CommentIDs.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(sid => Guid.Parse(sid)).Skip(offset).Take(limit);
            if (scIds.Any())
            {
                var comment = this._schoolCommentService.GetSchoolCommentByCommentId(scIds.ToList());
                var Exhibtion = _mapper.Map<List<SchoolCommentDto>, List<CommentList>>(comment);
                if (comment.Any())
                {
                    var UserIds = new List<Guid>();
                    UserIds.AddRange(comment.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
                    var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

                    for (int i = 0; i < Exhibtion.Count(); i++)
                    {
                        for (int j = 0; j < Exhibtion[i].Images.Count; j++)
                        {
                            string tempImage = Exhibtion[i].Images[j];

                            Exhibtion[i].Images[j] = _setting.QueryImager + tempImage;
                        }

                        var user = Users.FirstOrDefault(q => q.Id == comment[i].UserId);
                        if (Exhibtion[i].IsAnony)
                        {
                            Exhibtion[i].UserInfo = new Models.User.UserInfoVo { NickName = "匿名用户", HeadImager = "https://cdn.sxkid.com//images/AppComment/defaultUserHeadImage.png" };
                        }
                        else
                        {
                            Exhibtion[i].UserInfo = _mapper.Map<UserInfoDto, Models.User.UserInfoVo>(user);
                        }
                    }
                    ViewBag.Comments = Exhibtion;
                    return PartialView();
                }
            }
            return Content("notfound");
        }

        public IActionResult ArticleItemsPartialView(int offset = 0, int limit = 3)
        {
            var config = this._service.GetTheLast();
            //取文章
            var keys = config.ArticleIds.Split(';', StringSplitOptions.RemoveEmptyEntries).Skip(offset).Take(limit);

            if ((keys?.Any()).GetValueOrDefault())
            {
                var split_id_no = AidToIdOrNo(keys);
                var articles = this._articleService.GetEffectiveBy(split_id_no.ids, split_id_no.nos, false);
                if ((articles?.Any()).GetValueOrDefault())
                {
                    var covers = this._articleCoverService.GetCoversByIds(articles.Select(a => a.id).ToArray());
                    articles = articles.Select(a =>
                    {
                        a.Covers = covers.Where(c => a.id == c.articleID).ToList();
                        return a;
                    });
                    ViewBag.Articles = articles;
                    return PartialView();
                }
            }
            return Content("notfound");
        }

        /// <summary>
        /// 将配置中获取的文章主键(可能包含 id类型的和no类型的)进行分离。
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        private (Guid[] ids, int[] nos) AidToIdOrNo(IEnumerable<string> keys)
        {
            List<Guid> l_ids = new List<Guid>();
            List<int> n_nos = new List<int>();
            foreach (var item in keys)
            {
                if (Guid.TryParse(item, out Guid id))
                {
                    l_ids.Add(id);
                }
                else
                {
                    int no_int32 = (int)UrlShortIdUtil.Base322Long(item);
                    n_nos.Add(no_int32);
                }
            }
            return (l_ids.ToArray(), n_nos.ToArray());
        }
    }
}