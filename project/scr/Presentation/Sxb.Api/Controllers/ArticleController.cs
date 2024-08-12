using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sxb.Api.Model;

namespace Sxb.Api.Controllers
{
    using PMS.OperationPlateform.Application.IServices;
    using PMS.OperationPlateform.Domain.Entitys;
    using PMS.OperationPlateform.Domain.Enums;
    using ProductManagement.Framework.Foundation;

    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private IArticleService articleService;
        private IArticleCoverService articleCoverService;
        private IArticleCommentService articleCommentService;

        public ArticleController(
            IArticleService articleService
            ,
            IArticleCoverService articleCoverService
            ,
            IArticleCommentService articleCommentService
            )
        {
            this.articleCoverService = articleCoverService;
            this.articleService = articleService;
            this.articleCommentService = articleCommentService;
        }

        [HttpPost]
        public ActionResult<ResponseModel<List<Article_V2>>> GetArticlesByIds(
       [FromBody] Guid[] ids
       )
        {
            var result = new ResponseModel<List<Article_V2>>();
            try
            {
                //查询出目标文章
                var articles = this.articleService.GetByIds(ids).ToList();
                if (articles != null && articles.Count > 0)
                {
                    //查询出目标背景图片
                    var effactiveIds = articles.Select(a => a.id).ToArray();
                    var covers = this.articleCoverService.GetCoversByIds(effactiveIds);
                    var comments = this.articleCommentService.Statistics_CommentsCount(ids);

                    List<article> _articles = new List<article>();
                    if (ids != null)
                    {
                        foreach (var id in ids)
                        {
                            var article = articles.Find(a => a.id == id);
                            if (article != null)
                            {
                                article.Covers = covers.Where(c => c.articleID == article.id).ToList();
                                article.CommentCount = comments.Where(c => c.Id == article.id).FirstOrDefault()?.Count ?? 0;
                                _articles.Add(article);
                            }
                        }
                    }
                    result.errCode = 1;
                    result.msg = "成功";
                    result.data = _articles.Select(a => new Article_V2()
                    {
                        Id = a.id,
                        Time = a.time.GetValueOrDefault(),
                        Title = a.title,
                        ViewCount = a.VirualViewCount,
                        Covers = a.Covers.Select(c => !string.IsNullOrWhiteSpace(c.ImgUrl)? c.ImgUrl: $"https://cos.sxkid.com/images/article/{c.articleID}/{c.photoID}.{((FileExtension)c.ext).ToString()}").ToList(),
                        CommentCount = a.CommentCount,
                        Layout = a.layout,
                        Digest = a.overview
                    }).ToList();
                }
                else
                {
                    result.errCode = -1;
                    result.msg = "找不到对应的文章";
                }
            }
            catch (Exception ex)
            {
                result.errCode = -1;
                result.msg = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 获取当前规则生成的文章短链接
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<ResponseModel<string>> GetShortLink(int no)
        {
            var response = new ResponseModel<string>();
            string No = UrlShortIdUtil.Long2Base32(no);
            response.errCode = 0;
            response.msg = "success";
            response.data = $"/article/{No}.html";
            return response;
        }
    }
}