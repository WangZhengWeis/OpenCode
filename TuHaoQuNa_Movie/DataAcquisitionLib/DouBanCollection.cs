using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utility;
using Utillity;
using Model.DouBan;

namespace DataAcquisitionLib
{
    public class DouBanCollection
    {
        HttpWebResponseUtility hwru;
        BLL.DouBan.Movie_BLL douban_movie_bll;

        public DouBanCollection()
        {
            hwru = new HttpWebResponseUtility();
            douban_movie_bll = new BLL.DouBan.Movie_BLL();
        }
        

        public Model.DouBan.Movie CollectionDouBanHtmlMovie(string doubanID, int timeout = 30000)
        {
            var res_str = "";
            CookieCollection cookie = new CookieCollection();
            var url = "https://movie.douban.com/subject/" + doubanID + "/";
            try
            {
                var request = hwru.SendRequest(url, null, null, null, null, "GET", out res_str, out cookie, Encoding.UTF8, timeout,true);
                if (request == null || request.StatusCode != HttpStatusCode.OK || res_str.Contains("https://sec.douban.com/a"))
                {
                    if (request.StatusCode == HttpStatusCode.NotFound)
                    {
                        throw new Exception();
                    }
                    if (res_str.Contains("https://sec.douban.com/a"))
                    {
                        ChangeIP();
                    }
                    return CollectionDouBanHtmlMovie(doubanID);
                }
                else
                {
                    Model.DouBan.Movie movie = new Model.DouBan.Movie();
                    HtmlDocument hd = new HtmlDocument();
                    hd.LoadHtml(res_str);
                    //标题
                    var title_node = hd.DocumentNode.SelectSingleNode("//span[@property='v:itemreviewed']");
                    movie.Title = title_node == null ? "" : title_node.InnerText.Trim();

                    Console.WriteLine("获取到影片："+ movie.Title);

                    //导演
                    movie.Director = new List<Model.DouBan.Celebrity>();
                    var directed_nodes = hd.DocumentNode.SelectNodes("//a[@rel='v:directedBy']");
                    if (directed_nodes!=null)
                        foreach (var item in directed_nodes)
                        {
                            var directed_name = item.InnerText.Trim();
                            var directed_id = item.GetAttributeValue("href","").Trim();
                            directed_id = directed_id.Contains("/celebrity") ? directed_id.Replace("/celebrity/", "").Replace("/", "") : null;
                            if (string.IsNullOrWhiteSpace(directed_id))
                                continue;
                            var directed_obj = douban_movie_bll.SingleCelebrity(directed_id);
                            if (directed_obj == null)
                                directed_obj = CollectionDouBanHtmlCelebrity(directed_id);
                            movie.Director.Add(directed_obj);
                        }

                    //编剧
                    movie.Screenwriter = new List<Model.DouBan.Celebrity>();
                    var screenwriter_sapn_node = hd.DocumentNode.SelectSingleNode("//span[text()='编剧']");
                    if (screenwriter_sapn_node != null)
                    {
                        var screenwriter_nodes = screenwriter_sapn_node.ParentNode.SelectNodes("./span[@class='attrs']/a");
                        if (screenwriter_nodes != null)
                            foreach (var item in screenwriter_nodes)
                            {
                                var screenwriter_name = item.InnerText.Trim();
                                var screenwriter_id = item.GetAttributeValue("href", "").Trim();
                                screenwriter_id = screenwriter_id.Contains("/celebrity") ? screenwriter_id.Replace("/celebrity/", "").Replace("/", "") : null;
                                if (string.IsNullOrWhiteSpace(screenwriter_id))
                                    continue;
                                var screenwriter_obj = douban_movie_bll.SingleCelebrity(screenwriter_id);
                                if (screenwriter_obj == null)
                                    screenwriter_obj = CollectionDouBanHtmlCelebrity(screenwriter_id);
                                movie.Screenwriter.Add(screenwriter_obj);
                            }
                    }
                    //明星
                    movie.Performer = new List<Celebrity>();
                    var performer_nodes = hd.DocumentNode.SelectNodes("//a[@rel='v:starring']");
                    if (performer_nodes!=null)
                        foreach (var item in performer_nodes)
                        {
                            var performer_name = item.InnerText.Trim();
                            var performer_id = item.GetAttributeValue("href", "").Trim();
                            performer_id = performer_id.Contains("/celebrity") ? performer_id.Replace("/celebrity/", "").Replace("/", "") : null;
                            if (string.IsNullOrWhiteSpace(performer_id))
                                continue;
                            var performer_obj = douban_movie_bll.SingleCelebrity(performer_id);
                            if (performer_obj == null)
                                performer_obj = CollectionDouBanHtmlCelebrity(performer_id);
                            movie.Performer.Add(performer_obj);
                        }
                    //类型
                    movie.FilmGenres = new List<FilmGenre>();
                    var genre_nodes = hd.DocumentNode.SelectNodes("//span[@property='v:genre']");
                    if (genre_nodes!=null)
                        foreach (var item in genre_nodes)
                        {
                            var genre_name = item.InnerText.Trim();
                            if (string.IsNullOrWhiteSpace(genre_name))
                                continue;
                            movie.FilmGenres.Add(new FilmGenre()
                            {
                                FilmGenreName = genre_name
                            });
                        }
                    //制片国家
                    movie.Countrys = new List<Country>();
                    var country_span_node = hd.DocumentNode.SelectSingleNode("//span[text()='制片国家/地区:']");
                    if (country_span_node!=null)
                    {
                        var country_span_node_ParentHtml = country_span_node.ParentNode.InnerHtml;
                        var sub_country_span_node_ParentHtml = country_span_node_ParentHtml.Substring(country_span_node_ParentHtml.IndexOf(country_span_node.OuterHtml) + country_span_node.OuterHtml.Length);
                        var countrys_str = sub_country_span_node_ParentHtml.Substring(0, sub_country_span_node_ParentHtml.IndexOf("<br>")).Trim();
                        if (!string.IsNullOrWhiteSpace(countrys_str))
                        {
                            var countrys = countrys_str.Split('/');
                            foreach (var item in countrys)
                                if (!string.IsNullOrWhiteSpace(item))
                                {
                                    var country_name = item.Trim();
                                    if (string.IsNullOrWhiteSpace(country_name))
                                        continue;
                                    movie.Countrys.Add(new Country()
                                    {
                                        CountryName = country_name
                                    });
                                }
                        }
                    }
                    //语言
                    movie.Languages = new List<Language>();
                    var language_span_node = hd.DocumentNode.SelectSingleNode("//span[text()='语言:']");
                    if (language_span_node!=null)
                    {
                        var language_span_node_ParentHtml = language_span_node.ParentNode.InnerHtml;
                        var sub_language_span_node_ParentHtml = language_span_node_ParentHtml.Substring(language_span_node_ParentHtml.IndexOf(language_span_node.OuterHtml) + language_span_node.OuterHtml.Length);
                        var languages_str = sub_language_span_node_ParentHtml.Substring(0, sub_language_span_node_ParentHtml.IndexOf("<br>")).Trim();
                        if (!string.IsNullOrWhiteSpace(languages_str))
                        {
                            var languages = languages_str.Split('/');
                            foreach (var item in languages)
                                if (!string.IsNullOrWhiteSpace(item))
                                {
                                    var language_name = item.Trim();
                                    if (string.IsNullOrWhiteSpace(language_name))
                                        continue;
                                    movie.Languages.Add(new Language()
                                    {
                                        LanguageName = language_name
                                    });
                                }
                        }
                    }
                    //上映时间
                    movie.ReleaseTimes = new List<ReleaseTime>();
                    var releasetime_nodes = hd.DocumentNode.SelectNodes("//span[@property='v:initialReleaseDate']");
                    if (releasetime_nodes != null)
                        foreach (var item in releasetime_nodes)
                        {
                            var releasetime_text = item.InnerText.Trim();
                            var releasetime_datestr = releasetime_text.Contains("(") ? releasetime_text.Substring(0, releasetime_text.IndexOf("(")) : releasetime_text;
                            var releasetime_address = releasetime_text.Replace(releasetime_datestr, "").Replace("(", "").Replace(")", "").Trim();
                            //对releasetime_date深度加工
                            var dates = releasetime_datestr.Split('-');
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
                                movie.ReleaseTimes.Add(new ReleaseTime() {
                                    DouBanID = doubanID,
                                    Place = releasetime_address,
                                    ReleaseDate = releasetime_date
                                });
                        }
                    //片长
                    movie.LongTime = null;
                    var runtime_node = hd.DocumentNode.SelectSingleNode("//span[@property='v:runtime']");
                    if (runtime_node != null)
                    {
                        movie.LongTime = Utils.StrToInt(runtime_node.GetAttributeValue("content", "0"),0);
                        movie.LongTime = movie.LongTime == 0 ? null : movie.LongTime;
                    }
                    //更多名称
                    movie.MoreName = new List<MovieName>();
                    var morename_span_node = hd.DocumentNode.SelectSingleNode("//span[text()='又名:']");
                    if (morename_span_node!=null)
                    {
                        var morename_span_node_ParentHtml = morename_span_node.ParentNode.InnerHtml;
                        var sub_morename_span_node_ParentHtml = morename_span_node_ParentHtml.Substring(morename_span_node_ParentHtml.IndexOf(morename_span_node.OuterHtml) + morename_span_node.OuterHtml.Length);
                        var morenames_str = sub_morename_span_node_ParentHtml.Substring(0, sub_morename_span_node_ParentHtml.IndexOf("<br>")).Trim();
                        if (!string.IsNullOrWhiteSpace(morenames_str))
                        {
                            var morenames = morenames_str.Split('/');
                            foreach (var item in morenames)
                            {
                                var morename = item.Trim();
                                if (string.IsNullOrWhiteSpace(morename))
                                    continue;
                                movie.MoreName.Add(new MovieName()
                                {
                                    DouBanID = doubanID,
                                    MovieTitle = morename
                                });
                            }
                        }
                    }
                    //IMDbID
                    movie.IMDbID = null;
                    var imdb_node = hd.DocumentNode.SelectSingleNode("//div[@id='info']/a[@target='_blank'][@rel='nofollow'][last()]");
                    if (imdb_node != null)
                    {
                        movie.IMDbID = string.IsNullOrWhiteSpace(imdb_node.InnerText.Trim()) ? null : imdb_node.InnerText.Trim();
                        movie.IMDbID = movie.IMDbID.Length > 10 ? null : movie.IMDbID;
                    }
                    //评分
                    movie.Score = null;
                    var score_node = hd.DocumentNode.SelectSingleNode("//strong[@property='v:average']");
                    if (score_node != null)
                    {
                        movie.Score = Utils.StrToFloat(score_node.InnerText.Trim(), 0);
                        movie.Score = movie.Score == 0 ? null : movie.Score;
                    }
                    //简介
                    movie.Describe = null;
                    var summary_node = hd.DocumentNode.SelectSingleNode("//span[@property='v:summary']");
                    if (summary_node != null)
                        movie.Describe = summary_node.InnerText.Trim();
                    //封面
                    movie.Poster = null;
                    movie.PosterID = null;
                    var poster_node = hd.DocumentNode.SelectSingleNode("//div[@id='mainpic']/a[@class='nbgnbg']/img");
                    if (poster_node!=null)
                    {
                        //https://img3.doubanio.com/view/movie_poster_cover/lpst/public/p2457983084.webp
                        var img_lpst = poster_node.GetAttributeValue("src", "");
                        if (!string.IsNullOrWhiteSpace(img_lpst)&& img_lpst.Contains("lpst"))
                        {
                            movie.Poster = new Image() {
                                Large = img_lpst,
                                Medium = img_lpst.Replace("lpst", "spst"),
                                Small = img_lpst.Replace("lpst", "ipst"),
                            };
                        }
                    }
                    //DouBanID
                    movie.DouBanID = doubanID;

                    return movie;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Model.DouBan.Celebrity CollectionDouBanHtmlCelebrity(string celebrityID, int timeout = 30000)
        {
            var res_str = "";
            CookieCollection cookie = new CookieCollection();
            var url = "https://movie.douban.com/celebrity/" + celebrityID + "/";
            try
            {
                var request = hwru.SendRequest(url, null, null, null, null, "GET", out res_str, out cookie, Encoding.UTF8, timeout, true);
                if (request == null || request.StatusCode != HttpStatusCode.OK|| res_str.Contains("https://sec.douban.com/a"))
                {
                    
                    if (request.StatusCode == HttpStatusCode.NotFound)
                    {
                        throw new Exception();
                    }
                    if (res_str.Contains("https://sec.douban.com/a"))
                    {
                        ChangeIP();
                    }
                    return CollectionDouBanHtmlCelebrity(celebrityID);
                }
                else
                {
                    Model.DouBan.Celebrity celebrity = new Celebrity();
                    HtmlDocument hd = new HtmlDocument();
                    hd.LoadHtml(res_str);

                    //豆瓣ID
                    celebrity.CelebrityID = celebrityID;

                    //姓名
                    celebrity.Name = null;
                    var name_node = hd.DocumentNode.SelectSingleNode("//div[@id='content']/h1");
                    if (name_node!=null)
                        celebrity.Name = name_node.InnerText.Trim();

                    if (!string.IsNullOrWhiteSpace(celebrity.Name))
                        Console.WriteLine("获取到名人：" + celebrity.Name);

                    //图片
                    celebrity.Picture = null;
                    celebrity.PictureID = null;
                    var img_node = hd.DocumentNode.SelectSingleNode("//div[@id='headline']/div[@class='pic']/a[@class='nbg']/img");
                    if (img_node!=null)
                    {
                        //https://img3.doubanio.com/img/celebrity/medium/230.jpg
                        var img_medium = img_node.GetAttributeValue("src", "");
                        if (!string.IsNullOrWhiteSpace(img_medium)&& img_medium.Contains("medium"))
                        {
                            celebrity.Picture = new Image() {
                                Medium = img_medium,
                                Large = img_medium.Replace("medium", "large"),
                                Small = img_medium.Replace("medium", "small")
                            };
                        }
                    }
                    //性别
                    celebrity.Gender = null;
                    var gender_span_node = hd.DocumentNode.SelectSingleNode("//span[text()='性别']");
                    if (gender_span_node != null)
                    {
                        var gender_parent_node = gender_span_node.ParentNode;
                        if (gender_parent_node!=null)
                        {
                            var gender_str = gender_parent_node.InnerText.Trim().Replace("性别", "").Replace(":", "").Trim();
                            if (gender_str == "男")
                                celebrity.Gender = 1;
                            if (gender_str == "女")
                                celebrity.Gender = 0;
                        }
                    }
                    //性别
                    celebrity.Constellation = null;
                    var constellation_span_node = hd.DocumentNode.SelectSingleNode("//span[text()='星座']");
                    if (constellation_span_node != null)
                    {
                        var constellation_parent_node = constellation_span_node.ParentNode;
                        if (constellation_parent_node != null)
                        {
                            var constellation_str = constellation_parent_node.InnerText.Trim().Replace("星座", "").Replace(":", "").Trim();
                            celebrity.Constellation = constellation_str;
                        }
                    }
                    //出生日期
                    celebrity.Birthday = null;
                    var birthday_span_node = hd.DocumentNode.SelectSingleNode("//span[text()='出生日期']");
                    if (birthday_span_node != null)
                    {
                        var birthday_parent_node = birthday_span_node.ParentNode;
                        if (birthday_parent_node != null)
                        {
                            var birthday_str = birthday_parent_node.InnerText.Trim().Replace("出生日期", "").Replace(":", "").Trim();
                            var dates = birthday_str.Split('-');
                            DateTime? birthday_date = null;
                            try
                            {
                                if (dates.Length > 0)
                                    if (dates[0].Length == 4)
                                        birthday_date = new DateTime(Utils.StrToInt(dates[0], 0), dates.Length > 1 ? Utils.StrToInt(dates[1], 1) : 1, dates.Length > 2 ? Utils.StrToInt(dates[2], 1) : 1);
                            }
                            catch (Exception)
                            {
                                birthday_date = null;
                            }
                            celebrity.Birthday = birthday_date;
                        }
                    }
                    //出生地
                    celebrity.Homeplace = null;
                    var homeplace_span_node = hd.DocumentNode.SelectSingleNode("//span[text()='出生地']");
                    if (homeplace_span_node != null)
                    {
                        var homeplace_parent_node = homeplace_span_node.ParentNode;
                        if (homeplace_parent_node != null)
                        {
                            var homeplace_str = homeplace_parent_node.InnerText.Trim().Replace("出生地", "").Replace(":", "").Trim();
                            celebrity.Homeplace = homeplace_str;
                        }
                    }
                    //职业
                    celebrity.Occupations = new List<Occupation>();
                    var occupations_span_node = hd.DocumentNode.SelectSingleNode("//span[text()='职业']");
                    if (occupations_span_node != null)
                    {
                        var occupations_parent_node = occupations_span_node.ParentNode;
                        if (occupations_parent_node != null)
                        {
                            var occupations_str = occupations_parent_node.InnerText.Trim().Replace("职业", "").Replace(":", "").Trim();
                            if (!string.IsNullOrWhiteSpace(occupations_str))
                            {
                                var occupations = occupations_str.Split('/');
                                foreach (var item in occupations)
                                {
                                    var occupation_name = item.Trim();
                                    if (string.IsNullOrWhiteSpace(occupation_name))
                                        continue;
                                    celebrity.Occupations.Add(new Occupation()
                                    {
                                        OccupationName = occupation_name
                                    });
                                }
                            }
                        }
                    }
                    //更多外文名
                    celebrity.MoreNameEn = null;
                    var morenameen_span_node = hd.DocumentNode.SelectSingleNode("//span[text()='更多外文名']");
                    if (morenameen_span_node != null)
                    {
                        var morenameen_parent_node = morenameen_span_node.ParentNode;
                        if (morenameen_parent_node != null)
                        {
                            var morenameen_str = morenameen_parent_node.InnerText.Trim().Replace("更多外文名", "").Replace(":", "").Trim();
                            celebrity.MoreNameEn = morenameen_str;
                        }
                    }
                    //更多中文名
                    celebrity.MoreNameZh = null;
                    var morenamezh_span_node = hd.DocumentNode.SelectSingleNode("//span[text()='更多中文名']");
                    if (morenamezh_span_node != null)
                    {
                        var morenamezh_parent_node = morenamezh_span_node.ParentNode;
                        if (morenamezh_parent_node != null)
                        {
                            var morenamezh_str = morenamezh_parent_node.InnerText.Trim().Replace("更多中文名", "").Replace(":", "").Trim();
                            celebrity.MoreNameZh = morenamezh_str;
                        }
                    }
                    //imdb编号
                    celebrity.IMDbID = null;
                    var imdbid_span_node = hd.DocumentNode.SelectSingleNode("//span[text()='imdb编号']");
                    if (imdbid_span_node != null)
                    {
                        var imdbid_parent_node = imdbid_span_node.ParentNode;
                        if (imdbid_parent_node != null)
                        {
                            var imdbid_str = imdbid_parent_node.InnerText.Trim().Replace("imdb编号", "").Replace(":", "").Trim();
                            celebrity.IMDbID = imdbid_str;
                        }
                    }

                    return celebrity;

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataAcquisitionLib.DouBan.Movie CollectionDouBanApiMovie(string doubanID, int timeout = 30000)
        {
            var res_str = "";
            CookieCollection cookie = new CookieCollection();
            var url = "http://api.douban.com/v2/movie/subject/" + doubanID;
            try
            {
                var request = hwru.SendRequest(url, null, null, null, null, "GET", out res_str, out cookie, Encoding.UTF8, timeout);
                if (request == null || request.StatusCode != HttpStatusCode.OK)
                {
                    ChangeIP();
                    return CollectionDouBanApiMovie(doubanID);
                }
                else
                {
                    return res_str.ParseJSON<DataAcquisitionLib.DouBan.Movie>();
                }
            }
            catch (Exception)
            {
                Console.WriteLine(res_str);
                return null;
            }
        }

        private void ChangeIP(int time = 3000)
        {

            try
            {
                //Console.WriteLine("开始更换IP...");
                ////断线
                //var str_res = hwru.Get("http://192.168.31.1/cgi-bin/luci/;stok=6186d3eefba2d6f4b9cc56e0c158e8ba/api/xqnetwork/pppoe_stop", "", Encoding.UTF8);
                //Console.WriteLine(str_res);
                //Thread.Sleep(time);
                //str_res = hwru.Get("http://192.168.31.1/cgi-bin/luci/;stok=6186d3eefba2d6f4b9cc56e0c158e8ba/api/xqnetwork/pppoe_start", "", Encoding.UTF8);

                //Console.WriteLine(str_res);
                //Console.WriteLine("更换IP完毕");
                //Thread.Sleep(time);

                Console.WriteLine("开始更换IP...");
                //断开连接并重新拨号
                //断线
                RASDisplay ras = new RASDisplay();
                ras.Disconnect();
                Thread.Sleep(time);
                ras.Connect("ADSL");
                Console.WriteLine("更换IP完毕");

            }
            catch (Exception err)
            {
                Console.WriteLine("更换IP失败:" + err.Message);
            }
        }
    }
}
