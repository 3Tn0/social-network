namespace SocialNetwork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Chatadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Chats",
                c => new
                    {
                        sendingDate = c.DateTime(nullable: false),
                        senderId = c.Guid(nullable: false),
                        receiverId = c.Guid(nullable: false),
                        messageText = c.String(nullable: false),
                    })
                .PrimaryKey(t => new { t.sendingDate, t.senderId, t.receiverId });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Chats");
        }
    }
}
