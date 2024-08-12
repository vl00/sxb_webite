using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.CommentsManage.Application.IServices;
using Sxb.Inside.Response;
using PMS.CommentsManage.Domain.Common;
using Sxb.Inside.ViewModels.Comment;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.Inside.Controllers
{
    public class CommentController : Controller
    {
        private readonly ISchoolCommentService _commentService;

        public CommentController(ISchoolCommentService commentService) 
        {
            _commentService = commentService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ResponseResult HottestComment(HotComentQueryViewModel query) 
        {
            try
            {
                var result = _commentService.HottestSchool(new HotCommentQuery()
                {
                    Condition = query.Condition,
                    City = query.City,
                    Grade = query.Grade,
                    Type = query.Type,
                    Discount = query.Discount,
                    Diglossia = query.Diglossia,
                    Chinese = query.Chinese,
                    StartTime = query.StartTime,
                    EndTime = query.EndTime
                }, false);
                return ResponseResult.Success(result);
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
    }
}
