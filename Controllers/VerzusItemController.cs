using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Verzus.Controllers
{
    public class VerzusItemController : Controller
    {
        Models.VerzusLinqSqlDataContext c = new Models.VerzusLinqSqlDataContext();
        //
        // GET: /VerzusItem/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /VerzusItem/Details/5
        [HttpPost]
        public JsonResult Details(int id)
        {
            var vsItem = from vsi in c.VerzusItems where (vsi.ItemId == id) select vsi;
            Models.VerzusItem jsItem = vsItem.First();
            JsonResult r = Json(new { ItemId = jsItem.ItemId, ItemType = jsItem.ItemType, ItemContent = jsItem.ItemContent, ItemDateAdded = jsItem.ItemDateAdded });
            return r;
        }

        //
        // POST: /VerzusItem/Search
        [HttpPost]
        public JsonResult Search(string id)
        {
            var vsItems = from vsi in c.VerzusItems where (vsi.ItemContent.Contains(id)) select vsi;
            List<object> list = new List<object>();
            foreach (Models.VerzusItem item in vsItems)
            {
                list.Add(new { ItemId = item.ItemId, ItemType = item.ItemType, ItemContent = item.ItemContent, ItemDateAdded = item.ItemDateAdded });
            }
            return Json(list);
        }

        //
        // GET: /VerzusItem/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /VerzusItem/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /VerzusItem/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /VerzusItem/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /VerzusItem/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /VerzusItem/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
