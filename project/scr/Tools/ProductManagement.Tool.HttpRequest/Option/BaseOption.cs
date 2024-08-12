using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProductManagement.Tool.HttpRequest.Encrypt;
using Newtonsoft.Json;

namespace ProductManagement.Tool.HttpRequest.Option
{
    public abstract class BaseOption
    {
        /// <summary>
        /// 服务器的地址
        /// </summary>
        public abstract string UrlPath { get; }

        /// <summary>
        /// 要加密的内容的加密方式
        /// </summary>
        public virtual string Encrypt => DefaultEncrypt.Default;

        /// <summary>
        /// 请求头部的内容
        /// </summary>
        public Dictionary<string, string> Headers => new Dictionary<string, string>(_headers);

        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();

        /// <summary>
        /// 请求参数的内容
        /// KeyValuePairs 和 PostBody 二选一
        /// </summary>
        public List<KeyValuePair<string, RequestValue>> KeyValuePairs => new List<KeyValuePair<string, RequestValue>>(_keyValuePairs);

        /// <summary>
        /// KeyValuePairs 和 PostBody 二选一
        /// </summary>
        public object PostBody { get; set; }

        private readonly List<KeyValuePair<string, RequestValue>> _keyValuePairs = new List<KeyValuePair<string, RequestValue>>();


        /// <summary>
        /// 添加头部内容
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddHeader(string key, string value)
        {
            _headers[key.ToLower()] = value;
        }

        /// <summary>
        /// 添加请求参数内容
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddValue(string key, RequestValue value)
        {
            _keyValuePairs.Add(new KeyValuePair<string, RequestValue>(key, value));
        }


		public virtual bool IsJsonContent()
        {
            if (_headers.TryGetValue("contenttype", out string contentType))
            {
                return contentType.Equals("application/json", StringComparison.CurrentCultureIgnoreCase);
            }

            return false;
        }

        public override string ToString()
        {
            return string.Join("-", _keyValuePairs.Select(v => v));
        }
    }

    public class RequestValue
    {
        public RequestValue(string value, bool isEncrypt = false)
        {
            Value = value;
            IsEncrypt = isEncrypt;
        }

        public RequestValue(object value)
        {
            Value = value;
        }

        /// <summary>
        /// 请求的值
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// 请求的值是否加密
        /// </summary>
        public bool IsEncrypt { get; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
