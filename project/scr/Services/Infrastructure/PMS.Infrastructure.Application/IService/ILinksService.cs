using System;
using System.Collections.Generic;
using PMS.Infrastructure.Application.ModelDto;

namespace PMS.Infrastructure.Application.IService
{
    public interface ILinksService
    {
        List<LinksDto> GetLinks();

        void SetLinks(string title, string link, int sort = 0);
        void RemoveLinks(string title);
    }
}
