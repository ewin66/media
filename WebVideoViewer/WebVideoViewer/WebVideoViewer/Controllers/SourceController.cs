using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using WebVideoViewer.DataAccess;
using WebVideoViewer.Models;

namespace WebVideoViewer.Controllers
{
    public class SourceController : Controller
    {
        private SourceRepository _sourceRepository;

        public SourceController()
        {
            _sourceRepository = new SourceRepository(new video_streamingEntities());
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Present(String id)
        {
            if (id != null)
            {
                source cameraSource = _sourceRepository.GetSourceByID(id);
                return View(cameraSource);
            }
            else
                return View("Error");
        }
    }
}