using System;
using Dapper.Contrib.Extensions;

namespace PMS.PaidQA.Domain.Entities
{
	[Serializable]
	[Table("PayWalletLog")]
	public partial class PayWalletLog
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid? UserId {get;set;}

		 /// <summary> 
		 /// </summary> 
		public decimal Amount {get;set;}

		 /// <summary> 
		 ///  /// <summary> 
		 ///         /// 充值 
		 ///         /// </summary> 
		 ///         Recharge = 1, 
		 ///         /// <summary> 
		 ///         /// 支出 
		 ///         /// </summary> 
		 ///         Outgoings = 2, 
		 ///         /// <summary> 
		 ///         /// 收入 
		 ///         /// </summary> 
		 ///         Incomings = 3, 
		 ///         /// <summary> 
		 ///         /// 结算 
		 ///         /// </summary> 
		 ///         Settlement = 4, 
		 ///         /// <summary> 
		 ///         /// 服务费 
		 ///         /// </summary> 
		 ///         ServiceFee = 5, 
		 /// </summary> 
		public byte StatementType {get;set;}

		 /// <summary> 
		 /// </summary> 
		public byte IO {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid? OrderId {get;set;}

		 /// <summary> 
		 ///    /// <summary> 
		 ///         /// 1 上学问 
		 ///         /// </summary> 
		 ///         Ask = 1, 
		 ///         /// <summary> 
		 ///         /// 2 完成提现(结算) 
		 ///         /// </summary> 
		 ///         Withdraw = 2, 
		 ///         /// <summary> 
		 ///         /// 3 机构 
		 ///         /// </summary> 
		 ///         Org = 3, 
		 /// </summary> 
		public byte OrderType {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Remark {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime? CreatetTime {get;set;}

        public string FailedMsg { get; set; }


    }
}