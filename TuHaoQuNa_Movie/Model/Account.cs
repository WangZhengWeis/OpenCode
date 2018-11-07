using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [TableName("tb_Account")]
    [PrimaryKey("ID")]
    public class Account
    {
        public int ID { get; set; }

        public string Email { get; set; }

        public string NickName { get; set; }

        public string Password { get; set; }

        public int Status { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastTime { get; set; }
    }

    [TableName("tb_Key")]
    [PrimaryKey("SignKey", AutoIncrement = false)]
    public class Key
    {
        public string SignKey { get; set; }

        public int Status { get; set; }
    }

}
