using System;
using System.Collections.Generic;
using System.Linq;
using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Application.ModelDto;
using PMS.Infrastructure.Domain.IRepositories;

namespace PMS.Infrastructure.Application.Service
{
    public class LinksService: ILinksService
    {
        private readonly ILinksRepository _linksRepository;
        public LinksService(ILinksRepository linksRepository)
        {
            _linksRepository = linksRepository;
        }

        public List<LinksDto> GetLinks()
        {
            var result = _linksRepository.GetLinks();
            return result.Select(q=>new LinksDto {
                Title = q.Title,
                Href = q.Href,
                Sort = q.Sort
            }).ToList();
        }

        public void SetLinks(string title, string link,int sort = 0)
        {
            if(!_linksRepository.ExsitLinks(title))
            {
                _linksRepository.AddLinks(title, link, sort);
            }
            else
            {
                _linksRepository.UpdateLinks(title, link, sort);
            }
        }

        public void RemoveLinks(string title)
        {
            _linksRepository.RemoveLinks(title);
        }

        
    }
}
