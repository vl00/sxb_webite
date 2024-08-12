using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Infrastructure.Toolibrary;
using ProductManagement.Web.Areas.PartTimeJob.Models.ViewEntity;
using System.Text.RegularExpressions;
using ProductManagement.Web.Areas.PartTimeJob.Models;
using PMS.CommentsManage.Domain.Entities.ProcViewEntities;
using PMS.CommentsManage.Application.IServices.IProcViewService;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.Common;
using ProductManagement.Framework.Foundation;
using Microsoft.Extensions.Options;
using ProductManagement.Web.Areas.PartTimeJob.Models.Common;
using PMS.CommentsManage.Application.Common;
using PMS.School.Application.IServices;
using PMS.UserManage.Application.IServices;
using PMS.CommentsManage.Domain.Entities.Total;
using PMS.CommentsManage.Application.ModelDto;

namespace ProductManagement.Web.Areas.PartTimeJob.Controllers
{
    /// <summary>
    /// 统计
    /// </summary>
    [Area("PartTimeJob")]
    public class StatisticsController : BaseController
    {
        IProcParAdminService _parAdminService;
        IProcFindAllJobEntityService _AllJobEntityService;
        IPartTimeJobAdminService _partTimeJobAdmin;
        //问答详情
        IQuestionsAnswersInfoService _questionsAnswersInfo;
        //问答审核
        IQuestionsAnswerExamineService _questionsAnswerExamine;
        //点评审核
        ISchoolCommentExaminerService _schoolCommentExaminer;
        //点评详情
        ISchoolCommentService _schoolComment;
        //点评标签关联
        ISchoolTagService _schoolTagService;
        //点评图片
        ISchoolImageService _schoolImage;
        //配置
        private readonly Setting _setting;
        //学校service
        ISchoolService _schoolService;

        ISchoolInfoService _schoolInfoService;

        //用户角色
        private IPartTimeJobAdminRolereService _adminRolereService;

        //统计
        private IViewExaminerTotalService _viewExaminerTotal;

        IUserService _userService;

        public StatisticsController(IOptions<Setting> set, IUserService userService, ISchoolService schoolService, ISchoolTagService schoolTagService,
            ISchoolImageService schoolImage,IProcParAdminService parAdminService, IProcFindAllJobEntityService procFindAllJobEntity, 
            IPartTimeJobAdminService partTimeJobAdmin, IQuestionsAnswersInfoService questionsAnswersInfo, IQuestionsAnswerExamineService questionsAnswerExamine,
            ISchoolCommentExaminerService schoolCommentExaminer, ISchoolCommentService schoolComment, ISchoolInfoService schoolInfoService,
            IPartTimeJobAdminRolereService adminRolereService, IViewExaminerTotalService viewExaminerTotal)
        {
            _userService = userService;
            _schoolService = schoolService;
            _parAdminService = parAdminService;
            _AllJobEntityService = procFindAllJobEntity;
            _partTimeJobAdmin = partTimeJobAdmin;
            _questionsAnswersInfo = questionsAnswersInfo;
            _questionsAnswerExamine = questionsAnswerExamine;
            _schoolCommentExaminer = schoolCommentExaminer;
            _schoolComment = schoolComment;
            _schoolImage = schoolImage;
            _schoolTagService = schoolTagService;
            _setting = set.Value;
            _schoolInfoService = schoolInfoService;

            _adminRolereService = adminRolereService;
            _viewExaminerTotal = viewExaminerTotal;
        }   

        /// <summary>
        /// 管理员、供应商、兼职领队 统计页面
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewBag.Code = _admin.InvitationCode;
            List<SupplierTotalDto> supplier = new List<SupplierTotalDto>();
            if (int.Parse(_admin.Role) == 3)
            {
                //上月统计
                //DateTime UpBeginTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1, 0, 0, 0);
                //DateTime UptEndTime = new DateTime(UpBeginTime.Year, UpBeginTime.Month, UpBeginTime.AddMonths(+1).AddDays(-1).Day, 23, 59, 59, 59);
                //supplier.Add(_viewExaminerTotal.GetSupplierTotal(_admin.Id, UpBeginTime, UptEndTime));

                //本月统计
                DateTime CurrentBeginTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                DateTime CurrentEndTime = new DateTime(CurrentBeginTime.Year, CurrentBeginTime.Month, CurrentBeginTime.AddMonths(+1).AddDays(-1).Day, 23, 59, 59, 59);
                ViewBag.supplier = _viewExaminerTotal.GetSupplierTotal(_admin.Id, CurrentBeginTime, CurrentEndTime);
            }
            return View();
        }

        /// <summary>
        /// 获取该供应商下统计数据
        /// </summary>
        /// <param name="ParentId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public ModelResult<SupplierTotalDto> GetSupplierTotalDtoBySupplier(Guid ParentId,DateTime date) 
        {
            DateTime beginTime = new DateTime();
            DateTime endTime = new DateTime();
            if (date == null || date == default(DateTime))
            {
                //本月统计
                beginTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                endTime = new DateTime(beginTime.Year, beginTime.Month, beginTime.AddMonths(+1).AddDays(-1).Day, 23, 59, 59, 59);
            }
            else
            {
                beginTime = new DateTime(date.Year, date.Month, 1, 0, 0, 0);
                endTime = new DateTime(beginTime.Year, beginTime.Month, beginTime.AddMonths(+1).AddDays(-1).Day, 23, 59, 59, 59);
            }

            var data = _viewExaminerTotal.GetSupplierTotal(ParentId, beginTime, endTime);
            return new ModelResult<SupplierTotalDto>()
            {
                Data = data
            };
        }

        /// <summary>
        /// 根据父级类型获取该父级下所有子集元素
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="subset"></param>
        /// <returns></returns>
        public PageResult<List<ProcGetAdminByRoleTypeEntityVo>> GetEntityListByRoleType(string parentId, int type, int limit, int page,DateTime date)
        {
            DateTime beginTime = new DateTime();
            DateTime endTime = new DateTime();
            if (date == null || date == default(DateTime))
            {
                //本月统计
                beginTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                endTime = new DateTime(beginTime.Year, beginTime.Month, beginTime.AddMonths(+1).AddDays(-1).Day, 23, 59, 59, 59);
            }
            else
            {
                beginTime = new DateTime(date.Year, date.Month, 1, 0, 0, 0);
                endTime = new DateTime(beginTime.Year, beginTime.Month, beginTime.AddMonths(+1).AddDays(-1).Day, 23, 59, 59, 59);
            }

            var data = Mapper.Map<List<ProcGetAdminByRoleTypeEntity>, List<ProcGetAdminByRoleTypeEntityVo>>(_parAdminService.ExecGetAdminByRoleType(Guid.Parse(parentId), type, page, limit, beginTime, endTime, out int TotalNumber));
            
            if (int.Parse(_admin.Role) != 5)
            {
                if (data.Any()) 
                {
                    data.ForEach(x => x.Phone = Regex.Replace(x.Phone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2"));
                }
            }
            return new PageResult<List<ProcGetAdminByRoleTypeEntityVo>>()
            {
                rows = data,
                total = TotalNumber
            };
        }

        /// <summary>
        /// 根据兼职领队id获取兼职者的填写数据总条数
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public ModelResult<SupplierTotalDto> GetFindAllJobEntityService(string parentId,string phone,DateTime date)
        {
                if (phone != null && phone != "")
                {
                    if (phone.Length >= 11)
                    {
                        phone = phone.Replace(phone.Trim().Substring(3, 4), "____");
                    }
                }

                //var data = _AllJobEntityService.FindAllJobEntityByLeaderId(Guid.Parse(parentId), 1, 1, phone, out ProcFindAllJobEntityTotal procFindAllJobEntityTotal);
                SupplierTotalDto leaderTotal = new SupplierTotalDto();

                DateTime beginTime = new DateTime();
                DateTime endTime = new DateTime();
                if (date == null || date == default(DateTime))
                {
                    //本月统计
                    beginTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                    endTime = new DateTime(beginTime.Year, beginTime.Month, beginTime.AddMonths(+1).AddDays(-1).Day, 23, 59, 59, 59);
                }
                else 
                {
                    beginTime = new DateTime(date.Year, date.Month, 1, 0, 0, 0);
                    endTime = new DateTime(beginTime.Year, beginTime.Month, beginTime.AddMonths(+1).AddDays(-1).Day, 23, 59, 59, 59);
                }
            leaderTotal = _viewExaminerTotal.GetSupplierTotal(Guid.Parse(parentId), beginTime, endTime);

            return new ModelResult<SupplierTotalDto>()
            {
                Data = leaderTotal
            };
        }

        public ModelResult<SysAdminQuerySupplierTotalDto> GetSysAdminQuerySupplier(DateTime date) 
        {
            DateTime beginTime = new DateTime();
            DateTime endTime = new DateTime();
            if (date == null || date == default(DateTime))
            {
                //本月统计
                beginTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                endTime = new DateTime(beginTime.Year, beginTime.Month, beginTime.AddMonths(+1).AddDays(-1).Day, 23, 59, 59, 59);
            }
            else
            {
                beginTime = new DateTime(date.Year, date.Month, 1, 0, 0, 0);
                endTime = new DateTime(beginTime.Year, beginTime.Month, beginTime.AddMonths(+1).AddDays(-1).Day, 23, 59, 59, 59);
            }

            var data = _viewExaminerTotal.SysAdminQuerySupplierTotal(beginTime,endTime);
            return new ModelResult<SysAdminQuerySupplierTotalDto>()
            {
                Data = data
            };
        }



        /// <summary>
        /// 根据兼职领队id获取兼职者的填写数据总条数
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public ModelResult<SupplierTotalDto> GetFindAllJobTotal(string parentId, string phone, DateTime date)
        {
            if (phone != null && phone != "")
            {
                if (phone.Length >= 11)
                {
                    phone = phone.Replace(phone.Trim().Substring(3, 4), "____");
                }
            }


            //var data = _AllJobEntityService.FindAllJobEntityByLeaderId(Guid.Parse(parentId), 1, 1, phone, out ProcFindAllJobEntityTotal procFindAllJobEntityTotal);
            SupplierTotalDto leaderTotal = new SupplierTotalDto();

            DateTime beginTime = new DateTime();
            DateTime endTime = new DateTime();
            if (date == null || date == default(DateTime))
            {
                //本月统计
                beginTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                endTime = new DateTime(beginTime.Year, beginTime.Month, beginTime.AddMonths(+1).AddDays(-1).Day, 23, 59, 59, 59);
            }
            else
            {
                beginTime = new DateTime(date.Year, date.Month, 1, 0, 0, 0);
                endTime = new DateTime(beginTime.Year, beginTime.Month, beginTime.AddMonths(+1).AddDays(-1).Day, 23, 59, 59, 59);
            }
            leaderTotal = _viewExaminerTotal.GetLeaderCurrentMonthTotal(Guid.Parse(parentId), beginTime, endTime,phone);

            return new ModelResult<SupplierTotalDto>()
            {
                Data = leaderTotal
            };
        }

        /// <summary>
        /// 获取兼职人员实体集合
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public PageResult<List<ProcFindAllJobEntityList>> FindAllJobEntityList(string parentId, int limit, int page,DateTime date,string phone = "")
        {

            if(phone!=null && phone != "") 
            {
                if (phone.Length >= 11) 
                {
                    phone = phone.Replace(phone.Trim().Substring(3, 4), "____");
                }
            }

            DateTime beginTime = new DateTime();
            DateTime endTime = new DateTime();
            if (date == null || date == default(DateTime))
            {
                //本月统计
                beginTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                endTime = new DateTime(beginTime.Year, beginTime.Month, beginTime.AddMonths(+1).AddDays(-1).Day, 23, 59, 59, 59);
            }
            else
            {
                beginTime = new DateTime(date.Year, date.Month, 1, 0, 0, 0);
                endTime = new DateTime(beginTime.Year, beginTime.Month, beginTime.AddMonths(+1).AddDays(-1).Day, 23, 59, 59, 59);
            }

            var data = _AllJobEntityService.FindAllJobEntityByLeaderId(Guid.Parse(parentId), page, limit,phone,beginTime,endTime, out ProcFindAllJobEntityTotal procFindAllJobEntityTotal)?.ToList();


            if (!data.Any()) 
            {
                return new PageResult<List<ProcFindAllJobEntityList>>()
                {
                    rows = new List<ProcFindAllJobEntityList>(),
                    total = 0
                };
            }

            var JobAdminIds = data.Select(x => x.Id).ToList();
            var checkShield = _adminRolereService.GetList(x => JobAdminIds.Contains(x.AdminId) && x.Role == 1)?.Select(x => new PartTimeJobAdminRole() { AdminId = x.AdminId, Shield = x.Shield });

            if (int.Parse(_admin.Role) != 5)
            {
                data?.ForEach(x => {
                    x.Phone = Regex.Replace(x.Phone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                }) ;
            }

            if (checkShield != null) 
            {
                if (checkShield.Any()) 
                {
                    data?.ForEach(x => {
                        var admin = checkShield.Where(a => a.AdminId == x.Id).FirstOrDefault();
                        if (admin == null)
                        {
                            x.Shield = false;
                        }
                        else
                        {
                            x.Shield = admin.Shield;
                        }
                    });
                }
            }

            return new PageResult<List<ProcFindAllJobEntityList>>()
            {
                rows = data,
                total = procFindAllJobEntityTotal.JobTotal
            };
        }

        /// <summary>
        /// 获取兼职人员的点评与问答【审核人员】
        /// </summary>
        /// <param name="JobId">管理员id【兼职人员、审核人员】</param>
        /// <param name="isExaminerAdmin">1：兼职人员，2：审核人员</param>
        /// <returns></returns>
        public IActionResult GetCommentAndAnswerByJob(Guid JobId, int isExaminerAdmin)
        {
            ViewBag.JobId = JobId;
            ViewBag.isExaminerAdmin = isExaminerAdmin;

            return View();
        }

        /// <summary>
        /// 获取该兼职人员写的点评【审核人员】
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="type">1：兼职人员，2：审核人员</param>
        /// <returns></returns>
        public PageResult<List<CommentVo>> GetAllCommentByJob(Guid Id, int type, string school, int limit, int page)
        {
            int Total = 0;
            List<CommentVo> comment;
            List<SchoolComment> AllComment;
            var admin = _partTimeJobAdmin.GetModelById(Id);

            if (type == 1)
            {
                AllComment = _schoolComment.GetSchoolCommentByAdminId(admin.Id, 0, 0, out Total);
            }
            else
            {
                var SchoolCommentExamines = _schoolCommentExaminer.GetSchoolCommentByAdminId(admin.Id, 0, 0,new List<Guid>(), out Total);
                AllComment = (from item in SchoolCommentExamines select item.SchoolComment).ToList();
            }

            if (school == null)
            {
                if (AllComment != null)
                {
                    Total = AllComment.Count();
                    AllComment = AllComment.Skip((page - 1) * limit).Take(limit).ToList();
                    comment = ChangeCommentVo(AllComment);
                }
                else
                {
                    Total = 0;
                    AllComment = null;
                    comment = null;
                }
            }
            else
            {
                if (AllComment != null)
                {
                    comment = ChangeCommentVo(AllComment);
                    var searchData = (from item in comment where item.SchoolName.Contains(school) select item).ToList();
                    comment = searchData.Skip((page - 1) * limit).Take(limit).ToList();
                    Total = searchData.Count();
                }
                else
                {
                    comment = null;
                    Total = 0;
                }
            }
            

            return new PageResult<List<CommentVo>>()
            {
                rows = comment,
                total = Total
            };
        }

        /// <summary>
        /// 点评实体转换
        /// </summary>
        /// <param name="AllComment"></param>
        /// <returns></returns>
        public List<CommentVo> ChangeCommentVo(List<SchoolComment> AllComment)
        {
            List<CommentVo> comment = new List<CommentVo>();

            if (!AllComment.Any()) 
            {
                return comment;
            }

            var school = _schoolInfoService.GetSchoolName(AllComment.Select(x => x.SchoolSectionId).ToList());
             var examinerAdmin = _partTimeJobAdmin.GetJobAdminNameByIds(AllComment.Where(x=>x.SchoolCommentExamine != null).Select(x => x.SchoolCommentExamine.AdminId).ToList());

            foreach (SchoolComment item in AllComment)
            {
                CommentVo commentVo = new CommentVo();
                commentVo.Id = item.Id;
                commentVo.Content = item.Content;

                commentVo.SchoolName = school.Where(x=>x.SchoolSectionId == item.SchoolSectionId).FirstOrDefault().SchoolName;

                commentVo.AddTime = item.AddTime.ToString("yyyy-MM-dd");
                //检测该点评是否被审核
                if (item.SchoolCommentExamine != null)
                {
                    //commentVo.ExamineAdmin = _partTimeJobAdmin.GetModelById(item.SchoolCommentExamine.AdminId).Name;
                    commentVo.ExamineAdmin = examinerAdmin.Where(x => item.SchoolCommentExamine.AdminId == x.Id).FirstOrDefault().Name;
                }
                commentVo.ExamineStatus = item.State.Description();
                comment.Add(commentVo);
            }
            return comment;
        }

        /// <summary>
        /// 获取该兼职人员所有问答【审核人员】
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public PageResult<List<AnswerVo>> GetAllAnswerByJob(Guid Id, int type, string school, int limit, int page)
        {
            int total = 0;
            var admin = _partTimeJobAdmin.GetModelById(Id);
            List<QuestionsAnswersInfo> AllAnswers;

            if (type == 1)
            {
                AllAnswers = _questionsAnswersInfo.GetQuestionsAnswerByAdminId(admin.Id, 0, 0, out total);
            }
            else
            {
                var QuestionsAnswerExamines = _questionsAnswerExamine.GetAnswerInfoByAdminId(admin.Id, 0, 0, new List<Guid>(), out total);
                AllAnswers = (from item in QuestionsAnswerExamines select item.QuestionsAnswersInfo).ToList();
            }

            List<AnswerVo> answers;

            if (school == null)
            {
                total = AllAnswers.Count();
                AllAnswers = AllAnswers.Skip((page - 1) * limit).Take(limit).ToList();
                answers = ChangeAnswer(AllAnswers);
            }
            else
            {
                answers = ChangeAnswer(AllAnswers);
                var searchData = (from item in answers where item.School.Contains(school) select item).ToList();
                total = searchData.Count();
                answers = searchData.Skip((page - 1) * limit).Take(limit).ToList();
            }

            return new PageResult<List<AnswerVo>>()
            {
                rows = answers,
                total = total
            };
        }

        /// <summary>
        /// 问答详情转换视图
        /// </summary>
        /// <param name="AllAnswers"></param>
        /// <returns></returns>
        public List<AnswerVo> ChangeAnswer(List<QuestionsAnswersInfo> AllAnswers)
        {
            List<AnswerVo> answers = new List<AnswerVo>();
            if (!AllAnswers.Any()) 
            {
                return answers;
            }

            var school = _schoolInfoService.GetSchoolName(AllAnswers.Select(x => x.QuestionInfo.SchoolSectionId).ToList());
            var examinerAdmin = _partTimeJobAdmin.GetJobAdminNameByIds(AllAnswers.Where(x=>x.QuestionsAnswerExamine!=null).Select(x => x.QuestionsAnswerExamine.AdminId).ToList());

            foreach (QuestionsAnswersInfo item in AllAnswers)
            {
                AnswerVo answerVo = new AnswerVo();
                answerVo.Id = item.Id;
                answerVo.AddTime = item.CreateTime.ToString("yyyy-MM-dd");
                answerVo.Question = item.QuestionInfo.Content;
                answerVo.Answer = item.Content;

                //answerVo.School = _schoolService.GetSchoolExtension(item.QuestionInfo.SchoolSectionId)?.FullSchoolName;

                answerVo.School = school.Where(x => x.SchoolSectionId == item.QuestionInfo.SchoolSectionId).FirstOrDefault().SchoolName;

                answerVo.AnswerWrite = _userService.GetUserInfo(item.UserId).NickName;
                if (item.QuestionsAnswerExamine != null)
                {
                    //answerVo.ExamineAdmin = _partTimeJobAdmin.GetModelById(item.QuestionsAnswerExamine.AdminId).Name;
                    answerVo.ExamineAdmin = examinerAdmin.Where(x => x.Id == item.QuestionsAnswerExamine.AdminId).FirstOrDefault().Name;
                }
                answerVo.ExamineStatus = item.State.Description();
                answers.Add(answerVo);
            }
            return answers;
        }

        /// <summary>
        /// 问答详情
        /// </summary>
        /// <param name="name"></param>
        /// <param name="question"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public IActionResult AnswerInfo(Guid answerId)
        {
            var model = _questionsAnswersInfo.GetModelById(answerId);
            Dictionary<string, string> dir = new Dictionary<string, string>();
            dir.Add("name", _userService.GetUserInfo(model.UserId).NickName);
            dir.Add("question", model.QuestionInfo.Content);
            dir.Add("answer", model.Content);
            ViewBag.Answer = dir;
            return View();
        }

        /// <summary>
        /// 点评详情
        /// </summary>
        /// <param name="CommentId"></param>
        /// <returns></returns>
        public IActionResult CommentInfo(Guid CommentId)
        {
            CommentDetail detail = new CommentDetail();
            detail.schoolComment =  _schoolComment.QueryComment(CommentId);
            detail.commentUser = _userService.GetUserInfo(detail.schoolComment.UserId).NickName;
            detail.tags = _schoolTagService.GetSchoolTagByCommentId(detail.schoolComment.Id);
            detail.schoolImages = _schoolImage.GetImageByDataSourceId(detail.schoolComment.Id);
            detail.schoolImages.ForEach(x => x.ImageUrl = _setting.queryImager + x.ImageUrl);

            if (detail.schoolComment.Score.IsAttend)
            {
                detail.teach = SchoolScoreToStart.scoreValue(detail.schoolComment.Score.TeachScore);
                detail.hard = SchoolScoreToStart.scoreValue(detail.schoolComment.Score.HardScore);
                detail.envir = SchoolScoreToStart.scoreValue(detail.schoolComment.Score.EnvirScore);
                detail.manage = SchoolScoreToStart.scoreValue(detail.schoolComment.Score.ManageScore);
                detail.life = SchoolScoreToStart.scoreValue(detail.schoolComment.Score.LifeScore);
            }
            else
            {
                detail.population = SchoolScoreToStart.scoreValue(detail.schoolComment.Score.AggScore);
            }

           
            ViewBag.CommentDetail = detail;
            return View();
        }
        

    }
}