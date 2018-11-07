using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication_Player.Controllers
{
    public class PlayerController : Controller
    {
        // GET: Player
        public ActionResult Index(string url, string name)
        {
            ViewBag.Title = name;
            ViewBag.Url = url;
            return View();
        }

        public ActionResult Play(string type, string vid)
        {
            ViewBag.Type = type;
            ViewBag.Vid = vid;
            return View();
        }
    }
}