using System;
using System.ComponentModel.DataAnnotations;

namespace TriviaServer
{
	public class User
	{

		public Guid Id { get; set; }
		public string FacebookId { get; set; }
		public string Email { get; set; }
		 
		[Required]
		public virtual Statistics Stats { get; set; }

		public User ()
		{
			Stats = new Statistics ();
		}
	}
}

