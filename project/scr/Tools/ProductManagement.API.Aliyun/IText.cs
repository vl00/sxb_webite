using ProductManagement.API.Aliyun.Model;
using System.Threading.Tasks;
namespace ProductManagement.API.Aliyun
{
    public interface IText
    {
        //文本垃圾内容检测 https://help.aliyun.com/document_detail/70439.html
        Task<CommonResponse<GarbageCheckResponse[]>> GarbageCheck(GarbageCheckRequest request);

        Task<bool> Check(GarbageCheckRequest request);
        Task<bool> GarbageCheck(string text);
    }
}
