using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Infrastructure.Configs.Models
{
    public class WXTemplateMsgField {

        public string FieldName { get; set; }
        public string Value { get; set; }

        public string Color { get; set; } = "#000000";
    }

    public class WXTemplateMsg
    {
        public string TemplateId { get; set; }
        public string Url { get; set; }
        public List<WXTemplateMsgField> Fields { get; set; }

    }
}
