using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.OperationPlateform.Application.Dtos;
using PMS.OperationPlateform.Application.IServices;
using PMS.School.Application.IServices;
using Sxb.PCWeb.Areas.School.Models;
using Sxb.PCWeb.Controllers;
using Sxb.PCWeb.Response;

namespace Sxb.PCWeb.Areas.School
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SchoolScoreController : ApiBaseController
    {
        ISchoolService _schoolService;

        public SchoolScoreController(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        public async Task<ResponseResult> GetSchoolExtScore(Guid extID)
        {
            var result = ResponseResult.Success();
            if (extID == Guid.Empty) return ResponseResult.Failed("Params error");
            var indexs = await _schoolService.schoolExtScoreIndexsAsync();
            var scores = await _schoolService.GetSchoolExtScoreAsync(extID);

            if (indexs?.Any() == true && scores?.Any() == true)
            {
                var resultItem = new SchoolScoreResponse()
                {
                    CurrentIndex = new IndexItem()
                    {
                        ID = 22,
                        Name = indexs.FirstOrDefault(p => p.Id == 22).Index_Name,
                        ParentID = 0
                    },
                    Score = scores.FirstOrDefault(p => p.IndexId == 22)?.Score
                };

                var indexIDs = scores.Select(p => p.IndexId).ToArray();
                var secondLeve = indexs.Where(p => indexIDs.Contains(p.Id) && p.ParentId == 22);
                var thirdLevel = indexs.Where(p => indexIDs.Contains(p.Id) && p.ParentId != 22);

                resultItem.SubItems = new List<SchoolScoreResponse>();

                foreach (var item in secondLeve)
                {
                    var secondSubItem = new SchoolScoreResponse()
                    {
                        CurrentIndex = new IndexItem()
                        {
                            ID = item.Id,
                            Name = item.Index_Name,
                            ParentID = item.ParentId
                        },
                        Score = scores.FirstOrDefault(p => p.IndexId == item.Id)?.Score,
                        SubItems = new List<SchoolScoreResponse>()
                    };

                    var subItems = thirdLevel.Where(p => p.ParentId == item.Id);
                    if (subItems?.Any() == true)
                    {
                        foreach (var thirdItem in subItems)
                        {
                            secondSubItem.SubItems.Add(new SchoolScoreResponse()
                            {
                                CurrentIndex = new IndexItem()
                                {
                                    ID = thirdItem.Id,
                                    Name = thirdItem.Index_Name,
                                    ParentID = thirdItem.ParentId
                                },
                                Score = scores.FirstOrDefault(p => p.IndexId == thirdItem.Id)?.Score
                            });

                        }
                    }
                    resultItem.SubItems.Add(secondSubItem);
                }

                result.Data = resultItem;
            }

            return result;
        }


    }
}

