using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication_NewPC
{
    public class Movie
    {
        public Model.GaoQing.Movie MovieInfo { get; set; }

        public List<Model.GaoQing.MovieTorrent> MovieTorrent { get; set; }

        public List<Model.GaoQing.Subtitle> MovieSubtitle { get; set; }

        public List<Model.Player> Player { get; set; }
    }
}