using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Data.Entity.Migrations.Design;
using System.IO;
using System.Resources;
using System.Data.Entity.Migrations;

namespace TriviaServer
{
	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterRoutes (RouteCollection routes)
		{
			routes.IgnoreRoute ("{resource}.axd/{*pathInfo}");

			routes.MapRoute (
				"Default",
				"{controller}/{action}/{id}",
				new { controller = "Home", action = "Index", id = "" }
			);

		}

		public static void RegisterGlobalFilters (GlobalFilterCollection filters)
		{
			filters.Add (new HandleErrorAttribute ());
		}

		protected void Application_Start ()
		{
			// role and db actions 
			//AddMigrations(1);
			//UpdateDatabase ();
			//return;
			//Roles.CreateRole ("User");
			//Roles.CreateRole ("Admin");

			// debugging roles

			AreaRegistration.RegisterAllAreas ();
			RegisterGlobalFilters (GlobalFilters.Filters);
			RegisterRoutes (RouteTable.Routes);
		}


		public void ClearUsers()
		{
			foreach(MembershipUser user in Membership.GetAllUsers())
			{
				Membership.DeleteUser (user.UserName);
			}
		}

		public void AddMigrations(int i)
		{
			var config = new DataMigrations();
			var scaffolder = new MigrationScaffolder(config);
			var migration = scaffolder.Scaffold("Migration"+i);

			File.WriteAllText("Migrations/"+migration.MigrationId + ".cs", migration.UserCode);

			File.WriteAllText("Migrations/"+migration.MigrationId + ".Designer.cs", migration.DesignerCode);

			using (var writer = new ResXResourceWriter("Migrations/"+migration.MigrationId + ".resx"))
			{
				foreach (var resource in migration.Resources)
				{
					writer.AddResource(resource.Key, resource.Value);
				}
			}
		}

		public void UpdateDatabase()
		{
			/*			var configuration = new DataMigrations();
			configuration.TargetDatabase = new DbConnectionInfo (
				"DataContext");

				//	"Server=.\\SQLEXPRESS;Database=CriminalProcedure;Trusted_Connection=True;", 
				//"System.Data.SqlClient");
			var migrator = new DbMigrator(configuration);
			migrator.Update();*/
			var configuration = new DataMigrations();

			var migrator = new DbMigrator(configuration);

			migrator.Update();
		}
	}
}
