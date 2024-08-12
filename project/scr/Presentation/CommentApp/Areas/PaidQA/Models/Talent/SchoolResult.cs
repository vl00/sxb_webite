using iSchool;
using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using PMS.School.Infrastructure.Common;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Models.Talent
{
    public class SchoolResult : SchoolExtDto
    {

        /// 学校图册
        /// </summary>
        public List<KeyValueDto<string, string, string>> SchoolPhotos { get; set; }


        /// <summary>
        /// 品牌图
        /// </summary>
        public List<SchoolImageDto> SchoolBrands { get; set; }

        /// <summary>
        /// 视频
        /// </summary>
        public List<KeyValueDto<DateTime, string, byte, string, string>> SchoolVideos { get; set; }


        /// <summary>
        /// 学校类型
        /// </summary>
        public string SchoolType
        {
            get
            {
                return new SchFType0(this.Grade, this.Type, this.Discount, this.Diglossia, this.Chinese).GetDesc();
            }
        }

        /// <summary>
        /// 走读寄宿描述
        /// </summary>
        public string LodgingDesc
        {
            get
            {
                var lodgingType = LodgingUtil.Reason(this.Lodging, this.Sdextern);
                var lodgingDesc = ProductManagement.Framework.Foundation.EnumExtension.Description(lodgingType);
                return lodgingDesc;
            }
        }

        /// <summary>
        /// 学校认证
        /// </summary>
        public List<KeyValueDto<string>> Authentications
        {
            get
            {

                var authentication = string.IsNullOrEmpty(this.Authentication) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(this.Authentication);
                return authentication;
            }
        }

        public string TeaProportion
        {
            get
            {
                if (this.TsPercent != null)
                {
                    return $"1:{this.TsPercent}";
                }
                else {
                    return null;
                }
                
            }
        }
        public string ShortId { get {
                return UrlShortIdUtil.Long2Base32(this.SchoolNo);
            } }
    }
}
