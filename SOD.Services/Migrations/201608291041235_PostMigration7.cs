namespace SOD.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PostMigration7 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SodApprovers", "IsActive", c => c.Short(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SodApprovers", "IsActive", c => c.Boolean(nullable: false));
        }
    }
}
