﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanHang.Models;
using WebsiteBanHang.Models.DataAccess_Object;

namespace WebsiteBanHang.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(string url)
        {
            ViewBag.Url = url;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(TAIKHOAN model)
        {
            if (ModelState.IsValid)
            {
                TAIKHOAN tk = TAIKHOAN_DAO.Read(model.Username);
                if (tk == null)
                {
                    //Kiểm tra tài khoản có nằm bên bảng Khách hàng hay không?
                    //(Khách hàng mới đăng ký tài khoản)
                    KHACHHANG TaiKhoan_Moi = KHACHHANG_DAO.Read(model.Username);
                    if (TaiKhoan_Moi == null)
                    {
                        ModelState.AddModelError("Username", "Không tìm thấy tài khoản");
                        return View(model);
                    }

                    if (TaiKhoan_Moi.MatKhau != model.Password)
                    {
                        ModelState.AddModelError("Password", "Mật khẩu không đúng");
                        return View(model);
                    }
                    //Tạo mới trên dữ liệu tài khoản
                    tk = new TAIKHOAN()
                    {
                        Username = TaiKhoan_Moi.TaiKhoan,
                        Password = TaiKhoan_Moi.MatKhau,
                        Ma_LoaiTK = "TK2"

                    };
                    if (TAIKHOAN_DAO.Create(tk) != 0)
                    {
                        ModelState.AddModelError("Username", "Không tạo được tài khoản");
                        return View(model);
                    }
                }
                else
                {
                    if (tk.Password != model.Password)
                    {
                        ModelState.AddModelError("Password", "Mật khẩu không đúng");
                        return View(model);
                    }
                }

                //Đăng nhập
                Response.Cookies["TaiKhoan"].Value = model.Username;
                tk = TAIKHOAN_DAO.Read(model.Username);
                if (tk != null)
                {
                    Response.Cookies["ChucNang"].Value = tk.List_ChucNang;
                    if (model.Ghinho_Dangnhap == true)
                    {
                        Response.Cookies["TaiKhoan"].Expires = DateTime.Now.AddDays(7);
                        Response.Cookies["ChucNang"].Expires = DateTime.Now.AddDays(7);
                    }
                    else
                    {
                        Response.Cookies["TaiKhoan"].Expires = DateTime.Now.AddHours(4);
                        Response.Cookies["ChucNang"].Expires = DateTime.Now.AddHours(4);
                    }
                }

                if (String.IsNullOrEmpty(model.url))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    if (tk.Ma_LoaiTK == "TK1")
                    {
                        return RedirectToAction("Index", "QuanLySach");
                    }
                    else
                    {
                        return Redirect(model.url);
                    }
                }
            }
            return View(model);
        }
    }
}