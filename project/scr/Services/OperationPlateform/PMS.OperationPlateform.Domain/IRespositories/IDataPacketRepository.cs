using PMS.OperationPlateform.Domain.DTOs;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Domain.IRespositories
{
    public interface IDataPacketRepository : IBaseRepository<DataPacket>
    {
        Task<IEnumerable<DataPacket>> GetList(DataPacketStep? step, DataPacketStatus? status, DateTime startTime, DateTime endTime, int pageIndex, int pageSize);

        /// <summary>
        /// 获取每个页面每天的分组统计
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        Task<IEnumerable<DataPacketGroupDto>> GetGroups(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 获取资料包渠道关注用户列表
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        Task<IEnumerable<DataPacketUserDto>> GetSubscribeUsers(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 获取微信回复的关键词
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="startTime"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetWeixinReplyMsgs(string openId, DateTime startTime, string[] contents);
    }
}