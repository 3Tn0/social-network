namespace SocialNetwork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BaseEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        CommentId = c.Guid(nullable: false),
                        CommentDate = c.DateTime(nullable: false),
                        Text = c.String(nullable: false),
                        PostId = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.CommentId);
            
            CreateTable(
                "dbo.Editors",
                c => new
                    {
                        EditorRightsId = c.Guid(nullable: false),
                        AppointmentDate = c.DateTime(nullable: false),
                        CancellationDate = c.DateTime(),
                        CommunityId = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.EditorRightsId);
            
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        PostId = c.Guid(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        Text = c.String(nullable: false),
                        RightsId = c.Guid(nullable: false),
                        CommunityId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.PostId);
            
            CreateTable(
                "dbo.Subscriptions",
                c => new
                    {
                        SubscriptionId = c.Guid(nullable: false),
                        SubscriptionDate = c.DateTime(nullable: false),
                        SubscriptionCancelationDate = c.DateTime(),
                        CommunityId = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.SubscriptionId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Subscriptions");
            DropTable("dbo.Posts");
            DropTable("dbo.Editors");
            DropTable("dbo.Comments");
        }
    }
}
