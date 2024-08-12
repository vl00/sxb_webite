using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("comment")]
	public partial class comment
	{
		[ExplicitKey]
		public Guid id {get;set;}

		public byte type {get;set;}

		public Guid? forumID {get;set;}

		public Guid? parentID {get;set;}

		public Guid? replyID {get;set;}

		public Guid? userID {get;set;}

		public string title {get;set;}

		public string text {get;set;}

		public DateTime? time {get;set;}

		public bool? img {get;set;}

		public bool? show {get;set;}

		public bool? @checked {get;set;}

		public bool? toTop {get;set;}

		public int? likeCount {get;set;}

		public int? floorNum {get;set;}


	}
}