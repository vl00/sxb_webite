namespace Sxb.Web.Areas.Home.Models
{
    public class OrgTypeInfo
    {
        public int Key { get; set; }
        public string Value { get; set; }
        public int Sort { get; set; }
        public string Url
        {
            get
            {
                return $"/org/orgs/type{Key}";
            }
        }
    }
}
