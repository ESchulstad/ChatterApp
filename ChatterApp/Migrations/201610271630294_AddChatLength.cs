namespace ChatterApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChatLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Chats", "Content", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Chats", "Content", c => c.String());
        }
    }
}
