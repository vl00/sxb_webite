using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.School.Domain.Entities
{
	[Serializable]
	[Table("WeChatSouYiSouSchool")]
	public partial class WeChatSouYiSouSchool
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid? SId {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid? EId {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Name {get;set;}

		 /// <summary> 
		 /// Json´®
		 /// </summary> 
		public string Alias {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string WXSchoolId {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime? ModifiTime {get;set;}


		public List<string> GetAlias()
		{
			List<string> defaults = new List<string>();
			try
			{
				if (!string.IsNullOrEmpty(this.Alias))
				{
					return Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(this.Alias);
				}
				else
				{

					return defaults;
				}
			}
			catch {
				return defaults;
			}

		}



    }
}


