using System;
using System.Data.Entity;

namespace TriviaServer
{
	public class DataContext : DbContext
	{
		public DbSet<User> users { get; set; }

		public DbSet<Statistics> statistics { get; set; }

		public DbSet<Collectible> collectibles { get; set; }

		public DbSet<Achievement> achievements { get; set; }

		public DataContext ()
		{
		}
	}
}

