using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto.ModelVo;
using PMS.UserManage.Domain.Entities;
using Sxb.UserCenter.Models.ArticleViewModel;
using Sxb.UserCenter.Response;
using Sxb.UserCenter.Utils;
using Sxb.UserCenter.Utils.DtoToViewModel;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.UserCenter.Controllers
{
    public class ApiArticleController : Base
    {
        private IArticleService _articleService;
        private ITalentService _talentService;
        private readonly IUserService _userService;
        public ApiArticleController(IUserService userService, IArticleService articleService, ITalentService talentService)
        {
            _articleService = articleService;
            _userService = userService;
            _talentService = talentService;
        }

        /// <summary>
        /// 个人主页 文章列表
        /// </summary>
        /// <param name="search"></param>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ResponseResult HomeArticle(string search = "", Guid userId = default(Guid), int page = 1, int size = 10)
        {
            List<DataViewModel> datas = new List<DataViewModel>();
            Guid searchUserId = Guid.Empty, QueryUserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                searchUserId = user.UserId;
                QueryUserId = user.UserId;
            }

            if (userId != default)
            {
                //其他用户查看达人主页
                searchUserId = userId;
            }
            List<article> dataDtos = new List<article>();

            TalentEntity talent = _talentService.GetTalentByUserId(searchUserId.ToString());
            if (talent != null)
            {
                Guid talentId = talent.id;
                if (String.IsNullOrEmpty(search))
                {
                    dataDtos = _articleService.GetTalent(talentId, page, size).ToList();

                    if (!dataDtos.Any())
                    {
                        return ResponseResult.Success(new { article = datas, user = new { } });
                    }
                }
                else
                {
                    //调用es 返回文章id

                    //ES 模糊查询
                    List<Guid> articleIds = new List<Guid>();
                    if (!articleIds.Any())
                    {
                        return ResponseResult.Success(new { article = datas, user = new { } });
                    }
                    dataDtos = _articleService.GetTalent(talentId, page, size, articleIds).ToList();
                }
            }
            datas = ArticleDataToVoHelper.ArticleToViewModelHelper(dataDtos);

            var UserDto = _userService.GetUserInfo(searchUserId);
            var UserVo = UserHelper.UserDtoToVo(new List<PMS.UserManage.Application.ModelDto.UserInfoDto>() { UserDto }).FirstOrDefault();
            return ResponseResult.Success(new { article = datas, user = UserVo });
        }


    }
}
