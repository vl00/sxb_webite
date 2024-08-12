using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.CommentsManage.Application.IServices;
using PMS.OperationPlateform.Application.IServices;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace Sxb.Web.Areas.Article.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UgcController : ApiBaseController
    {
        private readonly IUgcService _ugcService;

        public UgcController(IUgcService ugcService)
        {
            _ugcService = ugcService;
        }

        public void T(Guid id, long no, MessageDataType type)
        {
            string url = Request.Scheme + "://" + Request.Host;
            switch (type)
            {
                case MessageDataType.Question:
                     url += string.Format("/question/{0}.html", UrlShortIdUtil.Long2Base32(no));
                    break;
                case MessageDataType.Comment:
                    url += string.Format("/comment/{0}.html", UrlShortIdUtil.Long2Base32(no));
                    break;
                default:
                    throw new NotSupportedException();
            }

            //以后异步
            _ugcService.Feedback(id);
            Response.Redirect(url);
        }
    }
}
