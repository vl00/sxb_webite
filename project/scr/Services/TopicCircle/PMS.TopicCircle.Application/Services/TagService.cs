using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.TopicCircle.Application.Services
{
    public class TagService : ApplicationService<Tag>, ITagService
    {
        ITagRepository _repository;
        public TagService(ITagRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TagDto>> GetByTopicID(Guid topicID)
        {
            var finds = await _repository.GetByTopicID(topicID);
            if (finds?.Any() == true)
            {
                return finds.Select(p => new TagDto()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Pid = p.ParentId
                });
            }
            return new TagDto[0];
        }

        public AppServiceResultDto<IEnumerable<TagHotDto>> GetHotTags(Guid circleId, int takeCount)
        {

            var hotTags = _repository.ExcuteUSP_STASTICHOTTAG(circleId, takeCount);
            var parentTags = _repository.GetBy(" id in @ids", new { ids = hotTags.Select(ht => ht.ParentId) });
            IEnumerable<TagHotDto> tagHotDtos = hotTags.Select(tg =>
            {
                var parentTag = parentTags.FirstOrDefault(ptg => ptg.Id == tg.ParentId);
                return new TagHotDto()
                {
                    Id = tg.Id,
                    Name = tg.Name,
                    PId = parentTag?.Id ?? -1,
                    PName = parentTag?.Name
                };
            });
            return AppServiceResultDto.Success(tagHotDtos);
        }

        AppServiceResultDto<IEnumerable<TagDto>> ITagService.GetTags()
        {
            return AppServiceResultDto.Success(_repository.GetTags().Select<Tag, TagDto>(t => t));

        }


    }
}
