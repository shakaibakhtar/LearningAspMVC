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
            return View();
        }

        public ActionResult GetAllItems()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var brandname = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            List<ItemModel> listsub = db.Items.Select(x => new ItemModel()
            {
                id = x.id,
                Name = x.Name
            }).ToList();

            int recordsTotal = listsub.Count;
            listsub = listsub.Skip(skip).Take(pageSize).ToList();
            var data = listsub.ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
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
                    return Json(new { Status = false, Data = "Item already exists." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var usr = db.Items.Add(itm);
                    db.SaveChanges();

                    return Json(new { Status = true, Data = "Item added Successfully." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = true, Data = ex.StackTrace }, JsonRequestBehavior.AllowGet);
            }
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

                if (itm != null)
                {
                    if (db.Items.Where(x => x.Name.Equals(item.Name)).Count() > 0)
                    {
                        return Json(new { Status = false, Data = "Item already exists." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        itm.Name = item.Name;

                        db.SaveChanges();

                        return Json(new { Status = true, Data = "Item Updated Successfully" });
                    }
                }
                return Json(new { Status = false, Data = "Item Not Found" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Data = ex.StackTrace });
            }
        }

        [HttpPost]
        public ActionResult Delete(int? id = 0)
        {
            try
            {
                Item itm = db.Items.Where(x => x.id == id).FirstOrDefault();

                if (itm == null)
                {
                    return Json(new { Status = true, Data = "Item Not Found." });
                }
                else
                {
                    db.Items.Remove(itm);
                    db.SaveChanges();

                    return Json(new { Status = true, Data = "Item Deleted Successfully." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Data = ex.StackTrace });
            }
        }
    }
}