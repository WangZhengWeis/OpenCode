using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DouBan
{
    [TableName("tb_ReleaseTime_DouBan")]
    [PrimaryKey("ID")]
    public class ReleaseTime
    {
        public int ID { get; set; }
        /// <summary>
        /// 地点
        /// </summary>
        public string Place { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime? ReleaseDate { get; set; }
        /// <summary>
        /// 豆瓣电影ID
        /// </summary>
        public string DouBanID { get; set; }
    }
}
