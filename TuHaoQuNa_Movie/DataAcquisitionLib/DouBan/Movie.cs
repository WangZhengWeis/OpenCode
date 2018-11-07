using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcquisitionLib.DouBan
{
    public class Movie
    {
        /// <summary>
        /// 条目id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 中文名
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 原名
        /// </summary>
        public string original_title { get; set; }
        /// <summary>
        /// 又名
        /// </summary>
        public List<string> aka { get; set; }
        /// <summary>
        /// 条目页URL
        /// </summary>
        public string alt { get; set; }
        /// <summary>
        /// 移动版条目页URL
        /// </summary>
        public string mobile_url { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        public Rating rating { get; set; }
        /// <summary>
        /// 评分人数
        /// </summary>
        public int ratings_count { get; set; }
        /// <summary>
        /// 想看人数
        /// </summary>
        public int wish_count { get; set; }
        /// <summary>
        /// 在看人数，如果是电视剧，默认值为0，如果是电影值为null
        /// </summary>
        public int? collect_count { get; set; }
        /// <summary>
        /// 电影海报图
        /// </summary>
        public Images images { get; set; }
        /// <summary>
        /// 条目分类, movie或者tv
        /// </summary>
        public string subtype { get; set; }
        /// <summary>
        /// 导演
        /// </summary>
        public List<Celebrity> directors { get; set; }
        /// <summary>
        /// 主演
        /// </summary>
        public List<Celebrity> casts { get; set; }
        ///// <summary>
        ///// 编剧
        ///// </summary>
        //public List<Celebrity> writers { get; set; }
        ///// <summary>
        ///// 官方网站
        ///// </summary>
        //public string website { get; set; }
        /// <summary>
        /// 豆瓣小站
        /// </summary>
        public string douban_site { get; set; }
        ///// <summary>
        ///// 如果条目类型是电影则为上映日期，如果是电视剧则为首Ï日期
        ///// </summary>
        //public List<string> pubdates { get; set; }
        ///// <summary>
        ///// 大陆上映日期，如果条目类型是电影则为上映日期，如果是电视剧则为首播日期
        ///// </summary>
        //public string mainland_pubdate { get; set; }
        ///// <summary>
        ///// 兼容性数据，未来会去掉，大陆上映日期，如果条目类型是电影则为上映日期，如果是电视剧则为首播日期
        ///// </summary>
        //public string pubdate { get; set; }
        ///// <summary>
        ///// 年代
        ///// </summary>
        //public string year { get; set; }
        ///// <summary>
        ///// 语言
        ///// </summary>
        //public List<string> languages { get; set; }
        ///// <summary>
        ///// 片长
        ///// </summary>
        //public List<string> durations { get; set; }
        /// <summary>
        /// 影片类型，最多提供3个
        /// </summary>
        public List<string> genres { get; set; }
        /// <summary>
        /// 制片国家/地区
        /// </summary>
        public List<string> countries { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        public string summary { get; set; }
        /// <summary>
        /// 短评数量
        /// </summary>
        public int comments_count { get; set; }
        /// <summary>
        /// 影评数量
        /// </summary>
        public int reviews_count { get; set; }
        /// <summary>
        /// 总季数(tv only)
        /// </summary>
        public int? seasons_count { get; set; }
        /// <summary>
        /// 当前季数(tv only)
        /// </summary>
        public int? current_season { get; set; }
        /// <summary>
        /// 当前季的集数(tv only)
        /// </summary>
        public int? episodes_count { get; set; }
        /// <summary>
        /// 影讯页URL(movie only)
        /// </summary>
        public string schedule_url { get; set; }
    }

    /// <summary>
    /// 评分
    /// </summary>
    public class Rating
    {
        /// <summary>
        /// 最低评分
        /// </summary>
        public double min { get; set; }
        /// <summary>
        /// 最高评分
        /// </summary>
        public double max { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        public double average { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string stars { get; set; }
    }
    /// <summary>
    /// 图片
    /// </summary>
    public class Images
    {
        /// <summary>
        /// 228x465px
        /// </summary>
        public string large { get; set; }
        /// <summary>
        /// 96x155px
        /// </summary>
        public string medium { get; set; }
        /// <summary>
        /// 64x103px
        /// </summary>
        public string small { get; set; }
    }
    /// <summary>
    /// 影人
    /// </summary>
    public class Celebrity
    {
        /// <summary>
        /// 影人条目id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 中文名
        /// </summary>
        public string name { get; set; }
        public string alt { get; set; }
        public Images avatars { get; set; }
    }
}
