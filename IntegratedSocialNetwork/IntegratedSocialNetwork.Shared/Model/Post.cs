using System;
using System.Collections.Generic;
using System.Text;

namespace IntegratedSocialNetwork.Model
{
	public class Post
	{
		public string id { get; set; }
		public User user { get; set; }
		public string message { get; set; }
	}
}
