using System;

namespace TriviaServer
{
	public class User
	{
		public Guid Id { get; set; }
		public string FacebookId { get; set; }
		public string Email { get; set; }
		 
		public virtual Statistics Stats { get; set; }

		public User ()
		{
		
		}
	}
}

