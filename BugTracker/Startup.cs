using Microsoft.Owin;
using Owin;
using System.Data.Entity.Migrations;

[assembly: OwinStartupAttribute(typeof(BugTracker.Startup))]
namespace BugTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();

            //var configuration = new Migrations.Configuration();
            //var migrator = new DbMigrator(configuration);
            //migrator.Update();
        }
    }
}
