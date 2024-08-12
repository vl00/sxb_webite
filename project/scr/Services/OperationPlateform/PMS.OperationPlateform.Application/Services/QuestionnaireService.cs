using System;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys.Signup;
using PMS.OperationPlateform.Domain.IRespositories;

namespace PMS.OperationPlateform.Application.Services
{
    //问卷调查
    public class QuestionnaireService: IQuestionnaireService
    {
        private readonly IQuestionnaireRepository _questionnaireRepository;
        public QuestionnaireService(IQuestionnaireRepository questionnaireRepository)
        {
            _questionnaireRepository = questionnaireRepository;
        }
        public (bool,string) AddSaleSign(SaleSignup entity)
        {
            if (_questionnaireRepository.CheckExistSaleSign(entity.Name.Trim(), entity.Phone.Trim()))
            {
                return (false, "您已经提交过了哦！");
            }

            if (_questionnaireRepository.AddSaleSign(entity))
            {
                return (true, "success");
            }
            else
            {
                return (false, "提交失败");
            }
        }
    }
}
