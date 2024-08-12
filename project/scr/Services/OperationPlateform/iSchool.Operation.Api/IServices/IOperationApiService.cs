using iSchool.Operation.Api.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace iSchool.Operation.Api.IServices
{


    /// <summary>
    /// 定义一系列访问 OperationPlateform 公布的接口
    /// </summary>
    public interface IOperationApiService
    {

        Task<List<ToolModule>> GetToolTypesAsync();





    }
}
