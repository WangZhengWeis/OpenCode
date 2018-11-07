using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DouBan
{
    [TableName("tb_Image_DouBan")]
    [PrimaryKey("ID")]
    public class Image
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 228x465px
        /// </summary>
        public string Large { get; set; }
        /// <summary>
        /// 96x155px
        /// </summary>
        public string Medium { get; set; }
        /// <summary>
        /// 64x103px
        /// </summary>
        public string Small { get; set; }
    }
}
