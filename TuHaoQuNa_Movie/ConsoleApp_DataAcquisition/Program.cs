using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace ConsoleApp_DataAcquisition
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("1、GaoQingList数据采集");
            Console.WriteLine("2、GaoQing详细信息采集");
            Console.WriteLine("3、GaoQing详细信息采集，只采集没有下载地址的");
            Console.WriteLine("4、采集优酷电影，只采集没有播放地址的");
            Console.WriteLine("5、采集优酷电视剧");
            Console.WriteLine("请输入功能编号:");
            string str = Console.ReadLine();
            switch (str)
            {
                case "1":
                    T1();
                    break;
                case "2":
                    T2();
                    break;
                case "3":
                    T3();
                    break;
                case "4":
                    T4();
                    break;
                case "5":
                    T5();
                    break;
            }
            Console.ReadLine();

        }
        static void T1()
        {
            do
            {
                var collection = new DataAcquisitionLib.GaoQingCollection();
                collection.CollectionMovies(Model.GaoQing.Family.电影);
                collection.CollectionMovies(Model.GaoQing.Family.电视剧);
                Console.WriteLine("休息2个小时...");
                System.Threading.Thread.Sleep(1000*60*60*2);
            } while (true);
        }

        static void T2()
        {
            BLL.GaoQing.Movie_BLL bll = new BLL.GaoQing.Movie_BLL();
            var collection = new DataAcquisitionLib.GaoQingCollection();

            collection.CollectionMovieInfo(40390, "fa635b901ee1");

            do
            {
                var list = bll.GetFullHashAndID();
                list.ForEach(m =>
                {
                    collection.CollectionMovieInfo(m.ID, m.Hash);
                });
            } while (true);
        }

        static void T3()
        {
            BLL.GaoQing.Movie_BLL bll = new BLL.GaoQing.Movie_BLL();
            var collection = new DataAcquisitionLib.GaoQingCollection();
            do
            {
                int page = 1;
                int pageSize = 100;
                int maxPage = 9;
                do
                {
                    var pageObj = bll.GetFullHashAndIDByPage(page, pageSize, null);
                    maxPage = (int)pageObj.TotalPages;
                    var list = pageObj.Items;
                    list.ForEach(m =>
                    {
                        collection.CollectionMovieInfo(m.ID, m.Hash);
                    });
                    page++;
                } while (page <= maxPage);
            } while (true);
        }

        static void T4()
        {
            var movie_bll = new BLL.GaoQing.Movie_BLL();
            var collection = new DataAcquisitionLib.YouKuCollection();
            do
            {
                int page = 1;
                int pageSize = 100;
                int maxPage = 9;
                do
                {
                    var pageObj = movie_bll.GetFullHashAndIDByPage(page, pageSize, Model.GaoQing.Family.电影, false, true, "youku");
                    maxPage = (int)pageObj.TotalPages;
                    pageObj.Items.ForEach(m => {
                        collection.CollectionYouKuHtmlMovie(m.ID, m.Name);
                    });
                    page++;
                } while (page <= maxPage);
            } while (true);
        }

        static void T5()
        {
            var movie_bll = new BLL.GaoQing.Movie_BLL();
            var collection = new DataAcquisitionLib.YouKuCollection();
            do
            {
                int page = 1;
                int pageSize = 100;
                int maxPage = 9;
                do
                {
                    var pageObj = movie_bll.GetFullHashAndIDByPage(page, pageSize, Model.GaoQing.Family.电视剧, false, false);
                    maxPage = (int)pageObj.TotalPages;
                    pageObj.Items.ForEach(m => {
                        collection.CollectionYouKuHtmlTv(m.ID, m.Name);
                    });
                    page++;
                } while (page <= maxPage);
            } while (true);
        }

    }
}
