using Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace Blog.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin       

        BlogDB dbContext = new BlogDB();

        public ActionResult Index(int page = 1)
        {
            if (Session["username"] == null)
            {
                Response.Redirect("Admin/Login");
            }
            var result = (from u in dbContext.admin
                          where u.username != "admin"
                          select u).ToList<admin>();
            int pageSize = 8;
            ViewBag.user = Session["username"]; //前台权限判断
            return View(result.ToPagedList<admin>(page, pageSize));
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            if (Session["username"] != null)
            {
                Response.Redirect("Welcome");
            }
            return View();
        }

        /// <summary>
        /// 从数据库中查找用户名，密码是否正确
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public JsonResult CheckLogin(string username, string password)
        {
            var result = from u in dbContext.admin
                         where u.username == username && u.password == password
                         select u;
            if (result.Count() > 0)
            {
                Session["username"] = username;
                return Json(new { status = 1, data = "登录成功！" });
            }
            else
            {
                return Json(new { status = 0, data = "登录失败！" });
            }                   
        }

        /// <summary>
        /// 用户登入后台显示的欢迎信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Welcome()
        {            
            return View();
        }

        /// <summary>
        /// 登出当前用户，此方法没有视图
        /// 在_LayoutBack视图文件中有使用
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            Session["username"] = null;
            return View("Login");
        }

        /// <summary>
        /// 修改密码，_LayoutBack视图文件中有使用
        /// </summary>
        /// <returns></returns>
        public ActionResult ModifyPasswd()
        {
            ViewBag.user = Session["username"]; //传参给前端
            return View();
        }

        /// <summary>
        /// 从数据库中查找该用户，并修改其原来的密码
        /// ModifyPasswd视图文件中调用
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        /// <param name="pwdd"></param>
        /// <returns></returns>
        public JsonResult CheckModifyPasswd(string user, string pwd, string pwdd)
        {
            var result = from u in dbContext.admin
                         where u.username == user
                         select u;
            foreach (var u in result)
            {
                u.password = pwd;
            }
            if (dbContext.SaveChanges() != 0)
            {
                Session["username"] = null;
                return Json(new { status = 1, data = "修改成功" });
            }
            else return Json(new { status = 0, data = "修改失败" });
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <returns></returns>
        public ActionResult UserAdd()
        {
            return View();
        }

        /// <summary>
        /// 向数据库中添加新的用户数据，在Admin/Index界面中使用
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <param name="pwdd"></param>
        /// <returns></returns>
        public JsonResult ProcessAdd(string userName, string pwd, string pwdd)
        {
            admin myuser = new admin();
            var result = from u in dbContext.admin
                         where u.username == userName
                         select u;
            if (result.Count() > 0) return Json(new { status = 0, data = "该用户已存在,请重新输入！" });
            else
            {
                myuser.username = userName;
                myuser.password = pwd;
                dbContext.admin.Add(myuser);
                if (dbContext.SaveChanges() != 0)
                    return Json(new { status = 1, data = "添加成功" });
                else return Json(new { status = 0, data = "添加失败" });
            }
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UpdateUser(int id)
        {
            admin myuser = dbContext.admin.Find(id);
            ViewBag.id = id;
            ViewBag.username = myuser.username;
            return View();
        }

        /// <summary>
        /// 更新用户信息，对数据库中信息进行更新
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <param name="pwdd"></param>
        /// <returns></returns>
        public JsonResult ProcessUpdate(int userId, string userName, string pwd, string pwdd)
        {
            admin myuser = dbContext.admin.Find(userId);
            myuser.password = pwd;
            if (dbContext.SaveChanges() != 0)
                return Json(new { status = 1, data = "修改成功" });
            else return Json(new { status = 0, data = "修改失败" });            
        }

        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="id"></param>
        public void ProcessDelete(int id)
        {
            admin myuser = dbContext.admin.Find(id);
            var result = from a in dbContext.article
                         where a.creator == id
                         select a;
            if (result.Count() > 0)  //如果要删除的user发表过文章，那么就把这些文章的creator=1，也就是“admin”
            {
                foreach (var a in result)
                    a.creator = 1;
            }
            dbContext.admin.Remove(myuser);
            dbContext.SaveChanges();
        }       
    }
}