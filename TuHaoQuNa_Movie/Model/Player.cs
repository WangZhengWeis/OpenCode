using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [TableName("tb_Player")]
    [PrimaryKey("ID")]
    public class Player
    {
        public int ID { get; set; }

        public int? MovieID { get; set; }

        public string Platform { get; set; }

        public string VID { get; set; }

        public string Name { get; set; }

        public string PlayerUrl { get; set; }

        public string Poster { get; set; }

        public string Power { get; set; }

        public string Category { get; set; }

        public int? Episodes { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastTime { get; set; }
    }
}
