using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Utility;

namespace DataAcquisitionLib
{
    public class YouKuCollection
    {

        HttpWebResponseUtility hwru;
        BLL.Player_BLL player_bll;
        public YouKuCollection()
        {
            hwru = new HttpWebResponseUtility();
            player_bll = new BLL.Player_BLL();
        }


        public void CollectionYouKuHtmlMovie(int movieID, string name, int timeout = 30000)
        {
            var res_str = "";
            CookieCollection cookie = new CookieCollection();
            var url = "http://www.soku.com/search_video/q_" + name;
            try
            {
                var request = hwru.SendRequest(url, null, null, null, null, "GET", out res_str, out cookie, Encoding.UTF8, timeout, true);
                if (request == null || request.StatusCode != HttpStatusCode.OK)
                {
                    if (request.StatusCode == HttpStatusCode.NotFound)
                    {
                        throw new Exception();
                    }
                    //return CollectionYouKuHtmlMovie(name);
                }
                else
                {
                    var list = AnalysisPlayer(res_str);
                    list.ForEach(m=> {
                        if (name == m.Name)
                            m.MovieID = movieID;
                        if (player_bll.Save(m)>0)
                            Console.WriteLine(m.Name+"\t写入数据库成功!");
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Model.Player> AnalysisPlayer(string html)
        {
            try
            {
                List<Model.Player> list = new List<Model.Player>();
                HtmlDocument hd = new HtmlDocument();
                hd.LoadHtml(html);
                var movie_nodes = hd.DocumentNode.SelectNodes("//div[@class='s_movie clearfix']");
                if (movie_nodes != null)
                    foreach (var item in movie_nodes)
                    {
                        var play_node = item.SelectSingleNode("./div[@class='s_inform ']/div[@class='s_detail']/div[@class='s_base']/h2[@class='base_name']/a");
                        if (play_node != null)
                        {
                            var play_url = play_node.GetAttributeValue("href", "");
                            if (play_url.Contains("http://cps.youku.com/redirect.html"))
                            {
                                play_url = play_url.Substring(play_url.IndexOf("url=") + 4);
                                play_url = Utils.UrlDecode(play_url);
                            }
                            if (play_url.Contains("http://v.youku.com/v_show/id_"))
                            {
                                Model.Player model = new Model.Player();

                                var title = play_node.GetAttributeValue("_log_title", "");
                                Console.WriteLine("电影:" + title);
                                model.Name = title;

                                play_url = play_url.Replace("==", "");
                                if (play_url.IndexOf("?") > 0)
                                    play_url = play_url.Substring(0, play_url.IndexOf("?"));
                                Console.WriteLine("播放地址:" + play_url);
                                model.PlayerUrl = play_url;

                                var vid = play_url.Replace("http://v.youku.com/v_show/id_", "");
                                if (vid.IndexOf(".") > 0)
                                    vid = vid.Substring(0, vid.IndexOf("."));
                                Console.WriteLine("视频ID:" + vid);
                                model.VID = vid;

                                model.Poster = null;
                                var poster_node = item.SelectSingleNode("./div[@class='s_poster']/div[@class='s_target']/img");
                                if (poster_node != null)
                                {
                                    var poster = poster_node.GetAttributeValue("src", "").Trim();
                                    Console.WriteLine("封面地址:" + poster);
                                    model.Poster = poster;
                                }

                                model.Power = null;
                                var power_node = item.SelectSingleNode("./div[@class='s_poster']/div[@class='s_tagrt']/span[@class='tip_bk']");
                                if (power_node != null)
                                {
                                    var power = power_node.InnerText.Trim();
                                    Console.WriteLine("播放权限:" + power);
                                    model.Power = power;
                                }
                                var type_node = item.SelectSingleNode("./div[@class='s_inform ']/div[@class='s_detail']/div[@class='s_base']/span[@class='base_type']");
                                if (type_node != null)
                                {
                                    var type = type_node.InnerText.Trim();
                                    Console.WriteLine("类型:" + type);
                                    model.Category = type;
                                }
                                model.CreateTime = DateTime.Now;
                                model.LastTime = DateTime.Now;
                                model.Episodes = null;
                                model.Platform = "youku";
                                list.Add(model);
                            }
                        }
                    }
                return list;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void CollectionYouKuHtmlTv(int movieID, string name, int timeout = 30000)
        {
            var res_str = "";
            CookieCollection cookie = new CookieCollection();
            var url = "http://www.soku.com/search_video/q_" + name;
            try
            {
                var request = hwru.SendRequest(url, null, null, null, null, "GET", out res_str, out cookie, Encoding.UTF8, timeout, true);
                if (request == null || request.StatusCode != HttpStatusCode.OK)
                {
                    if (request.StatusCode == HttpStatusCode.NotFound)
                    {
                        throw new Exception();
                    }
                    //return CollectionYouKuHtmlMovie(name);
                }
                else
                {
                    var list = AnalysisPlayerTv(res_str);
                    list.ForEach(m => {
                        if (name == m.Name)
                            m.MovieID = movieID;
                        if (player_bll.Save(m) > 0)
                            Console.WriteLine(m.Name+" 第"+m.Episodes.Value+"集" + "\t写入数据库成功!");
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Model.Player> AnalysisPlayerTv(string html)
        {
            try
            {
                List<Model.Player> list = new List<Model.Player>();
                HtmlDocument hd = new HtmlDocument();
                hd.LoadHtml(html);
                var movie_nodes = hd.DocumentNode.SelectNodes("//div[@class='s_tv clearfix']");
                if (movie_nodes != null)
                    foreach (var item in movie_nodes)
                    {
                        var play_node = item.SelectSingleNode("./div[@class='s_inform  s_madeform']/div[@class='s_detail']/div[@class='s_base']/h2[@class='base_name']/a");
                        if (play_node != null)
                        {
                            var play_url = play_node.GetAttributeValue("href", "");
                            if (play_url.Contains("http://cps.youku.com/redirect.html"))
                            {
                                play_url = play_url.Substring(play_url.IndexOf("url=") + 4);
                                play_url = Utils.UrlDecode(play_url);
                            }
                            if (play_url.Contains("http://v.youku.com/v_show/id_"))
                            {
                                Model.Player model = new Model.Player();

                                var title = play_node.GetAttributeValue("_log_title", "");
                                Console.WriteLine("电视剧:" + title);
                                model.Name = title;

                                model.Poster = null;
                                var poster_node = item.SelectSingleNode("./div[@class='s_poster']/div[@class='s_thumb']/div[@class='s_target']/img");
                                if (poster_node != null)
                                {
                                    var poster = poster_node.GetAttributeValue("src", "").Trim();
                                    Console.WriteLine("封面地址:" + poster);
                                    model.Poster = poster;
                                }

                                model.Power = null;
                                var power_node = item.SelectSingleNode("./div[@class='s_poster']/div[@class='s_thumb']/div[@class='s_tagrt']/span[@class='tip_bk']");
                                if (power_node != null)
                                {
                                    var power = power_node.InnerText.Trim();
                                    Console.WriteLine("播放权限:" + power);
                                    model.Power = power;
                                }
                                var type_node = item.SelectSingleNode("./div[@class='s_inform  s_madeform']/div[@class='s_detail']/div[@class='s_base']/span[@class='base_type']");
                                if (type_node != null)
                                {
                                    var type = type_node.InnerText.Trim();
                                    Console.WriteLine("类型:" + type);
                                    model.Category = type;
                                }
                                model.CreateTime = DateTime.Now;
                                model.LastTime = DateTime.Now;
                                model.Episodes = null;
                                model.Platform = "youku";

                                var plyer_li_nodes = item.SelectNodes("./div[@class='s_inform  s_madeform']/div[@class='s_detail']/div[@data-type='cataParent'][1]/div[@class='s_items all site14 ']/ul[@class='clearfix']/li");
                                plyer_li_nodes = plyer_li_nodes == null ? item.SelectNodes("./div[@class='s_inform  s_madeform']/div[@class='s_detail']/div[@data-type='cataParent'][1]/div[@class='s_items site14 ']/ul[@class='clearfix']/li") : plyer_li_nodes;
                                if (plyer_li_nodes!=null)
                                {
                                    foreach (var li_node in plyer_li_nodes)
                                    {
                                        var a_node = li_node.SelectSingleNode("./a");
                                        if (a_node!=null)
                                        {
                                            var t_player = a_node.GetAttributeValue("href", "");
                                            if (t_player.Contains("http://cps.youku.com/redirect.html"))
                                            {
                                                t_player = t_player.Substring(t_player.IndexOf("url=") + 4);
                                                t_player = Utils.UrlDecode(t_player);
                                            }
                                            if (t_player.Contains("http://v.youku.com/v_show/id_"))
                                            {
                                                var episodes_str = a_node.InnerText.Trim();
                                                var episodes = Utils.StrToInt(episodes_str, 0);
                                                if (episodes > 0)
                                                {
                                                    t_player = t_player.Replace("==", "");
                                                    if (t_player.IndexOf("?") > 0)
                                                        t_player = t_player.Substring(0, t_player.IndexOf("?"));
                                                    Console.WriteLine("播放地址:" + t_player);

                                                    var vid = t_player.Replace("http://v.youku.com/v_show/id_", "");
                                                    if (vid.IndexOf(".") > 0)
                                                        vid = vid.Substring(0, vid.IndexOf("."));
                                                    Console.WriteLine("视频ID:" + vid);
                                                    list.Add(new Model.Player()
                                                    {
                                                        Category = model.Category,
                                                        CreateTime = DateTime.Now,
                                                        Episodes = episodes,
                                                        LastTime = DateTime.Now,
                                                        Name = model.Name,
                                                        Platform = model.Platform,
                                                        PlayerUrl = t_player,
                                                        VID = vid,
                                                        Poster = model.Poster,
                                                        Power = model.Power
                                                    });
                                                }

                                            }
                                        }
                                    }
                                }

                                //play_url = play_url.Replace("==", "");
                                //if (play_url.IndexOf("?") > 0)
                                //    play_url = play_url.Substring(0, play_url.IndexOf("?"));
                                //Console.WriteLine("播放地址:" + play_url);
                                //model.PlayerUrl = play_url;

                                //var vid = play_url.Replace("http://v.youku.com/v_show/id_", "");
                                //if (vid.IndexOf(".") > 0)
                                //    vid = vid.Substring(0, vid.IndexOf("."));
                                //Console.WriteLine("视频ID:" + vid);
                                //model.VID = vid;

                                //list.Add(model);
                            }
                        }
                    }
                return list;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }

}
