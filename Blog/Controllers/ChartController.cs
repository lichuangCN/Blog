using Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class ChartController : Controller
    {
        BlogDB dbContext = new BlogDB();

        // GET: Chart
        public ActionResult Index()
        {
            if (Session["username"] == null)
            {
                Response.Redirect("~/Admin/Login");
            }
          
            var catesInt = dbContext.article.OrderBy(a => a.cateid).Select(a => a.cateid).Distinct();
            var cateName = dbContext.cate.OrderBy(a => a.id).Select(a => a.catname).Distinct();

            var catesId = new List<int>();
            var catesName = new List<string>();
            var articlesNum = new List<string>();

            foreach (int cate in catesInt)
            {
                catesId.Add(cate);
            }

            foreach (string cate in cateName)
            {
                catesName.Add(cate);
            }

            int c = 0;
            var articles = dbContext.article as IQueryable<article>;
            foreach (var item in catesId)
            {
                c += articles.Where(m => m.cateid == item).Count();
                articlesNum.Add(articles.Where(m => m.cateid == item).Count().ToString());
            }
            catesName.Add("所有文章");                       
            articlesNum.Add(c.ToString());
            ViewBag.cates = catesName;
            ViewBag.articles = articlesNum;                       
            return View();
        }
    }
}