namespace SocialNetwork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Friendship1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Friendships",
                c => new
                    {
                        applicantId = c.Guid(nullable: false),
                        aimPersonId = c.Guid(nullable: false),
                        friendshipAccepted = c.Boolean(nullable: false),
                        applyDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.applicantId, t.aimPersonId });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Friendships");
        }
    }
}
