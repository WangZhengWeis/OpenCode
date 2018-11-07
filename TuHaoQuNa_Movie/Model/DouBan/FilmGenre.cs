using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DouBan
{
    /// <summary>
    /// 电影类型
    /// </summary>
    [TableName("tb_FilmGenre_DouBan")]
    [PrimaryKey("ID")]
    public class FilmGenre
    {
        public int ID { get; set; }
        /// <summary>
        /// 电影类型名称
        /// </summary>
        public string FilmGenreName { get; set; }
    }
}
