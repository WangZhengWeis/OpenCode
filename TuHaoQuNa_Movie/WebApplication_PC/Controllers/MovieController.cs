using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication_PC.Controllers
{
    public class MovieController : Controller
    {
        BLL.GaoQing.Movie_BLL movie_bll = new BLL.GaoQing.Movie_BLL();

        // GET: Movie
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(int? type, int? country, int? director, int? screenwriter, int? actor, string year, string sort, string family = "电影", int page = 1)
        {
            int pageSize = 20;
            List<Model.GaoQing.BaseMovie> list = new List<Model.GaoQing.BaseMovie>();
            Model.GaoQing.Family e_family = Model.GaoQing.Family.电影;
            switch (family)
            {
                case "电视剧":
                    e_family = Model.GaoQing.Family.电视剧;
                    break;
            }
            switch (sort)
            {
                case "热门":
                    list = movie_bll.PageMovie(Model.GaoQing.MovieOrderBy.热门, e_family, page, pageSize, type, country, director, screenwriter, actor, year);
                    break;
                case "评分":
                    list = movie_bll.PageMovie(Model.GaoQing.MovieOrderBy.评分, e_family, page, pageSize, type, country, director, screenwriter, actor, year);
                    break;
                default:
                    list = movie_bll.PageMovie(Model.GaoQing.MovieOrderBy.最新, e_family, page, pageSize, type, country, director, screenwriter, actor, year);
                    break;
            }
            return View(list);
        }

        public ActionResult Genre()
        {
            return View(movie_bll.GetCategoryList());
        }

        public ActionResult Country()
        {
            return View(movie_bll.GetCountryList());
        }

        public ActionResult Year()
        {
            return View(movie_bll.GetYearGroup());
        }

        public ActionResult HotMovie(int page = 1)
        {
            return View(movie_bll.PageMovie(Model.GaoQing.MovieOrderBy.热门, null, page, 10));
        }
        
        public ActionResult ScoreMovie(int page = 1)
        {
            return View(movie_bll.PageMovie(Model.GaoQing.MovieOrderBy.评分, null, page, 10));
        }

        public ActionResult NewSubtitle(int page = 1)
        {
            return View();
        }

        public ActionResult Detailte(string id)
        {
            var model = movie_bll.SingleBaseMovieByHash(id);
            ViewBag.Title = model == null ? "" : model.Title;
            return View();
        }

        public ActionResult MovieInfo(string id)
        {
            return View(movie_bll.SingleMovieFullByHash(id));
        }

        public ActionResult RecommendMovies()
        {
            return View();
        }

        public ActionResult FilmWithDirector(string id)
        {
            return View(movie_bll.GetFilmWithDirectorMovieByHash(id));
        }

        public ActionResult StarringFilm(string id)
        {
            return View(movie_bll.GetStarringFilmMovieByHash(id));
        }

        public ActionResult FilmOfTheSameGenre(string id)
        {
            return View(movie_bll.GetFilmOfTheSameGenreMovieByHash(id));
        }

        public ActionResult DownloadMovie(string id)
        {
            return View(movie_bll.GetMovieTorrentByHash(id));
        }

        public ActionResult DownloadSubtitle(string id)
        {
            return View(movie_bll.GetMovieSubtitleByHash(id));
        }

        public ActionResult Search(string q)
        {
            ViewBag.Title = q;
            return View();
        }

        public ActionResult SearchList(string q, int page = 1)
        {
            return View(movie_bll.SearchMovie(q,page,6));
        }

    }
}