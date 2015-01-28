namespace BugTracker.Migrations
{
    using BugTracker.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            //This is a way of populating the database throught code
            if (!context.Roles.Any(r => r.Name == "Admin"))//The Lambda searches for a Name in Roles that equals "Admin"
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Project Manager"))//The Lambda searches for a Name in Roles that equals "Admin"
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }

            if (!context.Roles.Any(r => r.Name == "Developer"))//The Lambda searches for a Name in Roles that equals "Admin"
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }

            if (!context.Roles.Any(r => r.Name == "Submitter"))//The Lambda searches for a Name in Roles that equals "Admin"
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }


            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            //Add my self user as Administrator and Moderator
            if (!context.Users.Any(r => r.Email == "rasikrodri@gmail.com"))//The Lambda searches for a Name in Roles that equals "Admin"
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "rasikrodri@gmail.com",
                    Email = "rasikrodri@gmail.com",
                    FirstName = "Rasik",
                    LastName = "Rodriguez",
                    DisplayName = "Rasik Rodriguez",
                }, "rodr1ras1k");
            }

            //create a new BTUser entry to match the AspNetUsers entry
            var userId = userManager.FindByEmail("rasikrodri@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");
        }
    }
}
