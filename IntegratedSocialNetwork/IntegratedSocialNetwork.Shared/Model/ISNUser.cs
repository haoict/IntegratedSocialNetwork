using System;
using System.Collections.Generic;
using System.Text;

namespace IntegratedSocialNetwork.Model
{
	public class ISNUser
	{
		public ISNUser(string id, string name, string picture)
		{
			this.id = id;
			this.name = name;
			this.picture = picture;
		}

		public ISNUser()
		{

		}

		public string id { get; set; }
		public string name { get; set; }
		public string picture { get; set; }
	}
}
