using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialNetwork.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Сайт предназначен для предоставления пользователям инструмента общения, обмена мнениями и получения информации в сети Интернет.";

            return View();
        }

        public ActionResult Contact()
        {

            return View();
        }
    }
}