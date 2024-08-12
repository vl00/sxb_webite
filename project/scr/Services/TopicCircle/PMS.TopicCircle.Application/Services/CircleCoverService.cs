using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.TopicCircle.Application.Services
{
    public class CircleCoverService : ApplicationService<CircleCover>, ICircleCoverService
    {
        ICircleCoverRepository _repository;

        public CircleCoverService(ICircleCoverRepository repository, IWeChatAppClient weChatAppClient) : base(repository)
        {
            this._repository = repository;
        }


        /// <summary>
        /// 获取Circle的的背景图
        /// </summary>
        /// <param name="circle"></param>
        /// <returns></returns>
        internal Circle GetCover(Circle circle)
        {
            CircleCover cover = this._repository.GetBy("CircleId=@cid", new { cid = circle.Id }).FirstOrDefault();
            circle.Cover = cover;
            return circle;
        }


        /// <summary>
        /// 获取Circle的的背景图
        /// </summary>
        /// <param name="circles"></param>
        /// <returns></returns>
        internal IEnumerable<Circle> GetCover(IEnumerable<Circle> circles)
        {
            IEnumerable<Guid> circleIds = circles.Select(crcl => crcl.Id);
            IEnumerable<CircleCover> covers = this._repository.GetBy("CircleId in @circleIds", new { circleIds });
            return circles.Select(c =>
            {
                c.Cover = covers.FirstOrDefault(cover => cover.CircleId == c.Id);
                return c;
            });

        }

        /// <summary>
        /// 获取Circle的的背景图
        /// </summary>
        /// <param name="circles"></param>
        /// <returns></returns>
        public IEnumerable<CircleCover> GetCover(IEnumerable<Guid> circleIds)
        {
            IEnumerable<CircleCover> covers = this._repository.GetBy("CircleId in @circleIds", new { circleIds });
            return covers;
        }


  
    }
}
