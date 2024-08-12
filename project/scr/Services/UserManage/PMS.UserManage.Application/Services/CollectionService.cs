using System;
using System.Collections.Generic;
using System.Linq;
using PMS.CommentsManage.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.IRespositories;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Application.ModelDto.Message;
using PMS.UserManage.Application.ModelDto.ModelVo;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using ProductManagement.Framework.Foundation;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace PMS.UserManage.Application.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly ICollectionRepository _collectionRep;
        private readonly IQuestionsAnswersInfoService _questionsAnswersInfoService;
        private readonly ISchoolCommentService _schoolCommentService;
        private readonly IArticleRepository _articleRepository;
        private IApiService _apiService;

        public CollectionService(ICollectionRepository collectionRep, IApiService apiService, IQuestionsAnswersInfoService questionsAnswersInfoService, ISchoolCommentService schoolCommentService, IArticleRepository articleRepository)
        {
            _collectionRep = collectionRep;
            _apiService = apiService;
            _questionsAnswersInfoService = questionsAnswersInfoService;
            _schoolCommentService = schoolCommentService;
            _articleRepository = articleRepository;
        }
        public int GetCollectionCount(Guid userID)
        {
            return _collectionRep.GetCollectionCount(userID);
        }
        public bool AddCollection(Guid userID, Guid dataID, byte dataType)
        {
            //用户验证

            return _collectionRep.AddCollection(userID, dataID, dataType);
        }
        public bool RemoveCollection(Guid userID, Guid dataID)
        {
            return _collectionRep.RemoveCollection(userID, dataID);
        }
        public void RemoveCollections(Guid userID, List<Guid> dataIDs)
        {
            foreach (var dataID in dataIDs)
            {
                _collectionRep.RemoveCollection(userID, dataID);
            }
        }
        public bool IsCollected(Guid userID, Guid dataID, CollectionDataType dataType)
        {
            return _collectionRep.IsCollected(userID, dataID, (int)dataType);
        }
        public bool IsCollected(Guid userID, Guid dataID)
        {
            return _collectionRep.IsCollected(userID, dataID);
        }

        /// <summary>
        /// 获取用户的收藏列表
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="dataType"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<Guid> GetUserCollection(Guid userID, byte dataType, int page)
        {
            return _collectionRep.GetUserCollection(userID, dataType, page, 10);
        }
        public List<PMS.UserManage.Application.ModelDto.ModelVo.SchoolModel> GetSchoolCollection(Guid userID, double? lat = null, double? lng = null, int page = 1)
        {
            var iDList = GetUserCollection(userID, 1, page);
            var list = _apiService.GetCollectionExtAsync(iDList);
            if (list != null)
            {
                foreach (var school in list)
                {
                    if (lat != null && lng != null && school.Latitude != null && school.Longitude != null)
                    {
                        school.Distance = CommonHelper.FormatDistance(CommonHelper.GetDistance(lat.Value, lng.Value, school.Latitude.Value, school.Longitude.Value));
                    }
                }
            }
            return list;
        }
        public VoBase GetCommentCollection(Guid userID, string cookieStr, int page = 1)
        {
            var iDList = GetUserCollection(userID, 3, page);
            List<object> requestObj = new List<object>();
            foreach (var id in iDList)
            {
                requestObj.Add(new { dataIdType = 1, dataId = id.ToString() });
            }
            var res = _apiService.GetSchooCommentOrReply(requestObj, cookieStr);
            return res;
        }
        public List<QuestionVo> GetQACollection(Guid userID, string cookieStr, int page = 1)
        {
            var iDList = GetUserCollection(userID, 2, page);
            List<object> requestObj = new List<object>();
            foreach (var id in iDList)
            {
                requestObj.Add(new { dataIdType = 2, dataId = id.ToString() });
            }
            var result = _apiService.GetQuestionOrAnswer(requestObj, cookieStr);
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<QuestionAnswerData>(result.data.ToString());
            if (data == null || data.questionModels == null)
            {
                data = new QuestionAnswerData()
                {
                    questionModels = new List<QuestionVo>()
                };
            }
            List<QuestionVo> OutputResult = new List<QuestionVo>();
            foreach (var id in iDList)
            {
                var r = data.questionModels.Find(a => a.Id == id);
                if (r == null)
                {
                    OutputResult.Add(new QuestionVo() { Id = id });
                }
                else
                {
                    OutputResult.Add(r);
                }
            }
            return OutputResult;
        }
        public List<Data> GetArticleCollection(Guid userID, int page = 1)
        {
            var iDList = GetUserCollection(userID, 0, page);
            var result = _apiService.GetArticlesByIds(iDList);

            List<Data> OutputResult = new List<Data>();
            if (result == null)
            {
                return OutputResult;
            }
            foreach (Guid id in iDList)
            {
                var r = result.data.Find(a => a.id == id);
                if (r == null)
                {
                    OutputResult.Add(new Data() { id = id, isShow = false });
                }
                else
                {
                    OutputResult.Add(r);
                }
            }
            return OutputResult;
        }
        public PMS.UserManage.Domain.Common.RootModel GetSchoolCollectionID(Guid userID)
        {
            return new PMS.UserManage.Domain.Common.RootModel()
            {
                data = _collectionRep.GetUserCollection(userID, 1, 1, 1000)
            };
        }

        public List<Guid> GetUserCollection(Guid userID, byte dataType, int page = 1, int pageSize = 10)
        {
            return _collectionRep.GetUserCollection(userID, dataType, page, 10);
        }

        public List<Guid> GetPageUserCollection(Guid userID, byte dataType, ref int total, int page = 1, int pageSize = 10)
        {
            return _collectionRep.GetPageUserCollection(userID, dataType, ref total, page, 10);
        }

        public List<Guid> GetCollection(List<Guid> ids, Guid UserId)
        {
            return _collectionRep.GetCollection(ids, UserId);
        }

        public FollowFansTotal GetUserFollowFans(Guid userId)
        {
            return _collectionRep.GetUserFollowFans(userId);
        }

        public MydynamicTotal MydynamicTotal(Guid userId)
            => _collectionRep.MydynamicTotal(userId);

        /// <summary>
        /// 获取该用户的点评/回答的总关注数
        /// </summary>
        /// <returns></returns>
        public long GetCollectionUserTotal(Guid userId, DateTime? startDate, DateTime? endDate)
        {
            //获取用户的所有点评回答
            IEnumerable<Guid> answerIds = _questionsAnswersInfoService.GetList(s => s.UserId == userId
                            && (startDate == null || s.CreateTime >= startDate)
                            && (endDate == null || s.CreateTime <= endDate)).Select(s => s.Id);
            IEnumerable<Guid> commentIds = _schoolCommentService.GetList(s => s.CommentUserId == userId
                            && (startDate == null || s.AddTime >= startDate)
                            && (endDate == null || s.AddTime <= endDate)).Select(s => s.Id);

            //根据点评回答id获取关注数
            //long answersFollowTotal = _collectionRep.GetCollectionTotal(answerIds.ToList());
            //long commentFollowTotal = _collectionRep.GetCollectionTotal(commentIds.ToList());
            //return answersFollowTotal + commentFollowTotal;
            return _collectionRep.GetCollectionTotal(answerIds.Union(commentIds).ToList());
        }
    }
}
