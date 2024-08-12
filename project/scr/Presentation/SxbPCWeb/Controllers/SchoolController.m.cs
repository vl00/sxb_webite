using Microsoft.AspNetCore.Mvc;
using PMS.CommentsManage.Domain.Common;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using PMS.School.Domain.Dtos;
using PMS.School.Infrastructure.Common;
using ProductManagement.Framework.Foundation;
using Sxb.PCWeb.Common;
using Sxb.PCWeb.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace Sxb.PCWeb.Controllers
{
    public partial class SchoolController : Controller
    {
        /// <summary>
        /// 学校详情:学校图册
        /// </summary>
        [HttpGet]
        [Route("/{controller}/{action}/{id}")]
        [Route("/{controller}-{schoolNo}/{action}")]
        public async Task<ActionResult> ExtAtlas(Guid extId = default, Guid sid = default, Guid id = default,
            PMS.OperationPlateform.Domain.Enums.CourseType CourseType = CourseType.Unknown, string schoolNo = default)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                _historyService.AddHistory(user.UserId, extId, (byte)MessageDataType.School);
            }

            if (id != default) extId = id; //兼容旧id
            SchExtDto0 schext = null;
            if (!string.IsNullOrWhiteSpace(schoolNo))
            {
                schext = await _schService.GetSchextSimpleInfoViaShortNo(schoolNo);
            }
            else
            {
                schext = await _schService.GetSchextSimpleInfo(extId);
            }
            if (schext == null) return new ExtNotFoundViewResult();
            if (schoolNo == default)
            {
                return RedirectPermanent($"/school-{schext.ShortSchoolNo}/extatlas/");
            }
            extId = schext.Eid;
            sid = schext.Sid;

            ViewBag.IsSignSchool = _lSRFSchoolDetail.CheckSchIsLeaving(extId);
            ViewBag.ActiveCourseType = (int)CourseType;

            var latitude = Convert.ToDouble(Request.GetLatitude());
            var longitude = Convert.ToDouble(Request.GetLongitude());
            var data = _schoolService.GetSchoolExtRecruit(extId);
            //如果学校分部为null的话跳转到错误页面
            if (data == null)
                return new ExtNotFoundViewResult();

            var schoolVideos = await _schoolService.GetSchoolVideos(extId);
            var schoolImages = await _schoolService.GetSchoolImages(extId, null);

            //学校专访
            //var videos = _schoolService.GetExtVideo(extId, (byte)VideoType.Interview);
            //ViewBag.Videos = videos;
            ////学校荣誉
            //ViewBag.Schoolhonor = StrHelper.GetHtmlImageUrlList(data.Schoolhonor);
            ////学生荣誉
            //ViewBag.Studenthonor = StrHelper.GetHtmlImageUrlList(data.Studenthonor);
            ////校长风采
            //ViewBag.Principal = StrHelper.GetHtmlImageUrlList(data.Principal);
            ////教师风采
            //ViewBag.Teacher = StrHelper.GetHtmlImageUrlList(data.Teacher);
            ////硬件设施
            //ViewBag.Hardware = StrHelper.GetHtmlImageUrlList(data.Hardware);
            ////社团活动
            //ViewBag.Community = StrHelper.GetHtmlImageUrlList(data.Community);
            ////各个年级课程表
            //ViewBag.TimeTables = StrHelper.GetHtmlImageUrlList(data.TimeTables);
            ////作息时间表
            //ViewBag.Schedule = StrHelper.GetHtmlImageUrlList(data.Schedule);
            ////校车路线
            //ViewBag.Diagram = StrHelper.GetHtmlImageUrlList(data.Diagram);
            await Get_common(sid, extId);

            if (schoolVideos?.Any() == true)
            {
                ViewBag.SchoolVideos = schoolVideos;
            }
            if (schoolImages?.Any() == true)
            {
                ViewBag.SchoolImages = schoolImages;
            }
            return View(data);
        }

        /// <summary>
        /// 学校详情:相关信息
        /// </summary>
        [HttpGet]
        [Description("学校详情:相关信息")]
        [Route("/{controller}/{action}/{id}")]
        [Route("/{controller}-{schoolNo}/{action}")]
        public async Task<IActionResult> ExtMessage(Guid extId = default(Guid), Guid sid = default(Guid), Guid id = default(Guid),
            PMS.OperationPlateform.Domain.Enums.CourseType CourseType = CourseType.Unknown, string schoolNo = default)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                _historyService.AddHistory(user.UserId, extId, (byte)MessageDataType.School);
            }

            if (id != default) extId = id; //兼容旧id
            SchExtDto0 schext = null;
            if (!string.IsNullOrWhiteSpace(schoolNo))
            {
                schext = await _schService.GetSchextSimpleInfoViaShortNo(schoolNo);
            }
            else
            {
                schext = await _schService.GetSchextSimpleInfo(extId);
            }
            if (schext == null) return new ExtNotFoundViewResult();
            if (schoolNo == default)
            {
                return RedirectPermanent($"/school-{schext.ShortSchoolNo}/extmessage/");
            }
            extId = schext.Eid;
            sid = schext.Sid;
            ViewBag.Grade = schext.Grade;

            ViewBag.IsSignSchool = _lSRFSchoolDetail.CheckSchIsLeaving(extId);
            ViewBag.ActiveCourseType = (int)CourseType;

            var latitude = Convert.ToDouble(Request.GetLatitude());
            var longitude = Convert.ToDouble(Request.GetLongitude());
            var data = await _schoolService.GetSchoolExtDtoAsync(extId, latitude, longitude);
            //如果学校分部为null的话跳转到错误页面
            if (data == null)
                return new ExtNotFoundViewResult();


            var result = data;
            //对口学校地址
            var counterPart = new List<KeyValueDto<Guid>>();
            if (!string.IsNullOrEmpty(result.CounterPart))
            {
                //反序列化
                counterPart = JsonHelper.JSONToObject<List<KeyValueDto<Guid>>>(result.CounterPart);
                if (counterPart.Count > 0)
                {
                    counterPart = counterPart?.OrderByDescending(o => o.Year)?.GroupBy(o => o.Year).FirstOrDefault().ToList();
                    var counterPartExtId = counterPart.Select(p => p.Value);
                    var address = _schoolService.GetSchoolExtAdress(counterPartExtId.ToArray());
                    //获取短链接id
                    counterPart = counterPart.Select(p => new KeyValueDto<Guid> { Key = p.Key, Value = p.Value, Message = address.FirstOrDefault(a => a.Value == p.Value).Key, Other = address.FirstOrDefault(a => a.Value == p.Value).Other }).ToList();
                }
            }
            ViewBag.CounterPart = counterPart;

            //附近学位房
            var buildings = new StringBuilder();
            if (result.Longitude != null && result.Latitude != null && result.Longitude > 0 && result.Latitude > 0)
            {
                var list = (await _schoolService.GetBuildingDataAsync(result.ExtId, result.Latitude, result.Longitude)).Select(p => p.Key + "," + p.Value + "," + p.Message);
                buildings.Append(string.Join(",", list));
            }
            ViewBag.Build = buildings.ToString();

            //划片区域坐标  ben 0828暂时做不了
            //var rangePoints = new StringBuilder();
            //var listRangePoints = (await _schoolService.GetSchoolExtRangePointsAsync(result.ExtId));
            //if (listRangePoints.Any())
            //{
            //    rangePoints.Append(string.Join(",", listRangePoints.Select(p => p.Longitude + "," + p.Latitude + "," + p.Name)));
            //}
            //ViewBag.RangePoints = rangePoints.ToString();


            await Get_common(sid, extId);
            return View(result);
        }



        /// <summary>
        /// 学校详情：学校攻略
        /// </summary>
        [HttpGet]
        [Description("学校详情：学校攻略")]
        [Route("/{controller}/{action}/{id}")]
        [Route("/{controller}-{schoolNo}/{action}")]
        public async Task<IActionResult> ExtStrategy(Guid extId = default(Guid), Guid sid = default(Guid), Guid id = default(Guid),
            PMS.OperationPlateform.Domain.Enums.CourseType CourseType = CourseType.Unknown, string schoolNo = default)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                _historyService.AddHistory(user.UserId, extId, (byte)MessageDataType.School);
            }

            if (id != default) extId = id; //兼容旧id
            SchExtDto0 schext = null;
            if (!string.IsNullOrWhiteSpace(schoolNo))
            {
                schext = await _schService.GetSchextSimpleInfoViaShortNo(schoolNo);
            }
            else
            {
                schext = await _schService.GetSchextSimpleInfo(extId);
            }
            if (schext == null) return new ExtNotFoundViewResult();
            if (schoolNo == default)
            {
                return RedirectPermanent($"/school-{schext.ShortSchoolNo}/extstrategy/");
            }
            extId = schext.Eid;
            sid = schext.Sid;
            ViewBag.Grade = schext.Grade;
            ViewBag.IsSignSchool = _lSRFSchoolDetail.CheckSchIsLeaving(extId);
            ViewBag.ActiveCourseType = (int)CourseType;

            var latitude = Request.GetLatitude();
            var longitude = Request.GetLongitude();
            var data = await _schoolService.GetSchoolExtSimpleDtoAsync(Convert.ToDouble(longitude), Convert.ToDouble(latitude), extId);
            //如果学校分部为null的话跳转到错误页面
            if (data == null)
                return new ExtNotFoundViewResult();


            var result = _articleService.PCSearchList(Request.GetLocalCity(), null, 0, 6);
            IEnumerable<article> articles = result.articles;
            //学校动态
            var extArticle = _articleService.GetComparisionArticles_PageVersion(new List<Guid>() { extId },
                0,
                3);
            //ViewBag.ExtOffset = extOffset;
            //ViewBag.ExtLimit = extLimit;
            //ViewBag.ExtTotal = extArticle.total;
            //ViewBag.ExtTotalPage = (int)Math.Ceiling((double)extArticle.total / extLimit);
            //学校政策
            var policyArticle = _articleService.GetPolicyArticles_PageVersion(new List<PMS.OperationPlateform.Domain.DTOs.SchoolType> { new PMS.OperationPlateform.Domain.DTOs.SchoolType { Chinese = data.Chinese ?? false, Diglossia = data.Diglossia ?? false, Discount = data.Discount ?? false, SchoolGrade = data.Grade, Type = data.Type } }, new List<PMS.OperationPlateform.Domain.DTOs.Local> { new PMS.OperationPlateform.Domain.DTOs.Local { AreaId = data.Area, CityId = data.City, ProvinceId = data.Province } },
             0,
             3
            );
            //ViewBag.PolicyOffset = policyOffset;
            //ViewBag.PolicyLimit = policyLimit;
            //ViewBag.PolicyTotal = policyArticle.total;
            //ViewBag.PolicyTotalPage = (int)Math.Ceiling((double)policyArticle.total / policyLimit);
            //猜你喜欢
            var youwilllike = _articleService.IThinkYouWillLike(UserId, 5, 6);
            //热门攻略
            var hotPoint = await _articleService.HotPointArticles(Request.GetLocalCity(), UserId, 0, 5);

            var allAIds = new List<Guid>();
            if (articles?.Any() == true)
            {
                allAIds.AddRange(articles.Select(p => p.id));
            }
            if (youwilllike.articles?.Count() > 0)
            {
                allAIds.AddRange(youwilllike.articles.Select(p => p.id));
            }
            if (extArticle.articles?.Count() > 0)
            {
                allAIds.AddRange(extArticle.articles.Select(p => p.id));
            }
            if (policyArticle.articles?.Count() > 0)
            {
                allAIds.AddRange(policyArticle.articles.Select(p => p.id));
            }
            if (hotPoint.articles?.Count() > 0)
            {
                allAIds.AddRange(hotPoint.articles.Select(p => p.id));
            }
            //var allAIds = articles.Select(a => a.id)
            //                .Union(youwilllike.articles.Select(a => a.id))
            //                .Union(hotPoint.articles.Select(a => a.id))
            //                .Union(extArticle.articles.Select(a => a.id))
            //                .Union(policyArticle.articles.Select(a => a.id));

            var covers = _articleCoverService.GetCoversByIds(allAIds.ToArray());

            //学校动态
            ViewBag.ExtArticle = extArticle.articles.Select(a => new ViewModels.Article.ArticleListItemViewModel()
            {
                Id = a.id,
                No = UrlShortIdUtil.Long2Base32(a.No),
                Layout = a.layout,
                Type = string.IsNullOrEmpty(a.type) ? 1 : int.Parse(a.type),
                Title = a.title,
                ViewCount = a.VirualViewCount,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                Covers = covers.Where(c => c.articleID == a.id).Select(c => Article_CoverToLink(c)).ToList()
            });
            //学校政策
            ViewBag.PolicyArticle = policyArticle.articles.Select(a => new ViewModels.Article.ArticleListItemViewModel()
            {
                Id = a.id,
                No = UrlShortIdUtil.Long2Base32(a.No),
                Layout = a.layout,
                Type = string.IsNullOrEmpty(a.type) ? 1 : int.Parse(a.type),
                Title = a.title,
                ViewCount = a.VirualViewCount,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                Covers = covers.Where(c => c.articleID == a.id).Select(c => Article_CoverToLink(c)).ToList()
            });

            ViewBag.IThinkYouLike = youwilllike.articles.Select(a => new ViewModels.Article.ArticleListItemViewModel()
            {
                Id = a.id,
                No = UrlShortIdUtil.Long2Base32(a.No),
                Layout = a.layout,
                Type = string.IsNullOrEmpty(a.type) ? 1 : int.Parse(a.type),
                Title = a.title,
                ViewCount = a.VirualViewCount,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                Covers = covers.Where(c => c.articleID == a.id).Select(c => Article_CoverToLink(c)).ToList()
            });

            ViewBag.HotPoint = hotPoint.articles.Select(a => new ViewModels.Article.ArticleListItemViewModel()
            {
                Id = a.id,
                No = UrlShortIdUtil.Long2Base32(a.No),
                Layout = a.layout,
                Type = string.IsNullOrEmpty(a.type) ? 1 : int.Parse(a.type),
                Title = a.title,
                ViewCount = a.VirualViewCount,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                Covers = covers.Where(c => c.articleID == a.id).Select(c => Article_CoverToLink(c)).ToList()
            });

            Guid userId = UserId ?? Guid.Empty;
            await Get_common(sid, extId);

            return View(data);
        }

        /// <summary>
        /// 学校详情：学校攻略
        /// </summary>
        [HttpGet]
        [Description("学校详情：学校动态")]
        [Route("/{controller}/{action}/{id}")]
        [Route("/{controller}-{schoolNo}/{action}")]
        [Route("/{controller}-{schoolNo}/{action}/{page}")]
        public async Task<IActionResult> SchoolDynamic(Guid extId = default(Guid), Guid sid = default(Guid), Guid id = default(Guid), int offset = 0
            , int limit = 6, string schoolNo = default, int page = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                _historyService.AddHistory(user.UserId, extId, (byte)MessageDataType.School);
            }

            if (id != default) extId = id; //兼容旧id
            SchExtDto0 schext = null;
            if (!string.IsNullOrWhiteSpace(schoolNo))
            {
                schext = await _schService.GetSchextSimpleInfoViaShortNo(schoolNo);
            }
            else
            {
                schext = await _schService.GetSchextSimpleInfo(extId);
            }
            if (schext == null) return new ExtNotFoundViewResult();
            extId = schext.Eid;
            sid = schext.Sid;

            var latitude = Request.GetLatitude();
            var longitude = Request.GetLongitude();
            var data = await _schoolService.GetSchoolExtSimpleDtoAsync(Convert.ToDouble(longitude), Convert.ToDouble(latitude), extId);
            //如果学校分部为null的话跳转到错误页面
            if (data == null)
                return new ExtNotFoundViewResult();



            var result = _articleService.PCSearchList(Request.GetLocalCity(), null, 0, 6);
            IEnumerable<article> articles = result.articles;

            if (page > 0)
            {
                offset = (page - 1) * limit;
            }
            //学校动态
            var extArticle = _articleService.GetComparisionArticles_PageVersion(new List<Guid>() { extId },
                offset,
                limit);
            ViewBag.Offset = offset;
            ViewBag.Limit = limit;
            ViewBag.Total = extArticle.total;
            ViewBag.TotalPage = (int)Math.Ceiling((double)extArticle.total / limit);
            //猜你喜欢
            var youwilllike = _articleService.IThinkYouWillLike(UserId, 5, 6);
            //热门攻略
            var hotPoint = await _articleService.HotPointArticles(Request.GetLocalCity(), UserId, 0, 5);
            var allAIds = new List<Guid>();
            if (articles?.Any() == true)
            {
                allAIds.AddRange(articles.Select(p => p.id));
            }
            if (youwilllike.articles?.Count() > 0)
            {
                allAIds.AddRange(youwilllike.articles.Select(p => p.id));
            }
            if (extArticle.articles?.Count() > 0)
            {
                allAIds.AddRange(extArticle.articles.Select(p => p.id));
            }
            if (hotPoint.articles?.Count() > 0)
            {
                allAIds.AddRange(hotPoint.articles.Select(p => p.id));
            }
            //var allAIds = articles.Select(a => a.id)
            //                .Union(youwilllike.articles.Select(a => a.id))
            //                .Union(hotPoint.articles.Select(a => a.id))
            //                .Union(extArticle.articles.Select(a => a.id));
            var covers = _articleCoverService.GetCoversByIds(allAIds.ToArray());

            //学校动态
            ViewBag.ExtArticle = extArticle.articles.Select(a => new ViewModels.Article.ArticleListItemViewModel()
            {
                Id = a.id,
                No = UrlShortIdUtil.Long2Base32(a.No),
                Layout = a.layout,
                Type = string.IsNullOrEmpty(a.type) ? 1 : int.Parse(a.type),
                Title = a.title,
                ViewCount = a.VirualViewCount,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                Covers = covers.Where(c => c.articleID == a.id).Select(c => Article_CoverToLink(c)).ToList()
            });

            ViewBag.IThinkYouLike = youwilllike.articles.Select(a => new ViewModels.Article.ArticleListItemViewModel()
            {
                Id = a.id,
                No = UrlShortIdUtil.Long2Base32(a.No),
                Layout = a.layout,
                Type = string.IsNullOrEmpty(a.type) ? 1 : int.Parse(a.type),
                Title = a.title,
                ViewCount = a.VirualViewCount,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                Covers = covers.Where(c => c.articleID == a.id).Select(c => Article_CoverToLink(c)).ToList()
            });

            ViewBag.HotPoint = hotPoint.articles.Select(a => new ViewModels.Article.ArticleListItemViewModel()
            {
                Id = a.id,
                No = UrlShortIdUtil.Long2Base32(a.No),
                Layout = a.layout,
                Type = string.IsNullOrEmpty(a.type) ? 1 : int.Parse(a.type),
                Title = a.title,
                ViewCount = a.VirualViewCount,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                Covers = covers.Where(c => c.articleID == a.id).Select(c => Article_CoverToLink(c)).ToList()
            });
            ViewBag.IsSignSchool = _lSRFSchoolDetail.CheckSchIsLeaving(extId);
            await Get_common(sid, extId);
            return View(data);
        }

        [HttpGet]
        [Description("学校详情：升学政策")]
        [Route("/{controller}/{action}/{id}")]
        [Route("/{controller}-{schoolNo}/{action}")]
        [Route("/{controller}-{schoolNo}/{action}/{page}")]
        public async Task<IActionResult> SchoolPolicy(Guid extId = default(Guid), Guid sid = default(Guid), Guid id = default(Guid), int offset = 0, int limit = 6,
            string schoolNo = default, int page = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                _historyService.AddHistory(user.UserId, extId, (byte)MessageDataType.School);
            }

            if (id != default) extId = id; //兼容旧id
            SchExtDto0 schext = null;
            if (!string.IsNullOrWhiteSpace(schoolNo))
            {
                schext = await _schService.GetSchextSimpleInfoViaShortNo(schoolNo);
            }
            else
            {
                schext = await _schService.GetSchextSimpleInfo(extId);
            }
            if (schext == null) return new ExtNotFoundViewResult();
            extId = schext.Eid;
            sid = schext.Sid;

            var latitude = Request.GetLatitude();
            var longitude = Request.GetLongitude();
            var data = await _schoolService.GetSchoolExtSimpleDtoAsync(Convert.ToDouble(longitude), Convert.ToDouble(latitude), extId);
            //如果学校分部为null的话跳转到错误页面
            if (data == null)
                return new ExtNotFoundViewResult();
            if (page > 0)
            {
                offset = (page - 1) * limit;
            }

            var result = _articleService.PCSearchList(Request.GetLocalCity(), null, 0, 6);
            IEnumerable<article> articles = result.articles;

            //学校政策
            var policyArticle = _articleService.GetPolicyArticles_PageVersion(new List<PMS.OperationPlateform.Domain.DTOs.SchoolType> { new PMS.OperationPlateform.Domain.DTOs.SchoolType { Chinese = data.Chinese ?? false, Diglossia = data.Diglossia ?? false, Discount = data.Discount ?? false, SchoolGrade = data.Grade, Type = data.Type } }, new List<PMS.OperationPlateform.Domain.DTOs.Local> { new PMS.OperationPlateform.Domain.DTOs.Local { AreaId = data.Area, CityId = data.City, ProvinceId = data.Province } },
             offset,
             limit
            );
            ViewBag.Offset = offset;
            ViewBag.Limit = limit;
            ViewBag.Total = policyArticle.total;
            ViewBag.TotalPage = (int)Math.Ceiling((double)policyArticle.total / limit);
            //猜你喜欢
            var youwilllike = _articleService.IThinkYouWillLike(UserId, 5, 6);
            //热门攻略
            var hotPoint = await _articleService.HotPointArticles(Request.GetLocalCity(), UserId, 0, 5);
            var allAIds = articles.Select(a => a.id)
                            .Union(youwilllike.articles.Select(a => a.id))
                            .Union(hotPoint.articles.Select(a => a.id))
                            .Union(policyArticle.articles.Select(a => a.id));
            var covers = _articleCoverService.GetCoversByIds(allAIds.ToArray());


            //学校政策
            ViewBag.PolicyArticle = policyArticle.articles.Select(a => new ViewModels.Article.ArticleListItemViewModel()
            {
                Id = a.id,
                No = UrlShortIdUtil.Long2Base32(a.No),
                Layout = a.layout,
                Type = string.IsNullOrEmpty(a.type) ? 1 : int.Parse(a.type),
                Title = a.title,
                ViewCount = a.VirualViewCount,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                Covers = covers.Where(c => c.articleID == a.id).Select(c => Article_CoverToLink(c)).ToList()
            });

            ViewBag.IThinkYouLike = youwilllike.articles.Select(a => new ViewModels.Article.ArticleListItemViewModel()
            {
                Id = a.id,
                No = UrlShortIdUtil.Long2Base32(a.No),
                Layout = a.layout,
                Type = string.IsNullOrEmpty(a.type) ? 1 : int.Parse(a.type),
                Title = a.title,
                ViewCount = a.VirualViewCount,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                Covers = covers.Where(c => c.articleID == a.id).Select(c => Article_CoverToLink(c)).ToList()
            });

            ViewBag.HotPoint = hotPoint.articles.Select(a => new ViewModels.Article.ArticleListItemViewModel()
            {
                Id = a.id,
                No = UrlShortIdUtil.Long2Base32(a.No),
                Layout = a.layout,
                Type = string.IsNullOrEmpty(a.type) ? 1 : int.Parse(a.type),
                Title = a.title,
                ViewCount = a.VirualViewCount,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                Covers = covers.Where(c => c.articleID == a.id).Select(c => Article_CoverToLink(c)).ToList()
            });
            ViewBag.IsSignSchool = _lSRFSchoolDetail.CheckSchIsLeaving(extId);
            await Get_common(sid, extId);
            return View(data);
        }

        public Guid? UserId { get { return null; } }

        /// <summary>
        /// 文章列表项的时间格式
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        string ArticleListItemTimeFormart(DateTime dateTime)
        {

            var passBy_H = (int)(DateTime.Now - dateTime).TotalHours;

            if (passBy_H > 24 || passBy_H < 0)
            {
                //超过24小时,显示日期
                return dateTime.ToString("yyyy年MM月dd日");

            }
            if (passBy_H == 0)
            {
                return dateTime.ToString("刚刚");
            }
            else
            {
                return $"{passBy_H}小时前";
            }
        }

        /// <summary>
        /// 文章背景图转string 链接
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        string Article_CoverToLink(article_cover c)

        {
            return !string.IsNullOrWhiteSpace(c.ImgUrl) ? c.ImgUrl : $"https://cos.sxkid.com/images/article/{c.articleID}/{c.photoID}.{((FileExtension)c.ext).ToString()}";

        }

    }
}