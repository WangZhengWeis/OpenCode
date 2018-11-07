using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication_PC.Controllers
{
    public class XXXController : Controller
    {

        BLL.RARGB.Torrent_BLL bll = new BLL.RARGB.Torrent_BLL();

        // GET: XXX
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Random()
        {
            return View(bll.GetXXXMovie());
        }
    }
}