using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication_NewPC.Controllers
{
    public class HomeController : Controller
    {
        BLL.GaoQing.Movie_BLL movie_bll = new BLL.GaoQing.Movie_BLL();
        BLL.Player_BLL player_bll = new BLL.Player_BLL();

        // GET: Home
        public ActionResult Index()
        {
            List<Model.GaoQing.BaseMovie> list = new List<Model.GaoQing.BaseMovie>();
            list.AddRange(movie_bll.PageMovie(Model.GaoQing.MovieOrderBy.热门, Model.GaoQing.Family.电影, 1, 20));
            list.AddRange(movie_bll.PageMovie(Model.GaoQing.MovieOrderBy.热门, Model.GaoQing.Family.电视剧, 1, 20));
            return View(list);
        }

        public ActionResult Movie(string year = "all", int type = 0, int country = 0, int order = 0, int page = 1)
        {
            if (page < 1)
                page = 1;
            int? genreID = null;
            if (type > 0)
                genreID = type;

            int? countryID = null;
            if (country > 0)
                countryID = country;
            var family = Model.GaoQing.MovieOrderBy.最新;
            if (order == 1)
                family = Model.GaoQing.MovieOrderBy.热门;
            if (order == 2)
                family = Model.GaoQing.MovieOrderBy.评分;
            if (order == 3)
                family = Model.GaoQing.MovieOrderBy.可在线播放;
            //, int? countryID = null, int? directorID = null, int? screenwriterID = null, int? performerID = null, string year = null;
            var pageObj = movie_bll.PageMovieByPage(family, Model.GaoQing.Family.电影, page, 40,
                genreID,
                countryID,
                null,
                null,
                null,
                year=="all"?null:year
                );
            ViewBag.TotalPage = Convert.ToInt32(pageObj.TotalPages);
            ViewBag.Page = Convert.ToInt32(page);
            ViewBag.Type = type;
            ViewBag.Country = country;
            ViewBag.Year = year;
            ViewBag.Order = order;
            ViewBag.Url = "/movie";
            return View(pageObj.Items);
        }

        public ActionResult TvShow(string year = "all", int type = 0, int country = 0, int order = 0, int page = 1)
        {
            if (page < 1)
                page = 1;
            int? genreID = null;
            if (type > 0)
                genreID = type;

            int? countryID = null;
            if (country > 0)
                countryID = country;
            var family = Model.GaoQing.MovieOrderBy.最新;
            if (order == 1)
                family = Model.GaoQing.MovieOrderBy.热门;
            if (order == 2)
                family = Model.GaoQing.MovieOrderBy.评分;
            if (order == 3)
                family = Model.GaoQing.MovieOrderBy.可在线播放;
            var pageObj = movie_bll.PageMovieByPage(family, Model.GaoQing.Family.电视剧, page, 40,
                genreID,
                countryID,
                null,
                null,
                null,
                year == "all" ? null : year
                );
            ViewBag.TotalPage = Convert.ToInt32(pageObj.TotalPages);
            ViewBag.Page = Convert.ToInt32(page);
            ViewBag.Type = type;
            ViewBag.Country = country;
            ViewBag.Year = year;
            ViewBag.Order = order;
            ViewBag.Url = "/tvshow";
            return View(pageObj.Items);
        }



        public ActionResult Subject(string hash)
        {
            var movie = movie_bll.SingleMovieFullByHash(hash);
            ViewBag.Title = movie.Name;
            return View(new Movie() {
                MovieInfo = movie,
                MovieSubtitle = movie_bll.GetMovieSubtitleByHash(hash),
                MovieTorrent = movie_bll.GetMovieTorrentByHash(hash),
                Player = player_bll.GetPlayerByMovieID(movie.ID)
            });
        }

        public ActionResult Search(string q = "", int celebrity = 0, int page = 1)
        {
            if (page < 1)
                page = 1;
            var pageObj = new Page<Model.GaoQing.BaseMovie>();
            if (celebrity > 0)
            {
                q = "";
                var c = movie_bll.SingleCelebrity(celebrity);
                ViewBag.Keyword = c == null ? "" : c.Name;
                ViewBag.Title = c == null ? "" : c.Name;
                pageObj = movie_bll.SearchMovieByCelebrity(celebrity, page, 40);
            }
            else
            {
                ViewBag.Keyword = q;
                celebrity = 0;
                ViewBag.Title = q;
                pageObj = movie_bll.SearchMovieByKeyword(q, page, 40);
            }
            ViewBag.TotalPage = Convert.ToInt32(pageObj.TotalPages);
            ViewBag.Page = Convert.ToInt32(page);
            ViewBag.TotalItem = Convert.ToInt32(pageObj.TotalItems);
            ViewBag.Q = q;
            
            ViewBag.Celebrity = celebrity;
            return View(pageObj.Items);
        }

        public ActionResult Player(int id)
        {
            return View(player_bll.SinglePlayer(id));
        }

    }
}