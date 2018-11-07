using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataAcquisitionLib
{
    public class GaoQingCollection
    {
        HttpWebResponseUtility hwru;
        BLL.GaoQing.Movie_BLL gaoqing_movie_bll;
        public GaoQingCollection()
        {
            hwru = new HttpWebResponseUtility();
            gaoqing_movie_bll = new BLL.GaoQing.Movie_BLL();
        }

        public void CollectionMovies(Model.GaoQing.Family family, int timeout = 30000)
        {
            try
            {
                string res_html = null;
                string heat_res_html = null;
                CookieCollection res_cookie = null;

                var url = (family == Model.GaoQing.Family.电视剧 ? "https://gaoqing.fm/api/listtv?limit=500000&page=" : "https://gaoqing.fm/api/listfilm?limit=500000&page=") + 1;
                hwru.SendRequest(
                    url,
                    null,
                    null,
                    null,
                    null,
                    "GET",
                    out res_html,
                    out res_cookie,
                    Encoding.UTF8,
                    timeout
                );
                hwru.SendRequest(
                    url + "&order=热度",
                    null,
                    null,
                    null,
                    null,
                    "GET",
                    out heat_res_html,
                    out res_cookie,
                    Encoding.UTF8,
                    timeout
                );
                if (!string.IsNullOrEmpty(res_html)&& !string.IsNullOrEmpty(heat_res_html))
                {
                    var movies = AnalysisMovieHtml(res_html);
                    var heat_movies = AnalysisMovieHtml(heat_res_html);
                    if (movies != null)
                    {
                        if (movies.Count != 0)
                        {
                            Console.WriteLine("爬取到" + Enum.GetName(typeof(Model.GaoQing.Family), family) + "数据" + movies.Count + "条，正在分析中...");
                            for (int i = movies.Count - 1; i >= 0; i--)
                            {
                                var m = movies[i];
                                var heat_index = heat_movies.FindIndex(f => f.Hash == m.Hash);
                                //heat_movies.IndexOf(heat_movies.FirstOrDefault(f=>f.Hash == m.Hash))
                                //int test = heat_movies.FindIndex(f => f.Hash == "123136");
                                int heat = heat_index < 0 ? 0 : heat_movies.Count - heat_index;
                                var movie = new Model.GaoQing.BaseMovie()
                                {
                                    CreateTime = DateTime.Now,
                                    LastTime = DateTime.Now,
                                    Family = (int)family,
                                    Hash = m.Hash,
                                    Name = m.Name,
                                    Nd = Utils.StrToInt(m.Nd, 0),
                                    Poster = m.Poster,
                                    Rate = Utils.StrToDecimal(m.Rate, 0),
                                    Subject = string.IsNullOrWhiteSpace(m.Subject) ? null : m.Subject,
                                    Heat = heat,
                                    Description = null,
                                    Title = null,
                                    Episodes = null
                                };
                                movie.Nd = movie.Nd == 0 ? null : movie.Nd;
                                movie.Rate = movie.Rate == 0 ? null : movie.Rate;
                                movie.Heat = movie.Heat == 0 ? null : movie.Heat;
                                //做数据库操作
                                int id = gaoqing_movie_bll.IsHash(movie.Hash);
                                if (id < 0)
                                {
                                    //数据不存在
                                    id = gaoqing_movie_bll.AddBaseMovie(movie);
                                    Console.WriteLine("新" + Enum.GetName(typeof(Model.GaoQing.Family), family) + "“" + movie.Name + "”数据写入数据库成功...");
                                }
                                else
                                {
                                    //数据存在
                                    if (gaoqing_movie_bll.UpdateMovieHeatAndRate(id, movie.Heat, movie.Rate)>0)
                                    {
                                        Console.WriteLine("" + Enum.GetName(typeof(Model.GaoQing.Family), family) + "“" + movie.Name + "”数据更新成功...");
                                    }
                                }
                            }

                        }
                    }
                }
                else
                    throw new Exception("数据采集失败！请检查网络设置！");

            }
            catch (Exception ex)
            {
                Console.WriteLine("CollectionMovies发生异常了");
            }
        }
        
        private List<GaoQing.Movie> AnalysisMovieHtml(string res_html)
        {
            try
            {
                var list = res_html.ParseJSON<List<GaoQing.Movie>>();
                list.ForEach(m=> {
                    m.Poster = "/"+(string.IsNullOrWhiteSpace(m.Nd)?"others": m.Nd) + "/" + m.Hash + ".jpg";
                    m.Poster = string.IsNullOrWhiteSpace(m.Poster) ? null : "https://gaoqing.fm/uploads" + m.Poster;
                });
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public void CollectionMovieInfo(int movieID , string hash, int timeout = 30000)
        {
            try
            {
                string res_html = null;
                CookieCollection res_cookie = null;
                var url = "https://gaoqing.fm/view/" + hash;
                do
                {
                    hwru.SendRequest(
                        url,
                        null,
                        null,
                        null,
                        null,
                        "GET",
                        out res_html,
                        out res_cookie,
                        Encoding.UTF8,
                        timeout
                    );
                    if (!res_html.Contains("刷新频率太快"))
                    {
                        if (!string.IsNullOrEmpty(res_html))
                        {
                            Console.WriteLine("正在获取“" + hash + "”的详细信息...");
                            var movie = AnalysisMovieInfoHtml(res_html);
                            if (movie != null)
                            {
                                movie.Hash = hash;
                                movie.ID = movieID;
                                if (gaoqing_movie_bll.UpdateMovieInfo(movie) > 0)
                                    Console.WriteLine(hash + "详细信息更新成功！");
                            }

                            var magnets = AnalysisMovieDownloadHtml(res_html);
                            if (magnets != null)
                            {
                                Console.WriteLine("下载地址找到【" + magnets.Count + "】条...");
                                magnets.ForEach(m =>
                                {
                                    if (gaoqing_movie_bll.IsTorrent(m.Magnet, m.Torrent) < 0)
                                    {
                                        m.CreateTime = DateTime.Now;
                                        m.MovieID = movieID;
                                        if (gaoqing_movie_bll.InsertTorrent(m) > 0)
                                            Console.WriteLine("类型：" + m.Meta + "\t名称：" + m.Title + "\t" + "大小：" + m.Size + "\t磁力：" + m.Magnet);
                                    }
                                });
                            }

                            var subtitles = AnalysisMovieSubtitleHtml(res_html);
                            if (subtitles != null)
                            {
                                Console.WriteLine("字幕找到【" + subtitles.Count + "】条...");
                                subtitles.ForEach(m =>
                                {
                                    if (gaoqing_movie_bll.IsSubtitle(m.Download) < 0)
                                    {
                                        m.CreateTime = DateTime.Now;
                                        m.MovieID = movieID;
                                        if (gaoqing_movie_bll.InsertSubtitle(m) > 0)
                                            Console.WriteLine("类型：" + m.Meta + "\t名称：" + m.Title + "\t" + "大小：" + m.Size + "\t地址：" + m.Download);
                                    }
                                });
                            }
                        }
                        else
                            throw new Exception("数据采集失败！请检查网络设置！");
                    }
                    else
                    {
                        //Console.WriteLine(res_html);
                        //Console.WriteLine("休息中...");
                        System.Threading.Thread.Sleep(3000);
                    }
                } while (res_html.Contains("刷新频率太快"));
                

            }
            catch (Exception ex)
            {
                Console.WriteLine("CollectionMovieDownload发生异常了");
            }
        }
        
        private List<Model.GaoQing.MovieTorrent> AnalysisMovieDownloadHtml(string html)
        {

            try
            {
                List<Model.GaoQing.MovieTorrent> list = new List<Model.GaoQing.MovieTorrent>();
                HtmlDocument hd = new HtmlDocument();
                hd.LoadHtml(html);
                var node = hd.DocumentNode.SelectSingleNode("//table[@id='cili']");
                var info_nodes = node.SelectNodes("./tr");
                if (info_nodes!=null)
                {
                    for (int i = 0; i < (info_nodes.Count>10?info_nodes.Count - 2: info_nodes.Count); i++)
                    {
                        var item = info_nodes[i];
                        string clear = item.GetAttributeValue("id", "") == null ? null : item.GetAttributeValue("id", "").Trim();
                        var name = item.SelectSingleNode("./td/b") == null ? "" : item.SelectSingleNode("./td/b").InnerText.Trim();
                        string size_str = item.SelectSingleNode("./td/span/span[1]") == null ? null : item.SelectSingleNode("./td/span/span[1]").InnerText.Trim();
                        decimal? size = null;
                        if (!string.IsNullOrWhiteSpace(size_str))
                        {
                            if (size_str.Contains("TB"))
                                size = Convert.ToDecimal(Utils.StrToDouble(size_str.Replace("TB", "").Trim(), 0) * (1024.0 * 1024.0 * 1024.0));
                            if (size_str.Contains("GB"))
                                size = Convert.ToDecimal(Utils.StrToDouble(size_str.Replace("GB", "").Trim(), 0) * (1024.0 * 1024.0));
                            if (size_str.Contains("MB"))
                                size = Convert.ToDecimal(Utils.StrToDouble(size_str.Replace("MB", "").Trim(), 0) * (1024.0));
                            if (size_str.Contains("KB"))
                                size = Convert.ToDecimal(Utils.StrToDouble(size_str.Replace("KB", "").Trim(), 0));
                            size = size == 0 ? null : size;
                        }
                        //decimal? size = Convert.ToDecimal(string.IsNullOrWhiteSpace(size_str) ? 0 : ((size_str.Contains("GB") ? Utils.StrToDouble(size_str.Replace("GB", "").Trim(), 0) : ((size_str.Contains("MB") ? Utils.StrToDouble(size_str.Replace("MB", "").Trim(), 0) : (size_str.Contains("KB") ? Utils.StrToDouble(size_str.Replace("KB", "").Trim(), 0) : 0)) * 1024.0)) * 1024.0 * 1024.0));
                        var meta = item.SelectSingleNode("./td/span/span[2]") == null ? null : item.SelectSingleNode("./td/span/span[2]").InnerText.Trim();
                        var bthash = item.SelectSingleNode("./td/span/span[3]/a[1]") == null ? null : item.SelectSingleNode("./td/span/span[3]/a[1]").GetAttributeValue("href", "").Trim();
                        var magnet = item.SelectSingleNode("./td/span/span[3]/a[2]") == null ? null : item.SelectSingleNode("./td/span/span[3]/a[2]").GetAttributeValue("href", "").Trim();

                        list.Add(new Model.GaoQing.MovieTorrent()
                        {
                            Clear = clear,
                            Size = size == 0 ? null : size,
                            CreateTime = DateTime.Now,
                            Magnet = string.IsNullOrWhiteSpace(magnet) ? null : magnet,
                            Meta = string.IsNullOrWhiteSpace(meta) ? null : meta,
                            Title = name,
                            Torrent = string.IsNullOrWhiteSpace(bthash) ? null : bthash
                        });
                    }
                }
                
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<Model.GaoQing.Subtitle> AnalysisMovieSubtitleHtml(string html)
        {
            try
            {
                List<Model.GaoQing.Subtitle> list = new List<Model.GaoQing.Subtitle>();
                HtmlDocument hd = new HtmlDocument();
                hd.LoadHtml(html);
                var node = hd.DocumentNode.SelectSingleNode("//table[@id='subtable']");
                var info_nodes = node.SelectNodes("./tr");
                if (info_nodes != null)
                {
                    for (int i = 0; i < (info_nodes.Count > 10 ? info_nodes.Count - 2 : info_nodes.Count); i++)
                    {
                        var item = info_nodes[i];
                        string clear = item.GetAttributeValue("id", "") == null ? null : item.GetAttributeValue("id", "").Trim();
                        var name = item.SelectSingleNode("./td/span/b") == null ? "" : item.SelectSingleNode("./td/span/b").InnerText.Trim();
                        string size_str = item.SelectSingleNode("./td/span[2]/span[1]") == null ? null : item.SelectSingleNode("./td/span[2]/span[1]").InnerText.Trim();
                        decimal? size = null;
                        if (!string.IsNullOrWhiteSpace(size_str))
                        {
                            if (size_str.Contains("TB"))
                                size = Convert.ToDecimal(Utils.StrToDouble(size_str.Replace("TB", "").Trim(), 0) * (1024.0 * 1024.0 * 1024.0));
                            if (size_str.Contains("GB"))
                                size = Convert.ToDecimal(Utils.StrToDouble(size_str.Replace("GB", "").Trim(), 0) * (1024.0 * 1024.0));
                            if (size_str.Contains("MB"))
                                size = Convert.ToDecimal(Utils.StrToDouble(size_str.Replace("MB", "").Trim(), 0) * (1024.0));
                            if (size_str.Contains("KB"))
                                size = Convert.ToDecimal(Utils.StrToDouble(size_str.Replace("KB", "").Trim(), 0));
                            size = size == 0 ? null : size;
                        }
                        //decimal? size = Convert.ToDecimal(
                        //    string.IsNullOrWhiteSpace(size_str) ? 
                        //    0 : 
                        //    ((size_str.Contains("GB") ? 
                        //        Utils.StrToDouble(size_str.Replace("GB", "").Trim(), 0) : 
                        //        ((size_str.Contains("MB") ? 
                        //            Utils.StrToDouble(size_str.Replace("MB", "").Trim(), 0) : 
                        //            (size_str.Contains("KB") ? 
                        //                Utils.StrToDouble(size_str.Replace("KB", "").Trim(), 0) : 
                        //                0
                        //            )
                        //        ) * 1024.0)
                        //     ) * 1024.0 * 1024.0)
                        //     );
                        var source = item.SelectSingleNode("./td/span[2]/span[2]") == null ? null : item.SelectSingleNode("./td/span[2]/span[2]").InnerText.Trim();
                        var format_nodes = item.SelectNodes("./td/span[2]/span[@class='label label-danger']");
                        var format_str = "";
                        if (format_nodes!=null)
                        {
                            foreach (var m in format_nodes)
                            {
                                var t_format = m.InnerText.Trim();
                                if (!string.IsNullOrWhiteSpace(t_format))
                                {
                                    format_str += t_format + ",";
                                }
                            }
                        }
                        format_str = format_str.TrimEnd(',');

                        var language_node = item.SelectSingleNode("./td/span[2]/span[last()]/span");
                        var language = "";
                        var meat = "";
                        if (language_node != null)
                        {
                            language = language_node.GetAttributeValue("title", "").Trim().Replace(" ", ",");
                            meat = language_node.InnerText.Trim();
                        }

                        var download = "";
                        var download_node = item.SelectSingleNode("./td/span[2]/span[last()]/a");
                        if (download_node != null)
                            download = download_node.GetAttributeValue("href", "");

                        list.Add(new Model.GaoQing.Subtitle()
                        {
                            Clear = clear,
                            Size = size == 0 ? null : size,
                            CreateTime = DateTime.Now,
                            Title = name,
                            Format = format_str,
                            Language = string.IsNullOrWhiteSpace(language) ? null : language,
                            Meta = string.IsNullOrWhiteSpace(meat) ? null : meat,
                            Download = string.IsNullOrWhiteSpace(download) ? null : download,
                            Source = string.IsNullOrWhiteSpace(source) ? null : source

                        });
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Model.GaoQing.Movie AnalysisMovieInfoHtml(string html)
        {

            try
            {
                Model.GaoQing.Movie model = new Model.GaoQing.Movie();
                HtmlDocument hd = new HtmlDocument();
                hd.LoadHtml(html);

                //Title
                model.Title = "";
                var title_node = hd.DocumentNode.SelectSingleNode("//div[@id='mainrow']/div[@class='row']/div[@class='col-md-9']/div[@class='row']/div[@class='col-md-12']/h2/a");
                if (title_node != null)
                    model.Title = title_node.InnerText.Trim();

                //Description
                model.Description = null;
                var des_node = hd.DocumentNode.SelectSingleNode("//p[@id='des-ex']") ==null?hd.DocumentNode.SelectSingleNode("//p[@id='des-full']"): hd.DocumentNode.SelectSingleNode("//p[@id='des-ex']");
                if (des_node != null)
                    model.Description = des_node.InnerHtml;
                
                var viewfilm_node = hd.DocumentNode.SelectSingleNode("//div[@id='viewfilm']");
                if (viewfilm_node!=null)
                {
                    var viewfilm_html = viewfilm_node.InnerHtml;

                    //Director
                    var director_span_node = viewfilm_node.SelectSingleNode("./span[text()='导演']");
                    if (director_span_node!=null)
                    {
                        var director_span_html = director_span_node.OuterHtml;
                        var director_html = viewfilm_html.Substring(viewfilm_html.IndexOf(director_span_html) + director_span_html.Length + 1).Trim();
                        director_html = director_html.Substring(0, director_html.IndexOf("<br>"));
                        if (!string.IsNullOrWhiteSpace(director_html))
                        {
                            HtmlDocument director_hd = new HtmlDocument();
                            director_hd.LoadHtml(director_html);
                            var director_str = director_hd.DocumentNode.InnerText;
                            if (!string.IsNullOrWhiteSpace(director_str))
                            {
                                var directors = director_str.Split('/');
                                if (directors != null)
                                {
                                    model.Director = new List<Model.GaoQing.Celebrity>();
                                    directors.ToList().ForEach(m => {
                                        if (!string.IsNullOrWhiteSpace(m))
                                            model.Director.Add(new Model.GaoQing.Celebrity() { Name = m.Trim() });
                                    });
                                }
                            }
                        }
                    }
                    //Screenwriter
                    var screenwriter_span_node = viewfilm_node.SelectSingleNode("./span[text()='编剧']");
                    if (screenwriter_span_node != null)
                    {
                        var screenwriter_span_html = screenwriter_span_node.OuterHtml;
                        var screenwriter_html = viewfilm_html.Substring(viewfilm_html.IndexOf(screenwriter_span_html) + screenwriter_span_html.Length + 1).Trim();
                        screenwriter_html = screenwriter_html.Substring(0, screenwriter_html.IndexOf("<br>"));
                        if (!string.IsNullOrWhiteSpace(screenwriter_html))
                        {
                            HtmlDocument screenwriter_hd = new HtmlDocument();
                            screenwriter_hd.LoadHtml(screenwriter_html);
                            var screenwriter_str = screenwriter_hd.DocumentNode.InnerText;
                            if (!string.IsNullOrWhiteSpace(screenwriter_str))
                            {
                                var screenwriters = screenwriter_str.Split('/');
                                if (screenwriters != null)
                                {
                                    model.Screenwriter = new List<Model.GaoQing.Celebrity>();
                                    screenwriters.ToList().ForEach(m => {
                                        if (!string.IsNullOrWhiteSpace(m))
                                            model.Screenwriter.Add(new Model.GaoQing.Celebrity() { Name = m.Trim() });
                                    });
                                }
                            }
                        }
                    }
                    //Performer
                    var performer_span_node = viewfilm_node.SelectSingleNode("./span[text()='主演']");
                    if (performer_span_node != null)
                    {
                        var performer_span_html = performer_span_node.OuterHtml;
                        var performer_html = viewfilm_html.Substring(viewfilm_html.IndexOf(performer_span_html) + performer_span_html.Length + 1).Trim();
                        performer_html = performer_html.Substring(0, performer_html.IndexOf("<br>"));
                        if (!string.IsNullOrWhiteSpace(performer_html))
                        {
                            HtmlDocument performer_hd = new HtmlDocument();
                            performer_hd.LoadHtml(performer_html);
                            var performer_str = performer_hd.DocumentNode.InnerText;
                            if (!string.IsNullOrWhiteSpace(performer_str))
                            {
                                var performers = performer_str.Split('/');
                                if (performers != null)
                                {
                                    model.Performer = new List<Model.GaoQing.Celebrity>();
                                    performers.ToList().ForEach(m => {
                                        if (!string.IsNullOrWhiteSpace(m))
                                            model.Performer.Add(new Model.GaoQing.Celebrity() { Name = m.Trim() });
                                    });
                                }
                            }
                        }
                    }
                    //Category
                    var category_span_node = viewfilm_node.SelectSingleNode("./span[text()='类型']");
                    if (category_span_node != null)
                    {
                        var category_span_html = category_span_node.OuterHtml;
                        var category_html = viewfilm_html.Substring(viewfilm_html.IndexOf(category_span_html) + category_span_html.Length + 1).Trim();
                        category_html = category_html.Substring(0, category_html.IndexOf("<br>"));
                        if (!string.IsNullOrWhiteSpace(category_html))
                        {
                            HtmlDocument category_hd = new HtmlDocument();
                            category_hd.LoadHtml(category_html);
                            var category_str = category_hd.DocumentNode.InnerText;
                            if (!string.IsNullOrWhiteSpace(category_str))
                            {
                                var categorys = category_str.Split('/');
                                if (categorys != null)
                                {
                                    model.Category = new List<Model.GaoQing.Category>();
                                    categorys.ToList().ForEach(m => {
                                        if (!string.IsNullOrWhiteSpace(m))
                                            model.Category.Add(new Model.GaoQing.Category() { Name = m.Trim() });
                                    });
                                }
                            }
                        }
                    }
                    //Country
                    var country_span_node = viewfilm_node.SelectSingleNode("./span[text()='地区']");
                    if (country_span_node != null)
                    {
                        var country_span_html = country_span_node.OuterHtml;
                        var country_html = viewfilm_html.Substring(viewfilm_html.IndexOf(country_span_html) + country_span_html.Length + 1).Trim();
                        country_html = country_html.Substring(0, country_html.IndexOf("<br>"));
                        if (!string.IsNullOrWhiteSpace(country_html))
                        {
                            HtmlDocument country_hd = new HtmlDocument();
                            country_hd.LoadHtml(country_html);
                            var country_str = country_hd.DocumentNode.InnerText;
                            if (!string.IsNullOrWhiteSpace(country_str))
                            {
                                var countrys = country_str.Split('/');
                                if (countrys != null)
                                {
                                    model.Country = new List<Model.GaoQing.Country>();
                                    countrys.ToList().ForEach(m => {
                                        if (!string.IsNullOrWhiteSpace(m))
                                            model.Country.Add(new Model.GaoQing.Country() { Name = m.Trim() });
                                    });
                                }
                            }
                        }
                    }
                    //ReleaseTime
                    var releasetime_span_node = viewfilm_node.SelectSingleNode("./span[text()='上映']");
                    if (releasetime_span_node != null)
                    {
                        var releasetime_span_html = releasetime_span_node.OuterHtml;
                        var releasetime_html = viewfilm_html.Substring(viewfilm_html.IndexOf(releasetime_span_html) + releasetime_span_html.Length + 1).Trim();
                        releasetime_html = releasetime_html.Substring(0, releasetime_html.IndexOf("<br>"));
                        if (!string.IsNullOrWhiteSpace(releasetime_html))
                        {
                            HtmlDocument releasetime_hd = new HtmlDocument();
                            releasetime_hd.LoadHtml(releasetime_html);
                            var releasetime_str = releasetime_hd.DocumentNode.InnerText;
                            if (!string.IsNullOrWhiteSpace(releasetime_str))
                            {
                                var releasetimes = releasetime_str.Split('/');
                                if (releasetimes != null)
                                {
                                    model.ReleaseTime = new List<Model.GaoQing.ReleaseTime>();
                                    releasetimes.ToList().ForEach(m => {
                                        if (!string.IsNullOrWhiteSpace(m))
                                        {
                                            string place = m.IndexOf(")") > 0 && m.IndexOf("(") > 0 ? m.Substring(m.IndexOf("(") + 1, m.IndexOf(")") - m.IndexOf("(") - 1) : null;

                                            //对releasetime_date深度加工
                                            var place_str = string.IsNullOrWhiteSpace(place) ? "" : "(" + place + ")";
                                            var dates = string.IsNullOrWhiteSpace(place) ? m.Split('-') : m.Replace(place_str, "").Trim().Split('-');
                                            DateTime? releasetime_date = null;
                                            try
                                            {
                                                if (dates.Length > 0)
                                                    if (dates[0].Length == 4)
                                                        releasetime_date = new DateTime(Utils.StrToInt(dates[0], 0), dates.Length > 1 ? Utils.StrToInt(dates[1], 1) : 1, dates.Length > 2 ? Utils.StrToInt(dates[2], 1) : 1);
                                            }
                                            catch (Exception)
                                            {
                                                releasetime_date = null;
                                            }
                                            if (releasetime_date != null)
                                                model.ReleaseTime.Add(new Model.GaoQing.ReleaseTime()
                                                {
                                                    Place = place,
                                                    ReleaseDate = releasetime_date
                                                });
                                        }
                                    });
                                }
                            }
                        }
                    }
                    var fasttime_span_node = viewfilm_node.SelectSingleNode("./span[text()='首播']");
                    if (fasttime_span_node != null)
                    {
                        var fasttime_span_html = fasttime_span_node.OuterHtml;
                        var fasttime_html = viewfilm_html.Substring(viewfilm_html.IndexOf(fasttime_span_html) + fasttime_span_html.Length + 1).Trim();
                        fasttime_html = fasttime_html.Substring(0, fasttime_html.IndexOf("<br>"));
                        if (!string.IsNullOrWhiteSpace(fasttime_html))
                        {
                            HtmlDocument fasttime_hd = new HtmlDocument();
                            fasttime_hd.LoadHtml(fasttime_html);
                            var fasttime_str = fasttime_hd.DocumentNode.InnerText;
                            if (!string.IsNullOrWhiteSpace(fasttime_str))
                            {
                                var fasttimes = fasttime_str.Split('/');
                                if (fasttimes != null)
                                {
                                    model.ReleaseTime = new List<Model.GaoQing.ReleaseTime>();
                                    fasttimes.ToList().ForEach(m => {
                                        if (!string.IsNullOrWhiteSpace(m))
                                        {
                                            string place = m.IndexOf(")") > 0 && m.IndexOf("(") > 0 ? m.Substring(m.IndexOf("(") + 1, m.IndexOf(")") - m.IndexOf("(") - 1) : null;

                                            //对fasttime_date深度加工
                                            var place_str = string.IsNullOrWhiteSpace(place) ? "" : "(" + place + ")";
                                            var dates = string.IsNullOrWhiteSpace(place) ? m.Split('-') : m.Replace(place_str, "").Trim().Split('-');
                                            DateTime? fasttime_date = null;
                                            try
                                            {
                                                if (dates.Length > 0)
                                                    if (dates[0].Length == 4)
                                                        fasttime_date = new DateTime(Utils.StrToInt(dates[0], 0), dates.Length > 1 ? Utils.StrToInt(dates[1], 1) : 1, dates.Length > 2 ? Utils.StrToInt(dates[2], 1) : 1);
                                            }
                                            catch (Exception)
                                            {
                                                fasttime_date = null;
                                            }
                                            if (fasttime_date != null)
                                                model.ReleaseTime.Add(new Model.GaoQing.ReleaseTime()
                                                {
                                                    Place = place,
                                                    ReleaseDate = fasttime_date
                                                });
                                        }
                                    });
                                }
                            }
                        }
                    }
                    //LongTime
                    var longtime_span_node = viewfilm_node.SelectSingleNode("./span[text()='片长']");
                    if (longtime_span_node != null)
                    {
                        var longtime_span_html = longtime_span_node.OuterHtml;
                        var longtime_html = viewfilm_html.Substring(viewfilm_html.IndexOf(longtime_span_html) + longtime_span_html.Length + 1).Trim();
                        longtime_html = longtime_html.Substring(0, longtime_html.IndexOf("<br>"));
                        if (!string.IsNullOrWhiteSpace(longtime_html))
                        {
                            HtmlDocument longtime_hd = new HtmlDocument();
                            longtime_hd.LoadHtml(longtime_html);
                            var longtime_str = longtime_hd.DocumentNode.InnerText;
                            if (!string.IsNullOrWhiteSpace(longtime_str))
                            {
                                var longtimes = longtime_str.Split('/');
                                if (longtimes != null)
                                {
                                    model.LongTime = new List<Model.GaoQing.LongTime>();
                                    longtimes.ToList().ForEach(m => {
                                        if (!string.IsNullOrWhiteSpace(m))
                                        {
                                            string country = m.IndexOf(")") > 0 && m.IndexOf("(") > 0 ? m.Substring(m.IndexOf("(") + 1, m.IndexOf(")") - m.IndexOf("(") - 1) : null;
                                            var min_str = m.Replace("分钟", "").Trim();
                                            min_str = string.IsNullOrWhiteSpace(country) ? min_str : min_str.Replace("(" + country + ")", "").Trim();
                                            int min = Utils.StrToInt(min_str, 0);
                                            if (min != 0)
                                                model.LongTime.Add(new Model.GaoQing.LongTime()
                                                {
                                                    Country = country,
                                                    Minute = min
                                                });
                                        }
                                    });
                                }
                            }
                        }
                    }
                    var onetime_span_node = viewfilm_node.SelectSingleNode("./span[text()='单集片长']");
                    if (onetime_span_node != null)
                    {
                        var onetime_span_html = onetime_span_node.OuterHtml;
                        var onetime_html = viewfilm_html.Substring(viewfilm_html.IndexOf(onetime_span_html) + onetime_span_html.Length + 1).Trim();
                        onetime_html = onetime_html.Substring(0, onetime_html.IndexOf("<br>"));
                        if (!string.IsNullOrWhiteSpace(onetime_html))
                        {
                            HtmlDocument onetime_hd = new HtmlDocument();
                            onetime_hd.LoadHtml(onetime_html);
                            var onetime_str = onetime_hd.DocumentNode.InnerText;
                            if (!string.IsNullOrWhiteSpace(onetime_str))
                            {
                                var onetimes = onetime_str.Split('/');
                                if (onetimes != null)
                                {
                                    model.LongTime = new List<Model.GaoQing.LongTime>();
                                    onetimes.ToList().ForEach(m => {
                                        if (!string.IsNullOrWhiteSpace(m))
                                        {
                                            string country = m.IndexOf(")") > 0 && m.IndexOf("(") > 0 ? m.Substring(m.IndexOf("(") + 1, m.IndexOf(")") - m.IndexOf("(") - 1) : null;
                                            var min_str = m.Replace("分钟", "").Trim();
                                            min_str = string.IsNullOrWhiteSpace(country) ? min_str : min_str.Replace("(" + country + ")", "").Trim();
                                            int min = Utils.StrToInt(min_str, 0);
                                            if (min != 0)
                                                model.LongTime.Add(new Model.GaoQing.LongTime()
                                                {
                                                    Country = country,
                                                    Minute = min
                                                });
                                        }
                                    });
                                }
                            }
                        }
                    }
                    //MoreName
                    var morename_span_node = viewfilm_node.SelectSingleNode("./span[text()='又名']");
                    if (morename_span_node != null)
                    {
                        var morename_span_html = morename_span_node.OuterHtml;
                        var morename_html = viewfilm_html.Substring(viewfilm_html.IndexOf(morename_span_html) + morename_span_html.Length + 1).Trim();
                        morename_html = morename_html.Substring(0, morename_html.IndexOf("<br>"));
                        if (!string.IsNullOrWhiteSpace(morename_html))
                        {
                            HtmlDocument morename_hd = new HtmlDocument();
                            morename_hd.LoadHtml(morename_html);
                            var morename_str = morename_hd.DocumentNode.InnerText;
                            if (!string.IsNullOrWhiteSpace(morename_str))
                            {
                                var morenames = morename_str.Split('/');
                                if (morenames != null)
                                {
                                    model.MoreName = new List<Model.GaoQing.MovieName>();
                                    morenames.ToList().ForEach(m => {
                                        if (!string.IsNullOrWhiteSpace(m))
                                            model.MoreName.Add(new Model.GaoQing.MovieName() { Name = m.Trim() });
                                    });
                                }
                            }
                        }
                    }
                    //Episodes
                    model.Episodes = null;
                    var episodes_span_node = viewfilm_node.SelectSingleNode("./span[text()='集数']");
                    if (episodes_span_node != null)
                    {
                        var episodes_span_html = episodes_span_node.OuterHtml;
                        var episodes_html = viewfilm_html.Substring(viewfilm_html.IndexOf(episodes_span_html) + episodes_span_html.Length + 1).Trim();
                        episodes_html = episodes_html.Substring(0, episodes_html.IndexOf("<br>"));
                        if (!string.IsNullOrWhiteSpace(episodes_html))
                        {
                            model.Episodes = Utils.StrToInt(episodes_html.Trim(), 0);
                            model.Episodes = model.Episodes == 0 ? null : model.Episodes;
                        }
                    }
                    //Season
                    var season_span_node = viewfilm_node.SelectSingleNode("./span[text()='季数']");
                    if (season_span_node != null)
                    {
                        var season_span_html = season_span_node.OuterHtml;
                        var season_html = viewfilm_html.Substring(viewfilm_html.IndexOf(season_span_html) + season_span_html.Length + 1).Trim();
                        season_html = season_html.Substring(0, season_html.IndexOf("<br>"));
                        if (!string.IsNullOrWhiteSpace(season_html))
                        {
                            HtmlDocument season_hd = new HtmlDocument();
                            season_hd.LoadHtml(season_html);
                            var option_nodes = season_hd.DocumentNode.SelectNodes("//option");
                            if (option_nodes!=null)
                            {
                                model.Season = new List<Model.GaoQing.Season>();
                                foreach (var item in option_nodes)
                                {
                                    var t_href = item.GetAttributeValue("value", "");
                                    var hash = t_href.Replace("https://gaoqing.fm/view/", "").Trim();
                                    hash = hash.IndexOf("&") > 0 ? hash.Substring(0, hash.IndexOf("&")) : hash;
                                    var t_html = season_html.Substring(season_html.IndexOf(item.OuterHtml) + item.OuterHtml.Length);
                                    t_html = t_html.Substring(0, t_html.IndexOf("<"));
                                    var t_val = Utils.StrToInt(t_html.Trim(), 0);
                                    if (!string.IsNullOrWhiteSpace(hash)&& t_val!=0)
                                    {
                                        model.Season.Add(new Model.GaoQing.Season() {
                                            Hash = hash.Trim(),
                                            SeasonNum = t_val
                                        });
                                    }
                                }
                            }
                        }
                    }


                    //var viewfilm_text = viewfilm_node.InnerText.Replace(" ", "").Replace("\t", "").Replace("\n", "");
                    //var viewfilm_text_sub = viewfilm_text.Substring(0, viewfilm_text.LastIndexOf("打分：")).Replace("导演：", "").Replace("主演：", ",").Replace("类型：", ",").Replace("地区：", ",").Replace("上映：", ",").Replace("片长：", ",").Replace("又名：", ",");
                    //var viewfilms = viewfilm_text_sub.Split(',');
                    ////Director
                    //if (viewfilms.Length > 0)
                    //{
                    //    var director_str = viewfilms[0];
                    //    if (!string.IsNullOrWhiteSpace(director_str))
                    //    {
                    //        var directors = director_str.Split('/');
                    //        if (directors!=null)
                    //        {
                    //            model.Director = new List<Model.GaoQing.Celebrity>();
                    //            directors.ToList().ForEach(m=>{
                    //                if (!string.IsNullOrWhiteSpace(m))
                    //                    model.Director.Add(new Model.GaoQing.Celebrity() { Name = m.Trim() });
                    //            });
                    //        }
                    //    }
                    //}
                    ////Performer
                    //if (viewfilms.Length > 1)
                    //{
                    //    var performer_str = viewfilms[1];
                    //    if (!string.IsNullOrWhiteSpace(performer_str))
                    //    {
                    //        var performers = performer_str.Split('/');
                    //        if (performers != null)
                    //        {
                    //            model.Performer = new List<Model.GaoQing.Celebrity>();
                    //            performers.ToList().ForEach(m => {
                    //                if (!string.IsNullOrWhiteSpace(m))
                    //                    model.Performer.Add(new Model.GaoQing.Celebrity() { Name = m.Trim() });
                    //            });
                    //        }
                    //    }
                    //}
                    ////Category
                    //if (viewfilms.Length > 2)
                    //{
                    //    var category_str = viewfilms[2];
                    //    if (!string.IsNullOrWhiteSpace(category_str))
                    //    {
                    //        var categorys = category_str.Split('/');
                    //        if (categorys != null)
                    //        {
                    //            model.Category = new List<Model.GaoQing.Category>();
                    //            categorys.ToList().ForEach(m => {
                    //                if (!string.IsNullOrWhiteSpace(m))
                    //                    model.Category.Add(new Model.GaoQing.Category() { Name = m.Trim() });
                    //            });
                    //        }
                    //    }
                    //}
                    ////Country
                    //if (viewfilms.Length > 3)
                    //{
                    //    var country_str = viewfilms[3];
                    //    if (!string.IsNullOrWhiteSpace(country_str))
                    //    {
                    //        var countrys = country_str.Split('/');
                    //        if (countrys != null)
                    //        {
                    //            model.Country = new List<Model.GaoQing.Country>();
                    //            countrys.ToList().ForEach(m => {
                    //                if (!string.IsNullOrWhiteSpace(m))
                    //                    model.Country.Add(new Model.GaoQing.Country() { Name = m.Trim() });
                    //            });
                    //        }
                    //    }
                    //}
                    ////ReleaseTime
                    //if (viewfilms.Length > 4)
                    //{
                    //    var releasetime_str = viewfilms[4];
                    //    if (!string.IsNullOrWhiteSpace(releasetime_str))
                    //    {
                    //        var releasetimes = releasetime_str.Split('/');
                    //        if (releasetimes != null)
                    //        {
                    //            model.ReleaseTime = new List<Model.GaoQing.ReleaseTime>();
                    //            releasetimes.ToList().ForEach(m => {
                    //                if (!string.IsNullOrWhiteSpace(m))
                    //                {
                    //                    string place = m.IndexOf(")") > 0 && m.IndexOf("(") > 0 ? m.Substring(m.IndexOf("(")+1, m.IndexOf(")")- m.IndexOf("(")-1) : null;

                    //                    //对releasetime_date深度加工
                    //                    var place_str = string.IsNullOrWhiteSpace(place) ? "" : "(" + place + ")";
                    //                    var dates = string.IsNullOrWhiteSpace(place)?m.Split('-'):m.Replace(place_str,"").Trim().Split('-');
                    //                    DateTime? releasetime_date = null;
                    //                    try
                    //                    {
                    //                        if (dates.Length > 0)
                    //                            if (dates[0].Length == 4)
                    //                                releasetime_date = new DateTime(Utils.StrToInt(dates[0], 0), dates.Length > 1 ? Utils.StrToInt(dates[1], 1) : 1, dates.Length > 2 ? Utils.StrToInt(dates[2], 1) : 1);
                    //                    }
                    //                    catch (Exception)
                    //                    {
                    //                        releasetime_date = null;
                    //                    }
                    //                    if (releasetime_date != null)
                    //                        model.ReleaseTime.Add(new Model.GaoQing.ReleaseTime() {
                    //                            Place = place,
                    //                            ReleaseDate = releasetime_date
                    //                        });
                    //                }
                    //            });
                    //        }
                    //    }
                    //}
                    ////LongTime
                    //if (viewfilms.Length > 5)
                    //{
                    //    var longtime_str = viewfilms[5];
                    //    if (!string.IsNullOrWhiteSpace(longtime_str))
                    //    {
                    //        var longtimes = longtime_str.Split('/');
                    //        if (longtimes != null)
                    //        {
                    //            model.LongTime = new List<Model.GaoQing.LongTime>();
                    //            longtimes.ToList().ForEach(m => {
                    //                if (!string.IsNullOrWhiteSpace(m))
                    //                {
                    //                    string country = m.IndexOf(")") > 0 && m.IndexOf("(") > 0 ? m.Substring(m.IndexOf("(") + 1, m.IndexOf(")") - m.IndexOf("(") - 1) : null;
                    //                    var min_str = m.Replace("分钟", "").Trim();
                    //                    min_str = string.IsNullOrWhiteSpace(country) ? min_str : min_str.Replace("(" + country + ")", "").Trim();
                    //                    int min = Utils.StrToInt(min_str, 0);
                    //                    if (min != 0)
                    //                        model.LongTime.Add(new Model.GaoQing.LongTime() {
                    //                            Country = country,
                    //                            Minute = min
                    //                        });
                    //                }
                    //            });
                    //        }
                    //    }
                    //}
                    ////MoreName
                    //if (viewfilms.Length > 6)
                    //{
                    //    var morename_str = viewfilms[6];
                    //    if (!string.IsNullOrWhiteSpace(morename_str))
                    //    {
                    //        var morenames = morename_str.Split('/');
                    //        if (morenames != null)
                    //        {
                    //            model.MoreName = new List<Model.GaoQing.MovieName>();
                    //            morenames.ToList().ForEach(m => {
                    //                if (!string.IsNullOrWhiteSpace(m))
                    //                    model.MoreName.Add(new Model.GaoQing.MovieName() { Name = m.Trim() });
                    //            });
                    //        }
                    //    }
                    //}

                }

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
