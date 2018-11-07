using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// 国家
    /// </summary>
    [TableName("tb_Country")]
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
    }

    /// <summary>
    /// 电影类型
    /// </summary>
    [TableName("tb_Category")]
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
    }
}
