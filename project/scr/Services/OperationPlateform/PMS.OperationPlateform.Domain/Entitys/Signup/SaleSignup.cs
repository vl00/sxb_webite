using System;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys.Signup
{
    [Table("SignupSale")]
    public class SaleSignup
    {
        public Guid? UserId { get; set; }

        public string Name { get; set; }
        public DateTime SignDate { get; set; }
        public string Grade { get; set; }
        public int? AgreeShare { get; set; }
        public string Phone { get; set; }
        public string Question1 { get; set; }
        public string Question2 { get; set; }
        public string Question3 { get; set; }
        public string Question4 { get; set; }
        public string Question5 { get; set; }
        public string Question6 { get; set; }
        public string Question7 { get; set; }
        public string Question8 { get; set; }
        public string Question9 { get; set; }
        public string Question10 { get; set; }
        public string Question11 { get; set; }
    }
}
