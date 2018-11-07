using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication_API.Controllers
{
    public class SubtitleController : ApiController
    {
        BLL.GaoQing.Movie_BLL movie_bll = new BLL.GaoQing.Movie_BLL();
        public Models.MovieSubtitle Get(string hash, string category)
        {
            category = category.IndexOf(" ") > 0 ? category.Substring(0, category.IndexOf(" ")) : category;
            Models.MovieSubtitle model = new Models.MovieSubtitle();
            var subtitles = movie_bll.GetMovieSubtitleByHash(hash);
            model.source = new List<string>(){
                "BluRay ("+subtitles.Count(c=>c.Clear.Contains("BluRay"))+")",
                "HDTV ("+subtitles.Count(c=>c.Clear.Contains("HDTV"))+")",
                "WEBDL ("+subtitles.Count(c=>c.Clear.Contains("WEB-DL"))+")",
                "其他 ("+subtitles.Count(c=>c.Clear.Contains("未知"))+")"
            };
            model.subtitle = new List<Models.Subtitle>();
            switch (category)
            {
                case "BluRay":
                    subtitles.Where(w => w.Clear.Contains("BluRay")).ToList().ForEach(m => {
                        model.subtitle.Add(new Models.Subtitle()
                        {
                            ihash = m.Download.Replace("https://gaoqing.fm/down_file.php?ihash=", "").Trim(),
                            clear = m.Clear,
                            suffix = m.Format,
                            id = m.ID.ToString(),
                            name = m.Title
                        });
                    });
                    model.selected = model.source[0];
                    break;
                case "HDTV":
                    subtitles.Where(w => w.Clear.Contains("HDTV")).ToList().ForEach(m => {
                        model.subtitle.Add(new Models.Subtitle()
                        {
                            ihash = m.Download.Replace("https://gaoqing.fm/down_file.php?ihash=", "").Trim(),
                            clear = m.Clear,
                            suffix = m.Format,
                            id = m.ID.ToString(),
                            name = m.Title
                        });
                    });
                    model.selected = model.source[1];
                    break;
                case "WEBDL":
                    subtitles.Where(w => w.Clear.Contains("WEB-DL")).ToList().ForEach(m => {
                        model.subtitle.Add(new Models.Subtitle()
                        {
                            ihash = m.Download.Replace("https://gaoqing.fm/down_file.php?ihash=", "").Trim(),
                            clear = m.Clear,
                            suffix = m.Format,
                            id = m.ID.ToString(),
                            name = m.Title
                        });
                    });
                    model.selected = model.source[2];
                    break;
                case "其他":
                    subtitles.Where(w => w.Clear.Contains("未知")).ToList().ForEach(m => {
                        model.subtitle.Add(new Models.Subtitle()
                        {
                            ihash = m.Download.Replace("https://gaoqing.fm/down_file.php?ihash=", "").Trim(),
                            clear = m.Clear,
                            suffix = m.Format,
                            id = m.ID.ToString(),
                            name = m.Title
                        });
                    });
                    model.selected = model.source[3];
                    break;

            }
            return model;
        }
    }
}