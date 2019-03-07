using Blog.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class CateController : Controller
    {
     
        BlogDB dbContext = new BlogDB();
        // GET: Cate
        public ActionResult Index(int page = 1)
        {
            if (Session["username"] == null)
            {
                Response.Redirect("~/Admin/Login");
            }
            var result = (from c in dbContext.cate
                          where c.catname != "未分类"
                          select c).ToList<cate>();
            int pageSize = 8;

            return View(result.ToPagedList<cate>(page, pageSize));
        }

        /// <summary>
        /// 添加类别
        /// </summary>
        /// <returns></returns>
        public ActionResult AddCate()
        {
            return View();
        }

        /// <summary>
        /// 添加类别的数据库存储过程
        /// </summary>
        /// <param name="cateName"></param>
        /// <returns></returns>
        public JsonResult ProcessAdd(string cateName)
        {
            cate mycate = new cate();
            var result = from c in dbContext.cate
                         where c.catname == cateName
                         select c;
            if (result.Count() > 0) return Json(new { status = 0, data = "该分类已存在,请重新输入！" });
            else
            {
                mycate.catname = cateName;
                dbContext.cate.Add(mycate);
                if (dbContext.SaveChanges() != 0)
                    return Json(new { status = 1, data = "添加成功" });
                else return Json(new { status = 0, data = "添加失败" });
            }
        }

        /// <summary>
        /// 更新类别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UpdateCate(int id)
        {
            cate mycate = dbContext.cate.Find(id);
            ViewBag.id = mycate.id;
            ViewBag.catename = mycate.catname;
            return View();
        }

        /// <summary>
        ///更新类别的数据库存储过程 
        /// </summary>
        /// <param name="cateId"></param>
        /// <param name="cateName"></param>
        /// <returns></returns>
        public JsonResult ProcessUpdate(int cateId, string cateName)
        {
            var result = from c in dbContext.cate
                         where c.catname == cateName
                         select c;
            if (result.Count() > 0) return Json(new { status = 0, data = "该分类已存在,请重新输入！" });
            else
            {
                cate mycate = dbContext.cate.Find(cateId);
                mycate.catname = cateName;
                if (dbContext.SaveChanges() != 0)
                    return Json(new { status = 1, data = "修改成功" });
                else return Json(new { status = 0, data = "修改失败" });
            }
        }

        /// <summary>
        /// 删除类别的方法，没有视图
        /// </summary>
        /// <param name="id"></param>
        public void ProcessDelete(int id)
        {
            cate mycate = dbContext.cate.Find(id);
            var result = from a in dbContext.article
                         where a.cateid == id
                         select a;
            if (result.Count() > 0)     //如果要删除的分类下有文章，那么就把这些文章的cateid=1，也就是“未分类”
            {
                foreach (var a in result)
                    a.cateid = 1;
            }
            dbContext.cate.Remove(mycate);
            dbContext.SaveChanges();
        }
    }
}