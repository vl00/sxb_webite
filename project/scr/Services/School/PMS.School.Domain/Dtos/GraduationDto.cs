using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.School.Domain.Dtos
{
    public class GraduationDto
    {
        Guid id;

        public Guid Id 
        {
            get => id;
            set => id = value;
        }

        public Guid Sid { get; set; }
        public Guid Extid
        {
            get => id;
            set => id = value;
        }

        /// <summary>
        /// 年份
        /// </summary>
        public int? Y { get; set; } //
        /// <summary>
        /// 1=国际; 2=国内
        /// </summary>
        public int? G { get; set; } //
        /// <summary>
        /// （仅对国际有效）
        /// 0=不屏蔽; 1=屏蔽无数据项
        /// </summary>
        public int P { get; set; }
        /// <summary>
        /// 搜索大学
        /// </summary>
        public string SF { get; set; }
        public int? I1 { get; set; }
        public int? I2 { get; set; }
        public int? I3 { get; set; }
        /// <summary>
        /// 国内排序方式 1=录取排序; 2=拼音排序
        /// </summary>
        public int? O2 { get; set; }
        public int? II1 { get; set; }

        public IDictionary<string, object> ToDict()
        {
            var dict = new Dictionary<string, object>();
            dict[nameof(this.Sid)] = this.Sid;
            dict[nameof(this.Extid)] = this.Extid;
            if (this.Y != null) dict[nameof(this.Y)] = this.Y;
            if (this.G != null) dict[nameof(this.G)] = this.G;
            if (this.P != 0) dict[nameof(this.P)] = this.P;
            if (!string.IsNullOrEmpty(this.SF)) dict[nameof(this.SF)] = this.SF;
            if (this.I1 != null) dict[nameof(this.I1)] = this.I1;
            if (this.I2 != null) dict[nameof(this.I2)] = this.I2;
            if (this.I3 != null) dict[nameof(this.I3)] = this.I3;
            if (this.O2 != null) dict[nameof(this.O2)] = this.O2;
            if (this.II1 != null) dict[nameof(this.II1)] = this.II1;
            return dict;
        }
    }
}
