using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication_API.Models
{
    public class Movie
    {
        public string hash { get; set; }

        public string subject { get; set; }

        public string name { get; set; }

        public string nd { get; set; }

        public string rate { get; set; }
    }

    public class MovieInfo : Movie
    {
        public string country { get; set; }

        public string foname { get; set; }

        public string fupdate { get; set; }

        public string type { get; set; }
    }


    public class MovieFullInfo : MovieInfo
    {
        public string description { get; set; }
    }

    public class Search
    {
        public string count { get; set; }

        public List<MovieInfo> films { get; set; }
    }

    public class Torrent
    {
        public string magnet { get; set; }

        public string meta { get; set; }

        public string name { get; set; }

        public string size { get; set; }
    }

    public class MovieTorrent
    {
        public string selected { get; set; }

        public List<string> source { get; set; }

        public List<Torrent> cililian { get; set; }
    }

    public class Subtitle
    {
        public string clear { get; set; }
        public string id { get; set; }

        public string ihash { get; set; }

        public string name { get; set; }

        public string suffix { get; set; }

    }

    public class MovieSubtitle
    {
        public string selected { get; set; }

        public List<string> source { get; set; }

        public List<Subtitle> subtitle { get; set; }
    }

}