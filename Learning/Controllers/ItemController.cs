using Learning.Models;
using Learning.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Learning.Controllers
{
    public class ItemController : Controller
    {
        LearningDbEntities db = new LearningDbEntities();

        public ActionResult Index()
        {
            try
            {
                var ItemsList = db.Items.Select(x => new ItemModel
                {
                    id = x.id,
                    Name = x.Name
                }).ToList();

                return View(new ItemViewModel() { ItemsList = ItemsList });
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.StackTrace;
            }

            return View();
        }

        // GET: Item
        public ActionResult Create()
        {
            try
            {
                var ItemsList = db.Items.Select(x => new ItemModel
                {
                    id = x.id,
                    Name = x.Name
                }).ToList();

                return View(new ItemViewModel() { ItemsList = ItemsList });
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.StackTrace;
            }

            return View();
        }

        [HttpPost]
        public ActionResult Create(ItemModel item)
        {
            try
            {
                Item itm = new Item() { Name = item.Name };

                if (db.Items.Where(x => x.Name.Equals(item.Name)).Count() > 0)
                {
                    ViewBag.msg = "Item already exists.";
                }
                else
                {
                    var usr = db.Items.Add(itm);
                    db.SaveChanges();

                    ViewBag.msg = "Item added Successfully.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.StackTrace;
            }

            return View();
        }

        public ActionResult Read(int? id = 0)
        {
            try
            {
                var item = db.Items.Where(x => x.id == id).Select(x => new ItemModel
                {
                    id = x.id,
                    Name = x.Name
                }).FirstOrDefault();

                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.StackTrace;
            }
            return View();
        }

        public ActionResult Update(int? id = 0)
        {
            try
            {
                var item = db.Items.Where(x => x.id == id).Select(x => new ItemModel
                {
                    id = x.id,
                    Name = x.Name
                }).FirstOrDefault();

                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.StackTrace;
            }
            return View();
        }
        [HttpPost]
        public ActionResult Update(ItemModel item)
        {
            try
            {
                var itm = db.Items.Where(x => x.id == item.id).FirstOrDefault();
                itm.Name = item.Name;

                db.SaveChanges();

                ViewBag.msg = "Item Updated Successfully";
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
            }
            return View();
        }

        public ActionResult Delete(int? id = 0)
        {
            try
            {
                Item itm = db.Items.Where(x => x.id == id).FirstOrDefault();

                if (itm == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Items.Remove(itm);
                    db.SaveChanges();

                    ViewBag.msg = "Item Deleted Successfully.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.StackTrace;
            }
            return RedirectToAction("Index");
        }
    }
}