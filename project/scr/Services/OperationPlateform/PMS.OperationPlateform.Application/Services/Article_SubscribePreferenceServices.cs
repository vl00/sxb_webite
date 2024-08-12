using PMS.OperationPlateform.Application.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace PMS.OperationPlateform.Application.Services
{
    using Domain.IRespositories;
    using PMS.OperationPlateform.Domain.Entitys;

    public class Article_SubscribePreferenceServices: IArticle_SubscribePreferenceServices
    {
        IArticle_SubscribePreferenceRepository currentRepository;

        public Article_SubscribePreferenceServices(IArticle_SubscribePreferenceRepository article_SubscribePreferenceRepository)
        {
            this.currentRepository = article_SubscribePreferenceRepository;
        }



        public Article_SubscribePreference GetByUserId(Guid userId)
        {
           
          return  this.currentRepository.Select("UserId=@UserId", new { UserId = userId }).FirstOrDefault();
        }


        public Article_SubscribePreference Add(Article_SubscribePreference model)
        {

            return this.currentRepository.Add(model);
        }
        public Article_SubscribePreference Update(Article_SubscribePreference model,params string[] fileds)
        {

            return this.currentRepository.Update(model,fileds);
        }


        public Article_SubscribePreference Set(Article_SubscribePreference model)
        {
           var exsist = this.currentRepository.Select("id=@id", new { id = model.Id });
            if (exsist.Any())
            {
              model =   this.currentRepository.Update(model);
            }
            else {
              model =   this.currentRepository.Add(model);
            }

            return model;
        }

    }
}
