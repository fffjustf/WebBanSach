using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanHang.Models;
using WebsiteBanHang.Models.DataAccess_Object;
using System.Configuration;
using Common;

namespace WebsiteBanHang.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        Data_Entities db = new Data_Entities();
        //Lấy giỏ hàng
        public List<GIOHANG> LayGioHang()
        {
            List<GIOHANG> listGioHang = Session["GioHang"] as List<GIOHANG>;
            if (listGioHang == null)
            {
                //Nếu giỏ hàng chưa tồn tại thì tiến hành khởi tạo list giỏ hàng (session giỏ hàng)
                listGioHang = new List<GIOHANG>();
                Session["GioHang"] = listGioHang;
            }
            return listGioHang;
        }

        //Xây dựng trang giỏ hàng
        public ActionResult GioHang()
        {
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<GIOHANG> listGioHang = LayGioHang();
            return View(listGioHang);
        }

    }
}