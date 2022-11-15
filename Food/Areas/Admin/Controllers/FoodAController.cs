using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Food.Models;
using System.IO;
using System.Data.Entity;

namespace AdminFood.Areas.Admin.Controllers
{
    public class FoodAController : Controller
    {
        FoodStoreEntities db = new FoodStoreEntities();
        public ActionResult Index(int? size, int? page, string sortProperty, string searchString, string sortOrder = "", int categoryID = 0)
        {
            if (Session["Account"] == null)
            {
                return RedirectToAction("Login", "UserA");
            }
            ViewBag.Keyword = searchString;
            ViewBag.Subject = categoryID;
            var books = db.Products.Include(b => b.Category);
            if (!String.IsNullOrEmpty(searchString))
                books = books.Where(b => b.Name.Contains(searchString));
            if (!String.IsNullOrEmpty(searchString))
                books = books.Where(b => b.Name.Contains(searchString));
            if (categoryID != 0)
                books = books.Where(c => c.CategoryID == categoryID);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name");
            
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "5", Value = "5" });
            items.Add(new SelectListItem { Text = "10", Value = "10" });
            items.Add(new SelectListItem { Text = "20", Value = "20" });
            items.Add(new SelectListItem { Text = "25", Value = "25" });
            items.Add(new SelectListItem { Text = "50", Value = "50" });
            items.Add(new SelectListItem { Text = "100", Value = "100" });
            items.Add(new SelectListItem { Text = "200", Value = "200" });
            foreach (var item in items)
            {
                if (item.Value == size.ToString()) item.Selected = true;
            }
            ViewBag.Size = items;
            ViewBag.CurrentSize = size;
            page = page ?? 1; 
            int pageSize = (size ?? 5);
            ViewBag.pageSize = pageSize;
            int pageNumber = (page ?? 1);
            int checkTotal = (int)(books.ToList().Count / pageSize) + 1;
            if (pageNumber > checkTotal) pageNumber = checkTotal;
            return View(books.OrderBy(n=>n.ProductID).ToPagedList(pageNumber, pageSize));
        }
       [HttpGet]
        public ActionResult Create()
        {
            if (Session["Account"] == null)
            {
                return RedirectToAction("Login", "UserA");
            }
            ViewBag.CategoryID = new SelectList(db.Categories.ToList().OrderBy(n => n.Name),"CategoryID","Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Product product,FormCollection f,HttpPostedFileBase fFileUpload)
        {
            ViewBag.CategoryID = new SelectList(db.Categories.ToList().OrderBy(n => n.Name), "CategoryID", "Name");
            if (fFileUpload == null)
            {
                ViewBag.ThongBao = "Hãy Chọn Ảnh";
                ViewBag.Name = f["sTenSP"];
                ViewBag.Decription = f["sMota"];
                ViewBag.Price = (f["mGiaBan"]);
                ViewBag.Weight = f["sTrongLuong"];
                ViewBag.CategoryID = new SelectList(db.Categories.ToList().OrderBy(n => n.Name), "CategoryID", "Name",int.Parse(f["CategoryID"]));
                return View();
            }
            else
            {
                if(ModelState.IsValid)
                {
                    var sFileName = Path.GetFileName(fFileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images"), sFileName);
                    if(!System.IO.File.Exists(path))
                    {
                        fFileUpload.SaveAs(path);
                        
                    }
                    product.Image = sFileName;
                    product.Name = f["sTenSP"];
                    product.Decription = f["sMota"].Replace("<p>", "").Replace("</p>", "\n");
                    product.Price = int.Parse(f["mGiaBan"]);
                    product.Weight = f["sTrongLuong"];
                    product.CategoryID = int.Parse(f["CategoryID"]);
                    db.Products.Add(product);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View();
            }
        }
        public ActionResult Details(int id)
        {
            if (Session["Account"] == null)
            {
                return RedirectToAction("Login", "UserA");
            }
            var product = db.Products.SingleOrDefault(n => n.ProductID == id);
            if(product==null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(product);
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Session["Account"] == null)
            {
                return RedirectToAction("Login", "UserA");
            }
            var product = db.Products.SingleOrDefault(n => n.ProductID == id);
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(product);
        }
        [HttpPost,ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            var product = db.Products.SingleOrDefault(n => n.ProductID == id);
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            var orderDetail = db.OrderDetails.Where(n => n.ProductID == id);
            if (orderDetail.Count() >0)
            {
                ViewBag.ThongBao = "Sản Phẩm này đang có trong chi tiết đặt hàng<br>" + "Nếu muốn xóa thì phải xóa hết mã này trong chi tiết đặt hàng";
                return View(product);
            }
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["Account"] == null)
            {
                return RedirectToAction("Login", "UserA");
            }
            var product = db.Products.SingleOrDefault(n => n.ProductID == id);
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.CategoryID = new SelectList(db.Categories.ToList().OrderBy(n => n.Name), "CategoryID", "Name",product.CategoryID);
            return View(product);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection f, HttpPostedFileBase fFileUpload,int id)
        {
            var product = db.Products.SingleOrDefault(n => n.ProductID == id);
            ViewBag.CategoryID = new SelectList(db.Categories.ToList().OrderBy(n => n.Name), "CategoryID", "Name");
            if (ModelState.IsValid)
            {
                if (fFileUpload!= null && fFileUpload.ContentLength > 0)
                {
                    
                    var sFileName = Path.GetFileName(fFileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images"), sFileName);
                    if (!System.IO.File.Exists(path))
                    {
                        fFileUpload.SaveAs(path);
                    }
                    product.Image = sFileName;
                    
                }
                product.Name = f["sTenSP"];
                product.Decription = f["sMota"].Replace("<p>", "").Replace("</p>", "\n");
                product.Price = int.Parse(f["mGiaBan"]);
                product.Weight = f["sTrongLuong"];
                product.CategoryID = int.Parse(f["CategoryID"]);
                db.SaveChanges();
                return RedirectToAction("Index");
                
            }
            return View(product);
        }
    }
}