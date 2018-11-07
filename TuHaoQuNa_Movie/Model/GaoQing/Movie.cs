using ORM;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GaoQing
{

    #region Model

    /// <summary>
    /// 电影
    /// </summary>
    [TableName("tb_Movie_GaoQing")]
    [PrimaryKey("ID")]
    public class BaseMovie
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 业务ID
        /// </summary>
        public string Hash { get; set; }
        /// <summary>
        /// 豆瓣ID
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 电影名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        public decimal? Rate { get; set; }
        /// <summary>
        /// 封面
        /// </summary>
        public string Poster { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int? Nd { get; set; }
        /// <summary>
        /// 类别 电影=1，电视剧=2
        /// </summary>
        public int Family { get; set; }
        /// <summary>
        /// 热度
        /// </summary>
        public int? Heat { get; set; }
        /// <summary>
        /// 集数
        /// </summary>
        public int? Episodes { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 数据创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 数据最后修改时间
        /// </summary>
        public DateTime LastTime { get; set; }

    }

    /// <summary>
    /// 电影
    /// </summary>
    public class Movie : BaseMovie
    {
        /// <summary>
        /// 导演
        /// </summary>
        [Ignore]
        public List<Celebrity> Director { get; set; }
        /// <summary>
        /// 编剧
        /// </summary>
        [Ignore]
        public List<Celebrity> Screenwriter { get; set; }
        /// <summary>
        /// 明星
        /// </summary>
        [Ignore]
        public List<Celebrity> Performer { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        [Ignore]
        public List<Country> Country { get; set; }
        /// <summary>
        /// 时长
        /// </summary>
        [Ignore]
        public List<LongTime> LongTime { get; set; }
        /// <summary>
        /// 上映时间
        /// </summary>
        [Ignore]
        public List<ReleaseTime> ReleaseTime { get; set; }
        /// <summary>
        /// 更多名称
        /// </summary>
        [Ignore]
        public List<MovieName> MoreName { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        [Ignore]
        public List<Category> Category { get; set; }
        /// <summary>
        /// 季
        /// </summary>
        [Ignore]
        public List<Season> Season { get; set; }

    }

    /// <summary>
    /// 电视剧季组
    /// </summary>
    [TableName("tb_Season_GaoQing")]
    public class Season
    {
        public string SeasonID { get; set; }

        public int? MovieID { get; set; }

        public string Hash { get; set; }

        public int SeasonNum { get; set; }
    }

    /// <summary>
    /// 名人
    /// </summary>
    [TableName("tb_Celebrity_GaoQing")]
    [PrimaryKey("ID")]
    public class Celebrity
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        
    }

    /// <summary>
    /// 国家
    /// </summary>
    [TableName("tb_Country_GaoQing")]
    [PrimaryKey("ID")]
    public class Country
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 国家ID
        /// </summary>
        public int? CID { get; set; }
    }

    /// <summary>
    /// 片长
    /// </summary>
    [TableName("tb_LongTime_GaoQing")]
    public class LongTime
    {
        /// <summary>
        /// 电影ID
        /// </summary>
        public int MovieID { get; set; }
        /// <summary>
        /// 分钟
        /// </summary>
        public int Minute { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        public string Country { get; set; }
    }

    /// <summary>
    /// 上映时间
    /// </summary>
    [TableName("tb_ReleaseTime_GaoQing")]
    public class ReleaseTime
    {
        /// <summary>
        /// 电影ID
        /// </summary>
        public int MovieID { get; set; }
        /// <summary>
        /// 地点
        /// </summary>
        public string Place { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime? ReleaseDate { get; set; }
    }

    /// <summary>
    /// 电影类型
    /// </summary>
    [TableName("tb_Category_GaoQing")]
    [PrimaryKey("ID")]
    public class Category
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 类型ID
        /// </summary>
        public int? CID { get; set; }
    }

    /// <summary>
    /// 电影名称
    /// </summary>
    [TableName("tb_MovieName_GaoQing")]
    public class MovieName
    {
        /// <summary>
        /// 电影ID
        /// </summary>
        public int MovieID { get; set; }
        /// <summary>
        /// 电影标题
        /// </summary>
        public string Name { get; set; }
    }

    [TableName("tb_MovieTorrent_GaoQing")]
    [PrimaryKey("ID")]
    public class MovieTorrent
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 电影ID
        /// </summary>
        public int MovieID { get; set; }
        /// <summary>
        /// 磁力
        /// </summary>
        public string Magnet { get; set; }
        /// <summary>
        /// 种子
        /// </summary>
        public string Torrent { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 大小 KB
        /// </summary>
        public decimal? Size { get; set; }
        /// <summary>
        /// 清晰度
        /// </summary>
        public string Clear { get; set; }
        /// <summary>
        /// 清晰度描述
        /// </summary>
        public string Meta { get; set; }
        /// <summary>
        /// 数据创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 字幕
    /// </summary>
    [TableName("tb_MovieSubtitle_GaoQing")]
    [PrimaryKey("ID")]
    public class Subtitle
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 电影ID
        /// </summary>
        public int MovieID { get; set; }
        /// <summary>
        /// 下载地址
        /// </summary>
        public string Download { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 大小 KB
        /// </summary>
        public decimal? Size { get; set; }
        /// <summary>
        /// 清晰度
        /// </summary>
        public string Clear { get; set; }
        /// <summary>
        /// 语言
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// 语言描述
        /// </summary>
        public string Meta { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 格式
        /// </summary>
        public string Format { get; set; }
        /// <summary>
        /// 数据创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

    }

    [TableName("tb_Relation_MoviePerformer_GaoQing")]
    public class Relation_MoviePerformer
    {
        public int MovieID { get; set; }

        public int CelebrityID { get; set; }
    }

    [TableName("tb_Relation_MovieDirector_GaoQing")]
    public class Relation_MovieDirector
    {
        public int MovieID { get; set; }

        public int CelebrityID { get; set; }
    }

    [TableName("tb_Relation_MovieScreenwriter_GaoQing")]
    public class Relation_MovieScreenwriter
    {
        public int MovieID { get; set; }

        public int CelebrityID { get; set; }
    }

    [TableName("tb_Relation_MovieCountry_GaoQing")]
    public class Relation_MovieCountry
    {
        public int MovieID { get; set; }

        public int CountryID { get; set; }
    }

    [TableName("tb_Relation_MovieCategory_GaoQing")]
    public class Relation_MovieCategory
    {
        public int MovieID { get; set; }

        public int CategoryID { get; set; }
    }

    #endregion


    #region Enum

    /// <summary>
    /// 类别
    /// </summary>
    public enum Family
    {
        电影 = 1,
        电视剧 = 2
    }

    /// <summary>
    /// 电影排序
    /// </summary>
    public enum MovieOrderBy
    {
        热门 = 1,
        评分 = 2,
        最新 = 3,
        可在线播放 = 4
    }

    #endregion

}
