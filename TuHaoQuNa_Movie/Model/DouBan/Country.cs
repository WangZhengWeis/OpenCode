using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DouBan
{
    /// <summary>
    /// 国家
    /// </summary>
    [TableName("tb_Country_DouBan")]
    [PrimaryKey("ID")]
    public class Country
    {
        public int ID { get; set; }
        /// <summary>
        /// 国家名称
        /// </summary>
        public string CountryName { get; set; }
    }
}
