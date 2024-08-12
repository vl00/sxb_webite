namespace PMS.School.Domain.Dtos
{
    public class NavigationDto
    {
        /// <summary>
        /// 导航名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string IconUrl { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 跳转地址
        /// </summary>
        public string TargetUrl { get; set; }
    }
}
