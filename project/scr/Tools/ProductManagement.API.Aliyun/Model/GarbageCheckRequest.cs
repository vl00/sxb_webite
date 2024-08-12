using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Aliyun.Model
{

   public  class GarbageCheckRequest
    {
        /// <summary>
        /// 可为空，该字段用于标识业务场景。针对不同的业务场景，您可以配置不同的内容审核策略，以满足不同场景下不同的审核标准或算法策略的需求。您可以通过云盾内容安全控制台创建业务场景（bizType），或者通过工单联系我们帮助您创建业务场景。
        /// </summary>
        public string bizType { get; set; }

        /// <summary>
        /// 指定检测场景，取值：antispam。
        /// </summary>
        public string[] scenes { get; set; }

        /// <summary>
        /// 文本检测任务列表，包含一个或多个元素。每个元素是个结构体，最多可添加100个元素，即最多对100段文本进行检测。每个元素的具体结构描述见task
        /// </summary>
        public List<Task> tasks { get; set; }


        public class Task {

            /// <summary>
            /// 可为空，JSON结构体 客户端信息，参见公共参数中的公共查询参数。
            /// </summary>
            public string clientInfo { get; set; }

            /// <summary>
            /// 数据Id,可为空，需要保证在一次请求中所有的Id不重复。
            /// </summary>
            public string dataId { get; set; }

            /// <summary>
            /// 待检测文本，最长10000个字符
            /// </summary>
            public string content { get; set; }
        }
    }

        

}
