using System;
using System.Collections.Generic;

namespace TriviaServer
{
	public class Question
	{
		public int Id { get; set; }
		public string Text { get; set; }

		public virtual List<Answer> Answers { get; set; }
		public virtual Answer Answer { get; set; }

		public Question ()
		{
		}
	}
}

