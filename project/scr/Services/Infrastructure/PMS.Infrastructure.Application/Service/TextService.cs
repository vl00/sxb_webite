using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PMS.Infrastructure.Application.IService;
using ProductManagement.API.Aliyun;
using ProductManagement.API.Aliyun.Model;

namespace PMS.Infrastructure.Application.Service
{
    public class TextService: ITextService
    {
        private readonly IText _textClient;
        public TextService(IText textClient)
        {
            _textClient = textClient;
        }

        public async Task<bool> GreenTextCheck(string Keywords)
        {
            if (string.IsNullOrWhiteSpace(Keywords))
                return true;

            var result = await _textClient.GarbageCheck(new GarbageCheckRequest
            {
                scenes = new string[] {
                    "antispam"},
                tasks = new List<GarbageCheckRequest.Task>
                {
                    new GarbageCheckRequest.Task
                    {
                        content = Keywords
                    }
                }
            }) ;

            if(result.code == 200 && result.data?.Length>0)
            {
                var data = result.data[0];
                if (data.code == 200)
                {
                    var blockResult = data.results.ToList().Where(q => q.suggestion == "block");
                    if (blockResult.Any())
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return true;
        }
        public async Task<GarbageCheckResponse[]> GreenTextCheckDetail(string Keywords)
        {
            if (string.IsNullOrWhiteSpace(Keywords))
                return null;

            var result = await _textClient.GarbageCheck(new GarbageCheckRequest
            {
                scenes = new string[] {
                    "antispam"},
                tasks = new List<GarbageCheckRequest.Task>
                {
                    new GarbageCheckRequest.Task
                    {
                        content = Keywords
                    }
                }
            });

            if (result.code == 200 && result.data?.Length > 0)
            {
                return result.data;
            }
            return null;
        }
    }
}
