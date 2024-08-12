using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Application.ModelDto.ModelVo;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Application.IServices
{
    public interface IApiService
    {
        ArticleModel GetArticlesByIds(List<Guid> iDList);

        VoBase GetQuestionOrAnswer(List<object> requestObj, string cookieStr);

        VoBase GetSchooCommentOrReply(List<object> requestObj, string cookieStr);

        List<SchoolModel> GetCollectionExtAsync(List<Guid> iDList);

        bool CheckIsLogOut(string cookieStr);
    }
}
