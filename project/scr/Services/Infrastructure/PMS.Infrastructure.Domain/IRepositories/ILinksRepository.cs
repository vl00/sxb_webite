using System;
using System.Collections.Generic;
using PMS.Infrastructure.Domain.Entities;

namespace PMS.Infrastructure.Domain.IRepositories
{
    public interface ILinksRepository
    {
        List<Links> GetLinks();
        bool RemoveLinks(string title);
        bool ExsitLinks(string title);
        bool AddLinks(string title, string link, int sort = 0);
        bool UpdateLinks(string title, string link, int sort = 0);
    }
}
