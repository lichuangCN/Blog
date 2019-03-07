using Blog.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class LinkController : Controller
    {
        // GET: Link
        BlogDBEntities dbContext = new BlogDBEntities();
        // GET: Link
        public ActionResult Index(int page = 1)
        {
            if (Session["username"] == null)
            {
                Response.Redirect("~/Admin/Login");
            }
            var result = (from l in dbContext.link
                          select l).ToList<link>();
            int pageSize = 8;

            return View(result.ToPagedList<link>(page, pageSize));
        }

        /// <summary>
        /// 添加新链接
        /// </summary>
        /// <returns></returns>
        public ActionResult AddLink()
        {
            return View();
        }

        /// <summary>
        /// 添加新链接的存过程
        /// </summary>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public JsonResult ProcessAdd(string title, string url, string desc)
        {
            link mylink = new link();
            var result = from l in dbContext.link
                         where l.title == title
                         select l;
            if (result.Count() > 0) return Json(new { status = 0, data = "该友情链接已存在,请重新输入！" });
            else
            {
                mylink.title = title;
                mylink.url = url;
                mylink.desc = desc;
                dbContext.link.Add(mylink);
                if (dbContext.SaveChanges() != 0)
                    return Json(new { status = 1, data = "添加成功" });
                else return Json(new { status = 0, data = "添加失败" });
            }
        }

        /// <summary>
        /// 更新链接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UpdateLink(int id)
        {
            link mylink = dbContext.link.Find(id);
            ViewBag.id = mylink.id;
            ViewBag.title = mylink.title;
            ViewBag.url = mylink.url;
            ViewBag.desc = mylink.desc;
            return View();
        }

        /// <summary>
        /// 更新链接的数据库存储过程
        /// </summary>
        /// <param name="linkId"></param>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public JsonResult ProcessUpdate(int linkId, string title, string url, string desc)
        {
            var result = from l in dbContext.link
                         where l.title == title
                         select l;
            if (result.Count() > 0) return Json(new { status = 0, data = "该友情链接已存在,请重新输入！" });
            else
            {
                link mylink = dbContext.link.Find(linkId);
                mylink.title = title;
                mylink.url = url;
                mylink.desc = desc;
                if (dbContext.SaveChanges() != 0)
                    return Json(new { status = 1, data = "修改成功" });
                else return Json(new { status = 0, data = "修改失败" });
            }
        }

        /// <summary>
        /// 删除链接的存储过程
        /// </summary>
        /// <param name="id"></param>
        public void ProcessDelete(int id)
        {
            link mylink = dbContext.link.Find(id);
            dbContext.link.Remove(mylink);
            dbContext.SaveChanges();
        }

        /// <summary>
        /// 批量删除链接的存储过程
        /// </summary>
        /// <param name="idStr"></param>
        public void ManyDelete(string idStr)
        {
            string str = idStr.Substring(0, idStr.LastIndexOf(","));
            string[] idTemp = str.Split(',');
            link mylink;
            foreach (string s in idTemp)
            {
                mylink = dbContext.link.Find(int.Parse(s));
                dbContext.link.Remove(mylink);
            }
            dbContext.SaveChanges();
        }
    }
}