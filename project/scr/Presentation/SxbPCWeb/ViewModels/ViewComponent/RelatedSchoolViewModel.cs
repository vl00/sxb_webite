using PMS.School.Domain.Dtos;
using Sxb.PCWeb.ViewModels.School;
using System.Collections.Generic;

namespace Sxb.PCWeb.ViewModels.ViewComponent
{
    public class RelatedSchoolViewModel
    {
        public IEnumerable<SchExtDto0> NearSchool { get; set; }
        public IEnumerable<SimpleHotSchoolDto> HotSchools { get; set; }
        public IEnumerable<SchoolCommentCardViewModel> HotCommentSchools { get; set; }
        public IEnumerable<SchoolQuestionCardViewModel> HotQuestionSchools { get; set; }
    }
}
