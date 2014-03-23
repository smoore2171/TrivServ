﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Serialization;
using System.IO;

namespace TriviaServer.Controllers
{
	public struct wireQuestion
	{
		public Guid id;
		public string question;
		public string answer1;
		public string answer2;
		public string answer3;
		public string answer4;
		public string answerId;
	}

    public class ApiController : Controller
    {
		DataContext db = new DataContext();

		public string RegisterUser()
        {
			return "";
        }

		/// <summary>
		/// Registers the user.
		/// </summary>
		/// <returns>The user.</returns>
		/// <param name="username">Username.</param>
		/// <param name="email">Email.</param>
		/// <param name="passwordHash">Password hash.</param>
		[HttpPost]
		public string RegisterUser(string email, string password, string facebookId, string Id)
		{
			try
			{
				// Make sure this only runs on SSL
				if (facebookId != "")
				{
					email = facebookId;
				}

				// Attempt to register the user
				MembershipCreateStatus createStatus;
				Membership.CreateUser(email, password, email, "question", "answer", true, null, out createStatus);

				if (createStatus == MembershipCreateStatus.Success)
				{
					User s = new User();
					s.Id = Guid.Parse (Id);
					s.Email = email;
					s.FacebookId = facebookId;
					db.users.Add (s);
					db.SaveChanges ();
					//Roles.AddUserToRole (email, "Users");
					//FormsAuthentication.SetAuthCookie (email, false /*									 createPersistentCookie */);
					return "true";
				} else
				{
					//ModelState.AddModelError ("", ErrorCodeToString (createStatus));
					return createStatus.ToString ();
					//return "false";
				}
			}
			catch(Exception e)
			{
				return e.Message;
			}
		}

		/// <summary>
		/// Registers the user.
		/// </summary>
		/// <returns>The user.</returns>
		/// <param name="username">Username.</param>
		/// <param name="email">Email.</param>
		/// <param name="passwordHash">Password hash.</param>
		[HttpPost]
		public string Login(string email, string password, string facebookId)
		{
			// make sure this runs on ssl
			if (facebookId != "")
			{
				email = facebookId;
			}
			if (Membership.ValidateUser (email, password))
			{
				return db.users.Where(u => u.Email == email).Single().Id+"";
			} else
			{
				return "Incorrect username or password";
			}
		}

		/// <summary>
		/// Gets the statistics.
		/// </summary>
		/// <returns>The statistics.</returns>
		/// <param name="userId">User identifier.</param>
		public string GetStatistics(string userId)
		{
			Guid uid = Guid.Parse (userId);
			User user = db.users.Where (s => s.Id == uid).Single ();

			TextWriter text = new StringWriter ();
			XmlSerializer x = new XmlSerializer (typeof(Statistics));
			x.Serialize (text, user.Stats);

			return text.ToString();
		}

		/// <summary>
		/// Updates the statistics.
		/// </summary>
		/// <returns>The statistics.</returns>
		/// <param name="stats">Stats.</param>
		public string UpdateStatistics(Statistics stats, string userId)
		{
			Guid uid = Guid.Parse (userId);

			User user = db.users.Where (s => s.Id == uid).Single ();
			user.Stats.SectionProgress = stats.SectionProgress;
			user.Stats.NodesProgress = stats.NodesProgress;
			user.Stats.QuestionsCorrect = stats.QuestionsCorrect;
			user.Stats.QuestionsTotal = stats.QuestionsTotal;


			foreach (Collectible c in stats.Collectibles)
			{
				if(!user.Stats.Collectibles.Contains(c))
					user.Stats.Collectibles.Add (c);
			}

			foreach (Achievement a in stats.Achievements)
			{
				if(!user.Stats.Achievements.Contains(a))
					user.Stats.Achievements.Add (a);
			}

			foreach (LevelScore s in stats.LevelScores)
			{
				if (!user.Stats.LevelScores.Contains (s))
					user.Stats.LevelScores.Add (s);
				else if (user.Stats.LevelScores.Where (st => st.Id == s.Id).Single ().Score < s.Score)
				{
					user.Stats.LevelScores.Where (st => st.Id == s.Id).Single ().Score = s.Score;
				}
			}

			db.SaveChanges ();
			return "true";
		}

		/// <summary>
		/// Returns a butt-load of questions for the system to cache
		/// </summary>
		/// <returns>The questions.</returns>
		public string GetQuestions()
		{
			// don't have the data contexts for this yet
			List<wireQuestion> questions = new List<wireQuestion> () {
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What is the addictive chemical in cigarettes?",
					answer1 = "Alcohol",
					answer2 = "Carbon Dioxide",
					answer3 = "Sodium",
					answer4 = "Nicotine",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What unit is force measured in?",
					answer1 = "Newman",
					answer2 = "Neutron",
					answer3 = "Pushes",
					answer4 = "Newton",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What is the tough outer layer of a seed called?",
					answer1 = "Seed Blanket",
					answer2 = "Seed Shelf",
					answer3 = "Seed Shell",
					answer4 = "Seed Coat",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What country did King Hussein rule?",
					answer1 = "Vietnam",
					answer2 = "Canada",
					answer3 = "England",
					answer4 = "Jordan",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "Who was the leader of the Soviet Union who resigned on Dec 25 1991?",
					answer1 = "Yeltsin",
					answer2 = "Sununu",
					answer3 = "Popov",
					answer4 = "Gorbachev",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "Which war was concluded by the Treaty of Utrecht?",
					answer1 = "The War Of Austrian Succession",
					answer2 = "The Boer War",
					answer3 = "The Thirty Years' War",
					answer4 = "War Of Spanish Succession",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "Which role did Bill Murray play in the movie \"Ghostbusters\"?",
					answer1 = "Dr. Raymond Stantz",
					answer2 = "Dr. Egon Spenglar",
					answer3 = "Winston Zeddemore",
					answer4 = "Dr. Peter Venkman",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "In what film was actor Antonio Banderas' first English-speaking role?",
					answer1 = "Tequila Sunrise",
					answer2 = "Havana",
					answer3 = "Moon Over Parador",
					answer4 = "The Mambo Kings",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What was Johnny Depp's directorial debut?",
					answer1 = "Boys",
					answer2 = "The Nick Of Time",
					answer3 = "The Rock",
					answer4 = "The Brave",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "How many members of the 2004 Red Sox had previously won a World Series?",
					answer1 = "7 Members",
					answer2 = "4 Members",
					answer3 = "Zero",
					answer4 = "2 Members",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What is the addictive chemical in cigarettes?",
					answer1 = "Alcohol",
					answer2 = "Carbon Dioxide",
					answer3 = "Sodium",
					answer4 = "Nicotine",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What unit is force measured in?",
					answer1 = "Newman",
					answer2 = "Neutron",
					answer3 = "Pushes",
					answer4 = "Newton",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What is the tough outer layer of a seed called?",
					answer1 = "Seed Blanket",
					answer2 = "Seed Shelf",
					answer3 = "Seed Shell",
					answer4 = "Seed Coat",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What country did King Hussein rule?",
					answer1 = "Vietnam",
					answer2 = "Canada",
					answer3 = "England",
					answer4 = "Jordan",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "Who was the leader of the Soviet Union who resigned on Dec 25 1991?",
					answer1 = "Yeltsin",
					answer2 = "Sununu",
					answer3 = "Popov",
					answer4 = "Gorbachev",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "Which war was concluded by the Treaty of Utrecht?",
					answer1 = "The War Of Austrian Succession",
					answer2 = "The Boer War",
					answer3 = "The Thirty Years' War",
					answer4 = "War Of Spanish Succession",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "Which role did Bill Murray play in the movie \"Ghostbusters\"?",
					answer1 = "Dr. Raymond Stantz",
					answer2 = "Dr. Egon Spenglar",
					answer3 = "Winston Zeddemore",
					answer4 = "Dr. Peter Venkman",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "In what film was actor Antonio Banderas' first English-speaking role?",
					answer1 = "Tequila Sunrise",
					answer2 = "Havana",
					answer3 = "Moon Over Parador",
					answer4 = "The Mambo Kings",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What was Johnny Depp's directorial debut?",
					answer1 = "Boys",
					answer2 = "The Nick Of Time",
					answer3 = "The Rock",
					answer4 = "The Brave",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "How many members of the 2004 Red Sox had previously won a World Series?",
					answer1 = "7 Members",
					answer2 = "4 Members",
					answer3 = "Zero",
					answer4 = "2 Members",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What is the addictive chemical in cigarettes?",
					answer1 = "Alcohol",
					answer2 = "Carbon Dioxide",
					answer3 = "Sodium",
					answer4 = "Nicotine",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What unit is force measured in?",
					answer1 = "Newman",
					answer2 = "Neutron",
					answer3 = "Pushes",
					answer4 = "Newton",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What is the tough outer layer of a seed called?",
					answer1 = "Seed Blanket",
					answer2 = "Seed Shelf",
					answer3 = "Seed Shell",
					answer4 = "Seed Coat",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What country did King Hussein rule?",
					answer1 = "Vietnam",
					answer2 = "Canada",
					answer3 = "England",
					answer4 = "Jordan",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "Who was the leader of the Soviet Union who resigned on Dec 25 1991?",
					answer1 = "Yeltsin",
					answer2 = "Sununu",
					answer3 = "Popov",
					answer4 = "Gorbachev",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "Which war was concluded by the Treaty of Utrecht?",
					answer1 = "The War Of Austrian Succession",
					answer2 = "The Boer War",
					answer3 = "The Thirty Years' War",
					answer4 = "War Of Spanish Succession",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "Which role did Bill Murray play in the movie \"Ghostbusters\"?",
					answer1 = "Dr. Raymond Stantz",
					answer2 = "Dr. Egon Spenglar",
					answer3 = "Winston Zeddemore",
					answer4 = "Dr. Peter Venkman",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "In what film was actor Antonio Banderas' first English-speaking role?",
					answer1 = "Tequila Sunrise",
					answer2 = "Havana",
					answer3 = "Moon Over Parador",
					answer4 = "The Mambo Kings",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What was Johnny Depp's directorial debut?",
					answer1 = "Boys",
					answer2 = "The Nick Of Time",
					answer3 = "The Rock",
					answer4 = "The Brave",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "How many members of the 2004 Red Sox had previously won a World Series?",
					answer1 = "7 Members",
					answer2 = "4 Members",
					answer3 = "Zero",
					answer4 = "2 Members",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What is the addictive chemical in cigarettes?",
					answer1 = "Alcohol",
					answer2 = "Carbon Dioxide",
					answer3 = "Sodium",
					answer4 = "Nicotine",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What unit is force measured in?",
					answer1 = "Newman",
					answer2 = "Neutron",
					answer3 = "Pushes",
					answer4 = "Newton",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What is the tough outer layer of a seed called?",
					answer1 = "Seed Blanket",
					answer2 = "Seed Shelf",
					answer3 = "Seed Shell",
					answer4 = "Seed Coat",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What country did King Hussein rule?",
					answer1 = "Vietnam",
					answer2 = "Canada",
					answer3 = "England",
					answer4 = "Jordan",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "Who was the leader of the Soviet Union who resigned on Dec 25 1991?",
					answer1 = "Yeltsin",
					answer2 = "Sununu",
					answer3 = "Popov",
					answer4 = "Gorbachev",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "Which war was concluded by the Treaty of Utrecht?",
					answer1 = "The War Of Austrian Succession",
					answer2 = "The Boer War",
					answer3 = "The Thirty Years' War",
					answer4 = "War Of Spanish Succession",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "Which role did Bill Murray play in the movie \"Ghostbusters\"?",
					answer1 = "Dr. Raymond Stantz",
					answer2 = "Dr. Egon Spenglar",
					answer3 = "Winston Zeddemore",
					answer4 = "Dr. Peter Venkman",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "In what film was actor Antonio Banderas' first English-speaking role?",
					answer1 = "Tequila Sunrise",
					answer2 = "Havana",
					answer3 = "Moon Over Parador",
					answer4 = "The Mambo Kings",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What was Johnny Depp's directorial debut?",
					answer1 = "Boys",
					answer2 = "The Nick Of Time",
					answer3 = "The Rock",
					answer4 = "The Brave",
					answerId = "3",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "How many members of the 2004 Red Sox had previously won a World Series?",
					answer1 = "7 Members",
					answer2 = "4 Members",
					answer3 = "Zero",
					answer4 = "2 Members",
					answerId = "3",
				},

			
			}; 
			TextWriter text = new StringWriter ();
			XmlSerializer x = new XmlSerializer (typeof(List<wireQuestion>));
			x.Serialize (text, questions);

			return text.ToString();
		}

		/// <summary>
		/// Gets the friend leader boards - returns all
		/// </summary>
		/// <returns>The friend leader boards.</returns>
		/// <param name="friends">Friends.</param>
		public string GetFriendLeaderBoards(List<string> friends)
		{
			List<User> friendUsers = new List<TriviaServer.User>(); 

			foreach(string fid in friends)
			{
				if(db.users.Where( f => f.FacebookId == fid).Count() > 0)
					friendUsers.Add(db.users.Where( f => f.FacebookId == fid).Single());
			}

			List<Statistics> friendStats = new List<Statistics> ();

			foreach (User u in friendUsers)
			{
				friendStats.Add (u.Stats);
			}

			// return xml for stats

			return "";
		}

		/// <summary>
		/// Gets the global leaderboards. by 20's
		/// </summary>
		/// <returns>The global leaderboards.</returns>
		public string GetGlobalRatioLeaderboards(int page)
		{
			//List<Statistics> topStats = db.statistics.Where(a => a.

			return "";
		}

		public string GetFriendsForCollectible(Collectible collectible)
		{
			List<User> friends = db.users.Where (f => f.Stats.Collectibles.Contains (collectible)).ToList ();

			// return XML representation of list

			return "";
		}


    }
}
