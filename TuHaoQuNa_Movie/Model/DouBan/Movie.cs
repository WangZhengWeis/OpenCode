using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DouBan
{
    public class Movie : BaseMovie
    {
        /// <summary>
        /// 导演
        /// </summary>
        [ResultColumn]
        public List<Celebrity> Director { get; set; }
        /// <summary>
        /// 明星
        /// </summary>
        [ResultColumn]
        public List<Celebrity> Performer { get; set; }
        /// <summary>
        /// 编剧
        /// </summary>
        [ResultColumn]
        public List<Celebrity> Screenwriter { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [ResultColumn]
        public List<FilmGenre> FilmGenres { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        [ResultColumn]
        public List<Country> Countrys { get; set; }
        /// <summary>
        /// 语言
        /// </summary>
        [ResultColumn]
        public List<Language> Languages { get; set; }
        /// <summary>
        /// 上映时间
        /// </summary>
        [ResultColumn]
        public List<ReleaseTime> ReleaseTimes { get; set; }
        
        /// <summary>
        /// 更多名称
        /// </summary>
        [ResultColumn]
        public List<MovieName> MoreName { get; set; }
        
        /// <summary>
        /// 海报
        /// </summary>
        [ResultColumn]
        public Image Poster { get; set; }
        
    }
    [TableName("tb_Movie_DouBan")]
    [PrimaryKey("DouBanID", AutoIncrement = false)]
    public class BaseMovie
    {
        /// <summary>
        /// 豆瓣ID
        /// </summary>
        public string DouBanID { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        ///// <summary>
        ///// 原名
        ///// </summary>
        //public string OriginalTitle { get; set; }
        ///// <summary>
        ///// 评分人数
        ///// </summary>
        //public int RatingsCount { get; set; }
        ///// <summary>
        ///// 想看人数
        ///// </summary>
        //public int WishCount { get; set; }
        ///// <summary>
        ///// 看过人数
        ///// </summary>
        //public int CollectCount { get; set; }

        /// <summary>
        /// 片长
        /// </summary>
        public int? LongTime { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        public double? Score { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 海报ID
        /// </summary>
        public int? PosterID { get; set; }
        /// <summary>
        /// IMDbID
        /// </summary>
        public string IMDbID { get; set; }
    }


    [TableName("tb_Relation_MovieLanguage_DouBan")]
    [PrimaryKey("ID")]
    public class Relation_MovieLanguage
    {
        public int ID { get; set; }
        public int LanguageID { get; set; }
        public string DouBanID { get; set; }
    }

    [TableName("tb_Relation_MovieDirector_DouBan")]
    [PrimaryKey("ID")]
    public class Relation_MovieDirector
    {
        public int ID { get; set; }
        public string DirectorID { get; set; }
        public string DouBanID { get; set; }
    }


    [TableName("tb_Relation_MoviePerformer_DouBan")]
    [PrimaryKey("ID")]
    public class Relation_MoviePerformer
    {
        public int ID { get; set; }
        public string PerformerID { get; set; }
        public string DouBanID { get; set; }
    }

    [TableName("tb_Relation_MovieScreenwriter_DouBan")]
    [PrimaryKey("ID")]
    public class Relation_MovieScreenwriter
    {
        public int ID { get; set; }
        public string ScreenwriterID { get; set; }
        public string DouBanID { get; set; }
    }

    [TableName("tb_Relation_MovieFilmGenre_DouBan")]
    [PrimaryKey("ID")]
    public class Relation_MovieFilmGenre
    {
        public int ID { get; set; }
        public int FilmGenreID { get; set; }
        public string DouBanID { get; set; }
    }

    [TableName("tb_Relation_MovieCountry_DouBan")]
    [PrimaryKey("ID")]
    public class Relation_MovieCountry
    {
        public int ID { get; set; }
        public int CountryID { get; set; }
        public string DouBanID { get; set; }
    }

}
