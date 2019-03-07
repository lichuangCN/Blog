using Blog.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        BlogDB dbContext = new BlogDB();

        public ActionResult Index(int page = 1)
        {
            int pageSize = 6;
            var result = (from a in dbContext.article
                          orderby a.time descending
                          select a).ToList<article>();
            return View(result.ToPagedList<article>(page, pageSize));
        }

        /// <summary>
        /// 文章的类别
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ArticlesOfCate(int id, int page = 1)
        {
            var articleResult = (from a in dbContext.article
                                 where a.cateid == id
                                 select a).ToList<article>();
            int pageSize = 6;
            return View("Index", articleResult.ToPagedList<article>(page, pageSize));
        }

        /// <summary>
        /// 显示对应的文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult articleDetail(int id)  //显示一篇文章的详细内容
        {
            return View();
        }

        /// <summary>
        /// 显示文章的类别
        /// </summary>
        /// <returns></returns>
        public PartialViewResult showCates()
        {
            var result = (from c in dbContext.cate where c.catname != "未分类" select c).ToList<cate>();
            return PartialView(result);
        }

        /// <summary>
        /// 显示最新的8篇文章
        /// </summary>
        /// <returns></returns>
        public PartialViewResult showLastest()
        {
            var result = (from a in dbContext.article
                          orderby a.time descending
                          select a).Take<article>(8).ToList<article>();  //取最新的8篇文章
            return PartialView(result);
        }
    }
}