using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PMS.Search.Application.IServices;
using PMS.Search.Application.ModelDto;
using PMS.Search.Domain.Entities;
using PMS.Search.Domain.IRepositories;

namespace PMS.Search.Application.Services
{
    public class ImprotService : IImportService
    {
        private readonly IDataImport _dataImport;
        public ImprotService(IDataImport dataImport)
        {
            _dataImport = dataImport;
        }

        public List<SearchLastTimeDto> GetLastUpdate(string indexName = null)
        {
            var result = _dataImport.GetLastUpdate(indexName);
            return result.Select(q => new SearchLastTimeDto
            {
                Name = q.Name,
                LastTime = q.UpdateTime
            }).ToList();
        }
        public void CreateArticleIndex()
        {
            _dataImport.CreateArticleIndex();
        }

        public void CreateSvsSchoolIndex()
        {
            _dataImport.CreateSvsSchoolIndex();
        }
        
            public void CreateSchoolIndex()
        {
            _dataImport.CreateSchoolIndex();
            //
            //
            //_dataImport.CreateSchoolRankIndex();
            //_dataImport.CreateArticleIndex();

            //_dataImport.PutMappingSchool();
            //_dataImport.PutMappingSignSchool();
            //_dataImport.PutMappingComment();
            //_dataImport.PutMappingQuestion();
            //_dataImport.PutMappingSchoolRank();
            //_dataImport.PutMappingArticle();
        }
        public void CreateCommentIndex()
        {
            _dataImport.CreateCommentIndex();
        }

        public void CreateQuestionIndex()
        {
            _dataImport.CreateQuestionIndex();
        }
        public void CreateAnswerIndex()
        {
            _dataImport.CreateAnwserIndex();
        }
        public void CreateLiveIndex()
        {
            _dataImport.CreateLiveIndex();
        }
        public void CreateRankIndex()
        {
            _dataImport.CreateSchoolRankIndex();
        }

        public void CreateHistoryIndex()
        {
            _dataImport.PutMappingHistory();
        }
        public bool SetHistory(string Application, int channel, string keyWord, int cityCode, Guid? UserId, string UUID)
        {
            //过滤记录非中文、字母、数字和常见符号
            var pattern = "[^\u4e00-\u9fa5\u3000\u0020A-Za-z0-9_-]";
            if (Regex.IsMatch(keyWord, pattern))
            {
                return true;
            }
            return _dataImport.SetHistory(Application, channel, keyWord, cityCode, UserId, UUID);
        }
        public bool RemoveUserHistory(Guid userId)
        {
            return _dataImport.RemoveUserHistory(userId);
        }

        public void ImportArticleData(List<SearchArticle> list)
        {
            _dataImport.ImportArticleData(list);
        }

        public void ImportCommentData(List<SearchComment> list)
        {
            _dataImport.ImportCommentData(list);
        }

        public void ImportQuestionData(List<SearchQuestion> list)
        {
            _dataImport.ImportQuestionData(list);
        }

        public void ImportSchoolData(List<SearchSchool> school)
        {
            _dataImport.ImportSchoolData(school);
        }

        public void ImportSchoolRankData(List<SearchSchoolRank> list)
        {
            _dataImport.ImportSchoolRankData(list);
        }

        public void ImportSingSchoolData(List<SearchSignSchool> list)
        {
            _dataImport.ImportSingSchoolData(list);
        }

        public void CreateEESchoolIndex()
        {
            _dataImport.PutMappingEESchool();
        }

        public void ImportEESchoolData(List<SearchEESchool> list)
        {
            _dataImport.ImportEESchoolData(list);
        }

        public void UpdateSchoolData(List<SearchSchool> list)
        {
            _dataImport.UpdateSchoolData(list);
        }
        public void UpdateCommentData(List<SearchComment> list)
        {
            _dataImport.UpdateCommentData(list);
        }

        public void UpdateQuestionData(List<SearchQuestion> list)
        {
            _dataImport.UpdateQuestionData(list);
        }

        public void CreateSignSchool()
        {
            _dataImport.PutMappingSignSchool();
        }

        public void UpdateSignSchoolData(List<SearchSignSchool> list)
        {
            _dataImport.UpdateSignSchoolData(list);
        }

        public void CreateSchoolSearch()
        {
            _dataImport.PutMappingSchoolSearch();
        }

        public bool ImportSearchSchoolData(List<BTSearchSchool> list)
        {
            return _dataImport.ImportSearchSchoolData(list);
        }

        public bool DelSchoolData(List<string> List)
        {
            return _dataImport.DelSchoolData(List);
        }

        public bool UpdateBDSchooData(List<BTSearchSchool> list)
        {
            return _dataImport.UpdateBDSchooData(list);
        }

        public SearchLastUpdateTime GetBDLastCreateTime()
        {
            return _dataImport.GetBDLastCreateTime();
        }

        public bool DelArticleData(List<Guid> deleteIds)
        {
            return _dataImport.DelArticleData(deleteIds);
        }

        public void ImportTopicData(List<SearchTopic> list)
        {
            _dataImport.ImportTopicData(list);
        }
        public void UpdateTopicData(List<SearchTopic> list)
        {
            _dataImport.UpdateTopicData(list);
        }

        public void UpdateSvsSchool(long province, string isDeleted)
        {
            _dataImport.UpdateSvsSchool(province, isDeleted);
        }
        public void CreateCourseIndex()
        {
            _dataImport.CreateCourseIndex();
        }
        public void CreateOrganizationIndex()
        {
            _dataImport.CreateOrganizationIndex();
        }
        public void CreateEvaluationIndex()
        {
            _dataImport.CreateEvaluationIndex();
        }
    }
}
