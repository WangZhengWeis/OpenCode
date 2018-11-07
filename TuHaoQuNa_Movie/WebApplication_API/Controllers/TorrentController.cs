using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication_API.Controllers
{
    public class TorrentController : ApiController
    {
        BLL.GaoQing.Movie_BLL movie_bll = new BLL.GaoQing.Movie_BLL();
        public Models.MovieTorrent Get(string hash, string category)
        {
            category = category.IndexOf(" ")>0 ? category.Substring(0, category.IndexOf(" ")): category;
            Models.MovieTorrent model = new Models.MovieTorrent();
            var torrents = movie_bll.GetMovieTorrentByHash(hash);
            model.source = new List<string>(){
                "BluRay ("+torrents.Count(c=>c.Clear.Contains("原盘")||c.Clear.Contains("3D"))+")",
                "1080P ("+torrents.Count(c=>c.Clear.Contains("1080"))+")",
                "720P ("+torrents.Count(c=>c.Clear.Contains("720"))+")",
                "标清 ("+torrents.Count(c=>c.Clear.Contains("WEB-DL")|| c.Clear.Contains("硬字幕版")|| c.Clear.Contains("HDTV"))+")",
                "其他 ("+torrents.Count(c=>c.Clear.Contains("非高清或错误"))+")"
            };
            model.cililian = new List<Models.Torrent>();
            switch (category)
            {
                case "BluRay":
                    torrents.Where(w => w.Clear.Contains("原盘") || w.Clear.Contains("3D")).ToList().ForEach(m=> {
                        model.cililian.Add(new Models.Torrent() {
                            magnet = m.Magnet.Replace("magnet:?xt=urn:btih:","").Trim(),
                            meta = m.Meta,
                            name = m.Title,
                            size = string.Format("{0:N2}", (Convert.ToDouble(m.Size) / (1024.0 * 1024.0)))+" GB"
                        });
                    });
                    model.selected = model.source[0];
                    break;
                case "1080P":
                    torrents.Where(w => w.Clear.Contains("1080")).ToList().ForEach(m => {
                        model.cililian.Add(new Models.Torrent()
                        {
                            magnet = m.Magnet.Replace("magnet:?xt=urn:btih:", "").Trim(),
                            meta = m.Meta,
                            name = m.Title,
                            size = string.Format("{0:N2}", (Convert.ToDouble(m.Size) / (1024.0 * 1024.0))) + " GB"
                        });
                    });
                    model.selected = model.source[1];
                    break;
                case "720P":
                    torrents.Where(w => w.Clear.Contains("720")).ToList().ForEach(m => {
                        model.cililian.Add(new Models.Torrent()
                        {
                            magnet = m.Magnet.Replace("magnet:?xt=urn:btih:", "").Trim(),
                            meta = m.Meta,
                            name = m.Title,
                            size = string.Format("{0:N2}", (Convert.ToDouble(m.Size) / (1024.0 * 1024.0))) + " GB"
                        });
                    });
                    model.selected = model.source[2];
                    break;
                case "标清":
                    torrents.Where(w => w.Clear.Contains("WEB-DL") || w.Clear.Contains("硬字幕版") || w.Clear.Contains("HDTV")).ToList().ForEach(m => {
                        model.cililian.Add(new Models.Torrent()
                        {
                            magnet = m.Magnet.Replace("magnet:?xt=urn:btih:", "").Trim(),
                            meta = m.Meta,
                            name = m.Title,
                            size = string.Format("{0:N2}", (Convert.ToDouble(m.Size) / (1024.0 * 1024.0))) + " GB"
                        });
                    });
                    model.selected = model.source[3];
                    break;
                case "其他":
                    torrents.Where(w => w.Clear.Contains("非高清或错误")).ToList().ForEach(m => {
                        model.cililian.Add(new Models.Torrent()
                        {
                            magnet = m.Magnet.Replace("magnet:?xt=urn:btih:", "").Trim(),
                            meta = m.Meta,
                            name = m.Title,
                            size = string.Format("{0:N2}", (Convert.ToDouble(m.Size) / (1024.0 * 1024.0))) + " GB"
                        });
                    });
                    model.selected = model.source[4];
                    break;

            }
            return model;
        }
    }
}