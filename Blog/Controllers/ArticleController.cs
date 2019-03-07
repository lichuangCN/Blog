using Blog.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class ArticleController : Controller
    {
        //创建一个数据库的实体模型实例
        BlogDB dbContext = new BlogDB();
        // GET: Article
        public ActionResult Index(int page = 1)
        {
            if (Session["username"] == null)
            {
                Response.Redirect("~/Admin/Login");
            }
            int pageSize = 8;
            if (Session["username"].ToString() == "admin")
            {
                var result = (from a in dbContext.article
                              select a).ToList<article>();
                return View(result.ToPagedList<article>(page, pageSize));
            }
            else
            {
                string user = Session["username"].ToString();
                var result = (from a in dbContext.article
                              where a.admin.username == user
                              select a).ToList<article>();
                return View(result.ToPagedList<article>(page, pageSize));
            }
        }

        /// <summary>
        /// 以更新时间为基准从数据库中获取
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ViewResult sortByUpdateTime(int page = 1)
        {
            int pageSize = 8;
            if (Session["username"].ToString() == "admin")
            {
                var result = (from a in dbContext.article
                              orderby a.time descending
                              select a).ToList<article>();
                return View("Index", result.ToPagedList<article>(page, pageSize));
            }
            else
            {
                string user = Session["username"].ToString();
                var result = (from a in dbContext.article
                              where a.admin.username == user
                              orderby a.time descending
                              select a).ToList<article>();
                return View("Index", result.ToPagedList<article>(page, pageSize));
            }
        }

        /// <summary>
        /// 添加新的文章
        /// </summary>
        /// <returns></returns>
        public ActionResult addArticle()
        {
            var result = (from c in dbContext.cate
                          select c).ToList<cate>();
            ViewBag.author = Session["username"].ToString();
            return View(result);
        }

        /// <summary>
        /// 添加新文章的数据库存储过程
        /// </summary>
        /// <param name="title"></param>
        /// <param name="catid"></param>
        /// <param name="author"></param>
        /// <param name="desc"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ProcessAdd(string title, int catid, string author, string desc, string content)
        {
            article myarticle = new article();
            myarticle.title = title;
            myarticle.cateid = catid;
            myarticle.content = content;
            myarticle.desc = desc;
            myarticle.time = DateTime.Now;
            var result = from a in dbContext.admin
                         where a.username == author
                         select a;
            foreach (var a in result)
            {
                myarticle.creator = a.id;
            }
            dbContext.article.Add(myarticle);
            if (dbContext.SaveChanges() != 0)
            {
                return Json(new { status = 1, data = "添加成功！" });
            }
            else
            {
                return Json(new { status = 0, data = "添加失败！" });
            }

        }

        /// <summary>
        /// 更新文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UpdateArticle(int id)
        {
            article myarticle = dbContext.article.Find(id);
            ViewBag.id = myarticle.id;
            ViewBag.atitle = myarticle.title;
            ViewBag.author = myarticle.admin.username;
            ViewBag.cateName = myarticle.cate.catname;
            ViewBag.content = myarticle.content;
            ViewBag.desc = myarticle.desc;
            var result = (from c in dbContext.cate
                          select c).ToList<cate>();
            return View(result);
        }

        /// <summary>
        /// 更新文章的数据库存储过程
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="catid"></param>
        /// <param name="author"></param>
        /// <param name="desc"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ProcessUpdate(int id, string title, int catid, string author, string desc, string content)
        {
            article myarticle = dbContext.article.Find(id);
            myarticle.title = title;
            myarticle.cateid = catid;
            myarticle.content = content;
            myarticle.desc = desc;
            myarticle.time = DateTime.Now;
            if (dbContext.SaveChanges() != 0)
            {
                return Json(new { status = 1, data = "修改成功！" });
            }
            else
            {
                return Json(new { status = 0, data = "修改失败！" });
            }
        }

        /// <summary>
        /// 删除一条文章的数据库存储过程
        /// </summary>
        /// <param name="id"></param>
        public void ProcessDelete(int id)
        {
            article myarticle = dbContext.article.Find(id);
            dbContext.article.Remove(myarticle);
            dbContext.SaveChanges();
        }

        /// <summary>
        /// 显示详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult Detail(int id)
        {
            article myarticle = dbContext.article.Find(id);
            ViewBag.atitle = myarticle.title;
            ViewBag.author = myarticle.admin.username;
            ViewBag.creatTime = myarticle.time;
            ViewBag.cateName = myarticle.cate.catname;
            ViewBag.content = myarticle.content;
            return View();
        }
    }
}