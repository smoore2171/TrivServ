namespace TriviaServer
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Users",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FacebookId = c.String(unicode: false),
                        Email = c.String(unicode: false),
                        Stats_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Statistics", t => t.Stats_Id)
                .Index(t => t.Stats_Id);
            
            CreateTable(
                "Statistics",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Progress = c.Int(nullable: false),
                        QuestionsCorrect = c.Int(nullable: false),
                        QuestionsTotal = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Achievements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        Statistics_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Statistics", t => t.Statistics_Id)
                .Index(t => t.Statistics_Id);
            
            CreateTable(
                "Collectibles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        Statistics_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Statistics", t => t.Statistics_Id)
                .Index(t => t.Statistics_Id);
            
        }
        
        public override void Down()
        {
            DropIndex("Collectibles", new[] { "Statistics_Id" });
            DropIndex("Achievements", new[] { "Statistics_Id" });
            DropIndex("Users", new[] { "Stats_Id" });
            DropForeignKey("Collectibles", "Statistics_Id", "Statistics");
            DropForeignKey("Achievements", "Statistics_Id", "Statistics");
            DropForeignKey("Users", "Stats_Id", "Statistics");
            DropTable("Collectibles");
            DropTable("Achievements");
            DropTable("Statistics");
            DropTable("Users");
        }
    }
}
