using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace TriviaServer.Controllers
{
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
		public bool RegisterUser(string email, string password, string name, string facebookId)
		{
			// Make sure this only runs on SSL


			// Attempt to register the user
			MembershipCreateStatus createStatus;
			Membership.CreateUser(email, password, email, "question", "answer", true, null, out createStatus);

			if (createStatus == MembershipCreateStatus.Success)
			{
				User s = new User();
				s.Email = email;
				s.FacebookId = facebookId;
				db.users.Add (s);
				db.SaveChanges ();
				//Roles.AddUserToRole (email, "Users");
				//FormsAuthentication.SetAuthCookie (email, false /*									 createPersistentCookie */);
				return true;
			} else
			{
				//ModelState.AddModelError ("", ErrorCodeToString (createStatus));
				return false;
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
		public string Login(string email, string password)
		{
			// make sure this runs on ssl

			if (Membership.ValidateUser (email, password))
			{
				return email;
			} else
			{
				return "Incorrect username or password";
			}
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
			user.Stats.Progress = stats.Progress;
			user.Stats.QuestionsCorrect = stats.QuestionsCorrect;
			user.Stats.QuestionsTotal = stats.QuestionsTotal;

			foreach (Collectible c in stats.Collectibles)
			{
				user.Stats.Collectibles.Add (c);
			}

			foreach (Achievement a in stats.Achievements)
			{
				user.Stats.Achievements.Add (a);
			}

			return "true";
		}

		/// <summary>
		/// Returns a butt-load of questions for the system to cache
		/// </summary>
		/// <returns>The questions.</returns>
		public string GetQuestions()
		{
			// don't have the data contexts for this yet

			return "";
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
