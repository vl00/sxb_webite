using PMS.UserManage.Domain.Entities;
using Sxb.UserCenter.Models.MessageViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Utils.DtoToViewModel
{
    public class TipsToViewModelHelper
    {
        public static TipsTotalViewModel TipsToViewModel(List<MessageTipsTotal> TipsTotals) 
        {
            TipsTotalViewModel tipsTotalView = new TipsTotalViewModel();
            foreach (var item in TipsTotals)
            {
                if (item.Type == PMS.UserManage.Domain.Common.Tips.InviteMe)
                {
                    tipsTotalView.InviteMe = item.Total;
                }
                else if (item.Type == PMS.UserManage.Domain.Common.Tips.LikeTotal)
                {
                    tipsTotalView.LikeTotal = item.Total;
                }
                else if (item.Type == PMS.UserManage.Domain.Common.Tips.NewFans)
                {
                    tipsTotalView.NewFans = item.Total;
                }
                else 
                {
                    tipsTotalView.ReplyMe = item.Total;
                }
            }
            return tipsTotalView;
        }

    }
}
