using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication_API.Controllers
{
    public class SummaryController : ApiController
    {

        BLL.GaoQing.Movie_BLL movie_bll = new BLL.GaoQing.Movie_BLL();

        public List<Models.Movie> Get(string type, int showNum)
        {
            List<Models.Movie> list = new List<Models.Movie>();
            Model.GaoQing.MovieOrderBy movieorderby = Model.GaoQing.MovieOrderBy.最新;
            switch (type)
            {
                case "hot":
                    movieorderby = Model.GaoQing.MovieOrderBy.热门;
                    break;
                case "score":
                    movieorderby = Model.GaoQing.MovieOrderBy.评分;
                    break;
            }
            var movies = movie_bll.PageMovie(movieorderby, null, 1, showNum);
            movies.ForEach(m => {
                list.Add(new Models.Movie() {
                    hash = m.Hash,
                    name = m.Name,
                    nd = m.Nd.HasValue?m.Nd.ToString():"",
                    subject = m.Subject,
                    rate = string.Format("{0:N1}",m.Rate)
                });
            });
            return list;
        }
    }
}