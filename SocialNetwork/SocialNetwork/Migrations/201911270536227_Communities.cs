namespace SocialNetwork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Communities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Communities",
                c => new
                    {
                        CommunityId = c.Guid(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        Name = c.String(nullable: false),
                        UserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.CommunityId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Communities");
        }
    }
}
