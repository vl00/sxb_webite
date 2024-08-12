using System;
using PMS.OperationPlateform.Domain.Entitys.Signup;

namespace PMS.OperationPlateform.Domain.IRespositories
{
    public interface IQuestionnaireRepository
    {
        bool AddSaleSign(SaleSignup entity);
        bool CheckExistSaleSign(string name, string phone);
    }
}
