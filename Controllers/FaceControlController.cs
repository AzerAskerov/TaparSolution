using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TaparSolution.Controllers
{
    public class FaceControlController : Controller
    {
        // GET: FaceControlController
        public ActionResult Index()
        {
            return View("FaceControlView.cshtml");
        }

        // GET: FaceControlController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: FaceControlController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FaceControlController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: FaceControlController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: FaceControlController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: FaceControlController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: FaceControlController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
