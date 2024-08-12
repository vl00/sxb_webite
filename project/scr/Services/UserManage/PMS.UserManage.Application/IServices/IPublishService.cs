using PMS.UserManage.Application.ModelDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Application.IServices
{
    public interface IPublishService
    {
        VoBase GetCommentList(string cookieStr, int page);
        VoBase GetQAList(string cookieStr, int page);
    }
}
