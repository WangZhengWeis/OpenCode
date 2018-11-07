using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Utility;
using Utillity;

namespace DataAcquisitionLib
{
    
    public class RARGBCollection
    {

        private event EventHandler OnCollectionTorrentClear;//数据删除完成引发的事件

        HttpWebResponseUtility hwru;
        BLL.RARGB.Torrent_BLL rargb_torrent_bll;
        public RARGBCollection()
        {
            hwru = new HttpWebResponseUtility();
            rargb_torrent_bll = new BLL.RARGB.Torrent_BLL();
        }


        public void CollectionRARGBHtmlMovieList(int page, out int lastpage, string url = "https://rarbg.is/torrents.php?category=movies", int timeout = 30000)
        {
            var res_str = "";
            CookieCollection cookie = new CookieCollection();
            url += ("&page=" + page);
            lastpage = 999999999;
            try
            {
                //var list = new List<Model.RARGB.Torrent>();
                var request = hwru.SendRequest(url, null, null, null, null, "GET", out res_str, out cookie, Encoding.UTF8, timeout, true);
                if (request == null || request.StatusCode != HttpStatusCode.OK || res_str.Contains("Please wait while we try to verify your browser...") || res_str.Contains("We have too many requests from your ip in the past 24h"))
                {
                    if (request.StatusCode == HttpStatusCode.NotFound)
                    {
                        throw new Exception();
                    }
                    if (res_str.Contains("Please wait while we try to verify your browser...") || res_str.Contains("We have too many requests from your ip in the past 24h"))
                    {
                        ChangeIP();
                    }
                    CollectionRARGBHtmlMovieList(page, out lastpage);
                }
                else
                {
                    HtmlDocument hd = new HtmlDocument();
                    hd.LoadHtml(res_str);
                    var tr_nodes = hd.DocumentNode.SelectNodes("//tr[@class='lista2']");
                    if (tr_nodes!=null)
                        foreach (var item in tr_nodes)
                        {
                            var a_node = item.SelectSingleNode("./td[2]/a[1]");
                            if (a_node!=null)
                            {
                                var title = a_node.InnerText.Trim();
                                var href = a_node.GetAttributeValue("href","").Trim();
                                var token = href.Substring(href.LastIndexOf("/")+1);
                                if (!string.IsNullOrWhiteSpace(token))
                                {
                                    if (new BLL.RARGB.Torrent_BLL().InsertToken(new Model.RARGB.RAGGBToken()
                                    {
                                        Token = token,
                                        ActionStatus = 0
                                    }) != null)
                                    {
                                        Console.WriteLine("Token:" + token + "\t写入成功！");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Token:" + token + "\t写入失败！");
                                    }
                                    ////先判断Token存不存在
                                    //var obj = rargb_torrent_bll.SingleTorrent(token);
                                    //if (obj==null)
                                    //{
                                    //    var thread = new Thread(new ParameterizedThreadStart(Run_CollectionRARGBHtmlMovieTorrent));
                                    //    thread.Name = token;
                                    //    //OnCollectionTorrentClear += new EventHandler(Thread_OnCollectionTorrentClear);
                                    //    thread.Start(token);
                                    //    //Console.WriteLine("正在采集："+token);
                                    //    //var model = CollectionRARGBHtmlMovieTorrent(token);
                                    //    //if (model != null)
                                    //    //{
                                    //    //    list.Add(model);
                                    //    //}
                                    //}
                                }
                            }
                        }
                    var lastpage_node = hd.DocumentNode.SelectSingleNode("//div[@id='pager_links']/*[last()]");
                    if (lastpage_node!=null)
                        lastpage = Utils.StrToInt(lastpage_node.InnerText.Trim(), lastpage);
                }
                //return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public Model.RARGB.Torrent CollectionRARGBHtmlMovieTorrent(string token, int timeout = 30000)
        {
            var res_str = "";
            CookieCollection cookie = new CookieCollection();
            var url = "https://rarbg.is/torrent/" + token;
            try
            {
                var model = new Model.RARGB.Torrent();
                model.Token = token;
                var request = hwru.SendRequest(url, null, null, null, null, "GET", out res_str, out cookie, Encoding.UTF8, timeout, true);
                if (request == null || request.StatusCode != HttpStatusCode.OK || res_str.Contains("Please wait while we try to verify your browser...")||res_str.Contains("We have too many requests from your ip in the past 24h"))
                {
                    if (request.StatusCode == HttpStatusCode.NotFound)
                    {
                        throw new Exception();
                    }
                    if (res_str.Contains("Please wait while we try to verify your browser...")|| res_str.Contains("We have too many requests from your ip in the past 24h"))
                    {
                        ChangeIP();
                    }
                    return CollectionRARGBHtmlMovieTorrent(token);
                }
                else
                {
                    HtmlDocument hd = new HtmlDocument();
                    hd.LoadHtml(res_str);
                    //name
                    var name_node = hd.DocumentNode.SelectSingleNode("//h1[@itemprop='name']");
                    if (name_node != null)
                    {
                        //name
                        var name = name_node.InnerText.Trim();
                        model.Title = name;
                    }
                    Console.WriteLine("正在分析："+ model.Title);
                    //download
                    var torrent_td_node = hd.DocumentNode.SelectSingleNode("//td[text()=' Torrent:']");
                    if (torrent_td_node != null)
                    {
                        var torrent_node = torrent_td_node.ParentNode.SelectSingleNode("./td[last()]");
                        if (torrent_node!=null)
                        {
                            var torrent_a_node = torrent_node.SelectSingleNode("./a[1]");
                            var magnet_a_node = torrent_node.SelectSingleNode("./a[2]");
                            if (torrent_a_node!=null)
                            {
                                //https://rarbg.is
                                var torrent_url = torrent_a_node.GetAttributeValue("href", "").Trim();
                                torrent_url = string.IsNullOrWhiteSpace(torrent_url) ? null : "https://rarbg.is" + torrent_url;
                                model.TorrentUrl = torrent_url;
                            }
                            if (magnet_a_node != null)
                            {
                                var magnet_url = magnet_a_node.GetAttributeValue("href", "").Trim();
                                model.MagnetUrl = magnet_url;
                                //magnet:?xt=urn:btih:84e8b76691d27356dcb5b898ec3a8d7cfc5435f4&dn=The.Lifeguard.2013.1080p.BluRay.H264.AAC-RARBG&tr=http%3A%2F%2Ftracker.trackerfix.com%3A80%2Fannounce&tr=udp%3A%2F%2F9.rarbg.me%3A2710&tr=udp%3A%2F%2F9.rarbg.to%3A2710
                                var magnet_token = magnet_url.Replace("magnet:?xt=urn:btih:", "").Trim();
                                if (!string.IsNullOrWhiteSpace(magnet_token))
                                    magnet_token = magnet_token.Contains("&") ? magnet_token.Substring(0, magnet_token.IndexOf("&")) : magnet_token;
                                model.MagnetHash = magnet_token;
                            }
                        }
                    }
                    //Poster
                    var poster_td_node = hd.DocumentNode.SelectSingleNode("//td[text()=' Poster:']");
                    if (poster_td_node!=null)
                    {
                        var poster_node = poster_td_node.ParentNode.SelectSingleNode("./td[last()]/img");
                        if (poster_node!=null)
                        {
                            var poster_url = poster_node.GetAttributeValue("src", "").Trim();
                            poster_url = string.IsNullOrWhiteSpace(poster_url) ? null : (poster_url.Contains("http") ? poster_url : "http:" + poster_url);
                            model.Poster = poster_url;
                        }
                    }
                    //IMDb
                    var imdb_img_node = hd.DocumentNode.SelectSingleNode("//img[@src='//dyncdn.me/static/20/img/imdb3.png']");
                    if (imdb_img_node!=null)
                    {
                        var imdb_node = imdb_img_node.ParentNode.ParentNode.SelectSingleNode("./td[last()]/a");
                        if (imdb_node!=null)
                        {
                            var imdb_url = imdb_node.InnerText.Trim();
                            var imdb_id = string.IsNullOrWhiteSpace(imdb_url) ? null : imdb_url.Replace("http://imdb.com/title/", "").Replace("/","").Trim();
                            model.IMDbID = imdb_id;
                        }
                    }
                    //Category
                    var category_td_node = hd.DocumentNode.SelectSingleNode("//td[text()=' Category:']");
                    if (category_td_node!=null)
                    {
                        var category_node = category_td_node.ParentNode.SelectSingleNode("./td[last()]/a");
                        if (category_node!=null)
                        { 
                            var category_str = category_node.InnerText.Trim();
                            if (!string.IsNullOrWhiteSpace(category_str))
                            {
                                model.Category = new Model.RARGB.Category() { CategoryName = category_str };
                            }
                        }
                    }
                    //Size
                    var size_td_node = hd.DocumentNode.SelectSingleNode("//td[text()=' Size:']");
                    if (size_td_node!=null)
                    {
                        var size_node = size_td_node.ParentNode.SelectSingleNode("./td[last()]");
                        if (size_node!=null)
                        {
                            var size_str = size_node.InnerText.Trim();
                            double? size = null;
                            try
                            {
                                size = string.IsNullOrWhiteSpace(size_str) ? 0 : ((size_str.Contains("GB") ? Utils.StrToFloat(size_str.Replace("GB", "").Trim(), 0) : ((size_str.Contains("MB") ? Utils.StrToFloat(size_str.Replace("MB", "").Trim(), 0) : (size_str.Contains("KB") ? Utils.StrToFloat(size_str.Replace("KB", "").Trim(), 0) : 0)) * 1024.0)) * 1024.0 * 1024.0);
                                size = size == 0 ? null : size;
                            }
                            catch (Exception)
                            {
                                size = null;
                            }
                            model.Size = size;
                        }
                    }
                    //Added
                    var added_td_node = hd.DocumentNode.SelectSingleNode("//td[text()=' Added:']");
                    if (added_td_node!=null)
                    {
                        var added_node = added_td_node.ParentNode.SelectSingleNode("./td[last()]/span");
                        if (added_node!=null)
                        {
                            DateTime? added_time = null;
                            try
                            {
                                added_time = Convert.ToDateTime(added_node.InnerText.Trim());
                            }
                            catch (Exception)
                            { 
                            }
                            model.Added = added_time;
                        }
                    }

                }
                return model;
            }
            catch (Exception ex)
            {
                return CollectionRARGBHtmlMovieTorrent(token);
            }
        }

        public void Run_CollectionRARGBHtmlMovieTorrent(object obj)
        {
            try
            {
                string token = obj.ToString();
                Console.WriteLine("线程:" + Thread.CurrentThread.Name + "\t正在采集：" + token);
                var model = CollectionRARGBHtmlMovieTorrent(token);
                if (model != null)
                {
                    if (new BLL.RARGB.Torrent_BLL().Insert(model) > 0)
                        Console.WriteLine("线程:" + Thread.CurrentThread.Name + "\t" + model.Title + "\t写入数据库成功！");
                    else
                        Console.WriteLine("线程:" + Thread.CurrentThread.Name + "\t" + model.Title + "\t写入数据库失败！");
                }

            }
            catch (Exception)
            {
            }
            finally
            {
                //OnCollectionTorrentClear(this, new EventArgs());//引发完成事件
                //Monitor.Exit(this);//取消锁定
            }
        }

        //void Thread_OnCollectionTorrentClear(object sender, EventArgs e)
        //{
        //    Console.WriteLine("执行完了，停止了所有线程的执行。");
        //    //threadTwo.Abort();
        //    //threadOne.Abort();

        //}

        private void ChangeIP(int time = 3000)
        {
            
            try
            {
                Monitor.Enter(this);//锁定，保持同步


                //6186d3eefba2d6f4b9cc56e0c158e8ba

                //Console.WriteLine("开始更换IP...");
                ////断线
                //var str_res = hwru.Get("http://192.168.31.1/cgi-bin/luci/;stok=6186d3eefba2d6f4b9cc56e0c158e8ba/api/xqnetwork/pppoe_stop", "", Encoding.UTF8);
                //Console.WriteLine(str_res);
                //Thread.Sleep(time);
                //str_res = hwru.Get("http://192.168.31.1/cgi-bin/luci/;stok=6186d3eefba2d6f4b9cc56e0c158e8ba/api/xqnetwork/pppoe_start", "", Encoding.UTF8);

                //Console.WriteLine(str_res);
                //Console.WriteLine("更换IP完毕");
                //Thread.Sleep(time);

                //http://192.168.31.1/cgi-bin/luci/;stok=a97bafa48609a54c8bebcf5c894c7833/api/xqnetwork/pppoe_status

                Console.WriteLine("开始更换IP...");
                //断开连接并重新拨号
                //断线
                RASDisplay ras = new RASDisplay();
                ras.Disconnect();
                Thread.Sleep(time);
                ras.Connect("ADSL");
                Console.WriteLine("更换IP完毕");

                Monitor.Exit(this);//取消锁定

            }
            catch (Exception err)
            {
                Console.WriteLine("更换IP失败:" + err.Message);
            }
        }

    }
}
