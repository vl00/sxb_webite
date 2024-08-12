using PMS.OperationPlateform.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.IServices
{
    public interface IArticle_SubscribePreferenceServices
    {

        Article_SubscribePreference GetByUserId(Guid userId);

        Article_SubscribePreference Add(Article_SubscribePreference model);

        Article_SubscribePreference Update(Article_SubscribePreference model, params string[] fileds);
    }
}
