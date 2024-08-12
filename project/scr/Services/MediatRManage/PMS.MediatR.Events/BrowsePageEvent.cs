using System;
using MediatR;

namespace PMS.MediatR.Events
{
    public class BrowsePageEvent : INotification
    {
        /// <summary>
        /// 浏览页面事件
        /// </summary>
        public BrowsePageEvent()
        {
        }
    }
}
