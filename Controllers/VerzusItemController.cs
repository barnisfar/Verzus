﻿using System;
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
            JsonResult r = Json(new { ItemId = jsItem.ItemId, ItemType = jsItem.ItemType, ItemContent = jsItem.ItemContent.Trim(), ItemDateAdded = jsItem.ItemDateAdded });
            return r;
        }

        //
        // POST: /VerzusItem/Search
        [HttpPost]
        public JsonResult Search(string id)
        {
            var vsItems = from vsi in c.VerzusItems where (vsi.ItemContent.Contains(id)) select vsi;
            
            List<object> list = pack(vsItems);
            
            return Json(list);
        }

        //
        // POST: /VerzusItem/Search
        [HttpPost]
        public JsonResult SearchByIds(List<int> idList)
        {
            var vsItems = from vsi in c.VerzusItems where (idList.Contains((int)vsi.ItemId)) select vsi;
            
            List<object> list = pack(vsItems);
            
            return Json(list);
        }

        //
        // POST: /VerzusItem/Matters
        [HttpPost]
        public JsonResult JMatters()
        {
            var vsItems = from vsi in c.VerzusItems where vsi.ItemIsContext == 1 select vsi;
            
            List<object> list = pack(vsItems);
            
            return Json(list);
        }

        //
        // GET: /VerzusItem/Matters
        public ActionResult Matters()
        {
            var vsItems = from vsi in c.VerzusItems where vsi.ItemIsContext == 1 select vsi;

            ViewBag.VerzusItems = vsItems;
            ViewBag.Title = "Matters";

            return View("Subjects");
        }

        //
        // GET: /VerzusItem/Matters
        public ActionResult Competitors()
        {
            var vsItems = from vsi in c.VerzusItems where vsi.ItemIsSubject == 1 select vsi;

            ViewBag.VerzusItems = vsItems;
            ViewBag.Title = "Competitors";

            return View("Subjects");
        }

        private List<object> pack(IQueryable vsItems) {
            List<object> list = new List<object>();
            foreach (Models.VerzusItem item in vsItems)
            {
                list.Add(new { ItemId = item.ItemId, ItemType = item.ItemType, ItemContent = item.ItemContent.Trim(), ItemDateAdded = item.ItemDateAdded });
            }
            return list;
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

        [HttpPost]
        public JsonResult GetResult(string term)
        {

            var searchTerm = term;
            var web = new System.Net.WebClient();

            web.Headers.Add("Referrer", "http://www.verzus.it/");
            var result = web.DownloadString(String.Format(
                   "https://www.googleapis.com/customsearch/v1?key=AIzaSyBa5X9iZRvFlNr6-8q9roX_exXbI7BXlO0&cx=013036536707430787589:_pqjad5hr1a&alt=json&num=1&q={0}",
                   searchTerm));
            var imgResult = web.DownloadString(String.Format(
                   "https://www.googleapis.com/customsearch/v1?key=AIzaSyBa5X9iZRvFlNr6-8q9roX_exXbI7BXlO0&cx=013036536707430787589:_pqjad5hr1a&alt=json&num=1&q=site:{0}",
                   searchTerm));

            web.Dispose();
            return Json("[" + result + "," + imgResult + "]");
        }
    }
}
