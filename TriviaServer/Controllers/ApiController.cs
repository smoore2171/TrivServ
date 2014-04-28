using System;
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
					answerId = "4",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What unit is force measured in?",
					answer1 = "Newman",
					answer2 = "Neutron",
					answer3 = "Pushes",
					answer4 = "Newton",
					answerId = "4",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What is the tough outer layer of a seed called?",
					answer1 = "Seed Blanket",
					answer2 = "Seed Shelf",
					answer3 = "Seed Shell",
					answer4 = "Seed Coat",
					answerId = "4",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What country did King Hussein rule?",
					answer1 = "Vietnam",
					answer2 = "Canada",
					answer3 = "England",
					answer4 = "Jordan",
					answerId = "4",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "Who was the leader of the Soviet Union who resigned on Dec 25 1991?",
					answer1 = "Yeltsin",
					answer2 = "Sununu",
					answer3 = "Popov",
					answer4 = "Gorbachev",
					answerId = "4",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "Which war was concluded by the Treaty of Utrecht?",
					answer1 = "The War Of Austrian Succession",
					answer2 = "The Boer War",
					answer3 = "The Thirty Years' War",
					answer4 = "War Of Spanish Succession",
					answerId = "4",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "Which role did Bill Murray play in the movie \"Ghostbusters\"?",
					answer1 = "Dr. Raymond Stantz",
					answer2 = "Dr. Egon Spenglar",
					answer3 = "Winston Zeddemore",
					answer4 = "Dr. Peter Venkman",
					answerId = "4",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "In what film was actor Antonio Banderas' first English-speaking role?",
					answer1 = "Tequila Sunrise",
					answer2 = "Havana",
					answer3 = "Moon Over Parador",
					answer4 = "The Mambo Kings",
					answerId = "4",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "What was Johnny Depp's directorial debut?",
					answer1 = "Boys",
					answer2 = "The Nick Of Time",
					answer3 = "The Rock",
					answer4 = "The Brave",
					answerId = "4",
				},
				new wireQuestion() 
				{ 
					id = Guid.NewGuid(),
					question = "How many members of the 2004 Red Sox had previously won a World Series?",
					answer1 = "7 Members",
					answer2 = "4 Members",
					answer3 = "Zero",
					answer4 = "2 Members",
					answerId = "4",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What animal plants millions of trees a year by forgetting where its nuts are buried?",
					answer1 = "Bear",
					answer2 = "Squirrel",
					answer3 = "Turtle",
					answer4 = "Chipmunk",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of the following is classified as a dinosaur?",
					answer1 = "Triceratops",
					answer2 = "Pteranodon",
					answer3 = "Plesiosaur",
					answer4 = "All of the above",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What does Nintendo translate to?",
					answer1 = "Good gaming for all",
					answer2 = "We would like to play",
					answer3 = "Mario is the best",
					answer4 = "Leave luck to heaven",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Hair is made from the same material as?",
					answer1 = "Fingernails",
					answer2 = "Moles",
					answer3 = "Bones",
					answer4 = "Muscles",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How many musketeers were there in the novel The Three Musketeers?",
					answer1 = "1",
					answer2 = "2",
					answer3 = "3",
					answer4 = "4",
					answerId = "4",
				},


				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How many furlongs are there in a mile?",
					answer1 = "20",
					answer2 = "8",
					answer3 = "2",
					answer4 = "147",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which artist had their CD pressed in the United States first?",
					answer1 = "Vanilla Ice",
					answer2 = "Johnny Cash",
					answer3 = "Bruce Springsteen",
					answer4 = "50 cent",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What planet rotates clockwise?",
					answer1 = "Venus",
					answer2 = "Saturn",
					answer3 = "Earth",
					answer4 = "Neptune",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which animal is the only animal that can’t jump?",
					answer1 = "Hoaztin",
					answer2 = "Elephant",
					answer3 = "Turtle",
					answer4 = "Javelina",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "In which country is tree hugging illegal",
					answer1 = "Russia",
					answer2 = "Brazil",
					answer3 = "China",
					answer4 = "Kenya",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which animal can bend its knees backward?",
					answer1 = "Flamingo",
					answer2 = "Sea Star",
					answer3 = "Tiger",
					answer4 = "Deer",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which planet has density so low that it can float in water?",
					answer1 = "Mercury",
					answer2 = "Pluto",
					answer3 = "Uranus",
					answer4 = "Saturn",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "The Earth moves through space at which rate?",
					answer1 = "5,200 km/s",
					answer2 = "530 km/s",
					answer3 = "70 km/s",
					answer4 = "2 km/s",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How far does the moon travel away from the earth, every year?",
					answer1 = "3.8 cm",
					answer2 = "10 cm",
					answer3 = ".5 cm",
					answer4 = "1.2 cm",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Where is the Temple of the Tooth located?",
					answer1 = "Naples, Italy",
					answer2 = "East Lansing, Michigan",
					answer3 = "Nairobi, Kenya",
					answer4 = "Kandy, Sri Lanka",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which animal species is the only that is able to hold its tail vertically while walking?",
					answer1 = "Rat",
					answer2 = "Giraffe",
					answer3 = "Domestic Dog",
					answer4 = "Domestic Cat",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of these sounds has the same frequency as an idle diesel engine?",
					answer1 = "Motorcycle",
					answer2 = "Cat’s purr",
					answer3 = "Bat’s heartbeat",
					answer4 = "Grasshopper’s chirp",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Mature cats spend how much of their time lightly sleeping?",
					answer1 = "50%",
					answer2 = "30%",
					answer3 = "15%",
					answer4 = "2%",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What do cat’s primarily use their whiskers for?",
					answer1 = "Smelling",
					answer2 = "Seeing",
					answer3 = "Balancing",
					answer4 = "Measuring distances",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of these animals can go from a sleep state to full alertness the quickest?",
					answer1 = "Cats",
					answer2 = "Dogs",
					answer3 = "Rats",
					answer4 = "Dolphins",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What are groups of cats called?",
					answer1 = "Gang",
					answer2 = "Clique",
					answer3 = "Clowder",
					answer4 = "Murder",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which animal requires gravity to swollow?",
					answer1 = "Aardvark",
					answer2 = "Tiger",
					answer3 = "Bird",
					answer4 = "Ape",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which animal cannot swallow with its eyes open?",
					answer1 = "Mouse",
					answer2 = "Frog",
					answer3 = "Dog",
					answer4 = "Whale",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the state bird of Michigan?",
					answer1 = "Cardinal",
					answer2 = "Hummingbird",
					answer3 = "Blue Jay",
					answer4 = "Robin",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "At which rate does a hummingbird’s heart beat?",
					answer1 = "150 bpm",
					answer2 = "400 bpm",
					answer3 = "1000 bpm",
					answer4 = "55 bpm",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How many ways can you make change for a dollar?",
					answer1 = "10",
					answer2 = "152",
					answer3 = "247",
					answer4 = "293",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How many times does lightning hit the earth every minute?",
					answer1 = "6,000",
					answer2 = "10",
					answer3 = "300",
					answer4 = "3,500",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What are a group of Kangaroos called?",
					answer1 = "A club",
					answer2 = "A shrowed",
					answer3 = "A cloud",
					answer4 = "A mob",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What shape is the pupil of an Octopus?",
					answer1 = "Star",
					answer2 = "Oval",
					answer3 = "Rectangle",
					answer4 = "Circle",
					answerId = "3",
				},


				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What year was the original Star Wars released?",
					answer1 = "1983",
					answer2 = "1980",
					answer3 = "1977",
					answer4 = "1991",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is a human’s ideal temperature (Fahrenheit)?",
					answer1 = "98.6 degrees",
					answer2 = "100 degrees",
					answer3 = "88.4 degrees",
					answer4 = "96.8 degrees",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the largest island in the world?",
					answer1 = "Hawaii",
					answer2 = "Japan",
					answer3 = "Greenland",
					answer4 = "Australia",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "A lion’s roar can be heard from?",
					answer1 = "5 miles",
					answer2 = "10 feet",
					answer3 = "1 mile",
					answer4 = "15 miles",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of the following characters have been voiced by Jim Cummings?",
					answer1 = "Tazmanian Devil",
					answer2 = "Winnie the Pooh",
					answer3 = "Darkwing Duck",
					answer4 = "All of the above",
					answerId = "4",
				},


				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the meaning of life?",
					answer1 = "To love and be loved in return",
					answer2 = "42",
					answer3 = "To get rich",
					answer4 = "43",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What company makes Nilla Wafers?",
					answer1 = "Keebler",
					answer2 = "Kellogs",
					answer3 = "Nabisco",
					answer4 = "Pepperidge Farms",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the main ingredient in Fruit Snacks?",
					answer1 = "Fruit",
					answer2 = "High Fructose Corn Syrup",
					answer3 = "Sugar",
					answer4 = "Gelatin",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of these animals is a marsupial?",
					answer1 = "Racoon",
					answer2 = "Sloth",
					answer3 = "Squirrel",
					answer4 = "Sugar Glider",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What two colors together make orange?",
					answer1 = "Yellow and Orange",
					answer2 = "Red and Green",
					answer3 = "Yellow and Red",
					answer4 = "Orange and Red",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What year was Jurassic Park released to theaters?",
					answer1 = "1990",
					answer2 = "1991",
					answer3 = "1989",
					answer4 = "1993",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What movie is known for having the most cussing in it?",
					answer1 = "Pulp Fiction",
					answer2 = "Die Hard",
					answer3 = "Wolf of Wall Street",
					answer4 = "The Departed",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What year was the first land-grant University established in the United States?",
					answer1 = "1888",
					answer2 = "1877",
					answer3 = "1866",
					answer4 = "1855",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How far can the human eye see on the horizon?",
					answer1 = "10 feet",
					answer2 = "15 miles",
					answer3 = "20 miles",
					answer4 = "25 miles",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is special about calico cats?",
					answer1 = "They live longer than other breeds",
					answer2 = "They can see in color",
					answer3 = "They are almost always female",
					answer4 = "They can’t get cancer",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which one of these is not a type of nut?",
					answer1 = "Macalania",
					answer2 = "Hickory",
					answer3 = "Hazel",
					answer4 = "Pine",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How old is the Disney film, Snow White and the Seven Dwarves?",
					answer1 = "54",
					answer2 = "62",
					answer3 = "77",
					answer4 = "43",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What mythological creature consists of a goat, a lion and a snake?",
					answer1 = "Manticore",
					answer2 = "Cerberus",
					answer3 = "Chimera",
					answer4 = "Chimichanga",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of these was not a gift?",
					answer1 = "Statue of Liberty",
					answer2 = "The Resolute desk",
					answer3 = "The Senate gavel",
					answer4 = "The Golden Gate Bridge",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How many signs are there in the Zodiac?",
					answer1 = "6",
					answer2 = "8",
					answer3 = "12",
					answer4 = "24",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the national color of the Netherlands?",
					answer1 = "Red",
					answer2 = "Blue",
					answer3 = "Yellow",
					answer4 = "Orange",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What language is the ‘What the Fox Say’ song sung in?",
					answer1 = "Dutch",
					answer2 = "Bokmal",
					answer3 = "Norwegian",
					answer4 = "German",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What two actions are physically impossible do at once?",
					answer1 = "Breathe through the nose with the mouth open",
					answer2 = "Draw a triangle with their hands and a square with their feet",
					answer3 = "Sneeze with eyes open",
					answer4 = "Chewing gum and whistling",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How many consecutive years have the Detroit Red Wings gone to the playoffs?",
					answer1 = "15",
					answer2 = "17",
					answer3 = "20",
					answer4 = "22",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How many square feet in an acre?",
					answer1 = "5280",
					answer2 = "4350",
					answer3 = "3876",
					answer4 = "3333",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of the following is a Marvel Comics title?",
					answer1 = "Batman",
					answer2 = "Berserk",
					answer3 = "Deadpool",
					answer4 = "Swamp Thing",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of the following is not a Shakespeare play?",
					answer1 = "As You Like It",
					answer2 = "Henry VIII",
					answer3 = "King Lear",
					answer4 = "The King and I",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of the following monsters has Godzilla both fought and teamed up with?",
					answer1 = "Gigan",
					answer2 = "Orga",
					answer3 = "Anguirus",
					answer4 = "Megalon",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of the Seven Dwarves wore glasses?",
					answer1 = "Grumpy",
					answer2 = "Dopey",
					answer3 = "Doc",
					answer4 = "Sleepy",
					answerId = "3",
				},


				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Sashimi is a Japanese delicacy consisting of raw what?",
					answer1 = "Eggs",
					answer2 = "Meat",
					answer3 = "Vegetables",
					answer4 = "Seafood",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What element is not a gas at room temprature?",
					answer1 = "Hydrogen",
					answer2 = "Oxygen",
					answer3 = "Iron",
					answer4 = "Helium",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which country is located on an island?",
					answer1 = "Bolivia",
					answer2 = "Sri Lanka",
					answer3 = "Norway",
					answer4 = "Zimbabwe",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which unit of speed is used in maritime and air navigation?",
					answer1 = "Band",
					answer2 = "Rope",
					answer3 = "Loop",
					answer4 = "Knot",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What element has the chemical symbol C?",
					answer1 = "Carbon",
					answer2 = "Cadmium",
					answer3 = "Calcium",
					answer4 = "Chlorine",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of these cities has the most inhabitants?",
					answer1 = "New York",
					answer2 = "Shanghai",
					answer3 = "Moscow",
					answer4 = "Mumbai",
					answerId = "2",
				},


				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is an archipelago?",
					answer1 = "Headland",
					answer2 = "River delta",
					answer3 = "Island group",
					answer4 = "Mountain ranges",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "In which sport game are the sections not called “inngs”?",
					answer1 = "Baseball",
					answer2 = "Polo",
					answer3 = "Softball",
					answer4 = "Cricket",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What 2 physical features are longer on a hare than on a rabbit?",
					answer1 = "Ears and Legs",
					answer2 = "Teeth and Fur",
					answer3 = "Ears and Teeth",
		            answer4 = "Legs and Fur",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Who was the first Boy Scout to become a US president?",
					answer1 = "Richard Nixon",
					answer2 = "John F. Kennedy",
					answer3 = "George W. Bush",
					answer4 = "George Washington",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which country is reigned by a communist party?",
					answer1 = "Australia",
					answer2 = "China",
					answer3 = "Germany",
					answer4 = "Japan",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How much ml is “a fifth” originally?",
					answer1 = "650ml",
					answer2 = "750ml",
					answer3 = "757ml",
					answer4 = "823ml",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "‘Pho’ is from?",
					answer1 = "Vietnam",
					answer2 = "China",
					answer3 = "Thailand",
					answer4 = "Japan",
					answerId = "1",
				},



				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How many dragon balls in Dragon Ball Z?",
					answer1 = "5",
					answer2 = "6",
					answer3 = "7",
					answer4 = "14",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = " What percent of the weight of a two old pillow can be attributed to dust mites?",
					answer1 = "0",
					answer2 = "2",
					answer3 = "5",
					answer4 = "50",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How long can a Bed Bug live without food?",
					answer1 = "A day",
					answer2 = "A week",
					answer3 = "A month",
					answer4 = "A year",
					answerId = "4",
				},






				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of the following can be transmitted by Michigan mosquitoes?",
					answer1 = "LaCrosse Encephalitis",
					answer2 = "Malaria",
					answer3 = "West Nile Virus",
					answer4 = "All of the above",
					answerId = "4",
				},



				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which shape has no straight sides?",
					answer1 = "Parallelogram",
					answer2 = "Circle",
					answer3 = "Trapezoid",
					answer4 = "Triangle",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Who built the Sphinx?",
					answer1 = "Persians",
					answer2 = "Romans",
					answer3 = "Egyptians",
					answer4 = "Greeks",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the region of calm at the center of a cyclone or hurricane?",
					answer1 = "Eye",
					answer2 = "Mouth",
					answer3 = "Heart",
					answer4 = "Navel",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Who wrote ‘War and Peace’?",
					answer1 = "Leo Tolstoy",
					answer2 = "Franz Kafka",
					answer3 = "Ernest Hemingway",
					answer4 = "James Joyce",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "A ‘White Russian’ usually consists of vodka, Kahlua and which other ingredient?",
					answer1 = "Coke of Pepsi",
					answer2 = "Milk or Cream",
					answer3 = "Soda Water or Tonic",
					answer4 = "Orange or Strawberry Juice",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which bird is often associated with wisdom?",
					answer1 = "Owl",
					answer2 = "Vulture",
					answer3 = "Sparrow",
					answer4 = "Stork",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Who became leader of North Korea in 1994?",
					answer1 = "Choe Yong-rim",
					answer2 = "Kim Il-sung",
					answer3 = "Kim Jong-il",
					answer4 = "Kim Jong-un",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What painting technique is the youngest?",
					answer1 = "fresco",
					answer2 = "spray paint",
					answer3 = "oil painting",
					answer4 = "cave painting",
					answerId = "2",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What country is a monarchy?",
					answer1 = "China",
					answer2 = "Brazil",
					answer3 = "England",
					answer4 = "Mexico",
					answerId = "3",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is NOT an official language of Switzerland?",
					answer1 = "Danish",
					answer2 = "Italian",
					answer3 = "French",
					answer4 = "German",
					answerId = "1",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How do you say \"broken\" in German?",
					answer1 = "roto",
					answer2 = "kaputt",
					answer3 = "cassé",
					answer4 = "guasto",
					answerId = "2",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Who always had 'One more question…' as Detective Columbo?",
					answer1 = "Richard Gere",
					answer2 = "Leslie Nielson",
					answer3 = "Peter Falk",
					answer4 = "Harrison Ford",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What kind of bread has a pocket in it?",
					answer1 = "Pumpernickel",
					answer2 = "Rye",
					answer3 = "Pita",
					answer4 = "Wheat",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Triskaidekaphobia is the fear of what number?",
					answer1 = "18",
					answer2 = "13",
					answer3 = "21",
					answer4 = "1",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is used for scuba diving?",
					answer1 = "diving cap",
					answer2 = "diving bowler",
					answer3 = "diving cylinder",
					answer4 = "diving hat",
					answerId = "3",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "In which country was Ferrari founded in 1946?",
					answer1 = "Germany",
					answer2 = "Italy",
					answer3 = "France",
					answer4 = "Spain",
					answerId = "2",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "1, 1, 2, 3, 5, 8,... ?",
					answer1 = "15",
					answer2 = "12",
					answer3 = "13",
					answer4 = "10",
					answerId = "3",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "The US state of Michigan is said to be the birthplace of the American?",
					answer1 = "fast food industry",
					answer2 = "aerospace industry",
					answer3 = "entertainment industry",
					answer4 = "automotive industry",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of these is produced by insects?",
					answer1 = "Cotton",
					answer2 = "Silk",
					answer3 = "Diamond",
					answer4 = "Jute",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which type of animal is a tarantula?",
					answer1 = "Bird",
					answer2 = "Arachnid",
					answer3 = "Amphibian",
					answer4 = "Mammal",
					answerId = "2",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What are 5% expressed as a fraction?",
					answer1 = "1/20",
					answer2 = "5/20",
					answer3 = "10/50",
					answer4 = "1/5",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What state was once known as the Sandwich Islands?",
					answer1 = "Hawaii",
					answer2 = "Bahamas",
					answer3 = "Florida",
					answer4 = "Rhode Island",
					answerId = "1",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Who was Madonna’s first husband?",
					answer1 = "Guy Ritchie",
					answer2 = "Jason Donovan",
					answer3 = "Sean Penn",
					answer4 = "Nicolas Cage",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of the Following is a song by the band Queen??",
					answer1 = "Rock You",
					answer2 = "Bohemian Rhapsody",
					answer3 = "Don’t Stop Me Now",
					answer4 = "All of the Above",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of these is not a song by the band The Beatles?",
					answer1 = "Hard Days Night",
					answer2 = "Getting Better",
					answer3 = "Revolutionary",
					answer4 = "Rain",
					answerId = "3",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Produced by the brain, what natural painkiller is three times stronger than morphine?",
					answer1 = "Cadmium",
					answer2 = "Dopamine",
					answer3 = "Serotonin",
					answer4 = "Endorphin",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What animal is not a Chinese year?",
					answer1 = "Dolphin",
					answer2 = "Monkey",
					answer3 = "Dog",
					answer4 = "None of these",
					answerId = "1",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Angela Merkel is head of state from which country?",
					answer1 = "Angola",
					answer2 = "Germany",
					answer3 = "Russia",
					answer4 = "Saudi Arabia",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the state stone of Michigan?",
					answer1 = "Rose quartz",
					answer2 = "Petosky",
					answer3 = "Marble",
					answer4 = "Amethyst",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which is the largest cat species?",
					answer1 = "Cheetah",
					answer2 = "Puma",
					answer3 = "Tiger",
					answer4 = "Lion",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "An ECG (or EKG) measures the activity of the...?",
					answer1 = "intestine",
					answer2 = "brain",
					answer3 = "heart",
					answer4 = "lungs",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which vowel is not typically found on the top row of letters on a keyboard?",
					answer1 = "A",
					answer2 = "I",
					answer3 = "E",
					answer4 = "O",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Who wrote the best selling children’s book series, Redwall?",
					answer1 = "George R R Martin",
					answer2 = "J K Rowling",
					answer3 = "Brian Jacques",
					answer4 = "Roald Dahl",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the young of a tiger called?",
					answer1 = "Puppy",
					answer2 = "Fawn",
					answer3 = "Calf",
					answer4 = "Cub",
					answerId = "4",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the name of the famous music festival which took place in New York state in 1969?",
					answer1 = "summer fest",
					answer2 = "Woodstock",
					answer3 = "love parade",
					answer4 = "Pinkpop",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is a group of geese called?",
					answer1 = "Giggle",
					answer2 = "Band",
					answer3 = "Herd",
					answer4 = "Gaggle",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What European city can you find the “Mouth of Truth”?",
					answer1 = "Paris",
					answer2 = "Rome",
					answer3 = "Dublin",
					answer4 = "London",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What country has the most livestock?",
					answer1 = "Brazil",
					answer2 = "United States",
					answer3 = "Japan",
					answer4 = "India",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "The female fox is known as a ____?",
					answer1 = "Vixen",
					answer2 = "Jill",
					answer3 = "Doe",
					answer4 = "Joe",
					answerId = "1",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which unit of speed is used in maritime and air navigation?",
					answer1 = "loop",
					answer2 = "rope",
					answer3 = "knot",
					answer4 = "band",
					answerId = "3",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the Argentinean national beverage?",
					answer1 = "white coffee",
					answer2 = "wheat beer",
					answer3 = "cherry coke",
					answer4 = "mate tea",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "The olm belongs to which animal class?",
					answer1 = "Amphibia",
					answer2 = "Synapsida",
					answer3 = "Reptilia",
					answer4 = "Aves",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What country drinks the most coffee?",
					answer1 = "United States",
					answer2 = "United Kingdom",
					answer3 = "Japan",
					answer4 = "Netherlands",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the sound made by pigeons called?",
					answer1 = "Cluck",
					answer2 = "Coo",
					answer3 = "Chirp",
					answer4 = "Quack",
					answerId = "2",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which singer died in July 2011 at only 27 years of age?",
					answer1 = "Joss Stone",
					answer2 = "Amy Winehouse",
					answer3 = "Duffy",
					answer4 = "Alicia Keys",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which is the largest bird?",
					answer1 = "King Penguin",
					answer2 = "Ostrich",
					answer3 = "Emperor Penguin",
					answer4 = "None of the above",
					answerId = "2",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "The sound of a wolf is called?",
					answer1 = "Yelp",
					answer2 = "Howl",
					answer3 = "Grunt",
					answer4 = "Roar",
					answerId = "2",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What mountain range does the world’s highest mountain belong to?",
					answer1 = "Andes",
					answer2 = "Rocky Mountains",
					answer3 = "Himalayas",
					answer4 = "Alps",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is an invertebrate?",
					answer1 = "Animals with 4 legs",
					answer2 = "Animals with 6 legs",
					answer3 = "Animals with no backbone",
					answer4 = "Animals with a backbone",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of these cereals is not made from corn?",
					answer1 = "Raisin Bran",
					answer2 = "Honey Bunches of Oats",
					answer3 = "Frosted Flakes",
					answer4 = "Nature’s Path",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Who, as of 2014, has not won an Oscar?",
					answer1 = "Tom Hanks",
					answer2 = "Matthew McConaughey",
					answer3 = "Leonardo Dicaprio",
					answer4 = "Cate Blanchett",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What do puffins eat?",
					answer1 = "Big fish and zooplankton",
					answer2 = "Small fish and zooplankton",
					answer3 = "Turtle eggs",
					answer4 = "Crabs",
					answerId = "2",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Who played the role of the Godfather in the 1972 film of the same title?",
					answer1 = "Laurence Olivier",
					answer2 = "Frank Sinatra",
					answer3 = "Marlon Brando",
					answer4 = "James Dean",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Among sailors it is considered unlucky to kill which bird?",
					answer1 = "Pelican",
					answer2 = "Eagle",
					answer3 = "Vulture",
					answer4 = "Albatross",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the second fastest land animal?",
					answer1 = "Zebra",
					answer2 = "Horse",
					answer3 = "Greyhound",
					answer4 = "Jaguar",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which country has the highest population?",
					answer1 = "India",
					answer2 = "China",
					answer3 = "United States",
					answer4 = "Japan",
					answerId = "2",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What does ‘www’ stand for?",
					answer1 = "world wide wire",
					answer2 = "world wide water",
					answer3 = "world wide web",
					answer4 = "world wide wait",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the memory-span of a goldfish?",
					answer1 = "3 months",
					answer2 = "30 days",
					answer3 = "3 minutes",
					answer4 = "30 seconds",
					answerId = "1",
				},


				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Who’s on second?",
					answer1 = "Who",
					answer2 = "What",
					answer3 = "I don’t know",
					answer4 = "None of the above",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which bird can be found in most Chinese restaurants?",
					answer1 = "Shanghai Chicken",
					answer2 = "Peking Duck",
					answer3 = "Tianjin Turkey",
					answer4 = "Guizhou Goose",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How can we identify one gorilla from another?",
					answer1 = "Paw",
					answer2 = "Eyes",
					answer3 = "Nose-print",
					answer4 = "Hair",
					answerId = "3",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How many zeros are in the number one billion (American Short Scale)?",
					answer1 = "15",
					answer2 = "12",
					answer3 = "9",
					answer4 = "6",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which American Kennel Club Dog Group is the Golden Retriever classified in?",
					answer1 = "Sporting",
					answer2 = "Working",
					answer3 = "Toy",
					answer4 = "Non-Sporting",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the correct term for a cat with extra toes?",
					answer1 = "Polycarpal",
					answer2 = "Metatarsal",
					answer3 = "Polydactyl",
					answer4 = "Pterodactyl",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How many states composed the Confederate States of America?",
					answer1 = "11",
					answer2 = "9",
					answer3 = "10",
					answer4 = "12",
					answerId = "1",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which animal does hibernate?",
					answer1 = "wild boar",
					answer2 = "brown hare",
					answer3 = "arctic fox",
					answer4 = "European hedgehog",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which country has the largest land area?",
					answer1 = "United States",
					answer2 = "China",
					answer3 = "Russia",
					answer4 = "Canada",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the correct classification for Salamander?",
					answer1 = "Reptile",
					answer2 = "Amphibian",
					answer3 = "Aquatic",
					answer4 = "Terrestrial",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which British monarch had the shortest reign?",
					answer1 = "James II",
					answer2 = "Lady Jane Grey",
					answer3 = "Edward the Confessor",
					answer4 = "William I",
					answerId = "2",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Where does an Islamic muezzin call the people to prayer from?",
					answer1 = "town hall",
					answer2 = "stadium",
					answer3 = "minaret",
					answer4 = "steeple",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "In what measurement is the height of a horse calculated?",
					answer1 = "Feet",
					answer2 = "Inches",
					answer3 = "Hands",
					answer4 = "Yards",
					answerId = "3",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which country is reigned by a communist party?",
					answer1 = "China",
					answer2 = "Germany",
					answer3 = "Australia",
					answer4 = "Japan",
					answerId = "1",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Every atomic nucleus consists of at least...?",
					answer1 = "professors",
					answer2 = "protestants",
					answer3 = "protons",
					answer4 = "prototypes",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the original Gaelic name for Halloween?",
					answer1 = "kalan goañv",
					answer2 = "Samhain",
					answer3 = "Kalan Gwav",
					answer4 = "Calan Gaeaf",
					answerId = "2",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which country is located directly west of Argentina?",
					answer1 = "Brazil",
					answer2 = "Peru",
					answer3 = "Venezuela",
					answer4 = "Chile",
					answerId = "4",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What does the \"white dove\" symbol mean?",
					answer1 = "peace",
					answer2 = "pharmacy",
					answer3 = "progress",
					answer4 = "airport",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Who invented the first printing press for sheet music?",
					answer1 = "Petrucci",
					answer2 = "Marchiori",
					answer3 = "Handel",
					answer4 = "Gutenberg",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What numeral system is most commonly used today?",
					answer1 = "Roman",
					answer2 = "Chinese",
					answer3 = "Arabic",
					answer4 = "English",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "As of 2012, what is the most played PC game?",
					answer1 = "League of Legends",
					answer2 = "Team Fortress 2",
					answer3 = "Half Life",
					answer4 = "Super Mario Brothers",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the common voltage used in European homes?",
					answer1 = "350V",
					answer2 = "110V",
					answer3 = "400V",
					answer4 = "230V",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which ancient Wonder of the World was found in Giza?",
					answer1 = "Light house of Alexandria",
					answer2 = "Colossus of Rhodes",
					answer3 = "Great Pyramids",
					answer4 = "Temple of Artemis",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What are tomatoes considered?",
					answer1 = "Fruit",
					answer2 = "Vegetable",
					answer3 = "A weed",
					answer4 = "Protein",
					answerId = "1",
				},



				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which government has the longest history?",
					answer1 = "China",
					answer2 = "United State",
					answer3 = "Japan",
					answer4 = "United Kingdom",
					answerId = "3",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the largest planet in the solar system?",
					answer1 = "Saturn",
					answer2 = "Earth",
					answer3 = "Jupiter",
					answer4 = "Uranus",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How long did the Wright Brothers’ first flight last?",
					answer1 = "12 seconds",
					answer2 = "2 seconds",
					answer3 = "59 seconds",
					answer4 = "12 minutes",
					answerId = "3",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "A tangent touches a circle at… ?",
					answer1 = "just one point",
					answer2 = "no point at all",
					answer3 = "three points",
					answer4 = "exactly two points",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of these countries does not have the colors red, white, and blue on their flag?",
					answer1 = "Norway",
					answer2 = "France",
					answer3 = "Poland",
					answer4 = "Netherlands",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Under what name was Marvel Comics founded?",
					answer1 = "Time Comics",
					answer2 = "Marvelous Times",
					answer3 = "Atlas Comics",
					answer4 = "Timely Publications",
					answerId = "4",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Where is Tunisia located?",
					answer1 = "Asia",
					answer2 = "Africa",
					answer3 = "South America",
					answer4 = "Europe",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "On which date did the hit tv show ‘Doctor Who’ premiere?",
					answer1 = "August 28, 1961",
					answer2 = "February 9, 1970",
					answer3 = "October 12, 1966",
					answer4 = "November 23, 1963",
					answerId = "4",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What does the acronym BMI stand for?",
					answer1 = "body mass index",
					answer2 = "booty move instructor",
					answer3 = "Belgian milk icon",
					answer4 = "Bombay Military Institute",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What food is the Netherlands known for?",
					answer1 = "Nutella",
					answer2 = "Stroopwafel",
					answer3 = "Flan",
					answer4 = "Kielbasa",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What town was the birthplace of Elvis Presley?",
					answer1 = "St. Louis, Missouri",
					answer2 = "New Orleans, Louisiana",
					answer3 = "Memphis, Tennessee",
					answer4 = "Tupelo, Mississippi",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "A bronze statue of whom stands at the foot of the steps to the Philadelphia Art Museum?",
					answer1 = "Robocop",
					answer2 = "Jimmy Carter",
					answer3 = "Ron Jeremy",
					answer4 = "Rocky Balboa",
					answerId = "4",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What does a seismometer measure?",
					answer1 = "pressure",
					answer2 = "rainfall",
					answer3 = "ocean current",
					answer4 = "vibration",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the name of the creator of the show ‘The Simpsons’?",
					answer1 = "Dave Berry",
					answer2 = "Matt Groening",
					answer3 = "James Barrows",
					answer4 = "Charles Schulz",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Radicchio tastes ...?",
					answer1 = "sweet",
					answer2 = "bitter",
					answer3 = "sour",
					answer4 = "salty",
					answerId = "2",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Besides Manhattan, Brooklyn, Queens and the Bronx; what is the fifth borough of New York?",
					answer1 = "Staten Island",
					answer2 = "Roosevelt Island",
					answer3 = "Oak Island",
					answer4 = "Liberty Island",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "’Drop It Like It’s Hot’ is a hit for which recording artist?",
					answer1 = "Snoop Dogg ft Pharrell",
					answer2 = "Mario",
					answer3 = "Breaking Benjamin",
					answer4 = "Natalie",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How many Pokémon were in the first generation games for Nintendo?",
					answer1 = "100",
					answer2 = "200",
					answer3 = "151",
					answer4 = "251",
					answerId = "3",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is NOT needed to win a lottery?",
					answer1 = "lottery stake",
					answer2 = "lottery ticket",
					answer3 = "brainpower",
					answer4 = "luck",
					answerId = "3",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is a typical ingredient of a stew?",
					answer1 = "bananas",
					answer2 = "potatoes",
					answer3 = "coconuts",
					answer4 = "rhubarb",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the minimum amount of words for a book to be considered a novel?",
					answer1 = " 20,000 words",
					answer2 = "17,500 words",
					answer3 = "40,000 words",
					answer4 = "50,000 words",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "In which city is the movie ‘2 Fast 2 Furious’ set in?",
					answer1 = "Miami, FL",
					answer2 = "Orlando, FL",
					answer3 = "Los Angeles, CA",
					answer4 = "San Francisco, CA",
					answerId = "1",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "In what country is authentic mozzarella cheese produced?",
					answer1 = "Switzerland",
					answer2 = "Belgium",
					answer3 = "France",
					answer4 = "Italy",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How much is the original price of 1st generation 8G iphone?",
					answer1 = "$499",
					answer2 = "$599",
					answer3 = "$699",
					answer4 = "$799",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What was the first album of the alternative band Bush?",
					answer1 = "Sixteen Stone",
					answer2 = "Razorblade Suitcase",
					answer3 = "Flesh and Blood",
					answer4 = "The Great Divide",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the most uncommon eye color in humans?",
					answer1 = "Brown",
					answer2 = "Blue",
					answer3 = "Hazel",
					answer4 = "Green",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Movie question: Ellen Ripley doesn’t like ...?",
					answer1 = "zombies",
					answer2 = "gremlins",
					answer3 = "aliens",
					answer4 = "police officers",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the name of the main character in The Legend of Zelda video game series?",
					answer1 = "Zelda",
					answer2 = "Link",
					answer3 = "Ling",
					answer4 = "Malon",
					answerId = "2",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of our senses’ formal name is \"gustation\"?",
					answer1 = "Smell",
					answer2 = "Taste",
					answer3 = "Touch",
					answer4 = "Sight",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What cool is the 1-ball in a game of pool?",
					answer1 = "Red",
					answer2 = "Blue",
					answer3 = "Yellow",
					answer4 = "Green",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "In the animated tv series ‘The Simpsons’, what is Grampa Simpson’s first name?",
					answer1 = "George",
					answer2 = "Barney",
					answer3 = "Lenny",
					answer4 = "Abe",
					answerId = "4",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the flavor of the Turkish national drink, Raki?",
					answer1 = "Pistachio",
					answer2 = "Orange",
					answer3 = "Anise",
					answer4 = "Coconut",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of these is a potato variety?",
					answer1 = "Golden Piper",
					answer2 = "Maris Piper",
					answer3 = "Brushed Maris",
					answer4 = "Golden Maris",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is May’s birthstone?",
					answer1 = "Emerald",
					answer2 = "Ruby",
					answer3 = "Sapphire",
					answer4 = "Onyx",
					answerId = "1",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Why color does not belong to the CMYK color model?",
					answer1 = "cyan",
					answer2 = "yellow",
					answer3 = "cobalt",
					answer4 = "magenta",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of these is not a type of apple?",
					answer1 = "Fuji",
					answer2 = "Granny Smith",
					answer3 = "McIntosh",
					answer4 = "Honeycrunch",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "In which year did the first Del Taco open?",
					answer1 = "2000",
					answer2 = "1992",
					answer3 = "1964",
					answer4 = "1954",
					answerId = "3",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Who introduced the Glasnost policy in the former Soviet Union?",
					answer1 = "Leonid Brezhnev",
					answer2 = "Vladimir Putin",
					answer3 = "Boris Yeltsin",
					answer4 = "Mikhail Gorbachev",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Chocolate is made from what type of bean?",
					answer1 = "Cocoa",
					answer2 = "Black",
					answer3 = "Hershey’s",
					answer4 = "Green",
					answerId = "1",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is often used as a Christmas tree?",
					answer1 = "fir tree",
					answer2 = "oak tree",
					answer3 = "lime tree",
					answer4 = "birch tree",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "The first Starbucks outside of North America was opened in what country?",
					answer1 = "China",
					answer2 = "Japan",
					answer3 = "Netherlands",
					answer4 = "Australia",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How many holes are in the original WIFFLE ball?",
					answer1 = "5",
					answer2 = "8",
					answer3 = "10",
					answer4 = "12",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What video game failure made by Atari was buried in New Mexico?",
					answer1 = "Sonic the Hedgehog",
					answer2 = "E.T.the Extra-Terrestrial",
					answer3 = "Asteroids",
					answer4 = "Big Bird’s Egg Catch",
					answerId = "2",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of the Seven Wonders of the World can still be visited nowadays?",
					answer1 = "Statue of Zues at Olympia",
					answer2 = "Lighthouse of Alexandria",
					answer3 = "Colossus of Rhodes",
					answer4 = "Great Pyramid of Giza",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How many is in a baker’s dozen?",
					answer1 = "11",
					answer2 = "13",
					answer3 = "12",
					answer4 = "14",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Who is the author of the book ‘The Exorcist’?",
					answer1 = "David H. Keller",
					answer2 = "William P. Blatty",
					answer3 = "Robert Fleming",
					answer4 = "James Kisner",
					answerId = "2",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How many bones do sharks have in their bodies?",
					answer1 = "99",
					answer2 = "5",
					answer3 = "none",
					answer4 = "312",
					answerId = "3",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What beverage is not sparkling?",
					answer1 = "Sprite",
					answer2 = "Fanta",
					answer3 = "Orange Juice",
					answer4 = "Coca Cola",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is a perfect score in bowling?",
					answer1 = "200",
					answer2 = "100",
					answer3 = "150",
					answer4 = "300",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Cumulus and stratus are types of what?",
					answer1 = "Tornados",
					answer2 = "Snowflake",
					answer3 = "Waves",
					answer4 = "Clouds",
					answerId = "4",
				},


				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which city is the first Starbucks located?",
					answer1 = "San Francisco",
					answer2 = "New York",
					answer3 = "Detroit",
					answer4 = "Seattle",
					answerId = "4",
				},


				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What animal smells like popcorn?",
					answer1 = "Capybara",
					answer2 = "Binturong",
					answer3 = "Tapir",
					answer4 = "Koala",
					answerId = "2",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is not a handcraft?",
					answer1 = "Sewing",
					answer2 = "Knitting",
					answer3 = "Puzzling",
					answer4 = "Crocheting",
					answerId = "3",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the capital of Spain?",
					answer1 = "Bilbao",
					answer2 = "Valencia",
					answer3 = "Barcelona",
					answer4 = "Madrid",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Omphalophobia is the fear of what?",
					answer1 = "Over-eating",
					answer2 = "Touching ones navel",
					answer3 = "Balloons popping",
					answer4 = "Elephants",
					answerId = "2",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which is an ingredient of most cakes?",
					answer1 = "lettuce",
					answer2 = "mustard",
					answer3 = "pepper",
					answer4 = "flour",
					answerId = "4",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How high hangs the basket in modern basketball?",
					answer1 = "12 feet (3.65 meters)",
					answer2 = "8 feet (2.44 meters)",
					answer3 = "15 feet (4.75 meters)",
					answer4 = "10 feet (3.05 meters)",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What U.S. state has the most miles of coastline?",
					answer1 = "Hawaii",
					answer2 = "Texas",
					answer3 = "California",
					answer4 = "Alaska",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What ocean is on Peru’s western shore?",
					answer1 = "Arctic",
					answer2 = "Pacific",
					answer3 = "Indian",
					answer4 = "Atlantic",
					answerId = "2",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What food or cake is typical for Christmas?",
					answer1 = "gingerbread",
					answer2 = "ice cream",
					answer3 = "jelly beans",
					answer4 = "blueberry muffins",
					answerId = "1",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which city was John F. Kennedy murdered in?",
					answer1 = "Chicago",
					answer2 = "Dallas",
					answer3 = "Las Vegas",
					answer4 = "Seattle",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "In which year did Bill Gates co-found Microsoft with Paul Allen?",
					answer1 = "1970",
					answer2 = "1992",
					answer3 = "1985",
					answer4 = "1975",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of these characters was not voiced by Tim Curry?",
					answer1 = "Nigel Thornberry",
					answer2 = "Hexxus",
					answer3 = "Sharkster",
					answer4 = "King Almond",
					answerId = "4",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Who is married to Antonio Banderas?",
					answer1 = "Nicole Kidman",
					answer2 = "Meg Ryan",
					answer3 = "Teri Hatcher",
					answer4 = "Melanie Griffith",
					answerId = "4",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which animal is the heaviest?",
					answer1 = "sheep",
					answer2 = "mouse",
					answer3 = "buffalo",
					answer4 = "rabbit",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which airline company is also known as AA?",
					answer1 = "American Airlines",
					answer2 = "Alaska Airlines",
					answer3 = "None of the above",
					answer4 = "Armenian Airlines",
					answerId = "1",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What species was the first cloned mammal?",
					answer1 = "a cat",
					answer2 = "a sheep",
					answer3 = "a monkey",
					answer4 = "a rat",
					answerId = "2",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of the following is a broadleaf tree?",
					answer1 = "pine",
					answer2 = "oak",
					answer3 = "fir",
					answer4 = "spruce",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "A hinny is a cross between which two animals?",
					answer1 = "Horse and Donkey",
					answer2 = "Cow and Donkey",
					answer3 = "Cow and Sheep",
					answer4 = "Horse and Monkey",
					answerId = "1",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which animal is NOT an omnivore?",
					answer1 = "pig",
					answer2 = "rat",
					answer3 = "wolf",
					answer4 = "raven",
					answerId = "3",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is 50% of 72?",
					answer1 = "70",
					answer2 = "36",
					answer3 = "50",
					answer4 = "34",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Who wrote the books Riders, Polo and Wicked?",
					answer1 = "Jilly Cooper",
					answer2 = "Salman Rushdie",
					answer3 = "Vikram Seth",
					answer4 = "J.K. Rowling",
					answerId = "1",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of these cities is not located on the coast?",
					answer1 = "New York",
					answer2 = "Sao Paulo",
					answer3 = "Hongkong",
					answer4 = "Cape Town",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is 40% of 40?",
					answer1 = "16",
					answer2 = "8",
					answer3 = "20",
					answer4 = "10",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of these is not a Blizzard game?",
					answer1 = "World of Warcraft",
					answer2 = "Starcraft",
					answer3 = "Minecraft",
					answer4 = "Hearthstone",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of the following foods is the best source of calcium?",
					answer1 = "Chicken",
					answer2 = "Bread",
					answer3 = "Banana",
					answer4 = "Yogurt",
					answerId = "4",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How do you say ‘hello’ and ‘goodbye’ in Italian?",
					answer1 = "Scusi!",
					answer2 = "Ciao!",
					answer3 = "Salut!",
					answer4 = "Adios!",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the largest joint in the human body?",
					answer1 = "Elbow Joint",
					answer2 = "Ankle Joint",
					answer3 = "Thumb Joint",
					answer4 = "Knee Joint",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Who is Jennifer Garner married to?",
					answer1 = "Ben Affleck",
					answer2 = "Brad Pitt",
					answer3 = "Matthew Broderick",
					answer4 = "Johnny Depp",
					answerId = "1",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which gas is produced by rotting vegetation?",
					answer1 = "Methane",
					answer2 = "Carbon Monoxide",
					answer3 = "Nitrogen",
					answer4 = "Oxygen",
					answerId = "1",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Who voices Philoctetes in the 1997 movie \"Hercules\"?",
					answer1 = "Danny DeVito",
					answer2 = "Mark Hamill",
					answer3 = "Tim Curry",
					answer4 = "James Earl Jones",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the most expensive breed of dog?",
					answer1 = "Bull Mastiff",
					answer2 = "Great Dane",
					answer3 = "French Bulldog",
					answer4 = "Samoyed",
					answerId = "4",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "The ‘death spiral’ is an element of which sport?",
					answer1 = "Artistic Gymnastics",
					answer2 = "Horse-Riding",
					answer3 = "Pair Skating",
					answer4 = "Diving",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "On ‘Psych’, what is Gus’s other job?",
					answer1 = "Vacuum Salesman",
					answer2 = "Pharmaceutical Representitive",
					answer3 = "Insurance Salesman",
					answer4 = "Lawyer",
					answerId = "2",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "The organic compound hydrocarbon is the main component of ...?",
					answer1 = "Brown coal",
					answer2 = "Salt water",
					answer3 = "Crude oil",
					answer4 = "Stainless Steel",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of the following acts did NOT appear at the Woodstock Festival in 1968?",
					answer1 = "Plastic Penny",
					answer2 = "Joni Mitchell",
					answer3 = "The Pretty Things",
					answer4 = "Jefferson Airplane",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Who played the role of Buttercup in the film ‘The Princess Bride’?",
					answer1 = "Michelle Pfiefer",
					answer2 = "Robin Wright",
					answer3 = "Elizabeth Banks",
					answer4 = "Madonna",
					answerId = "2",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "In which sport is it not allowed to play with the left hand?",
					answer1 = "Golf",
					answer2 = "Cricket",
					answer3 = "Table Tennis",
					answer4 = "Polo",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Foosball was invented by who?",
					answer1 = "Louis P. Thornton",
					answer2 = "Harold Thornton",
					answer3 = "Lee Peppard",
					answer4 = "Harry Peppard",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which band member of Metallica died in a bus crash?",
					answer1 = "James Newsted",
					answer2 = "Lars Ulrich",
					answer3 = "Cliff Burton",
					answer4 = "James Hetfield",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which is NOT a vehicle?",
					answer1 = "carport",
					answer2 = "cabriolet",
					answer3 = "trick",
					answer4 = "van",
					answerId = "1",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "When did Christopher Columbus rediscover America?",
					answer1 = "1834",
					answer2 = "1299",
					answer3 = "1789",
					answer4 = "1492",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Chunk and his friends go on a wild treasure hunt in what movie?",
					answer1 = "The Emperor’s Secret",
					answer2 = "The Great Discovery",
					answer3 = "Body Troopers",
					answer4 = "The Goonies",
					answerId = "4",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What year was Eli Broad born?",
					answer1 = "1933",
					answer2 = "1953",
					answer3 = "1926",
					answer4 = "1949",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "When did Snoopy make his first appear in the comic strip ‘Peanuts’?",
					answer1 = "May 23, 1950",
					answer2 = "October 4, 1950",
					answer3 = "October 2, 1950",
					answer4 = "December 24, 1950",
					answerId = "2",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of these animals is not on the state flag for Michigan?",
					answer1 = "Eagle",
					answer2 = "Elk",
					answer3 = "Moose",
					answer4 = "Deer",
					answerId = "4",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the biggest bird of Prey on Earth?",
					answer1 = "Golden Eagle",
					answer2 = "Bald Eagle",
					answer3 = "White-Tailed Eagle",
					answer4 = "Andean Condor",
					answerId = "4",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "On a standard keyboard, which key is the largest?",
					answer1 = "The caps lock button",
					answer2 = "The enter button",
					answer3 = "The backspace button",
					answer4 = "The space button",
					answerId = "4",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "At the start of a game of chess, you would find 16 of which of the pieces?",
					answer1 = "Rook",
					answer2 = "Bishop",
					answer3 = "Queen",
					answer4 = "Pawn",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "In ‘Family Guy’, what is the name of the bar built by Peter Griffin?",
					answer1 = "Peter’s Bar",
					answer2 = "My Old Pub",
					answer3 = "Brandt’s Creek",
					answer4 = "Ye Olde Pube",
					answerId = "4",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the capital of Afghanistan?",
					answer1 = "Cairo",
					answer2 = "Beirut",
					answer3 = "Islamabad",
					answer4 = "Kabul",
					answerId = "4",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What was the original capital of Japan?",
					answer1 = "Osaka",
					answer2 = "Shinjuku",
					answer3 = "Kyoto",
					answer4 = "Narita",
					answerId = "3",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What do plants release during photosynthesis?",
					answer1 = "Water",
					answer2 = "Salt",
					answer3 = "Oxygen",
					answer4 = "Chlorine",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Who is Wilma Flintstone’s best friend?",
					answer1 = "Peral Slaghoople",
					answer2 = "Mrs. Slate",
					answer3 = "Gattu Rubble",
					answer4 = "Betty Rubble",
					answerId = "4",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is indispensable for the process of photosynthesis?",
					answer1 = "Oxygen",
					answer2 = "Earthworms",
					answer3 = "Light",
					answer4 = "Bees",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "The hit ‘Every Rose Has Its Thorn’ was sung by which lead singer?",
					answer1 = "Axl Rose",
					answer2 = "Bret Michaels",
					answer3 = "Vince Neil",
					answer4 = "David Lee Roth",
					answerId = "2",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the name of the Olympian goddess of love and beauty?",
					answer1 = "Urania",
					answer2 = "Hera",
					answer3 = "Aphrodite",
					answer4 = "Athena",
					answerId = "3",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What is the atomic number of the chemical element gold?",
					answer1 = "79",
					answer2 = "34",
					answer3 = "97",
					answer4 = "50",
					answerId = "1",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "How does a Spaniard say \"I love you\"?",
					answer1 = "Te Envidio",
					answer2 = "Te Ablando",
					answer3 = "Te Amo",
					answer4 = "Te Extrano",
					answerId = "3",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Who is a well-known entrepreneur?",
					answer1 = "Richard Branson",
					answer2 = "Natalie Portman",
					answer3 = "Albert Einstein",
					answer4 = "Bob Marley",
					answerId = "1",
				},

				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What’s the name of the pizza joint in film ‘Toy Story’?",
					answer1 = "Pizza Hut",
					answer2 = "Rocket Pizza",
					answer3 = "Planet Pizza",
					answer4 = "Pizza Planet",
					answerId = "4",
				},
				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "What causes the disease cholera?",
					answer1 = "Bacteria",
					answer2 = "All of these",
					answer3 = "Virus",
					answer4 = "Fungus",
					answerId = "1",
				},


				new wireQuestion()
				{
					id = Guid.NewGuid(),
					question = "Which of the following is from America?",
					answer1 = "Potato",
					answer2 = "Tomato",
					answer3 = "Carrot",
					answer4 = "Watermelon",
					answerId = "1",
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
