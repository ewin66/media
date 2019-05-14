using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using WebVideoViewer.Models;
using WebVideoViewer.DataAccess;

namespace WebVideoViewer.Controllers
{
    public class HomeController : Controller
    {
        private SourceRepository _sourceRepository;

        public HomeController()
        {
            _sourceRepository = new SourceRepository(new video_streamingEntities());
        }

        public ActionResult Index()
        {
            IEnumerable<source> sources = from s in _sourceRepository.GetSourcesForRemoteClient(Request.UserHostAddress, @"application/x-mpegURL")
                                          select s;
            return View(sources);
        }

        public ActionResult About()
        {
            ViewBag.Message = "AntaresX Video Viewer";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Future Concepts Contacts";

            return View();
        }
    }
}