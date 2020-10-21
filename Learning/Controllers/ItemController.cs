using Learning.Models;
using Learning.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Learning.Controllers
{
    public class ItemController : Controller
    {
        LearningDbEntities db = new LearningDbEntities();
        SqlConnection con;
        SqlCommand cmd;
        IQueryable<clsList> AllItemsList;

        string connString = ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;

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

            ///////////////////////////////////////////////////////////////

            List<ItemModel> listsub = new List<ItemModel>();

            DataTable dt = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(connString);
                con.Open();

                SqlCommand cmd = new SqlCommand("spGetItemsList", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ItemModel tmpItem = new ItemModel() {
                        id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Country = reader.GetString(2)
                    };

                    listsub.Add(tmpItem);
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
            }


            ///////////////////////////////////////////////////////////////

            //List<ItemModel> listsub = db.Items.Select(x => new ItemModel()
            //{
            //    id = x.id,
            //    Name = x.Name,
            //    CountryId = x.CountryId
            //}).ToList();

            int recordsTotal = listsub.Count;
            listsub = listsub.Skip(skip).Take(pageSize).ToList();
            var data = listsub.ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

        // GET: Item
        public ActionResult Create()
        {
            ViewBag.Countries = new SelectList(db.tblCountries.OrderBy(x => x.name), "id", "name");

            //con = new SqlConnection(connString);

            //try
            //{
            //    con.Open();
            //    using (SqlCommand cmd = new SqlCommand("spInsertUpdateUnit", con))
            //    {
            //        cmd.CommandType = CommandType.StoredProcedure;
            //        cmd.Parameters.Clear();
            //        cmd.Parameters.Add("@CheckReturn", SqlDbType.NVarChar, 300).Direction = ParameterDirection.Output;
            //        cmd.Dispose();
            //    }
            //    con.Close();
            //    con.Dispose();
            //}
            //catch (Exception ex)
            //{
            //    returnId = ex.Message.ToString();
            //    int? userId = User.UserId;
            //    clsSqlErrorLog.InsertError(userId, ex.Message.ToString(), "Unit");
            //}


            //var ItemsList = db.Items.Select(x => new ItemModel
            //{
            //    id = x.id,
            //    Name = x.Name
            //}).ToList();

            return View();
        }


        [HttpPost]
        public ActionResult Create(ItemModel item)
        {
            try
            {
                con = new SqlConnection(connString);
                cmd = new SqlCommand("spInsertUpdateItem", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@ItemName", SqlDbType.NVarChar).Value = item.Name;
                cmd.Parameters.Add("@InsertUpdateStatus", SqlDbType.NVarChar).Value = "Save";
                cmd.Parameters.Add("@CountryId", SqlDbType.Int).Value = item.CountryId;
                cmd.Parameters.Add("@CheckReturn", SqlDbType.NVarChar, 300).Direction = ParameterDirection.Output;


                con.Open();
                cmd.ExecuteNonQuery();

                string msg = cmd.Parameters["@CheckReturn"].Value.ToString();
                cmd.Dispose();
                con.Close();

                if (msg.Equals("Success"))
                {
                    return Json(new { Status = true, Data = "Item added Successfully." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Status = false, Data = "Item adding failed." }, JsonRequestBehavior.AllowGet);
                }
                //Item itm = new Item() { Name = item.Name };

                //if (db.Items.Where(x => x.Name.Equals(item.Name)).Count() > 0)
                //{
                //    return Json(new { Status = false, Data = "Item already exists." }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    var usr = db.Items.Add(itm);
                //    db.SaveChanges();

                //    return Json(new { Status = true, Data = "Item added Successfully." }, JsonRequestBehavior.AllowGet);
                //}
            }
            catch (Exception ex)
            {
                cmd.Dispose();
                con.Close();
                return Json(new { Status = false, Data = ex.StackTrace }, JsonRequestBehavior.AllowGet);
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
                ViewBag.Countries = new SelectList(db.tblCountries.OrderBy(x => x.name), "id", "name");

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
            string msg;
            try
            {
                con = new SqlConnection(connString);
                con.Open();

                cmd = new SqlCommand("spInsertUpdateItem", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                cmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = item.id;
                cmd.Parameters.Add("@ItemName", SqlDbType.NVarChar).Value = item.Name;
                cmd.Parameters.Add("@InsertUpdateStatus", SqlDbType.NVarChar).Value = "Update";
                cmd.Parameters.Add("@CountryId", SqlDbType.Int).Value = item.CountryId;
                cmd.Parameters.Add("@CheckReturn", SqlDbType.NVarChar, 300).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                msg = cmd.Parameters["@CheckReturn"].Value.ToString();
                cmd.Dispose();
                con.Close();
                con.Dispose();

                //var itm = db.Items.Where(x => x.id == item.id).FirstOrDefault();

                //if (itm != null)
                //{
                //    if (db.Items.Where(x => x.Name.Equals(item.Name)).Count() > 0)
                //    {
                //        return Json(new { Status = false, Data = "Item already exists." }, JsonRequestBehavior.AllowGet);
                //    }
                //    else
                //    {
                //        itm.Name = item.Name;

                //        db.SaveChanges();

                //        return Json(new { Status = true, Data = "Item Updated Successfully" });
                //    }
                //}
                //return Json(new { Status = false, Data = "Item Not Found" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Data = ex.StackTrace });
            }

            if (msg.Equals("Success"))
            {
                return Json(new { Status = true, Data = "Item updated Successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false, Data = "Item updation failed." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Delete(int? id = 0)
        {
            try
            {
                con = new SqlConnection(connString);
                cmd = new SqlCommand("spInsertUpdateItem", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@ItemName", SqlDbType.NVarChar).Value = "";
                cmd.Parameters.Add("@InsertUpdateStatus", SqlDbType.NVarChar).Value = "";
                cmd.Parameters.Add("@CountryId", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@CheckReturn", SqlDbType.NVarChar, 300).Direction = ParameterDirection.Output;


                con.Open();
                cmd.ExecuteNonQuery();

                string msg = cmd.Parameters["@CheckReturn"].Value.ToString();
                cmd.Dispose();
                con.Close();

                if (msg.Equals("Success"))
                {
                    return Json(new { Status = true, Data = "Item deleted Successfully." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Status = false, Data = "Item deletion failed." }, JsonRequestBehavior.AllowGet);
                }
                //Item itm = db.Items.Where(x => x.id == id).FirstOrDefault();

                //if (itm == null)
                //{
                //    return Json(new { Status = true, Data = "Item Not Found." });
                //}
                //else
                //{
                //    db.Items.Remove(itm);
                //    db.SaveChanges();

                //    return Json(new { Status = true, Data = "Item Deleted Successfully." });
                //}
            }
            catch (Exception ex)
            {
                cmd.Dispose();
                con.Close();
                return Json(new { Status = false, Data = ex.StackTrace });
            }
        }

        public JsonResult GetCountriesList(string searchTerm, int pageSize, int pageNumber)
        {
            AllItemsList = AllUserTypesListDetail();
            var select2pagedResult = new Select2PagedResult();
            var totalResults = 0;
            select2pagedResult.Results = GetPagedListOptions(searchTerm, pageSize, pageNumber, out totalResults);
            select2pagedResult.Total = totalResults;

            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        List<clsList> GetPagedListOptions(string searchTerm, int pageSize, int pageNumber, out int totalSearchRecords)
        {
            var allSearchedResults = GetAllSearchResults(searchTerm);
            totalSearchRecords = allSearchedResults.Count;
            return allSearchedResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        List<clsList> GetAllSearchResults(string searchTerm)
        {
            //AllItemsList = AllItemsListDetail();
            var resultList = new List<clsList>();

            if (!string.IsNullOrEmpty(searchTerm))
                resultList = AllItemsList.Where(n => n.text.ToLower().Contains(searchTerm.ToLower())).ToList();
            else
                resultList = AllItemsList.ToList();
            return resultList;
        }

        public IQueryable<clsList> AllUserTypesListDetail()
        {
            //string cacheKey = "Select2Options";
            ////check cache 
            //if (System.Web.HttpContext.Current.Cache[cacheKey] != null)
            //{
            //    return (IQueryable<clsList>)System.Web.HttpContext.Current.Cache[cacheKey];
            //}

            List<clsList> item = new List<clsList>();
            item = (from c in db.tblCountries

                    orderby c.id
                    select new clsList
                    {
                        id = c.id,
                        text = c.name
                    }).ToList();

            var result = item.AsQueryable();

            //cache results
            //System.Web.HttpContext.Current.Cache[cacheKey] = result;

            return result;
        }
    }
}