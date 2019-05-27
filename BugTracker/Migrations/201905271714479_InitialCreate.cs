namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GenericHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WhoMadeChangesId = c.String(maxLength: 128),
                        Values = c.Binary(),
                        ObjectType = c.String(),
                        ObjectId = c.Int(nullable: false),
                        Changed = c.DateTimeOffset(precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.WhoMadeChangesId)
                .Index(t => t.WhoMadeChangesId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Adress = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Zip = c.String(),
                        DisplayName = c.String(),
                        NewNotification = c.Int(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.GenericNotifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ObjectId = c.Int(nullable: false),
                        ObjectType = c.String(),
                        Title = c.String(nullable: false),
                        LinkToObjectDetails = c.String(),
                        FromUserId = c.String(maxLength: 128),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        ToUserId = c.String(maxLength: 128),
                        Read = c.Boolean(nullable: false),
                        Message = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.FromUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.ToUserId)
                .Index(t => t.FromUserId)
                .Index(t => t.ToUserId);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProjectUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.ProjectId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        Updated = c.DateTimeOffset(precision: 7),
                        ProjectId = c.Int(nullable: false),
                        TicketTypeId = c.Int(nullable: false),
                        TicketPriorityId = c.Int(nullable: false),
                        TicketStatusId = c.Int(nullable: false),
                        OwnerUserId = c.String(maxLength: 128),
                        Description = c.String(nullable: false),
                        AssignedToUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AssignedToUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerUserId)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .ForeignKey("dbo.TicketPriorities", t => t.TicketPriorityId, cascadeDelete: true)
                .ForeignKey("dbo.TicketStatus", t => t.TicketStatusId, cascadeDelete: true)
                .ForeignKey("dbo.TicketTypes", t => t.TicketTypeId, cascadeDelete: true)
                .Index(t => t.ProjectId)
                .Index(t => t.TicketTypeId)
                .Index(t => t.TicketPriorityId)
                .Index(t => t.TicketStatusId)
                .Index(t => t.OwnerUserId)
                .Index(t => t.AssignedToUserId);
            
            CreateTable(
                "dbo.TicketAttachements",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        TicketId = c.Int(nullable: false),
                        Description = c.String(),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        UserId = c.String(maxLength: 128),
                        FilesPath = c.String(),
                        FilesUrl = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.TicketId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.TicketComments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        TicketId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.TicketId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.TicketPriorities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TicketStatus",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.TicketTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Tickets", "TicketTypeId", "dbo.TicketTypes");
            DropForeignKey("dbo.Tickets", "TicketStatusId", "dbo.TicketStatus");
            DropForeignKey("dbo.Tickets", "TicketPriorityId", "dbo.TicketPriorities");
            DropForeignKey("dbo.TicketComments", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TicketComments", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.TicketAttachements", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TicketAttachements", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.Tickets", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Tickets", "OwnerUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tickets", "AssignedToUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProjectUsers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProjectUsers", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.GenericNotifications", "ToUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.GenericNotifications", "FromUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.GenericHistories", "WhoMadeChangesId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.TicketComments", new[] { "UserId" });
            DropIndex("dbo.TicketComments", new[] { "TicketId" });
            DropIndex("dbo.TicketAttachements", new[] { "UserId" });
            DropIndex("dbo.TicketAttachements", new[] { "TicketId" });
            DropIndex("dbo.Tickets", new[] { "AssignedToUserId" });
            DropIndex("dbo.Tickets", new[] { "OwnerUserId" });
            DropIndex("dbo.Tickets", new[] { "TicketStatusId" });
            DropIndex("dbo.Tickets", new[] { "TicketPriorityId" });
            DropIndex("dbo.Tickets", new[] { "TicketTypeId" });
            DropIndex("dbo.Tickets", new[] { "ProjectId" });
            DropIndex("dbo.ProjectUsers", new[] { "UserId" });
            DropIndex("dbo.ProjectUsers", new[] { "ProjectId" });
            DropIndex("dbo.GenericNotifications", new[] { "ToUserId" });
            DropIndex("dbo.GenericNotifications", new[] { "FromUserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.GenericHistories", new[] { "WhoMadeChangesId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.TicketTypes");
            DropTable("dbo.TicketStatus");
            DropTable("dbo.TicketPriorities");
            DropTable("dbo.TicketComments");
            DropTable("dbo.TicketAttachements");
            DropTable("dbo.Tickets");
            DropTable("dbo.ProjectUsers");
            DropTable("dbo.Projects");
            DropTable("dbo.GenericNotifications");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.GenericHistories");
        }
    }
}
