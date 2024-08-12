using System;
using Dapper.Contrib.Extensions;
using PMS.Infrastructure.Domain.Enums;

namespace PMS.Infrastructure.Domain.Entities
{
	[Serializable]
	[Table("ServerStorage")]
	public partial class ServerStorage
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
        public Guid Id { get; set; }


        public string HashKey { get; set; }

        /// <summary> 
        /// </summary> 
        public string Key {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Value {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime ExpireAt {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime CreateTime {get;set;}

        public ServerStorageDataType DataType { get; set; }


    }
}