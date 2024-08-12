using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.IServices
{
    using Domain.IRespositories;
    using PMS.OperationPlateform.Domain.DTOs;
    using PMS.OperationPlateform.Domain.Entitys;
    using ProductManagement.Framework.Foundation;
    using ProductManagement.Infrastructure.Models;
    using System.Linq;
    using System.Threading.Tasks;

    public class ArticleChoiceHotPointService : IArticleChoiceHotPointService
    {
        IArticleChoiceHotPointRepository _currentRepository;
        IArticleCoverService _articleCoverService;
        public ArticleChoiceHotPointService(IArticleChoiceHotPointRepository repository, IArticleCoverService articleCoverService)
        {
            _currentRepository = repository;
            this._articleCoverService = articleCoverService;
        }

        public async Task<PaginationModel<ArticleDto>> GetArticles(Guid id, int offset, int limit)
        {
            var pageResult = await _currentRepository.GetArticles(id, offset, limit);
            var ids = pageResult.data.Select(s => s.id);
            var covers = _articleCoverService.GetCoversByIds(ids.ToArray());
            var articleDtos = pageResult.data?.Select(a => new ArticleDto
            {
                Id = a.id,
                No = UrlShortIdUtil.Long2Base32(a.No),
                Layout = a.layout,
                Type = string.IsNullOrEmpty(a.type) ? 1 : int.Parse(a.type),
                Title = a.title,
                ViewCount = a.VirualViewCount,
                Time = a.time.GetValueOrDefault().ConciseTime("yyyy年MM月dd日"),
                Covers = covers.Where(c => c.articleID == a.id).Select(c => c.ToString()).ToList()
            });
            return PaginationModel<ArticleDto>.Build(articleDtos,pageResult.total);
        }

        public async Task<IEnumerable<ArticleChoiceHotPoint>> GetHotPoints(int city)
        {
            var (data,total) = _currentRepository.SelectPage("  [City]=@city And isShow=1  ", new { city }, "sort asc", null,true,0,6);
            if (data?.Any() == false) {
                  //城市没有的，用全国来代替
                 (data, total) = _currentRepository.SelectPage("  [City]=@city And isShow=1 ", new { city = 0 }, "sort asc", null, true, 0, 6);
            }
            return data;
        }
    }
}
