namespace SOD.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PostMigration8 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SodBlanketApprovals", "IsActive", c => c.Short(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SodBlanketApprovals", "IsActive", c => c.Boolean(nullable: false));
        }
    }
}
