using Microsoft.AspNetCore.Mvc;

namespace Sxb.UserCenter.Utils
{

    /// <summary>
    ///学校错误页面
    /// </summary>
    public class ExtNotFoundViewResult : ViewResult
    {
        public ExtNotFoundViewResult(string viewName)
        {
            ViewName = viewName;
            StatusCode = 406;
        }

        public ExtNotFoundViewResult()
        {
            ViewName = "/Views/Errors/ExtNotFound.cshtml";
            StatusCode = 406;
        }
    }
}
