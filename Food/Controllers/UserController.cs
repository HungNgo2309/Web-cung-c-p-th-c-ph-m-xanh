using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Food.Models;

namespace Food.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        FoodStoreEntities db = new FoodStoreEntities();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(FormCollection f, Account Ac, ShoppingCard card)
        {
            var Hoten = f["username"];
            var MatKhau = f["password"];
            var NLMatKhau = f["confirmpassword"];
            var Email = f["email"];
            var SDT = f["phone"];
            var Address = f["address"];
            if (String.IsNullOrEmpty(Hoten))
            {
                ViewData["err1"] = " Họ tên không được để trống";
            }
            else if (String.IsNullOrEmpty(MatKhau))
            {
                ViewData["err2"] = "Mật khẩu không được để trống";
            }
            else if (String.IsNullOrEmpty(NLMatKhau))
            {
                ViewData["err3"] = "Phải nhập lại mật khẩu";
            }
            else if (MatKhau != NLMatKhau)
            {
                ViewData["err3"] = "Mật khẩu nhập lại không trùng khớp";
            }
            else if (String.IsNullOrEmpty(Email))
            {
                ViewData["err4"] = "Email không được để trống";
            }
            else if (String.IsNullOrEmpty(SDT))
            {
                ViewData["err5"] = "Số điện thoại không được để trống";
            }
            else if (db.Accounts.SingleOrDefault(n => n.Email == Email) != null)
            {
                ViewBag.ThongBao = "Tài khoản này đã tồn tại";
            }
            else if (String.IsNullOrEmpty(Address))
            {
                ViewData["err6"] = "Địa chỉ không được để trống";
            }
            else if (db.Accounts.SingleOrDefault(n => n.PhoneNumber == SDT) != null)
            {
                ViewBag.ThongBao = "Số điện thoại này đã tồn tại. Vui lòng sử dụng số điện thoại khác";
            }
            else
            {
                Ac.Email = Email;
                Ac.UserName = Hoten;
                Ac.DisplayName = Hoten;
                Ac.Password = MatKhau;
                Ac.PhoneNumber = SDT;
                Ac.Status = 0;
                Ac.Address = Address;
                db.Accounts.Add(Ac);
                db.SaveChanges();
                Account ac = db.Accounts.SingleOrDefault(n => n.Email == Ac.Email);
                card.AccountID = ac.AccountID;
                db.ShoppingCards.Add(card);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            return this.Register();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection f)
        {
            int state = 0;
            if ((Request.QueryString["id"])!=null)
            {
                state = int.Parse(Request.QueryString["id"]);
            }
            var email = f["Email"];
            var Matkhau = f["Password"];
            if(String.IsNullOrEmpty(email))
            {
                ViewData["err1"] = "Email không được để trống";
            }
            else if(String.IsNullOrEmpty(Matkhau))
            {
                ViewData["err2"] = "Mật khẩu không được để trống";
            }
            else
            {
                Account ac = db.Accounts.SingleOrDefault(n => n.Email == email && n.Password == Matkhau);
               if(ac !=null)
                {
                    ViewBag.ThongBao = "Đăng nhập thành công";
                    Session["TaiKhoan"] = ac;
                    ShoppingCard sc = db.ShoppingCards.SingleOrDefault(n => n.AccountID == ac.AccountID);
                    Session["CartID"] = sc.CartID;
                    if (state == 2)
                    {
                        return RedirectToAction("GioHang", "Food");
                    }
                    return RedirectToAction("Index", "Food");
                }
                else
                {
                    ViewBag.ThongBao = "Email hoặc mật khẩu không đúng";
                }
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session["TaiKhoan"] = null;
            return RedirectToAction("Index", "Food");
        }
        [HttpGet]
        public ActionResult Info()
        {
            Account ac = (Account)Session["TaiKhoan"];
            return View(ac);
        }
        public ActionResult Info(int id, FormCollection f)
        {
            Account ac = db.Accounts.Single(n => n.AccountID == id);
            if (String.IsNullOrEmpty(f["name"]))
            {
                ViewData["err1"] = "Vui lòng điền đầy đủ Họ và Tên";
            }
            else if (String.IsNullOrEmpty(f["address"]))
            {
                ViewData["err2"] = "Địa chỉ không được để trống";
            }
            else if (String.IsNullOrEmpty(f["sdt"]))
            {
                ViewData["err3"] = "Vui lòng nhập số điện thoại";
            }
            else if (String.IsNullOrEmpty(f["email"]))
            {
                ViewData["err4"] = "Email không đươc để trống";
            }
            else
            {
                if (ac != null)
                {
                    ac.UserName = f["name"];
                    ac.Address = f["address"];
                    ac.PhoneNumber = f["sdt"];
                    ac.Email = f["email"];
                    db.SaveChanges();
                    ViewBag.ThongBao = "Cập nhật thông tin thành công";
                    return RedirectToAction("Index","Food");
                }
            }
            return this.Info();
        }
    }
}