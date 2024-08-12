using ProductManagement.Infrastructure.Configs.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Infrastructure.Configs
{

    /// <summary>
    /// 话题圈模块Option
    /// </summary>
    public class TopicOption
    {
        public const string Topic = "Topic";


        public TxtMsgTemplate BindAccountKFMsg { get; set; }


        public TxtMsgTemplate WelcomTalentCreateKFMsg { get; set; }


        public TxtMsgTemplate WelcomTalentCreateKFMiniAppMsg { get; set; }
    }

   
}
