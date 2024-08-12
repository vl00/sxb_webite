using PMS.UserManage.Application.ModelDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Application.IServices
{
    public interface ILikeService
    {
        VoBase GetCommentLike(string cookieStr, int page = 1);
        VoBase GetQALike(string cookieStr, int page = 1);
    }
}
