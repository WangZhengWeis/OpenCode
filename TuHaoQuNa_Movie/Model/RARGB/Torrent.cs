using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.RARGB
{
    [TableName("tb_Torrent_RARGB")]
    [PrimaryKey("ID")]
    public class Torrent
    {
        public int ID { get; set; }

        public string Token { get; set; }

        public string Title { get; set; }

        public string TorrentUrl { get; set; }

        public string MagnetUrl { get; set; }
        public string MagnetHash { get; set; }

        public string Poster { get; set; }

        public string IMDbID { get; set; }

        public int? CategoryID { get; set; }
        [ResultColumn]
        public Category Category { get; set; }

        public double? Size { get; set; }

        public DateTime? Added { get; set; }
    }

    [TableName("tb_Category_RARGB")]
    [PrimaryKey("CategoryID")]
    public class Category
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; }
    }


    [TableName("tb_Token_RARGB")]
    [PrimaryKey("Token", AutoIncrement = false)]
    public class RAGGBToken
    {
        public string Token { get; set; }

        public int ActionStatus { get; set; }
    }
}
