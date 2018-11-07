using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DouBan
{
    [TableName("tb_Language_DouBan")]
    [PrimaryKey("ID")]
    public class Language
    {
        public int ID { get; set; }
        /// <summary>
        /// 语言名称
        /// </summary>
        public string LanguageName { get; set; }
    }
}
