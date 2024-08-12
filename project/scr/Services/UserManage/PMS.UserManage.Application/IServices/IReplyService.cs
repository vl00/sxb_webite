using PMS.UserManage.Application.ModelDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Application.IServices
{
    public interface IReplyService
    {
        VoBase GetQAList(string cookieStr, int page = 1);
        VoBase GetCommentList(string cookieStr, int page = 1);
    }
}
