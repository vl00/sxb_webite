using ProductManagement.API.Http.Model;
using ProductManagement.Tool.HttpRequest.Option;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Option
{
    public class CollectedSearch : BaseOption
    {
        private string content = "";

        public CollectedSearch(IsCollected collected)
        {
            content = $"/apicollection/iscollected?dataID={collected.dataID}&userId={collected.userId}";
        }

        public override string UrlPath => content;
    }
}
