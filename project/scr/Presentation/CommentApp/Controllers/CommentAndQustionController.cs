using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities.AdCode;
using PMS.School.Domain.Common;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Infrastructure.Toolibrary;
using ProductManagement.Tool.Amap;
using ProductManagement.Tool.Amap.Result;
using Sxb.Web.Models;
using Sxb.Web.Models.Answer;
using Sxb.Web.Models.Comment;
using Sxb.Web.Models.Question;
using Sxb.Web.Response;
using Sxb.Web.ViewModels;
using Newtonsoft.Json;
using PMS.Infrastructure.Application.IService;
using System.Text;
using ProductManagement.Framework.Foundation;
using ProductManagement.API.Http;
using PMS.CommentsManage.Domain.Entities.Total;
using Sxb.Web.Utils;
using PMS.Infrastructure.Application.ModelDto;
using PMS.UserManage.Application.IServices;
using Sxb.Web.Models.User;
using Sxb.Web.Common;
using PMS.UserManage.Domain.Common;
using PMS.School.Application.IServices;
using System.Text.Encodings;
using System.Web;
using System.ComponentModel;
using PMS.UserManage.Application.ModelDto;

namespace Sxb.Web.Controllers
{
    public class CommentAndQustionController : BaseController
    {
        readonly ISchoolCommentService _commentService;
        readonly IQuestionInfoService _question;
        readonly IQuestionsAnswersInfoService _questionsAnswers;
        readonly ICityInfoService _cityCodeService;

        readonly ISchoolInfoService _schoolInfoService;
        readonly ISchoolService _schoolService;
        private readonly IUserService _userService;

        private readonly ImageSetting _setting;
        private readonly IMapper _mapper;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly ISearchClient _searchClient;

        private readonly IGiveLikeService _giveLikeService;

        public CommentAndQustionController(IOptions<ImageSetting> set,
            ICityInfoService cityService, ISchoolService schoolService,
            ISearchClient searchClient,
            ISchoolCommentService comment, IUserService userService,
            IQuestionInfoService question, IEasyRedisClient easyRedisClient,
            IQuestionsAnswersInfoService questionsAnswers, ISchoolInfoService schoolInfoService,
            IGiveLikeService giveLikeService,
            IMapper mapper)
        {
            _cityCodeService = cityService;
            _searchClient = searchClient;
            _commentService = comment;
            _question = question;
            _questionsAnswers = questionsAnswers;
            _schoolInfoService = schoolInfoService;
            _userService = userService;
            _schoolService = schoolService;
            _setting = set.Value;
            _mapper = mapper;
            _easyRedisClient = easyRedisClient;
            _giveLikeService = giveLikeService;
        }

        [Description("点评和问答页")]
        [Route("/{controller}")]
        [Route("/question")]
        [Route("/comment")]
        public async Task<IActionResult> Index(int localCityCode = 0, int type = 0)
        {
            try
            {
                if (localCityCode == 0)
                {
                    //获取当前用户所在城市
                    localCityCode = Request.GetLocalCity();
                }
                else
                {
                    //切换城市
                    Response.SetLocalCity(localCityCode.ToString());
                }

                //GetCodstring("广州");
                var adCodes = await _cityCodeService.IntiAdCodes();

                //将数据保存在前台
                ViewBag.LocalCity = adCodes.SelectMany(p => p.CityCodes).FirstOrDefault(p => p.Id == localCityCode);

                Guid userId = Guid.Empty;
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    userId = user.UserId;
                }

                List<AreaDto> history = new List<AreaDto>();
                if (userId != default(Guid))
                {
                    string Key = $"SearchCity:{userId}";
                    var historyCity = await _easyRedisClient.SortedSetRangeByRankAsync<AreaDto>(Key, 0, 10, StackExchange.Redis.Order.Descending);
                    history.AddRange(historyCity.ToList());
                }

                ViewBag.HotCity = JsonConvert.SerializeObject(await _cityCodeService.GetHotCity());

                ViewBag.History = JsonConvert.SerializeObject(history);
                //城市编码
                ViewBag.AllCity = JsonConvert.SerializeObject(adCodes);

                //检测当前用户身份 （true：校方用户 |false：普通用户）
                ViewBag.isSchoolRole = false;
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    //检测当前用户身份 （true：校方用户 |false：普通用户）
                    ViewBag.isSchoolRole = user.Role.Contains((int)UserRole.School);
                }

                ViewBag.Selected = type;
            }
            catch (Exception)
            {

                throw;
            }
            return View();
        }

        /// <summary>
        /// 点评列表
        /// </summary>
        /// <returns></returns>
        [Description("点评列表页")]
        public IActionResult CommentList()
        {
            return View();
        }

        /// <summary>
        /// 问题列表
        /// </summary>
        /// <returns></returns>
        [Description("问题列表页")]
        public IActionResult QuestionList()
        {
            return View();
        }

        /// <summary>
        /// 点评列表
        /// </summary>
        /// <param name="grade"></param>
        /// <param name="type"></param>
        /// <param name="isLodging">0：不住校，1：住校，2：未带该条件</param>
        /// <param name="order"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="schoolArea"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("点评列表")]
        public async Task<ResponseResult> SchoolComments(int CityCode, List<SchoolGrade> grade, List<SchoolType> type, List<int> isLodging, SelectedCommentOrder order, int PageIndex, int PageSize, List<int> schoolArea)
        {
            try
            {
                Guid userId = Guid.Empty;
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    userId = user.UserId;
                }

                string key = $"CommentInfos:{DesTool.Md5($"CityCode_{CityCode}:Grade_{string.Join("", grade)}:Type_{string.Join("", type)}:IsLodging_{string.Join("", isLodging)}:Order_{(int)order}:PageIndex_{PageIndex}:SchoolArea_{string.Join("", schoolArea)}")}";
                //var SchoolInfos = await _easyRedisClient.GetOrAddAsync(key, () =>
                //{
                //    //根据查询条件获取所有匹配到的学校分部id
                //    return _schoolService.AllSchoolSelected(CityCode, grade, type, isLodging, PageIndex, PageSize, (QuestionAndCommentOrder)(int)order, schoolArea);
                //}, new TimeSpan(0, 5, 0));

                //var SchoolInfos = await _easyRedisClient.GetOrAddAsync(key, () =>
                //{
                //    //根据查询条件获取所有匹配到的学校分部id
                //    return _schoolService.AllSchoolSelected(CityCode, grade, type, isLodging, PageIndex, PageSize, (QuestionAndCommentOrder)(int)order, schoolArea, true);
                //}, new TimeSpan(0, 5, 0));

                var SchoolInfos = _schoolService.AllSchoolSelected(CityCode, grade, type, isLodging, PageIndex, PageSize, (QuestionAndCommentOrder)(int)order, schoolArea, true);
                if (SchoolInfos == null)
                {
                    return ResponseResult.Success(new List<CommentList>());
                }

                key = $"CommentInfos_Comments:{DesTool.Md5($"CityCode_{CityCode}:Grade_{string.Join("", grade)}:Type_{string.Join("", type)}:IsLodging_{string.Join("", isLodging)}:Order_{(int)order}:PageIndex_{PageIndex}:SchoolArea_{string.Join("", schoolArea)}")}";
                //var comments = await _easyRedisClient.GetOrAddAsync(key, () =>
                //{
                //    return _commentService.AllSchoolSelectedComment(userId, SchoolInfos.Select(x => x.SchoolSectionId)?.ToList(), (int)order);
                //}, new TimeSpan(0, 5, 0));
                var comments = _commentService.AllSchoolSelectedComment(userId, SchoolInfos.Select(x => x.SchoolSectionId)?.ToList(), (int)order);

                if (comments.Count() == 0)
                {
                    return ResponseResult.Success(new
                    {
                        rows = new List<CommentList>()
                    });
                }

                var UserIds = new List<Guid>();
                comments.Where(q => q.CommentReplies != null).ToList().ForEach(p => { UserIds.AddRange(p.CommentReplies.Where(q => q.ParentUserId != null).Select(q => (Guid)q.ParentUserId)); });
                comments.Where(q => q.CommentReplies != null).ToList().ForEach(p => { UserIds.AddRange(p.CommentReplies.Select(q => q.ReplayUserId)); });
                UserIds.AddRange(comments.GroupBy(q => q.UserId).Select(p => p.Key).ToList());

                string userKey = $"UserInfos:{ DesTool.Md5(string.Join("_", UserIds))}";
                var Users = await _easyRedisClient.GetOrAddAsync(userKey, () =>
                {
                    return _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());
                }, new TimeSpan(0, 5, 0));
                var commentList = _mapper.Map<List<SchoolCommentDto>, List<CommentList>>(comments);
                if (commentList.Any())
                {
                    var like = _giveLikeService.CheckLike(commentList.Select(x => x.Id).ToList(), userId);
                    var likeTotal = _commentService.GetCommentLikeTotal(commentList.Select(x => x.Id).ToList());

                    List<Guid> schoolExtIds = comments.GroupBy(q => q.SchoolSectionId).Select(x => x.Key)?.ToList();

                    //string schoolKey = $"SchoolInfos:{ DesTool.Md5(string.Join("_", schoolExtIds))}";
                    //var SchoolInfos = await _easyRedisClient.GetOrAddAsync(schoolKey, () =>
                    //{
                    // _schoolInfoService.GetSchoolSectionByIds(schoolExtIds);
                    //}, new TimeSpan(0, 5, 0));

                    foreach (var x in commentList)
                    {
                        if (x.Images.Any())
                        {
                            var images = x.Images;
                            for (int i = 0; i < images.Count; i++)
                            {
                                images[i] = _setting.QueryImager + images[i];
                            }
                            x.Images = images;
                        }
                        var user = Users.FirstOrDefault(p => p.Id == x.UserId);
                        x.UserInfo = _mapper.Map<UserInfoVo>(Users.FirstOrDefault(q => q.Id == x.UserId)).ToAnonyUserName(x.IsAnony); 
                        var schoolInfo = SchoolInfos.Where(s => s.SchoolSectionId == x.SchoolSectionId).FirstOrDefault();
                        x.School = _mapper.Map<SchoolInfoDto, CommentList.SchoolInfoVo>(schoolInfo);
                        if (like.Contains(x.Id))
                        {
                            x.IsLike = true;
                        }
                        else
                        {
                            x.IsLike = false;
                        }
                        var comment = likeTotal.Where(l => l.Id == x.Id).FirstOrDefault();
                        x.LikeCount = comment == null ? 0 : comment.LikeCount;
                        x.ReplyCount = comment == null ? 0 : comment.ReplyCount;

                        x.No = x.No.ToLower();
                    }
                }
                if ((int)order == -1)
                {
                    commentList = commentList.OrderByDescending(o => o.School.CommentTotal).ToList();
                }
                else if ((int)order == 1)
                {
                    commentList = commentList.OrderByDescending(o => o.CreateTime).ToList();
                }
                else if ((int)order == 3)
                {
                    commentList = commentList.OrderByDescending(o => o.School.CommentTotal).ToList();
                }
                return ResponseResult.Success(new
                {
                    rows = commentList
                });
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 问题列表
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="grade"></param>
        /// <param name="type"></param>
        /// <param name="isLodging"></param>
        /// <param name="order"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="schoolArea"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("问题列表")]
        public async Task<ResponseResult> SchoolQustions(int CityCode, List<SchoolGrade> grade, List<SchoolType> type, List<int> isLodging, SelectedQuestionOrder order, int PageIndex, int PageSize, List<int> schoolArea = null)
        {
            try
            {
                Guid userId = Guid.Empty;
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    userId = user.UserId;
                }

                string key = $"QuestionInfos:{DesTool.Md5($"CityCode_{CityCode}:Grade_{string.Join("", grade)}:Type_{string.Join("", type)}:IsLodging_{string.Join("", isLodging)}:Order_{(int)order}:PageIndex_{PageIndex}:SchoolArea_{string.Join("", schoolArea)}")}";


                //var SchoolInfos = await _easyRedisClient.GetOrAddAsync(key, () =>
                //{
                //    //根据查询条件获取所有匹配到的学校分部id
                //    return _schoolService.AllSchoolSelected(CityCode, grade, type, isLodging, PageIndex, PageSize, (QuestionAndCommentOrder)(int)order, schoolArea, false)?.Select(x => x.SchoolSectionId).ToList();
                //}, new TimeSpan(0, 5, 0));
                var SchoolInfos = _schoolService.AllSchoolSelected(CityCode, grade, type, isLodging, PageIndex, PageSize, (QuestionAndCommentOrder)(int)order, schoolArea, false)?.ToList();

                if (SchoolInfos == null)
                {
                    return ResponseResult.Success();
                }
                //var qeustions = await _easyRedisClient.GetOrAddAsync(key, async () =>
                //{

                //    //根据查询条件获取所有匹配到的学校分部id
                //    List<Guid> schoolBranchIds = _schoolService.AllSchoolSelected(CityCode, grade, type, isLodging, PageIndex, PageSize, (QuestionAndCommentOrder)(int)order, schoolArea,false)?.Select(x=>x.SchoolSectionId).ToList();
                //    if (schoolBranchIds == null)
                //    {
                //        return null;
                //    }
                //    return await _question.AllSchoolSelectedQuestion(userId, schoolBranchIds);
                //}, new TimeSpan(1, 0, 0));

                key = $"QuestionInfos_Questions:{DesTool.Md5($"CityCode_{CityCode}:Grade_{string.Join("", grade)}:Type_{string.Join("", type)}:IsLodging_{string.Join("", isLodging)}:Order_{(int)order}:PageIndex_{PageIndex}:SchoolArea_{string.Join("", schoolArea)}")}";
                //var qeustions = await _easyRedisClient.GetOrAddAsync(key, () =>
                //{
                //    return _question.AllSchoolSelectedQuestion(userId, SchoolInfos.Select(x=>x.SchoolSectionId).ToList(), order);
                //}, new TimeSpan(0, 5, 0));
                var qeustions = await _question.AllSchoolSelectedQuestion(userId, SchoolInfos.Select(x => x.SchoolSectionId).ToList(), order);

                if (qeustions.Count == 0)
                {
                    return ResponseResult.Success(new
                    {
                        rows = new List<QuestionVo>()
                    });
                }

                var UserIds = new List<Guid>();
                qeustions.Where(q => q.answer != null).ToList().ForEach(p => { UserIds.AddRange(p.answer.Select(q => q.UserId)); });
                UserIds.AddRange(qeustions.GroupBy(q => q.UserId).Select(p => p.Key).ToList());


                string userKey = $"UserInfos:{ DesTool.Md5(string.Join("_", UserIds))}";
                var Users = await _easyRedisClient.GetOrAddAsync(userKey, () =>
                {
                    return _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());
                }, new TimeSpan(0, 5, 0));


                var questionLists = _mapper.Map<List<QuestionDto>, List<QuestionVo>>(qeustions);
                if (questionLists.Any())
                {
                    var like = _giveLikeService.CheckLike(questionLists.Select(x => x.Answer.Select(y => y.Id)).SelectMany(x => x).ToList(), userId);

                    //var schoolIds = qeustions.GroupBy(q => q.SchoolId).Select(q => q.Key).ToList();
                    //var extIds = qeustions.GroupBy(q => q.SchoolSectionId).Select(q => q.Key).ToList();

                    //string schoolKey = $"SchoolInfos:{ DesTool.Md5(string.Join("_", extIds))}";
                    //string schoolQaTotalKey = $"SchoolQaTotalInfos:{ DesTool.Md5(string.Join("_", schoolIds))}";

                    //var schoolInfos = await _easyRedisClient.GetOrAddAsync(schoolKey, () =>
                    //{
                    //    return _schoolInfoService.GetSchoolSectionQaByIds(extIds);
                    //}, new TimeSpan(0, 5, 0));

                    //var schoolQuestionTotal = await _easyRedisClient.GetOrAddAsync(schoolQaTotalKey, () =>
                    //{
                    //    return _question.SchoolTotalQuestion(schoolIds);
                    //}, new TimeSpan(0, 5, 0));
                    questionLists.ForEach(x =>
                    {
                        var schoolInfo = SchoolInfos.Where(q => q.SchoolSectionId == x.SchoolSectionId).FirstOrDefault();
                        x.School = new QuestionVo.SchoolInfo()
                        {
                            SchoolBranch = schoolInfo.SchoolBranch,
                            SchoolName = schoolInfo.SchoolName,
                            SchoolQuestionTotal = schoolInfo.SectionQuestionTotal,
                            IsAuth = schoolInfo.IsAuth,
                            IsInternactioner = schoolInfo.SchoolType == SchoolType.International,
                            LodgingType = (int)schoolInfo.LodgingType,
                            LodgingReason = schoolInfo.LodgingType.Description(),
                            SchoolType = (int)schoolInfo.SchoolType,
                            SchoolNo = schoolInfo.SchoolNo
                        };
                        //x.School = _mapper.Map<SchoolInfoQaDto, QuestionVo.SchoolInfo>(schoolInfo);
                        //if (x.School != null)
                        //{
                        //    x.School.SchoolQuestionTotal = schoolQuestionTotal.FirstOrDefault(q => q.Id == x.SchoolId)?.Total ?? 0;
                        //}
                        x.UserInfo = _mapper.Map<UserInfoVo>(Users.FirstOrDefault(q => q.Id == x.UserId));

                        if (x.Images.Any())
                        {
                            var images = x.Images;
                            for (int i = 0; i < images.Count; i++)
                            {
                                images[i] = _setting.QueryImager + images[i];
                            }
                            x.Images = images;
                        }
                        if (x.Answer != null)
                        {
                            x.Answer.ForEach(answer =>
                            {
                                answer.UserInfo = _mapper.Map<UserInfoVo>(Users.FirstOrDefault(q => q.Id == answer.UserId)).ToAnonyUserName(answer.IsAnony);
                                if (like.Contains(answer.Id))
                                {
                                    answer.IsLike = true;
                                }
                                else
                                {
                                    answer.IsLike = false;
                                }
                            });
                        }
                        x.No = x.No.ToLower();
                    });
                }

                return ResponseResult.Success(new
                {
                    rows = questionLists
                });
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        [HttpGet]
        [Description("获取问题回答")]
        public ResponseResult GetQuestionAnswer(Guid QuestionId)
        {
            try
            {
                Guid userId = Guid.Empty;
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    userId = user.UserId;
                }
                var rez = _question.GetQuestionById(QuestionId, userId);
                var answerVo = _mapper.Map<AnswerInfoDto, AnswerInfoVo>(rez.answer.FirstOrDefault());
                answerVo.UserInfo = _mapper.Map<UserInfoVo>(_userService.GetUserInfo(rez.UserId));
                return ResponseResult.Success(answerVo);
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }


        /// <summary>
        /// 模糊查询学校名称
        /// </summary>
        /// <param name="schoolName"></param>
        /// <param name="isComment">true：查询点评数据、false：</param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [Description("模糊查询学校名称")]
        public async Task<ResponseResult> GetInfoBySchoolName(string schoolName, int citycode, bool isComment, int PageIndex, int PageSize)
        {
            try
            {
                List<SchoolSectionCommentOrQuestionTotal> totals = new List<SchoolSectionCommentOrQuestionTotal>();

                Guid userId = Guid.Empty;
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    userId = user.UserId;
                }

                //根据学校名称模糊得到批量学校id
                List<Guid> SchoolId = new List<Guid>();
                var result = await _searchClient.Search(new ProductManagement.API.Http.Result.GetSchoolByNameAndAdCode() { name = schoolName, curpage = PageIndex, countperpage = PageSize, citycode = citycode });

                List<SchoolNameQuery> rez = new List<SchoolNameQuery>();
                int total = 0;
                if (result.items != null)
                {

                    SchoolId.AddRange(result.items.Select(x => x.id));

                    if (isComment)
                    {
                        totals.AddRange(_commentService.GetTotalBySchoolSectionIds(SchoolId));
                    }
                    else
                    {
                        totals.AddRange(_question.GetTotalBySchoolSectionIds(SchoolId));
                    }

                    if (result.items.Any())
                    {
                        total = result.pageinfo.totalrows;
                        foreach (var item in result.items)
                        {
                            SchoolNameQuery school = new SchoolNameQuery();
                            school.SchoolId = item.eid;
                            school.School = item.id;
                            school.SchoolName = item.name;
                            var temp = totals.FirstOrDefault(x => x.SchoolSectionId == item.id);
                            school.CurrentTotal = temp == null ? 0 : temp.Total;
                            rez.Add(school);
                        }
                    }
                }

                return ResponseResult.Success(new
                {
                    total = total,
                    rows = rez
                });
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

    }
}