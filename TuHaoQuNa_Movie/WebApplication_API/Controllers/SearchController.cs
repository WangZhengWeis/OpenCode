using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication_API.Controllers
{
    public class SearchController : ApiController
    {
        BLL.GaoQing.Movie_BLL movie_bll = new BLL.GaoQing.Movie_BLL();
        
        public Models.Search Get(int page, int limit, string search)
        {
            Models.Search obj = new Models.Search();
            obj.films = new List<Models.MovieInfo>();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var pageObj = movie_bll.SearchMovieByPage(search, page, limit);
                obj.count = pageObj.TotalItems.ToString();
                pageObj.Items.ForEach(m=> {
                    string country_str = "";
                    if (m.Country != null)
                        m.Country.ForEach(c => {
                            country_str += c.Name + "/";
                        });
                    country_str = country_str.TrimEnd('/').Replace("/", " / ");

                    string foname_str = "";
                    if (m.MoreName != null)
                        m.MoreName.ForEach(c => {
                            foname_str += c.Name + "/";
                        });
                    foname_str = foname_str.TrimEnd('/').Replace("/", " / ");

                    string fupdate_str = "";
                    if (m.ReleaseTime != null)
                        m.ReleaseTime.ForEach(c => {
                            if (c.ReleaseDate.HasValue)
                                fupdate_str += c.ReleaseDate.Value.ToString("yyyy-MM-dd") + (string.IsNullOrWhiteSpace(c.Place) ? "" : "(" + c.Place + ")") + "/";
                        });
                    fupdate_str = fupdate_str.TrimEnd('/').Replace("/", " / ");

                    string type_str = "";
                    if (m.Category != null)
                        m.Category.ForEach(c => {
                            type_str += c.Name + "/";
                        });
                    type_str = type_str.TrimEnd('/').Replace("/", " / ");

                    obj.films.Add(new Models.MovieInfo() {
                        hash = m.Hash,
                        name = m.Name,
                        nd = m.Nd.HasValue ? m.Nd.ToString() : "",
                        subject = m.Subject,
                        rate = string.Format("{0:N1}", m.Rate),
                        country = country_str,
                        foname = foname_str,
                        fupdate = fupdate_str,
                        type = type_str
                    });
                });
            }
            return obj;
        }
    }
}