using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication_API.Controllers
{
    public class ListfilmController : ApiController
    {
        BLL.GaoQing.Movie_BLL movie_bll = new BLL.GaoQing.Movie_BLL();
        
        public List<Models.Movie> Get(int page, int limit, string order = "", string type = "", string year = "")
        {
            List<Models.Movie> list = new List<Models.Movie>();
            Model.GaoQing.MovieOrderBy movieorderby = Model.GaoQing.MovieOrderBy.最新;
            switch (order)
            {
                case "热度":
                    movieorderby = Model.GaoQing.MovieOrderBy.热门;
                    break;
                case "评分":
                    movieorderby = Model.GaoQing.MovieOrderBy.评分;
                    break;
            }
            int? type_id = null;
            if (!string.IsNullOrWhiteSpace(type))
            {
                var type_obj = movie_bll.SingleCategoryByName(type);
                type_id = type_obj == null ? 0 : type_obj.ID;
            }
            var movies = movie_bll.PageMovie(movieorderby, Model.GaoQing.Family.电影, page, limit, type_id, null, null, null, null, year);
            movies.ForEach(m=> {
                list.Add(new Models.Movie() {
                    hash = m.Hash,
                    name = m.Name,
                    nd = m.Nd.HasValue ? m.Nd.ToString() : "",
                    subject = m.Subject,
                    rate = string.Format("{0:N1}", m.Rate)
                });
            });
            return list;
        }
    }
}