using System;
using PMS.OperationPlateform.Domain.Entitys.Signup;

namespace PMS.OperationPlateform.Application.IServices
{
    public interface IQuestionnaireService
    {
        (bool,string) AddSaleSign(SaleSignup entity);
    }
}
