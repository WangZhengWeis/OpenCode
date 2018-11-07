using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication_PC.Models
{


    public class RecommendMovie
    {
        public List<Model.GaoQing.Movie> SameDirector { get; set; }

        public List<Model.GaoQing.Movie> SameToStar { get; set; }

        public List<Model.GaoQing.Movie> SameType { get; set; }
    }

}