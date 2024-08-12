using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductManagement.Tool.Amap.Common;
using ProductManagement.Tool.Amap.Model;
using ProductManagement.Tool.Amap.Option;
using ProductManagement.Tool.Amap.Result;
using ProductManagement.Tool.HttpRequest;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProductManagement.Tool.Amap
{
    public class AmapClient : HttpBaseClient<AmapConfig>, IAmapClient
    {

        private new AmapConfig Config { get; }

        public AmapClient(HttpClient client, IOptions<AmapConfig> config, ILoggerFactory log)
                    : base(client, config.Value, log)
        {
            Config = config.Value;
        }

        public async Task<DistrictResult> GetDistrict(DistrictModel info)
        {
            DistrictOption option = new DistrictOption(
               info, Config
           );
            var result = await GetAsync<OriginDistrictResult, DistrictOption>(option);

            return new DistrictResult {
                Infocode = result.Infocode,
                Info = result.Info,
                Count = result.Count,
                Status = result.Status,
                Districts = result.Districts.Select(ToDistrictData).ToList()
            };
        }
        private DistrictData ToDistrictData(OriginDistrictResult.DistrictData data)
        {
            if (data == null )
                return null;

            string citycode = "";
            var dataJson = data.Citycode.ToString() as String;
            if (dataJson.Length > 0 && dataJson[0] == '[')
            {
                var temp_list = JsonConvert.DeserializeObject<List<string>>(dataJson);
                citycode = temp_list.Count>0? temp_list[0] : "";
            }
            else
            {
                var temp_list = dataJson;
                citycode = temp_list ;
            }
            return new DistrictData {
                Citycode  = citycode,
                Adcode = data.Adcode,
                Center = data.Center,
                Level = data.Level,
                Name = data.Name,
                Districts = data.Districts.Select(ToDistrictData).ToList()
            };

        }
        public async Task<CurrentLocation> GetCurrentLocation(string ip)
        {
            CurrentLocationOption currentLocation = new CurrentLocationOption(ip, Config);
            return await GetAsync<CurrentLocation, CurrentLocationOption>(currentLocation);
        }
    }
}
