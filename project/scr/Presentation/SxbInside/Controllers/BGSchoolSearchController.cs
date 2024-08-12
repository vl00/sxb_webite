using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.Search.Application.IServices;
using PMS.Search.Domain.Entities;
using Sxb.Inside.Response;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.Inside.Controllers
{
    public class BGSchoolSearchController : Controller
    {
        private ISearchService _search;

        public BGSchoolSearchController(ISearchService search) 
        {
            _search = search;
        }

        [HttpPost]
        public ResponseResult BGSearchSchool([FromBody]BDSchoolSearch search) 
        {
            if (search.AuditorIds != null) 
            {
                if (search.AuditorIds.Any())
                {
                    search.AuditorIds = search.AuditorIds.Select(x => x.ToLower()).ToList();
                }
            }

            if (search.EidtorIds != null) 
            {
                if (search.EidtorIds.Any())
                {
                    search.EidtorIds = search.EidtorIds.Select(x => x.ToLower()).ToList();
                }
            }
            
            var searchResult = _search.BTSchoolSearch(search);
            return ResponseResult.Success(searchResult);
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
    }
}
