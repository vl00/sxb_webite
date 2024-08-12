using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using PMS.School.Application.ModelDto;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.Models
{

    public class SearchArticleVM
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 短链
        /// </summary>
        public string ShortNo { get; set; }

        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 发表时间
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 阅读量
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// 列表布局方式 0 无图 1 小图 2 大图
        /// </summary>
        public byte Layout { get; set; }

        /// <summary>
        /// 封面图片
        /// </summary>
        public string Cover { get; set; }

        public static SearchArticleVM Convert(article s)
        {
            var vm = new SearchArticleVM() {
                Id = s.id,
                ShortNo = UrlShortIdUtil.Long2Base32(s.No),
                Title = s.title,
                Time = s.time.GetValueOrDefault().ConciseTime("yyyy年MM月dd日"),
                ViewCount = s.VirualViewCount,
                Layout = s.layout,
            };

            var c = s.Covers.FirstOrDefault();
            if (c != null)
            {
               vm.Cover = !string.IsNullOrWhiteSpace(c.ImgUrl) ? c.ImgUrl : $"https://cos.sxkid.com/images/article/{c.articleID}/{c.photoID}.{((FileExtension)c.ext).ToString()}";
            }
            return vm;
        }
    }

}
