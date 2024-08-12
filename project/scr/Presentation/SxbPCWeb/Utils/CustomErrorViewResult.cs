using Microsoft.AspNetCore.Mvc;

namespace Sxb.PCWeb.Utils
{
    /// <summary>
    ///学校错误页面
    /// </summary>
    public class CustomErrorViewResult : ViewResult
    {
        public CustomErrorViewResult(int statusCode)
        {
            ViewName = "/Views/Errors/CustomError.cshtml";
            StatusCode = statusCode;
        }
    }
}
