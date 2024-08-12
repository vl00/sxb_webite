using System;
using PMS.OperationPlateform.Domain.Entitys.Signup;
using PMS.OperationPlateform.Domain.IRespositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;

namespace PMS.OperationPlateform.Repository.Repositories
{
    /// <summary>
    /// 问卷
    /// </summary>
    public class QuestionnaireRepository: IQuestionnaireRepository
    {
        private readonly OperationCommandDBContext _DB;
        public QuestionnaireRepository(OperationCommandDBContext context)
        {
            _DB = context;
        }


        public bool AddSaleSign(SaleSignup entity)
        {
            string sql = @"INSERT INTO SignupSale VALUES (NewId(), @Name,@SignDate,@Grade,@AgreeShare,@Phone,
            @Question1,@Question2,@Question3,@Question4,@Question5,@Question6,@Question7,@Question8,
            @Question9,@Question10,@Question11,GETDATE(),@UserId)";
            return _DB.Execute(sql,new { entity.Name, entity.SignDate, entity.Grade, entity.AgreeShare, entity.Phone ,
                entity.Question1,
                entity.Question2,
                entity.Question3,
                entity.Question4,
                entity.Question5,
                entity.Question6,
                entity.Question7,
                entity.Question8,
                entity.Question9,
                entity.Question10,
                entity.Question11,
                entity.UserId
            }) > 0;
        }

        public bool CheckExistSaleSign(string name,string phone)
        {
            string sql = "select top 1 1 from SignupSale where Name = @name and Phone = @phone;";
            return _DB.QuerySingle<int>(sql, new { name , phone }) > 0;
        }
    }
}
