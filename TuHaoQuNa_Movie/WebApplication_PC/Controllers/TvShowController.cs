using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication_PC.Controllers
{
    public class TvShowController : Controller
    {
        // GET: TvShow
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HotTvShow()
        {
            return View();
        }
    }
}