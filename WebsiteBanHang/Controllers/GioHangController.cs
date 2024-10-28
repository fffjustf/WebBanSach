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
        //Tính tổng thành tiền
        private int TongTien()
        {
            int tongTien = 0;
            List<GIOHANG> listGioHang = Session["GioHang"] as List<GIOHANG>;
            if (listGioHang != null)
            {
                tongTien = listGioHang.Sum(n => n.thanhTien);
            }
            return tongTien;
        }
        //Xây dựng chức năng đặt hàng
        [HttpPost]
        public ActionResult DatHang()
        {
            //Kiểm tra giỏ hàng
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Thêm đơn hàng
            DON_DATHANG dh = new DON_DATHANG();
            List<GIOHANG> gh = LayGioHang();
            string user = Request.Cookies["TaiKhoan"].Value;
            KHACHHANG kh = db.KHACHHANG.FirstOrDefault(n => n.TaiKhoan == user);
            dh.Ma_KhachHang = kh.Ma_KhachHang;
            dh.Ngay_Dat = DateTime.Now;
            dh.Da_ThanhToan = false;
            if (db.DON_DATHANG.Count() == 0)
            {
                dh.Ma_DonHang = "MDH1";
            }
            else
            {
                dh.Ma_DonHang = "MDH" + db.DON_DATHANG.Count() + 1;
            }
            db.DON_DATHANG.Add(dh);
            db.SaveChanges();
            //Thêm chi tiết đơn hàng
            foreach (var item in gh)
            {
                CHITIET_DONHANG cTDH = new CHITIET_DONHANG();
                cTDH.Ma_DonHang = dh.Ma_DonHang;
                cTDH.Ma_Sach = item.maSach;
                cTDH.SoLuong = item.soLuong;
                cTDH.DonGia = item.giaBan;
                db.CHITIET_DONHANG.Add(cTDH);
            }
            db.SaveChanges();
            string content = System.IO.File.ReadAllText(Server.MapPath("~/template/neworder.html"));
            content = content.Replace("{{HoTen}}", kh.HoTen);
            content = content.Replace("{{Email}}", kh.Email);
            content = content.Replace("{{DiaChi}}", kh.DiaChi);
            content = content.Replace("{{DienThoai}}", kh.DienThoai);
            content = content.Replace("{{TongTien}}", TongTien().ToString("N0"));
            var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
            new MailHelper().SendMail(kh.Email, "Đơn hàng mới từ WebBinBen", content);
            new MailHelper().SendMail(toEmail, "Đơn hàng mới từ WebBinBen", content);
            return View("DatHang");
        }
    }
}