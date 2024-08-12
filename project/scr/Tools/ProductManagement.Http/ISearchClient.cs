using System.Threading.Tasks;
using ProductManagement.API.Http.Model;
using ProductManagement.API.Http.Result;

namespace ProductManagement.API.Http
{
    public interface ISearchClient
    {
        Task<SearchResult> Search(GetSchoolByNameAndAdCode para);
    }
}
