using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DouBan
{
    /// <summary>
    /// 电影名称
    /// </summary>
    [TableName("tb_MovieName_DouBan")]
    [PrimaryKey("ID")]
    public class MovieName
    {
        public int ID { get; set; }
        /// <summary>
        /// 电影标题
        /// </summary>
        public string MovieTitle { get; set; }
        /// <summary>
        /// 豆瓣电影ID
        /// </summary>
        public string DouBanID { get; set; }
    }
}
