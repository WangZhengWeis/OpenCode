using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DouBan
{
    public class BillBoard
    {
        public string ID { get; set; }

        public int Sort { get; set; }
    }

    public enum EnumBillBoard
    {
        热门 = 0,
        最新 = 1,
        经典 = 2,
        豆瓣高分 = 3,
        冷门佳片 = 4
    }
}
