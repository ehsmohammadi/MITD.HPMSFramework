namespace MITD.TestLayer.DataAccess.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ver0001 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TestEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TestProperty = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TestEntityItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TestEntityId = c.Int(nullable: false),
                        TestProperty = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TestEntities", t => t.TestEntityId, cascadeDelete: true)
                .Index(t => t.TestEntityId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.TestEntityItems", new[] { "TestEntityId" });
            DropForeignKey("dbo.TestEntityItems", "TestEntityId", "dbo.TestEntities");
            DropTable("dbo.TestEntityItems");
            DropTable("dbo.TestEntities");
        }
    }
}
