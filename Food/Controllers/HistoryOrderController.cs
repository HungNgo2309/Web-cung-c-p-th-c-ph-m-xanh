using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Food.Models;

namespace Food.Controllers
{
    public class HistoryOrderController : Controller
    {
        // GET: HistoryOrder
        FoodStoreEntities db = new FoodStoreEntities();
        public ActionResult ManageOrder()
        {
            Account ac = (Account)Session["TaiKhoan"];
            var mn = from s in db.OrderDetails
                     where s.Order.AccountID== ac.AccountID
                     select s;
            return View(mn);
        }
        public ActionResult DetailOrder(int id)
        {
            var mn = from s in db.OrderDetails
                     where s.OrderDetailID ==id
                     select s;
            return View(mn.Single());
        }
        public ActionResult Edit(int id,FormCollection f)
        {
            OrderDetail od = db.OrderDetails.SingleOrDefault(n => n.OrderDetailID == id);
            if(od!=null)
            {
                od.CustomerName = f["name"];
                od.Address = f["address"];
                od.PhoneNumber = f["sdt"];
                od.Email = f["email"];
                db.SaveChanges();
            }
            return RedirectToAction("ManageOrder", "HistoryOrder");
        }
    }
}