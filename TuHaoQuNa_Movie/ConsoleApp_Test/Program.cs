using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net;
using Utility;
using Utillity;
using System.Threading;

namespace ConsoleApp_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("开始采集数据了。。。");
            var movie_bll = new BLL.GaoQing.Movie_BLL();
            var collection = new DataAcquisitionLib.iQiYiCollection();
            int page = 1;
            int pageSize = 100;
            int maxPage = 9;
            do
            {
                var pageObj = movie_bll.GetFullHashAndIDByPage(page, pageSize, Model.GaoQing.Family.电影,false, true, "iqiyi");
                maxPage = (int)pageObj.TotalPages;
                pageObj.Items.ForEach(m=> {
                    collection.CollectioniQiYiHtmlMovie(m.ID, m.Name);
                });
                page++;
            } while (page<= maxPage);
            Console.WriteLine("数据采集完毕了。。。");
            Console.ReadLine();
        }
        
    }
}
