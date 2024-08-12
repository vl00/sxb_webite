using Microsoft.Extensions.Options;
using Nest;
using PMS.Search.Domain.Entities;
using PMS.Search.Domain.IRepositories;
using ProductManagement.Framework.SearchAccessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Search.Elasticsearch.Repositories
{
    public class UniversitySearch : IUniversitySearch
    {
        private readonly ElasticClient _client;
        private readonly EsConfig _esConfig;

        private readonly Index _universityIndex;

        public UniversitySearch(ISearch clientProvider, IOptions<EsConfig> options)
        {
            _client = clientProvider.GetClient();
            _esConfig = options.Value ?? new EsConfig();

            if (_esConfig.Indices != null)
            {
                _universityIndex = _esConfig.Indices.University;
            }
        }

        public async Task<List<SearchUniversity>> SearchUniversitys(string keyword, int? cityId, int? exculdeCityId, int? type, int pageIndex, int pageSize)
        {
            int from = (pageIndex - 1) * pageSize;
            int size = pageSize;

            Func<SortDescriptor<SearchUniversity>, IPromise<IList<ISort>>> sort = s =>
            {

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return s.Descending(item => item.Type)
                            .Ascending(item => item.FirstLetterName);
                }
                else
                {
                    return s.Descending(SortSpecialField.Score)
                            .Descending(item => item.Type)
                            .Ascending(item => item.FirstLetterName);
                }
            };

            var search = new SearchDescriptor<SearchUniversity>()
              .Index(_universityIndex.Alias)
              .From(from)
              .Size(size)
              .Sort(s => s
                  .Descending(SortSpecialField.Score)
                  .Descending(item => item.Type)
                  .Ascending(item => item.FirstLetterName)
              )
              ;

            var mustQuery = new List<Func<QueryContainerDescriptor<SearchUniversity>, QueryContainer>>() {
                q => q.Term(t=> t.Field(s => s.IsDeleted).Value(false)),
            };

            var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchUniversity>, QueryContainer>>
            {
                q => q.Match(t => t.Field("name").Query(keyword).Boost(2)),
                q => q.Term(t => t.Field("name.pinyin").Value(keyword).Boost(2)),
            };

            //关键字查询
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                mustQuery.Add(q => q.Match(t => t.Field("name.pinyin").Query(keyword)));
            }
            //城市查询
            if (cityId > 0)
            {
                mustQuery.Add(q => q.Term(t => t.Field(f => f.CityId).Value(cityId)));
            }
            //排除城市
            if (exculdeCityId > 0)
            {
                mustQuery.Add(q => q.Bool(b => b
                    .MustNot(m => m
                        .Term(t => t.Field(f => f.CityId).Value(exculdeCityId))
                    )
                ));
            }
            //学校类型查询
            if (type > 0)
            {
                //mustQuery.Add(q => q.Term(t => t.Field(f => f.Type).Value(type)));
                mustQuery.Add(q => q.Script(s => s
                    .Script(ss => ss.Source($"  (((int)doc['type'].value) & {type}) > 0  "))
                ));
            }

            Func<QueryContainerDescriptor<SearchUniversity>, QueryContainer> query = query = q => q.Bool(b => b.Must(mustQuery)); ;
            search.Query(q =>
                q.FunctionScore(fs => fs
                    .Query(query)
                    .BoostMode(FunctionBoostMode.Replace)
                    .Functions(fun => fun
                        .ScriptScore(
                            ss => ss.Script(sss => sss.Source(" Math.round(_score * 10) ")))
                        )
                ));

            var result = await _client.SearchAsync<SearchUniversity>(search);
            var data = result.Hits.Select(s => s.Source).ToList();

            return data;
        }
    }
}
