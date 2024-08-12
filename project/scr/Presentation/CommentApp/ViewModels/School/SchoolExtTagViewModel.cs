using System;
namespace Sxb.Web.ViewModels.School
{
    public class SchoolExtTagViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int International { get; set; }
        public bool active { get; set; } = false;
    }
}
