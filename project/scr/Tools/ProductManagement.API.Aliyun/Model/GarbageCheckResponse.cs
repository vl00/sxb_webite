using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Aliyun.Model
{
// normal：正常文本
//spam：含垃圾信息
//ad：广告
//politics：涉政
//terrorism：暴恐
//abuse：辱骂
//porn：色情
//flood：灌水
//contraband：违禁
//meaningless：无意义
//customized：自定义（例如命中自定义关键词）



    public class CommonResponse<T> where T:class
    {
        public string msg { get; set; }
        public int code { get; set; }
        public T data { get; set; }
        public string requestId { get; set; }
    }


  
    public class GarbageCheckResponse
    {
        /// <summary>
        /// 错误描述信息。
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 错误码，和HTTP的status code一致。
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 对应请求的dataId。
        /// </summary>
        public string dataId { get; set; }
        /// <summary>
        /// 返回结果。调用成功时（code=200），返回结果中包含一个或多个元素。每个元素是个结构体，具体结构描述见result。
        /// </summary>
        public Result[] results { get; set; }
        /// <summary>对应请求的内容。
        /// 
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 如果检测文本命中您自定义关键词词库中的词，该字段会返回，并将命中的关键词替换为“*”。
        /// </summary>
        public string filteredContent { get; set; }
        /// <summary>
        /// 该检测任务的ID。
        /// </summary>
        public string taskId { get; set; }

        public class Result
        {
            /// <summary>
            /// 结果为该分类的概率，取值范围为[0.00-100.00]。值越高，表示越有可能属于该分类。
            /// </summary>
            public float rate { get; set; }
            /// <summary>
            /// 建议用户执行的操作，取值范围：
            ///pass：文本正常
            ///review：需要人工审核
            ///block：文本违规，可以直接删除或者做限制处理
            /// </summary>
            public string suggestion { get; set; }
            /// <summary>
            /// 命中风险的详细信息，一条文本可能命中多条风险详情。具体结构描述见detail。
            /// </summary>
            public Detail[] details { get; set; }

            /// <summary>
            /// 检测结果的分类，与具体的scene对应。取值范围参考scene 和 label说明。
            /// </summary>
            public string label { get; set; }
            /// <summary>
            /// 检测场景，和调用请求中的场景对应。
            /// </summary>
            public string scene { get; set; }
        }

        public class Detail
        {

            /// <summary>
            /// 文本命中的关键词信息，用于提示您违规的原因，可能会返回多个命中的关键词。具体结构描述见hintWord。
            /// </summary>
            public Hintword[] hintWords { get; set; }

            /// <summary>
            /// 命中该风险的上下文信息。具体结构描述见context。
            /// </summary>
            public Context[] contexts { get; set; }
            /// <summary>
            /// 文本命中风险的分类，与具体的scene对应。取值范围参考scene 和 label说明。
            /// </summary>
            public string label { get; set; }
        }

        public class Hintword
        {
            /// <summary>
            /// 文本命中的系统关键词内容。
            /// </summary>
            public string context { get; set; }
        }

        public class Context
        {

            /// <summary>
            /// 命中您自定义文本库时，才会返回该字段，取值为创建风险文本库后系统返回的文本库code。
            /// </summary>
            public string libCode { get; set; }
            /// <summary>
            /// 命中自定义词库时，才有本字段。取值为创建词库时填写的词库名称
            /// </summary>
            public string libName { get; set; }
            /// <summary>
            /// 检测文本命中的风险内容上下文内容。如果命中了您自定义的风险文本库，则会返回命中的文本内容（关键词或相似文本）。
            /// </summary>
            public string context { get; set; }
        }

    }
}
