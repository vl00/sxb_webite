using iSchool;
using Microsoft.AspNetCore.Mvc;
using PMS.CommentsManage.Domain.Common;
using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using PMS.School.Infrastructure.Common;
using ProductManagement.Framework.Foundation;
using Sxb.PCWeb.Common;
using Sxb.PCWeb.Response;
using Sxb.PCWeb.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace Sxb.PCWeb.Controllers
{
    public partial class SchoolController : Controller
    {
        [HttpGet]
        public IActionResult Eges(Guid sid = default, Guid extId = default, Guid id = default)
        {
            if (id != default) extId = id; //兼容旧id

            if (extId != default)
            {
                var extInfo = _schService.GetSchextSimpleInfo(extId);
                if (extInfo == null || extInfo.Result == null)
                {
                    return new ExtNotFoundViewResult();
                }
                return RedirectPermanent($"/school-{extInfo.Result.ShortSchoolNo}/");
            }
            return new ExtNotFoundViewResult();
        }
        /// <summary>
        /// 学校详情 - 总评
        /// </summary>
        [Description("学校详情-总评")]
        [HttpGet]
        [Route("/{controller}/{action}/{id}")]
        [Route("/{controller}-{schoolNo}/{action}")]
        public async Task<IActionResult> Data(Guid sid = default, Guid extId = default, Guid id = default,
            PMS.OperationPlateform.Domain.Enums.CourseType CourseType = PMS.OperationPlateform.Domain.Enums.CourseType.Unknown,
            string schoolNo = default)
        {
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
                return RedirectPermanent($"/school-{schext.ShortSchoolNo}/data/");
            }
            extId = schext.Eid;
            sid = schext.Sid;

            ViewBag.Grade = schext.Grade;
            ViewBag.SchoolNo = schoolNo;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                //添加历史
                _historyService.AddHistory(user.UserId, extId, (byte)MessageDataType.School);
            }
            ViewBag.IsSignSchool = _lSRFSchoolDetail.CheckSchIsLeaving(extId);
            ViewBag.ActiveCourseType = (int)CourseType;
            ViewBag.ExtID = extId;
            //var latitude = Convert.ToDouble(Request.GetLatitude());
            //var longitude = Convert.ToDouble(Request.GetLongitude());

            //var (data, _) = await _schoolService.GetSchoolExtDtoAsync(extId, sid, latitude, longitude);
            //if (data == null) throw new Exception("");
            ////如果学校分部为null的话跳转到错误页面
            //if (data == null)
            //    return new ExtNotFoundViewResult();


            //获取学校的特征
            //ViewBag.Character = await _schoolService.GetSchoolCharacterAsync(extId);
            //var schoolScore = await GetSchoolExtScoreAsync(extId);
            //ViewBag.ScoreIndex = schoolScore.index;
            //ViewBag.Score = schoolScore.score;

            //获取学校点评评分
            //ViewBag.SchoolScoreToStart = SchoolScoreToStart.GetCurrentSchoolstart(_commentScoreService.GetAvgScoreBybaraBranchSchool(extId));
            ////热评学校
            //ViewBag.HottestSchoolView = hot.HottestSchool(await _commentService.HottestSchool(new HotCommentQuery()
            //{
            //    Condition = true,
            //    City = 440100,
            //    Grade = 1,
            //    Type = 1,
            //    Discount = false,
            //    Diglossia = false,
            //    Chinese = false,
            //    StartTime = DateTime.Parse("2019-07-31"),
            //    EndTime = DateTime.Parse("2019-08-02")
            //}, false));


            await Get_common(sid, extId);
            return View();
        }

        /// <summary>
        /// 学校详情 - 学校概况
        /// </summary>
        [HttpGet]
        [Route("/{controller}/detail/{id}")]
        [Route("/school-{schoolNo}")]
        [Description("学校详情-学校概况")]
        public async Task<IActionResult> Detail(Guid extId = default, Guid sid = default, Guid id = default,
            int city = 0, PMS.OperationPlateform.Domain.Enums.CourseType CourseType = PMS.OperationPlateform.Domain.Enums.CourseType.Unknown,
            string schoolNo = default)
        {
            if (city != 0) Response.SetLocalCity(city.ToString());
            if (id != default) extId = id; //兼容旧id //切换城市
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
                return RedirectPermanent($"/school-{schext.ShortSchoolNo}/");
            }

            ViewBag.Grade = schext.Grade;
            ViewBag.ExtId = extId;
            extId = schext.Eid;
            sid = schext.Sid;

            ViewBag.IsSignSchool = _lSRFSchoolDetail.CheckSchIsLeaving(extId);
            ViewBag.ActiveCourseType = (int)CourseType;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                //添加历史
                _historyService.AddHistory(user.UserId, extId, (byte)MessageDataType.School);
            }
            var latitude = Convert.ToDouble(Request.GetLatitude());
            var longitude = Convert.ToDouble(Request.GetLongitude());
            //var (data, _) = await _schoolService.GetSchoolExtDtoAsync(extId, sid, latitude, longitude);
            var datas = await _schoolService.GetSchoolExtDtoAsync(extId, latitude, longitude);
            var data = datas;
            //如果学校分部为null的话跳转到错误页面
            if (data == null) return new ExtNotFoundViewResult();

            //获取学校总分数
            var score = (await _schoolService.GetSchoolExtScoreAsync(extId)).FirstOrDefault(p => p.IndexId == 22);
            ViewBag.Score = score?.Score;

            //学校排行
            ViewBag.Ranks = _schoolRankService.GetH5SchoolRankInfoBy(new Guid[] { data.ExtId }).ToList();


            //升学成绩内容
            object achData = null;
            if (data.AchYear != null)
            {
                achData = _schoolService.GetAchData(extId, data.Grade, data.Type, data.AchYear.Value);
            }
            ViewBag.AchData = achData;

            //升学年份
            ViewBag.AchYear = data.AchYear;

            if (data.Grade == (byte)(SchoolGrade.SeniorMiddleSchool))
            {
                //毕业生去往国际大学榜单列表
                var foreignList = await _schService.GetForeignCollegeList(data.Achievement);
                //毕业生去往国内大学榜单列表
                var domesticList = await _schService.GetDomesticCollegeList(data.Achievement);
                var collegeList = new List<KeyValueDto<Guid, string, double, int, int>>();
                collegeList.AddRange(foreignList);
                collegeList.AddRange(domesticList);
                //国内大学也可能是在国际榜单中，所以要去重
                //collegeList 大于 0 ，Achievement 不为null，
                //collegeList 等于 0 ，语句不执行，Achievement不需要额外判断
                var colleges = (from cellege in collegeList
                                from achievement in data.Achievement
                                where achievement.Key == cellege.Key //&& achievement.Data == cellege.Data
                                select new KeyValueDto<Guid, string, double, int>()
                                {
                                    Key = cellege.Key,
                                    Value = cellege.Value,
                                    Message = cellege.Message,
                                    Data = achievement.Data
                                }).GroupBy(g => g.Key).Select(s => s.First()).OrderByDescending(o => o.Message).ToList();
                data.Achievement = colleges;

                //升学成绩-毕业去向 国际高中需要显示2个榜
                if (data.Type == (byte)(PMS.OperationPlateform.Domain.Enums.SchoolType.International))
                {
                    ViewBag.iRanks = foreignList;
                    ViewBag.LRanks = domesticList;
                }
            }
            //外教比例
            //string foreignTea = null;
            //if (data.ForeignTea != null && data.ForeignTea != 0)
            //{
            //    foreignTea = $"1:{data.ForeignTea}";
            //}
            //ViewBag.ForeignTea = foreignTea;
            string foreignTea = null;
            if (data.ForeignTea != null && data.ForeignTea != 0)
            {
                foreignTea = $"{data.ForeignTea}%";
            }
            ViewBag.ForeignTea = foreignTea;

            var date_Now = DateTime.Now;
            //热门学校点评|热问学校
            ViewBag.HottestSchoolView = hot.HottestSchool(await _commentService.HottestSchool(new HotCommentQuery()
            {
                Condition = true,
                City = Request.GetLocalCity(),
                Grade = 1,
                Type = 1,
                Discount = false,
                Diglossia = false,
                Chinese = false,
                StartTime = date_Now.AddMonths(-6),
                EndTime = date_Now
            }, false));

            //热问学校【侧边栏】
            ViewBag.HottestQuestionSchoolView = hot.HottestQuestionItem(await _questionInfoService.HottestSchool());
            ViewBag.SchoolNo = schoolNo;
            await Get_common(sid, extId);
            ViewBag.Page_Description = data.Intro.GetHtmlHeaderString(150);
            return View(data);
        }

        /// <summary>
        /// 学校详情 - 招生简章
        /// </summary>
        [HttpGet]
        [Description("学校详情-招生简章")]
        [Route("/{controller}/{action}/{id}")]
        [Route("/{controller}-{schoolNo}/{action}")]
        public async Task<IActionResult> ExtRecruit(Guid extId = default, Guid sid = default, Guid id = default,
            PMS.OperationPlateform.Domain.Enums.CourseType CourseType = PMS.OperationPlateform.Domain.Enums.CourseType.Unknown, string schoolNo = default)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                //添加历史
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
                return RedirectPermanent($"/school-{schext.ShortSchoolNo}/extrecruit/");
            }
            extId = schext.Eid;
            sid = schext.Sid;
            ViewBag.Grade = schext.Grade;
            ViewBag.IsSignSchool = _lSRFSchoolDetail.CheckSchIsLeaving(extId);
            ViewBag.ActiveCourseType = (int)CourseType;

            var data = _schoolService.GetSchoolExtRecruit(extId);
            //如果学校分部为null的话跳转到错误页面
            if (data == null)
                return new ExtNotFoundViewResult();

            //招生对象
            var targer = string.IsNullOrEmpty(data.Target) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(data.Target);
            ViewBag.Target = targer;
            //招生比例
            var proportion = "暂未收录";
            if (data.Proportion != null && data.Proportion != 0)
            {
                var pro = Math.Round(Convert.ToDecimal(data.Proportion), 0);
                //var proNumber = NumberHelper.ConvertToFraction((float)pro);
                proportion = $"1:{pro}";
            }
            ViewBag.Proportion = proportion;
            //录取分数线
            var point = string.IsNullOrEmpty(data.Point) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(data.Point);
            ViewBag.Point = point;
            //招生日期
            var date = string.IsNullOrEmpty(data.Date) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(data.Date);
            ViewBag.Date = date;

            /* //select * from dbo.OnlineSchoolExtRecruit where point <> '[]'
            录取分数线 Point
            奖学金计划 Scholarship
            招生日期表 Date
            往期入学考试内容 Pastexam <div/>
            // */

            await Get_common(sid, extId);

            return View(data);
        }

        /// <summary>
        /// 学校详情 - 毕业去向
        /// </summary>
        [Description("学校详情-毕业去向")]
        [Route("/{controller}-{schoolNo}/{action}")]
        [Route("/{controller}/{action}/{Id}")]
        public async Task<IActionResult> ExtAchievement(GraduationDto graduationDto, string schoolNo = default)
        {
            //var httpMethod = HttpContext.Request.Method?.ToLower();
            //return httpMethod == "get" ? Graduation_get(graduationDto) : Graduation_post(graduationDto);

            SchExtDto0 schext;

            if (!string.IsNullOrWhiteSpace(schoolNo))
            {
                schext = await _schService.GetSchextSimpleInfoViaShortNo(schoolNo);
            }
            else
            {
                schext = await _schService.GetSchextSimpleInfo(graduationDto.Id);
            }
            if (schext == null) return new ExtNotFoundViewResult();
            if (string.IsNullOrWhiteSpace(schoolNo)) return RedirectPermanent($"/school-{schext.ShortSchoolNo}/extachievement/");

            graduationDto.Extid = schext.Eid;
            graduationDto.Sid = schext.Sid;

            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                //添加历史
                _historyService.AddHistory(user.UserId, schext.Eid, (byte)MessageDataType.School);
            }

            var query = Request.Query;


            var latitude = Convert.ToDouble(Request.GetLatitude());
            var longitude = Convert.ToDouble(Request.GetLongitude());
            var data = await _schoolService.GetSchoolExtDtoAsync(graduationDto.Extid, latitude, longitude);
            //如果学校分部为null的话跳转到错误页面
            if (data == null)
                return new ExtNotFoundViewResult();

            var year = graduationDto.Y ?? data.AchYear;
            var yearAchievement = year == null ? data.Achievement : _schoolService.GetYearAchievementList(schext.Eid, data.Grade, (int)year);

            //升学成绩内容
            object achData = null;
            if (year != null) achData = _schoolService.GetAchData(graduationDto.Extid, data.Grade, data.Type, year.Value);
            ViewBag.AchData = achData;

            //获取年份
            var localData = _schoolService.GetAchievementList(graduationDto.Extid, data.Grade);
            ViewBag.Years = localData.Where(_ =>
               {
                   if (data.Grade == (byte)(SchoolGrade.SeniorMiddleSchool) && data.Type == (byte)(PMS.OperationPlateform.Domain.Enums.SchoolType.International))
                   {
                       if (graduationDto.G == 1)
                           return _.Data == 2;
                       return _.Data == 1;
                   }
                   return true;
               })
                //.Where(_ => _.Message == year)
                .Select(_ => _.Key).Distinct().OrderByDescending(_ => _)
                .ToArray();

            //升学成绩-毕业去向 国际高中需要显示2个榜
            var bty = graduationDto.G;
            if (data.Grade == (byte)(SchoolGrade.SeniorMiddleSchool) && data.Type == (byte)(PMS.OperationPlateform.Domain.Enums.SchoolType.International))
            {
                bty = bty ?? 1;

                //国际
                do
                {
                    if (bty != 1) break;
                    if (year == null) break;

                    var allinternRanks = await _schService.GetCollegeRankList();

                    var iranks = (
                        from rank in allinternRanks
                        select (rank.RankName,
                            Sort: (rank.RankName == "QS" ? 1 : rank.RankName == "US News" ? 2 : rank.RankName == "Times" ? 3 : -1),
                            List: rank.List.Where(_ => _.Year == year).SelectMany(r =>
                            {
                                foreach (var a in r.Items)
                                {
                                    foreach (var da in yearAchievement)
                                    {
                                        if (a.SchoolId == da.Key)
                                            a.Count = (int)da.Message;
                                    }
                                }
                                return r.Items;
                            }).OrderBy(_ => _.Sort) as IEnumerable<PMS.School.Domain.Dtos.AchievementInfos.RankItems.RankItem>
                        )
                    ).Where(_ => _.Sort > 0)
                    .ToArray();

                    var iranks1 = iranks.Where(_ => _.Sort == 1).Select(_ => _.List).FirstOrDefault();
                    if (graduationDto.P == 1) iranks1 = iranks1.Where(_ => _.Count > 0);
                    if (!string.IsNullOrEmpty(graduationDto.SF))
                    {
                        int i = 0, j = 0;
                        foreach (var itm in iranks1)
                        {
                            if (itm.SchoolName.IndexOf(graduationDto.SF) > -1) { j = ++i; break; }
                            else i++;
                        }
                        iranks1 = j > 0 ? iranks1 : Enumerable.Empty<PMS.School.Domain.Dtos.AchievementInfos.RankItems.RankItem>();
                        if (j > 0) graduationDto.I1 = graduationDto.I1 ?? Convert.ToInt32(Math.Ceiling(j / 10d));
                        if (j > 0) ViewBag.rechangeUrl = true;
                    }
                    ViewBag.iranks1 = (page(iranks1, graduationDto.I1 ?? 1, 10).ToArray(), iranks1.Count());

                    var iranks2 = iranks.Where(_ => _.Sort == 2).Select(_ => _.List).FirstOrDefault();
                    if (graduationDto.P == 1) iranks2 = iranks2.Where(_ => _.Count > 0);
                    if (!string.IsNullOrEmpty(graduationDto.SF))
                    {
                        int i = 0, j = 0;
                        foreach (var itm in iranks2)
                        {
                            if (itm.SchoolName.IndexOf(graduationDto.SF) > -1) { j = ++i; break; }
                            else i++;
                        }
                        iranks2 = j > 0 ? iranks2 : Enumerable.Empty<PMS.School.Domain.Dtos.AchievementInfos.RankItems.RankItem>();
                        if (j > 0) graduationDto.I2 = graduationDto.I2 ?? Convert.ToInt32(Math.Ceiling(j / 10d));
                        if (j > 0) ViewBag.rechangeUrl = true;
                    }
                    ViewBag.iranks2 = (page(iranks2, graduationDto.I2 ?? 1, 10).ToArray(), iranks2.Count());

                    var iranks3 = iranks.Where(_ => _.Sort == 3).Select(_ => _.List).FirstOrDefault();
                    if (graduationDto.P == 1) iranks3 = iranks3.Where(_ => _.Count > 0);
                    if (!string.IsNullOrEmpty(graduationDto.SF))
                    {
                        int i = 0, j = 0;
                        foreach (var itm in iranks3)
                        {
                            if (itm.SchoolName.IndexOf(graduationDto.SF) > -1) { j = ++i; break; }
                            else i++;
                        }
                        iranks3 = j > 0 ? iranks3 : Enumerable.Empty<PMS.School.Domain.Dtos.AchievementInfos.RankItems.RankItem>();
                        if (j > 0) graduationDto.I3 = graduationDto.I3 ?? Convert.ToInt32(Math.Ceiling(j / 10d));
                        if (j > 0) ViewBag.rechangeUrl = true;
                    }
                    ViewBag.iranks3 = (page(iranks3, graduationDto.I3 ?? 1, 10).ToArray(), iranks3.Count());
                } while (false);

                //bty == 2
            }

            do //国内
            {
                if ((bty ?? 2) != 2) break;
                if (year == null) break;
                //var localCollges = await _schService.GetLocalColleges();
                var lranks = localData.Where(_ => _.Key == year.Value).Where(_ =>
                    {
                        if (data.Grade == (byte)(SchoolGrade.SeniorMiddleSchool) && data.Type == (byte)(PMS.OperationPlateform.Domain.Enums.SchoolType.International))
                            return _.Data == 1;
                        return true;
                    })
                    .Select(_ => (SchoolId: _.Other, SchoolName: _.Value, Count: _.Message));
                //避免注释代码造成未知错误，此处重新赋值  lranks ，
                //目的是保持毕业去向显示与更多毕业去向显示排序一致
                if (data?.Achievement != null && data?.Achievement.Count > 0 && year.Value == data?.Achievement.FirstOrDefault().Data)
                {
                    lranks = data?.Achievement
                        .Where(w => w.Data == year.Value)
                        .Select(s => (SchoolId: s.Key, SchoolName: s.Value, Count: s.Message));
                }

                if ((graduationDto.O2 ?? 1) == 1) lranks = lranks.OrderByDescending(o => o.Count);
                else if ((graduationDto.O2 ?? 1) == 2) lranks = lranks.OrderBy(_ => _.SchoolName);
                if (!string.IsNullOrEmpty(graduationDto.SF))
                {
                    int i = 0, j = 0;
                    foreach (var itm in lranks)
                    {
                        if (itm.SchoolName.IndexOf(graduationDto.SF) > -1) { j = ++i; break; }
                        else i++;
                    }
                    lranks = j > 0 ? lranks : Enumerable.Empty<(Guid, string, double)>();
                    if (j > 0) graduationDto.II1 = graduationDto.II1 ?? Convert.ToInt32(Math.Ceiling(j / 30d));
                    if (j > 0) ViewBag.rechangeUrl = true;
                }
                ViewBag.LRanks = (page(lranks, graduationDto.II1 ?? 1, 30).ToArray(), lranks.Count());
            } while (false);

            ViewBag.Year = year;
            ViewBag.GraduationDto = graduationDto;
            ViewBag.IsSignSchool = _lSRFSchoolDetail.CheckSchIsLeaving(graduationDto.Extid);
            await Get_common(graduationDto.Sid, graduationDto.Extid);
            ViewBag.SchoolNo = schoolNo;

            ViewBag.OtherHead = "<meta name=\"robots\" content=\"noindex,nofollow\">";

            return View();

            IEnumerable<T> page<T>(IEnumerable<T> list, int pindex, int psize)
            {
                return list.Skip((pindex - 1) * psize).Take(psize);
            }
        }

        /// <summary>
        /// 毕业去向 搜索框输入内容时模糊匹配
        /// </summary>
        /// <param name="g">1=国际; 2=国内</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Autosearch4Achievement(string txt, byte grade, Guid sid, Guid eid, int year, int g = 1, int count = 10)
        {
            if (g == 1)
            {
                var schs = await _schService.GetInternationalRankSchools(txt, year, count);
                return Json(ResponseResult.Success(schs));
            }
            else if (g == 2)
            {
                var data = await _schoolService.GetSchoolExtDtoAsync(eid, 0, 0);
                //如果学校分部为null的话跳转到错误页面
                if (data == null)
                {
                    var rr = ResponseResult.Failed();
                    rr.status = ResponseCode.NoSchool;
                    return Json(rr);
                }

                var localData = _schoolService.GetAchievementList(eid, grade);
                var schs = localData.Where(_ => _.Key == year).Where(_ =>
                    {
                        if (data.Grade == (byte)(SchoolGrade.SeniorMiddleSchool) && data.Type == (byte)(PMS.OperationPlateform.Domain.Enums.SchoolType.International))
                        {
                            if (g == 1)
                                return _.Data == 2;
                            return _.Data == 1;
                        }
                        return true;
                    })
                    .Select(_ => (SchoolId: _.Other, SchoolName: _.Value))
                    .Where(_ => _.SchoolName.IndexOf(txt) > -1)
                    .Take(10)
                    .ToArray();
                return Json(ResponseResult.Success(schs));
            }
            else
            {
                return Json(ResponseResult.Failed("不是国际和国内"));
            }
        }

        /// <summary>
        /// 学校详情 - 后台公共逻辑
        /// </summary>
        [NonAction]
        private async Task Get_common(Guid sid, Guid extId, string y = null)
        {
            //var latitude = Convert.ToDouble(Request.GetLatitude());
            //var longitude = Convert.ToDouble(Request.GetLongitude());

            var user = User.Identity.IsAuthenticated ? User.Identity?.GetUserInfo() : null;

            var userCitycode = Request.GetLocalCity();
            if (Request.Query["city"].ToString() is string city0 && !string.IsNullOrEmpty(city0))
            {
                userCitycode = Convert.ToInt32(city0);
                //Response.SetLocalCity(city0);
            }
            //获取城市名
            ViewBag.CityName = await _cityService.GetCityName(userCitycode);
            //热门城市
            ViewBag.HotCity = await _cityService.GetHotCity();

            var schext = await _schService.GetSchextSimpleInfo(extId);

            var grade = schext.Grade;
            var type = schext.Type;
            var schtype = new SchFType0(grade, type, schext.Discount, schext.Diglossia, schext.Chinese);

            var action = RouteData.Values["Action"]?.ToString();

            var actionName = "学校概况";
            switch (action?.ToLower())
            {
                case "data":
                    actionName = "总评及评价";
                    break;
                case "extmessage":
                    actionName = "相关信息";
                    break;
                case "extrecruit":
                    actionName = "招生简章";
                    break;
                case "comment":
                    actionName = "家长点评";
                    break;
                case "question":
                    actionName = "学校问答";
                    break;
                case "extatlas":
                    actionName = "学校图册";
                    break;
                case "extstrategy":
                    actionName = "学校攻略";
                    break;
            }

            ViewBag.SchFType = schtype;
            ViewBag.UserName = user?.Name;
            ViewBag.Sid = sid;
            ViewBag.ExtId = extId;
            ViewBag.SchoolName0 = schext.SchName;
            ViewBag.SchoolName = $"{schext.SchName}-{schext.ExtName}";
            ViewBag.SchoolCityName = await _cityService.GetCityName(schext.City);
            ViewBag.SchoolGrade = schext.Grade;
            ViewBag.ActionName = actionName;
            ViewBag.SchoolCityCode = schext.City;

            //<<<<<<<<该学部是否有被收藏 获取用户名<<<<<<<<<
            {
                //var collectStatat = false;
                ////var userName = "";
                //if (User.Identity.IsAuthenticated)
                //{
                //    //userName = user.Name + "，";
                //    var config = _configuration.GetSection("SchoolCollection");
                //    var ip = config.GetSection("ServerUrl").Value;
                //    var port = string.IsNullOrEmpty(config.GetSection
                //        ("Port").Value) ? "" : ":" + config.GetSection("Port").Value;
                //    var request = new HttpRequestMessage(HttpMethod.Get, $"{ip}{port}?userId={user.UserId}&&dataId={extId}");
                //    var client = _clientFactory.CreateClient();
                //    try
                //    {
                //        var response = await client.SendAsync(request);
                //        if (response.StatusCode == HttpStatusCode.OK)
                //        {
                //            var responseResult = await response.Content.ReadAsStringAsync();
                //            var dir = JsonHelper.DataRowFromJSON(responseResult);
                //            if ((Int64)dir["status"] == 0)
                //            {
                //                if ((bool)dir["iscollected"] == true)
                //                {
                //                    collectStatat = true;
                //                }
                //            }
                //        }
                //    }
                //    catch (Exception)
                //    {
                //        collectStatat = false;
                //    }

                //}
                //ViewBag.CollectStatus = collectStatat;
            }

            //ua
            ViewBag.UA = UserAgentHelper.Check(Request);
            //ViewBag.Title = "";
            //ViewBag.Page_Description = "";
            //ViewBag.Page_keywords = "";
        }

        public async Task<(List<SchoolExtScoreIndex> index, List<SchoolExtScore> score)> GetSchoolExtScoreAsync(Guid extId)
        {
            var index = await _schoolService.schoolExtScoreIndexsAsync();
            if (index == null || index.Count == 0)
                return (null, null);
            else
            {
                var score = await _schoolService.GetSchoolExtScoreAsync(extId);
                return (index, score);
            }
        }
    }
}