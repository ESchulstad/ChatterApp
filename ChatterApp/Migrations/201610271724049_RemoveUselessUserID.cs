namespace ChatterApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUselessUserID : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "UserID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "UserID", c => c.Int());
        }
    }
}
