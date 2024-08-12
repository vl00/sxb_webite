using ProductManagement.Tool.HttpRequest.Option;

namespace ProductManagement.API.Http.Option
{
    public class SMSParam : BaseOption
    {
        public SMSParam(string templateId, string[] phones, string[] templateParams)
        {
            AddValue("templateId", new RequestValue(templateId));
            AddValue("phones", new RequestValue(phones));
            AddValue("templateParams", new RequestValue(templateParams));
        }

        public override string UrlPath => "/api/SMSApi/SendSxbMessage";
    }
}
