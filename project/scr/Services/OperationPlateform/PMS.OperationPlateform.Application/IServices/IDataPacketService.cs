using PMS.OperationPlateform.Application.Dtos;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Application.IServices
{
    public interface IDataPacketService
    {
        DataPacket Add(DataPacket dataPacket);
        bool SubscribeWxCallback(Guid id, Guid userId);
        bool UpdateStep(Guid id, DataPacketStep step);
        Task<IEnumerable<DataPacket>> GetStep(DataPacketStep step, bool uncheckTime = false);

        /// <summary>
        /// 资料包数据总表
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        Task<List<DataPacketSummary>> Summary(DateTime startDate, DateTime endDate);

        /// <summary>
        /// 资料包用户行为表
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        Task<List<DataPacketUserSummary>> UserSummary(DateTime startDate, DateTime endDate);
    }
}