using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.Services.Tags
{
    public class SchoolTagService : ISchoolTagService
    {
        private readonly ISchoolTagRepository _tagRepository;
        public SchoolTagService(ISchoolTagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public int Add(SchoolTag tag)
        {
            return _tagRepository.Add(tag);
        }

        public List<SchoolTag> GetSchoolTagByCommentId(Guid Id)
        {
            return _tagRepository.GetSchoolTagByCommentId(Id);
        }

        public List<SchoolTag> GetSchoolTagBySchoolId(Guid Id)
        {
            return _tagRepository.GetSchoolTagBySchoolId(Id);
        }

    }
}
