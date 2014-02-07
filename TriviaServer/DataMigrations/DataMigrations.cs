using System;
using System.Data.Entity.Migrations;
using MySql.Data.Entity;


namespace TriviaServer
{
	internal sealed class DataMigrations : DbMigrationsConfiguration<DataContext>
	{
		public DataMigrations()
		{
			AutomaticMigrationsEnabled = true;

			SetSqlGenerator("MySql.Data.MySqlClient", new MySqlMigrationSqlGenerator());
		}

		protected override void Seed(TriviaServer.DataContext context)
		{
			//WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
		}
	}
}

