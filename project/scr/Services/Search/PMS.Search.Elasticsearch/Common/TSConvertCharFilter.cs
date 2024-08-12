using System;
using System.Runtime.Serialization;
using Nest;
using Newtonsoft.Json;

namespace PMS.Search.Elasticsearch
{

    public class TSConvertCharFilter : ICharFilter
    {
        public string Type => "stconvert";

        public string Version { get ; set; }


        [PropertyName("convert_type")]
        public string ConvertType { get; set; }

        [PropertyName("delimiter")]
        public string Delimiter { get; set; }

        [PropertyName("keep_both")]
        public bool KeepBoth { get; set; }


        public TSConvertCharFilter(string convertType, string delimiter)
        {
            this.ConvertType = convertType;
            this.Delimiter = delimiter;
            this.KeepBoth = true;
        }
    }
}
