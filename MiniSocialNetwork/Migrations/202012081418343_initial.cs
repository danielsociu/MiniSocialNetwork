namespace MiniSocialNetwork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Comments", "Group_GroupId", "dbo.Groups");
            DropIndex("dbo.Comments", new[] { "Group_GroupId" });
            DropIndex("dbo.Groups", new[] { "User_Id" });
            DropColumn("dbo.Groups", "CreatorId");
            RenameColumn(table: "dbo.Groups", name: "User_Id", newName: "CreatorId");
            AlterColumn("dbo.Groups", "CreatorId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Groups", "CreatorId");
            DropColumn("dbo.Comments", "Group_GroupId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Comments", "Group_GroupId", c => c.Int());
            DropIndex("dbo.Groups", new[] { "CreatorId" });
            AlterColumn("dbo.Groups", "CreatorId", c => c.String(nullable: false));
            RenameColumn(table: "dbo.Groups", name: "CreatorId", newName: "User_Id");
            AddColumn("dbo.Groups", "CreatorId", c => c.String(nullable: false));
            CreateIndex("dbo.Groups", "User_Id");
            CreateIndex("dbo.Comments", "Group_GroupId");
            AddForeignKey("dbo.Comments", "Group_GroupId", "dbo.Groups", "GroupId");
        }
    }
}
