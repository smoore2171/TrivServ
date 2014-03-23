using System;
using System.Collections.Generic;

namespace TriviaServer
{
	public class Statistics
	{
		public int Id { get; set; }

		public int NodesProgress { get; set; }
		public int SectionProgress { get; set; }

		public int QuestionsCorrect { get; set; }

		public int QuestionsTotal { get; set; }

		public virtual List<Achievement> Achievements { get; set; }
		public virtual List<Collectible> Collectibles { get; set; }

		public virtual List<LevelScore> LevelScores { get; set; }

		public virtual User user{ get; set; } 

		public Statistics ()
		{
			SectionProgress = 0;
			NodesProgress = 0;
			QuestionsCorrect = 0;
			QuestionsTotal = 0;

			Achievements = new List<Achievement>();
			Collectibles = new List<Collectible>();
			LevelScores = new List<LevelScore>();
		}
	}
}

