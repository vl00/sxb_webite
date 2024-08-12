using System;
using System.Collections.Generic;
using PMS.Search.Application.ModelDto;
using PMS.Search.Domain.Entities;

namespace PMS.Search.Application.IServices
{
    public interface IImportService
    {
        void CreateHistoryIndex();
        bool SetHistory(string Application, int channel, string keyWord, int cityCode, Guid? UserId, string UUID);


        List<SearchLastTimeDto> GetLastUpdate(string indexName=null);
        void CreateSchoolIndex();
        void CreateCommentIndex();
        void CreateQuestionIndex();
        void CreateAnswerIndex();
        void CreateLiveIndex();
        void CreateRankIndex();
        void ImportSchoolData(List<SearchSchool> list);
        void ImportCommentData(List<SearchComment> list);
        void ImportQuestionData(List<SearchQuestion> list);
        void ImportSchoolRankData(List<SearchSchoolRank> list);
        void ImportArticleData(List<SearchArticle> list);
        void ImportSingSchoolData(List<SearchSignSchool> list);
        bool ImportSearchSchoolData(List<BTSearchSchool> list);


        void CreateEESchoolIndex();

        void CreateSignSchool();
        void CreateSchoolSearch();

        void ImportEESchoolData(List<SearchEESchool> list);
        void UpdateSchoolData(List<SearchSchool> list);
        void UpdateCommentData(List<SearchComment> list);
        void UpdateQuestionData(List<SearchQuestion> list);

        void UpdateSignSchoolData(List<SearchSignSchool> list);

        bool DelSchoolData(List<string> List);
        bool UpdateBDSchooData(List<BTSearchSchool> list);

        SearchLastUpdateTime GetBDLastCreateTime();


        bool DelArticleData(List<Guid> deleteIds);
        void CreateArticleIndex();

        void CreateSvsSchoolIndex();

        void ImportTopicData(List<SearchTopic> list);
        void UpdateTopicData(List<SearchTopic> list);
        bool RemoveUserHistory(Guid userId);
        void UpdateSvsSchool(long province, string isDeleted);
        void CreateCourseIndex();
        void CreateOrganizationIndex();
        void CreateEvaluationIndex();
    }
}
