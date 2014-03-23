namespace TriviaServer
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "LevelScores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Score = c.Int(nullable: false),
                        Statistics_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Statistics", t => t.Statistics_Id)
                .Index(t => t.Statistics_Id);
            
            AddColumn("Statistics", "NodesProgress", c => c.Int(nullable: false));
            AddColumn("Statistics", "SectionProgress", c => c.Int(nullable: false));
            AlterColumn("Users", "Stats_Id", c => c.Int(nullable: false));
            DropColumn("Statistics", "Progress");
        }
        
        public override void Down()
        {
            AddColumn("Statistics", "Progress", c => c.Int(nullable: false));
            DropIndex("LevelScores", new[] { "Statistics_Id" });
            DropForeignKey("LevelScores", "Statistics_Id", "Statistics");
            AlterColumn("Users", "Stats_Id", c => c.Int());
            DropColumn("Statistics", "SectionProgress");
            DropColumn("Statistics", "NodesProgress");
            DropTable("LevelScores");
        }
    }
}
