using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DouBan
{
    /// <summary>
    /// 名人
    /// </summary>
    [TableName("tb_Celebrity_DouBan")]
    [PrimaryKey("CelebrityID", AutoIncrement = false)]
    public class Celebrity
    {
        /// <summary>
        /// ID
        /// </summary>
        public string CelebrityID { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别  -1未知    0女  1男
        /// </summary>
        public int? Gender { get; set; }
        /// <summary>
        /// 星座
        /// </summary>
        public string Constellation { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// 出生地
        /// </summary>
        public string Homeplace { get; set; }
        /// <summary>
        /// 职业
        /// </summary>
        [ResultColumn]
        public List<Occupation> Occupations { get; set; }
        /// <summary>
        /// 更多外文名
        /// </summary>
        public string MoreNameEn { get; set; }
        /// <summary>
        /// 更多中文名
        /// </summary>
        public string MoreNameZh { get; set; }
        /// <summary>
        /// 照片
        /// </summary>
        [ResultColumn]
        public Image Picture { get; set; }
        /// <summary>
        /// 照片ID
        /// </summary>
        public int? PictureID { get; set; }
        /// <summary>
        /// IMDb编号
        /// </summary>
        public string IMDbID { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        public string Describe { get; set; }
    }

    [TableName("tb_Relation_CelebrityOccupation_DouBan")]
    [PrimaryKey("ID")]
    public class Relation_CelebrityOccupation
    {
        public int ID { get; set; }

        public string CelebrityID { get; set; }

        public int OccupationID { get; set; }
    }
}
