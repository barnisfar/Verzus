﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Linq;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Linq.SqlClient;
using System.Runtime.Serialization;

namespace Verzus.Controllers
{
    public class VerzusController : Controller
    {
        
        Models.VerzusLinqSqlDataContext c = new Models.VerzusLinqSqlDataContext();

        //
        // GET: /Verzus/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Verzus/Details/5

        public ActionResult Details(string urlKey, int context)
        {
            IEnumerable<Models.Verzus> vsEnum = (from v in c.Verzus where v.VerzusUrlKey == urlKey select v);

            if (context > 0) vsEnum.Where(x => x.VerzusItemContext == context);

            Models.Verzus vs = vsEnum.First();
            Models.VerzusItem vsi1 = (from i1 in c.VerzusItems where i1.ItemId == vs.VerzusItem1 select i1).First();
            Models.VerzusItem vsi2 = (from i2 in c.VerzusItems where i2.ItemId == vs.VerzusItem2 select i2).First();
            Models.VerzusItem vsiC = (from iC in c.VerzusItems where iC.ItemId == vs.VerzusItemContext select iC).First();

            ViewBag.date = vs.VerzusDateAdded;
            ViewBag.item1Id = vsi1.ItemId;
            ViewBag.item2Id = vsi2.ItemId;
            ViewBag.itemCId = vsiC.ItemId;
            ViewBag.item1Content = vsi1.ItemContent;
            ViewBag.item2Content = vsi2.ItemContent;
            ViewBag.itemCContent = vsiC.ItemContent;
            ViewBag.urlKey = vs.VerzusUrlKey;

            return View("Details");
        }

        //
        // GET: /Verzus/Get/5

        public ActionResult Get(string id)
        {
            return Details(id, 0);
        }

        //
        // POST: /Verzus/Search

        [HttpPost]
        public JsonResult JSearch(int item1, int item2, int context)
        {
            IEnumerable<Models.Verzus> vsEnum = (from v in c.Verzus select v);

            if (item1 > 0) vsEnum = vsEnum.Where(x => x.VerzusItem1 == item1);
            if (item2 > 0) vsEnum = vsEnum.Where(x => x.VerzusItem2 == item2);
            if (context > 0) vsEnum = vsEnum.Where(x => x.VerzusItemContext == context);

            List<object> list = new List<object>();
            foreach (Models.Verzus item in vsEnum)
            {
                list.Add(new { VerzusId = item.VerzusId, VerzusUrlKey = item.VerzusUrlKey.Trim(), VerzusItem1 = item.VerzusItem1, VerzusItem2 = item.VerzusItem2, VerzusItemContext = item.VerzusItemContext, VerzusDateAdded = item.VerzusDateAdded });
            }

            return Json(list);
        }

        //
        // GET: /Verzus/Search

        public ActionResult Search(string itemContent)
        {
            var vsEnum =    from vs in c.Verzus
                            join vsi1 in c.VerzusItems on vs.VerzusItem1 equals vsi1.ItemId
                            join vsi2 in c.VerzusItems on vs.VerzusItem2 equals vsi2.ItemId
                            join vsiC in c.VerzusItems on vs.VerzusItemContext equals vsiC.ItemId
                            where SqlMethods.Like(vsi1.ItemContent, itemContent)
                            || SqlMethods.Like(vsi2.ItemContent, itemContent)
                            || SqlMethods.Like(vsiC.ItemContent, itemContent)
                            select new Verzus.Models.VerzusStructure { vs = vs, vsi1 = vsi1, vsi2 = vsi2, vsiC = vsiC };

            ViewBag.ItemContent = itemContent;
            ViewBag.VerzusList = vsEnum;

            return View();
        }

        //
        // GET: /Verzus/Create

        public ActionResult Create()
        {

            return View();
        }

        //
        // POST: /Verzus/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {

                Models.VerzusItem vsItem1 = new Models.VerzusItem();
                Models.VerzusItem vsItem2 = new Models.VerzusItem();
                Models.VerzusItem vsItemC = new Models.VerzusItem();

                vsItem1.ItemId = Convert.ToInt32(collection["verzusitem1"]);
                vsItem2.ItemId = Convert.ToInt32(collection["verzusitem2"]);
                vsItemC.ItemId = Convert.ToInt32(collection["verzusitemcontext"]);

                vsItem1.ItemContent = collection["verzusitem1text"];
                vsItem2.ItemContent = collection["verzusitem2text"];
                vsItemC.ItemContent = collection["verzusitemcontexttext"];



                List<Models.VerzusItem> createdVerzusItems = new List<Models.VerzusItem>();
                createdVerzusItems.Add(vsItem1);
                createdVerzusItems.Add(vsItem2);
                createdVerzusItems.Add(vsItemC);

                Models.Verzus vs = new Models.Verzus();

                vs.VerzusItem1 = this.saveAndGetId(vsItem1, false);
                vs.VerzusItem2 = this.saveAndGetId(vsItem2, false);
                vs.VerzusItemContext = this.saveAndGetId(vsItemC, true);
                vs.VerzusUrlKey = vsItem1.ItemContent.Trim() + "-vs-" + vsItem2.ItemContent.Trim();
                vs.VerzusDateAdded = DateTime.Now;
                c.Verzus.InsertOnSubmit(vs);
                
                c.SubmitChanges();

                object param = new { urlKey = vs.VerzusUrlKey, context = vs.VerzusItemContext };

                return RedirectToAction("Details", param);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return View();
            }
        }

        private long saveAndGetId(Models.VerzusItem newItem, bool isContext)
        {
            IEnumerable<Models.VerzusItem> exist = from i in c.VerzusItems where SqlMethods.Like(i.ItemContent.Trim(), newItem.ItemContent) select i;
            if (exist.Any())
            {
                if (isContext)
                    exist.First().ItemIsContext = 1;
                else exist.First().ItemIsSubject = 1;

                c.SubmitChanges();
                
                return exist.First().ItemId;
            }
            
            newItem.ItemType = 1;

            newItem.ItemDateAdded = DateTime.Now;
            c.VerzusItems.InsertOnSubmit(newItem);
            c.SubmitChanges();

            return saveAndGetId(newItem, isContext);
        }

        //
        // GET: /Verzus/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Verzus/Edit/5

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
        // GET: /Verzus/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Verzus/Delete/5

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

        private List<object> pack(IQueryable vsItems)
        {
            List<object> list = new List<object>();
            foreach (Models.VerzusItem item in vsItems)
            {
                list.Add(new { ItemId = item.ItemId, ItemType = item.ItemType, ItemContent = item.ItemContent.Trim(), ItemDateAdded = item.ItemDateAdded });
            }
            return list;
        }
    }
}
