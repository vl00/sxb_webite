using PMS.PaidQA.Domain.Enums;

namespace Sxb.Web.Areas.PaidQA.Models.Order
{
    public class HasUnReadRequest
    {
        public OrderStatus? OrderStatus { get; set; } = null;
    }
    public class HasUnReadResponse
    {
        public int Total { get; set; }
        public int AskCount { get; set; }
        public int AnswerCount { get; set; }
    }
}
