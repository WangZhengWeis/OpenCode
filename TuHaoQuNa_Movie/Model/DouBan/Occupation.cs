using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DouBan
{
    /// <summary>
    /// 职业
    /// </summary>
    [TableName("tb_Occupation_DouBan")]
    [PrimaryKey("ID")]
    public class Occupation
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 职业名称
        /// </summary>
        public string OccupationName { get; set; }
    }
}
