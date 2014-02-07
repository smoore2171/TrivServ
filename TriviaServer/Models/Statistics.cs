using System;
using System.Collections.Generic;

namespace TriviaServer
{
	public class Statistics
	{
		public int Id { get; set; }

		public	int Progress { get; set; }

		public int QuestionsCorrect { get; set; }

		public int QuestionsTotal { get; set; }

		public virtual List<Achievement> Achievements { get; set; }
		public virtual List<Collectible> Collectibles { get; set; }

		public virtual User user{ get; set; } 

		public Statistics ()
		{
		}
	}
}

