using System;
using System.Collections.Generic;
using System.Linq;

namespace PMS.School.Domain.Entities
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SpellCode { get; set; }
        public int Type { get; set; }
        public int Sort { get; set; }
        public long No { get; set; }
    }


    public class TagFlat
    {
        /// <summary>
        /// 三级Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 短链
        /// </summary>
        public int No { get; set; }
        /// <summary>
        /// 三级名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 首字母
        /// </summary>
        public string SpellCode { get; set; }
        /// <summary>
        /// 父级Id(二级分类ID)
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// 父级名称(二级分类名称)
        /// </summary>
        public string ParentName { get; set; }
        /// <summary>
        /// 一级Id
        /// </summary>
        public int RootId { get; set; }
        /// <summary>
        /// 一级名称
        /// </summary>
        public string RootName { get; set; }

        public static List<object> GetTags(List<TagFlat> tags, int rootId)
        {
            var tagsData = tags.Where(q => q.RootId == rootId && q.ParentId > 0)
                .GroupBy(s => new { s.ParentName, s.ParentId })
                .Select(s => (object)new
                {
                    ParentId = s.Key.ParentId,
                    ParentName = string.IsNullOrWhiteSpace(s.Key.ParentName) ? "其它" : s.Key.ParentName,
                    Children = s.Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.No
                    }).ToList()
                })
                .ToList();

            //如果有无父级的项目, 则归于其他, 并加入最后
            var other = tags.Where(q => q.RootId == rootId && q.ParentId <= 0);
            if (other.Any())
            {
                tagsData.Add((object)new
                {
                    ParentId = 0,
                    ParentName = "其它",
                    Children = other.Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.No
                    }).ToList()
                });
            }
            return tagsData;
        }
    }
}
