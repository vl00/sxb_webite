using System;
using ProductManagement.Tool.HttpRequest.Option;

namespace ProductManagement.API.Http.Option
{
    public class UserRecommendOption : BaseOption
    {
        private string Content = "";


        public UserRecommendOption(Guid userid,int index,int size = 10)
        {
            AddValue("userid", new RequestValue(userid));
            AddValue("butype", new RequestValue("0"));

            Content = $"/getuserfavorites?curpage={index}&countperpage={size}";
        }
        public override string UrlPath => Content;
    }
}
