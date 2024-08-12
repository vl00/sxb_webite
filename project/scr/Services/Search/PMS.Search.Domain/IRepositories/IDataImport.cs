using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PMS.Search.Domain.Entities;

namespace PMS.Search.Domain.IRepositories
{
    public interface IDataImport
    {
        List<SearchLastUpdateTime> GetLastUpdate(string indexName = null);

        void CreateSchoolIndex();
        void CreateCommentIndex();
        void CreateQuestionIndex();
        void CreateAnwserIndex();
        void CreateLiveIndex();
        void CreateSchoolRankIndex();
        void CreateArticleIndex();

        void CreateSvsSchoolIndex();

        void PutMappingSchool();
        void PutMappingComment();
        void PutMappingQuestion();
        void PutMappingSchoolRank();
        void PutMappingArticle();
        void PutMappingSignSchool();
        void PutMappingSchoolSearch();

        void ImportSchoolData(List<SearchSchool> school);
        void ImportCommentData(List<SearchComment> list);
        void ImportQuestionData(List<SearchQuestion> list);
        void ImportSchoolRankData(List<SearchSchoolRank> list);
        void ImportArticleData(List<SearchArticle> list);
        void ImportSingSchoolData(List<SearchSignSchool> list);
        bool ImportSearchSchoolData(List<BTSearchSchool> list);

        void PutMappingHistory();
        bool SetHistory(string Application, int channel, string keyWord, int cityCode, Guid? UserId, string UUID);


        void PutMappingEESchool();
        void ImportEESchoolData(List<SearchEESchool> list);
        void UpdateSchoolData(List<SearchSchool> list);
        void UpdateCommentData(List<SearchComment> list);
        void UpdateQuestionData(List<SearchQuestion> list);
        void UpdateSignSchoolData(List<SearchSignSchool> list);
        bool DelSchoolData(List<string> List);
        bool UpdateBDSchooData(List<BTSearchSchool> list);
        
        SearchLastUpdateTime GetBDLastCreateTime();
        bool DelArticleData(List<Guid> deleteIds);
        void CreateTopicIndex();
        void UpdateTopicData(List<SearchTopic> list);
        void ImportTopicData(List<SearchTopic> list);
        void CreateCircleIndex();
        bool RemoveUserHistory(Guid userId);
        void UpdateSvsSchool(long province, string isDeleted);
        void CreateCourseIndex();
        void CreateOrganizationIndex();
        void CreateEvaluationIndex();
        bool UpdateSchoolViewCount(List<SearchSchoolViewCount> list);
        Task<bool> UpdateUVAsync(string index, IEnumerable<SearchUV> list);
        Task<bool> ResetUVAsync(string index);
    }
}
