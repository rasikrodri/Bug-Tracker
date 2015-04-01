namespace BugTracker.Migrations
{
    using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
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
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();


            //Add Priorities
            if (!context.TicketPriorities.Any(t => t.Name == "Normal"))
            { context.TicketPriorities.Add(new TicketPriority() { Name = "Normal" }); }
            if (!context.TicketPriorities.Any(t => t.Name == "Heigh"))
            { context.TicketPriorities.Add(new TicketPriority() { Name = "Heigh" }); }
            if (!context.TicketPriorities.Any(t => t.Name == "Low"))
            { context.TicketPriorities.Add(new TicketPriority() { Name = "Low" }); }
            if (!context.TicketPriorities.Any(t => t.Name == "Very Low"))
            { context.TicketPriorities.Add(new TicketPriority() { Name = "Very Low" }); }
            if (!context.TicketPriorities.Any(t => t.Name == "Very Heigh"))
            { context.TicketPriorities.Add(new TicketPriority() { Name = "Very Heigh" }); }

            //Add Type
            if (!context.TicketTypes.Any(t => t.Name == "Maintenance"))
            { context.TicketTypes.Add(new TicketType() { Name = "Maintenance" }); }
            if (!context.TicketTypes.Any(t => t.Name == "Develop"))
            { context.TicketTypes.Add(new TicketType() { Name = "Develope" }); }
            if (!context.TicketTypes.Any(t => t.Name == "Brain Storm"))
            { context.TicketTypes.Add(new TicketType() { Name = "Brain Storm" }); }

            //Add status
            if (!context.TicketStatus.Any(t => t.Name == "Open"))
            { context.TicketStatus.Add(new TicketStatus() { Name = "Open" }); }
            if (!context.TicketStatus.Any(t => t.Name == "Processing"))
            { context.TicketStatus.Add(new TicketStatus() { Name = "Processing" }); }
            if (!context.TicketStatus.Any(t => t.Name == "Canceled"))
            { context.TicketStatus.Add(new TicketStatus() { Name = "Canceled" }); }
            if (!context.TicketStatus.Any(t => t.Name == "Closed"))
            { context.TicketStatus.Add(new TicketStatus() { Name = "Closed" }); }


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
            if (!context.Roles.Any(r => r.Name == "Guest"))//The Lambda searches for a Name in Roles that equals "Admin"
            {
                roleManager.Create(new IdentityRole { Name = "Guest" });
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
            userManager.AddToRole(userId, "Project Manager");
            userManager.AddToRole(userId, "Developer");
            userManager.AddToRole(userId, "Submitter");

            //Add a guest User
            if (!context.Users.Any(r => r.Email == "guest@gmail.com"))//The Lambda searches for a Name in Roles that equals "Admin"
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "guest@gmail.com",
                    Email = "guest@gmail.com",
                    FirstName = "Guest",
                    LastName = "",
                    DisplayName = "Guest",
                }, "rodr1ras1k");
            }
            userId = userManager.FindByEmail("guest@gmail.com").Id;
            userManager.AddToRole(userId, "Guest");
            userManager.AddToRole(userId, "Admin");
            userManager.AddToRole(userId, "Project Manager");
            userManager.AddToRole(userId, "Developer");
            userManager.AddToRole(userId, "Submitter");


            //create a new BTUser entry to match the AspNetUsers entry
            userId = userManager.FindByEmail("guest@gmail.com").Id;
            userManager.AddToRole(userId, "Project Manager");
            userManager.AddToRole(userId, "Developer");
            userManager.AddToRole(userId, "Submitter");
            userManager.AddToRole(userId, "Guest");

            //Add 3 Project Managers
            CreateSpecificUserAndAsignRole("James", "Pmanager", "Project Manager, Developer, Submitter", context, userManager);
            CreateSpecificUserAndAsignRole("John", "Pmanager", "Project Manager, Developer, Submitter", context, userManager);
            CreateSpecificUserAndAsignRole("Gina", "Pmanager", "Project Manager, Developer, Submitter", context, userManager);

            //Add 5 Developers
            CreateSpecificUserAndAsignRole("Robert", "Developer", "Developer, Submitter", context, userManager);
            CreateSpecificUserAndAsignRole("Michael", "Developer", "Developer, Submitter", context, userManager);
            CreateSpecificUserAndAsignRole("William", "Developer", "Developer, Submitter", context, userManager);
            CreateSpecificUserAndAsignRole("David", "Developer", "Developer, Submitter", context, userManager);
            CreateSpecificUserAndAsignRole("Richard", "Developer", "Developer, Submitter", context, userManager);

            //Add 10 Developers
            CreateSpecificUserAndAsignRole("Charles", "Submitter", "Submitter", context, userManager);
            CreateSpecificUserAndAsignRole("Joseph", "Submitter", "Submitter", context, userManager);
            CreateSpecificUserAndAsignRole("Thomas", "Submitter", "Submitter", context, userManager);
            CreateSpecificUserAndAsignRole("Christopher", "Submitter", "Submitter", context, userManager);
            CreateSpecificUserAndAsignRole("Daniel", "Submitter", "Submitter", context, userManager);
            CreateSpecificUserAndAsignRole("Paul", "Submitter", "Submitter", context, userManager);
            CreateSpecificUserAndAsignRole("Mark", "Submitter", "Submitter", context, userManager);
            CreateSpecificUserAndAsignRole("Donald", "Submitter", "Submitter", context, userManager);
            CreateSpecificUserAndAsignRole("George", "Submitter", "Submitter", context, userManager);
            CreateSpecificUserAndAsignRole("Steven", "Submitter", "Submitter", context, userManager);

            //Create Projects
            DateTimeOffset created =  new DateTimeOffset(2008, 5, 1, 8, 6, 32, 
                                 new TimeSpan(1, 0, 0));
            var projectUsers = new List<ApplicationUser>();
            projectUsers.Add(context.Users.First(u => u.FirstName == "Robert"));
            projectUsers.Add(context.Users.First(u => u.FirstName == "Michael"));
            CreateProject_Tickets_Comments_AsignUsers("Asparagus Emotions Probe", projectUsers, 10,
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce posuere libero est, eget eleifend erat vulputate vel. Mauris maximus eros eu risus suscipit tincidunt. Cras eros elit, ullamcorper nec lacinia sed, pellentesque non sapien. In interdum vulputate dolor nec scelerisque. Duis eu odio ut massa viverra finibus vel id risus. Duis vitae risus malesuada nisi commodo aliquet. Praesent varius elit at lorem suscipit venenatis. Interdum et malesuada fames ac ante ipsum primis in faucibus. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos." +
                "Etiam vel congue tortor, quis finibus urna. Aliquam erat volutpat. In elementum, massa id ornare tristique, diam tortor mattis dui, vitae tincidunt nisl eros eu nunc. Etiam id congue libero. Phasellus efficitur imperdiet lorem quis malesuada. Etiam purus tellus, facilisis vitae placerat et, cursus id ligula. Aenean vel justo ac urna consectetur ultricies. Aenean tempus arcu eu vestibulum consequat. Vivamus ut est semper, pulvinar nunc ut, interdum orci.",
                "Very Heigh", "Maintenance", "Processing",
                context.Users.First(u => u.FirstName == "Robert"),
                context.Users.First(u => u.FirstName == "Charles"),
               created,
                CreateComments(4, created, projectUsers),
                null,
                context
                );

            created = new DateTimeOffset(2009,1, 17, 10, 6, 32,
                                 new TimeSpan(1, 0, 0));
            projectUsers = new List<ApplicationUser>();
            projectUsers.Add(context.Users.First(u => u.FirstName == "William"));
            projectUsers.Add(context.Users.First(u => u.FirstName == "David"));
            projectUsers.Add(context.Users.First(u => u.FirstName == "Richard"));
            CreateProject_Tickets_Comments_AsignUsers("Thunder Bird", projectUsers, 10,
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce posuere libero est, eget eleifend erat vulputate vel. Mauris maximus eros eu risus suscipit tincidunt. Cras eros elit, ullamcorper nec lacinia sed, pellentesque non sapien. In interdum vulputate dolor nec scelerisque. Duis eu odio ut massa viverra finibus vel id risus. Duis vitae risus malesuada nisi commodo aliquet. Praesent varius elit at lorem suscipit venenatis. Interdum et malesuada fames ac ante ipsum primis in faucibus. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos." +
                "Etiam vel congue tortor, quis finibus urna. Aliquam erat volutpat. In elementum, massa id ornare tristique, diam tortor mattis dui, vitae tincidunt nisl eros eu nunc. Etiam id congue libero. Phasellus efficitur imperdiet lorem quis malesuada. Etiam purus tellus, facilisis vitae placerat et, cursus id ligula. Aenean vel justo ac urna consectetur ultricies. Aenean tempus arcu eu vestibulum consequat. Vivamus ut est semper, pulvinar nunc ut, interdum orci.",
                "Very Heigh", "Maintenance", "Processing",
                context.Users.First(u => u.FirstName == "Robert"),
                context.Users.First(u => u.FirstName == "Charles"),
               created,
                CreateComments(4, created, projectUsers),
                null,
                context
                );
            
        }
        private List<TicketComment> CreateComments(int _howMany, DateTimeOffset _createdForm, List<ApplicationUser> _users)
        {
            var rand = new Random();
            var randUsers = new Random();
            var timeRand = new Random();
            List<DateTimeOffset> dtoList = new List<DateTimeOffset>();
            //Create the times of the comments
            for(int a=0;a<_howMany;a++)
            {
                dtoList.Add(new DateTimeOffset(_createdForm.Year, _createdForm.Month, timeRand.Next(_createdForm.Day, 27), timeRand.Next(_createdForm.Hour, 13),
                    timeRand.Next(_createdForm.Minute, 60), 32, new TimeSpan(1, 0, 0)));
            }
            //Orginize by time
            dtoList.Sort((a,b)=>DateTimeOffset.Compare(a,b));

            var comments = new List<TicketComment>();
            for(int i=0;i<_howMany;i++)
            {
                var comm = new TicketComment();

                var user = _users[randUsers.Next(0, _users.Count)];

                comments.Add(new TicketComment() { 
                    Description = ChooseComments(rand.Next(0,9)),
                    Created = dtoList[i],
                    User = user,
                    UserId = user.Id
                });
            }

            return comments;
        }
        private string ChooseComments(int _num)
        {
            switch(_num)
            {
                case 0:
                    return "Etiam vel congue tortor, quis finibus urna. Aliquam erat volutpat. In elementum, massa id ornare tristique, diam tortor mattis dui, vitae tincidunt nisl eros eu nunc. Etiam id congue libero. Phasellus efficitur imperdiet lorem quis malesuada. Etiam purus tellus, facilisis vitae placerat et, cursus id ligula. Aenean vel justo ac urna consectetur ultricies. Aenean tempus arcu eu vestibulum consequat. Vivamus ut est semper, pulvinar nunc ut, interdum orci.";
                case 1:
                    return "Etiam vel congue tortor, quis finibus urna. Aliquam erat volutpat. In elementum, massa id ornare tristique, diam tortor mattis dui, vitae tincidunt nisl eros eu nunc. Etiam id congue libero. Phasellus efficitur imperdiet lorem quis malesuada. Etiam purus tellus, facilisis vitae placerat et, cursus id ligula. Aenean vel justo ac urna consectetur ultricies. Aenean tempus arcu eu vestibulum consequat. Vivamus ut est semper, pulvinar nunc ut, interdum orci.";
                case 2:
                    return "Etiam vel congue tortor, quis finibus urna. Aliquam erat volutpat. In elementum, massa id ornare tristique, diam tortor mattis dui, vitae tincidunt nisl eros eu nunc. Etiam id congue libero. Phasellus efficitur imperdiet lorem quis malesuada. Etiam purus tellus, facilisis vitae placerat et, cursus id ligula. Aenean vel justo ac urna consectetur ultricies. Aenean tempus arcu eu vestibulum consequat. Vivamus ut est semper, pulvinar nunc ut, interdum orci.";
                case 3:
                    return "Etiam vel congue tortor, quis finibus urna. Aliquam erat volutpat. In elementum, massa id ornare tristique, diam tortor mattis dui, vitae tincidunt nisl eros eu nunc. Etiam id congue libero. Phasellus efficitur imperdiet lorem quis malesuada. Etiam purus tellus, facilisis vitae placerat et, cursus id ligula. Aenean vel justo ac urna consectetur ultricies. Aenean tempus arcu eu vestibulum consequat. Vivamus ut est semper, pulvinar nunc ut, interdum orci.";
                case 4:
                    return "Etiam vel congue tortor, quis finibus urna. Aliquam erat volutpat. In elementum, massa id ornare tristique, diam tortor mattis dui, vitae tincidunt nisl eros eu nunc. Etiam id congue libero. Phasellus efficitur imperdiet lorem quis malesuada. Etiam purus tellus, facilisis vitae placerat et, cursus id ligula. Aenean vel justo ac urna consectetur ultricies. Aenean tempus arcu eu vestibulum consequat. Vivamus ut est semper, pulvinar nunc ut, interdum orci.";
                case 5:
                    return "Etiam vel congue tortor, quis finibus urna. Aliquam erat volutpat. In elementum, massa id ornare tristique, diam tortor mattis dui, vitae tincidunt nisl eros eu nunc. Etiam id congue libero. Phasellus efficitur imperdiet lorem quis malesuada. Etiam purus tellus, facilisis vitae placerat et, cursus id ligula. Aenean vel justo ac urna consectetur ultricies. Aenean tempus arcu eu vestibulum consequat. Vivamus ut est semper, pulvinar nunc ut, interdum orci.";
                case 6:
                    return "Etiam vel congue tortor, quis finibus urna. Aliquam erat volutpat. In elementum, massa id ornare tristique, diam tortor mattis dui, vitae tincidunt nisl eros eu nunc. Etiam id congue libero. Phasellus efficitur imperdiet lorem quis malesuada. Etiam purus tellus, facilisis vitae placerat et, cursus id ligula. Aenean vel justo ac urna consectetur ultricies. Aenean tempus arcu eu vestibulum consequat. Vivamus ut est semper, pulvinar nunc ut, interdum orci.";
                case 7:
                    return "Etiam vel congue tortor, quis finibus urna. Aliquam erat volutpat. In elementum, massa id ornare tristique, diam tortor mattis dui, vitae tincidunt nisl eros eu nunc. Etiam id congue libero. Phasellus efficitur imperdiet lorem quis malesuada. Etiam purus tellus, facilisis vitae placerat et, cursus id ligula. Aenean vel justo ac urna consectetur ultricies. Aenean tempus arcu eu vestibulum consequat. Vivamus ut est semper, pulvinar nunc ut, interdum orci.";
                case 8:
                    return "Etiam vel congue tortor, quis finibus urna. Aliquam erat volutpat. In elementum, massa id ornare tristique, diam tortor mattis dui, vitae tincidunt nisl eros eu nunc. Etiam id congue libero. Phasellus efficitur imperdiet lorem quis malesuada. Etiam purus tellus, facilisis vitae placerat et, cursus id ligula. Aenean vel justo ac urna consectetur ultricies. Aenean tempus arcu eu vestibulum consequat. Vivamus ut est semper, pulvinar nunc ut, interdum orci.";
                case 9:
                    return "Etiam vel congue tortor, quis finibus urna. Aliquam erat volutpat. In elementum, massa id ornare tristique, diam tortor mattis dui, vitae tincidunt nisl eros eu nunc. Etiam id congue libero. Phasellus efficitur imperdiet lorem quis malesuada. Etiam purus tellus, facilisis vitae placerat et, cursus id ligula. Aenean vel justo ac urna consectetur ultricies. Aenean tempus arcu eu vestibulum consequat. Vivamus ut est semper, pulvinar nunc ut, interdum orci.";
                default:
                    return "Etiam vel congue tortor, quis finibus urna. Aliquam erat volutpat. In elementum, massa id ornare tristique, diam tortor mattis dui, vitae tincidunt nisl eros eu nunc. Etiam id congue libero. Phasellus efficitur imperdiet lorem quis malesuada. Etiam purus tellus, facilisis vitae placerat et, cursus id ligula. Aenean vel justo ac urna consectetur ultricies. Aenean tempus arcu eu vestibulum consequat. Vivamus ut est semper, pulvinar nunc ut, interdum orci.";
            }
        }

        private void CreateExtraUsers(BugTracker.Models.ApplicationDbContext context, UserManager<ApplicationUser> userManager, int _projectmanagers, int _developers, int _submitters)
        {
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();

            //Add 50 users as "Submitter"
            for (int i = 0; i < 50; i++)
            {
                string name = "Submitter." + i.ToString();
                string email = name.ToLower() + "@gmail.com";
                if (!context.Users.Any(r => r.Email == email))//The Lambda searches for a Name in Roles that equals "Admin"
                {
                    userManager.Create(new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FirstName = name,
                        LastName = "Anonimus",
                        DisplayName = name + " Anonimus",
                    }, "123456");
                }

                //create a new BTUser entry to match the AspNetUsers entry
                var u = userManager.FindByEmail(email).Id;
                userManager.AddToRole(u, "Submitter");
            }

            //Add 20 users as "Developer"
            for (int i = 0; i < 20; i++)
            {
                string name = "Developer." + i.ToString();
                string email = name.ToLower() + "@gmail.com";
                if (!context.Users.Any(r => r.Email == email))//The Lambda searches for a Name in Roles that equals "Admin"
                {
                    userManager.Create(new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FirstName = name,
                        LastName = "Anonimus",
                        DisplayName = name + " Anonimus",
                    }, "123456");
                }

                //create a new BTUser entry to match the AspNetUsers entry
                var u = userManager.FindByEmail(email).Id;
                userManager.AddToRole(u, "Developer");
            }

            //Add 10 users as "Project Manager"
            for (int i = 0; i < 10; i++)
            {
                string name = "Project.Manager." + i.ToString();
                string email = name.ToLower() + "@gmail.com";
                if (!context.Users.Any(r => r.Email == email))//The Lambda searches for a Name in Roles that equals "Admin"
                {
                    userManager.Create(new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FirstName = name,
                        LastName = "Anonimus",
                        DisplayName = name + " Anonimus",
                    }, "rodr1ras1k");
                }

                //create a new BTUser entry to match the AspNetUsers entry
                var u = userManager.FindByEmail(email).Id;
                userManager.AddToRole(u, "Project Manager");
            }
        }
        private void CreateSpecificUserAndAsignRole(string _firstName, string _lastName, string _Roles, ApplicationDbContext context, UserManager<ApplicationUser> _userManager)
        {
            if (!context.Users.Any(u => u.Email == _firstName + "@gmail.com"))
            { 
                _userManager.Create(new ApplicationUser
                {
                    UserName = _firstName + "@gmail.com",
                    Email = _firstName + "@gmail.com",
                    FirstName = _firstName,
                    LastName = _lastName,
                    DisplayName = _firstName + " " + _lastName,
                }, "123456");

                //Add the user to a Role
                string[] roles = _Roles.Split(',');
                var userId = _userManager.FindByEmail(_firstName + "@gmail.com").Id;
                foreach(var role in roles)
                    _userManager.AddToRole(userId, role.Trim());
            }
        }
        private void CreateProject_Tickets_Comments_AsignUsers(string _projectName, List<ApplicationUser> _projectUsers, int _amoutTickets, string _ticketsDescription,
            string _priority, string _type, string _status, ApplicationUser _assignee, ApplicationUser creator, DateTimeOffset _createdOn, List<TicketComment> _comments,
            TicketAttachement[] _attachements ,ApplicationDbContext _context)
        {
            //Create the project 
            var project = _context.Projects.FirstOrDefault(p => p.Name == _projectName);
            if(project == null)
            {
                project = new Project() { Name = _projectName};
                _context.Projects.Add(project);

                if (project.ProjectUsers.Count == 0)
                {
                    foreach (var pu in _projectUsers)
                        project.ProjectUsers.Add(new ProjectUser() { User = pu, UserId = pu.Id, Project = project, ProjectId = project.Id });
                }


                //Create the ticket
                var tic = new Random();
                List<string> names = new List<string>();
                names.Add("It crashes when saving");
                names.Add("The porgram crashes whn loading");
                names.Add("The list of clients apears empty");
                names.Add("All text desappears when I select a new item");
                names.Add("The application appears in black and white");
                names.Add("It crashes when loading a file");
                names.Add("Loading old files crashes the program");
                names.Add("The file menue desappears");
                names.Add("There is no save button");
                names.Add("If it crashes I loose all my work");
                names.Add("The program does not Load");
                List<string> templist = new List<string>();
                for (int c = 0; c < _amoutTickets; c++)
                {
                    var ticket = new Ticket();
                    _context.Tickets.Add(ticket);

                    project.Tickets.Add(ticket);

                    if(templist.Count()==0)
                        templist = names.ToList();
                    int ind = tic.Next(0, templist.Count);
                    ticket.Title = templist[ind];
                    templist.RemoveAt(ind);

                    ticket.Description = _ticketsDescription;
                    ticket.AssignedToUser = _assignee;
                    ticket.AssignedToUserId = _assignee.Id;

                    var priority = _context.TicketPriorities.First(p => p.Name == _priority);
                    ticket.TicketPriority = priority;
                    ticket.TicketPriorityId = priority.Id;
                    var type = _context.TicketTypes.First(p => p.Name == _type);
                    ticket.TicketType = type;
                    ticket.TicketTypeId = type.Id;
                    var status = _context.TicketStatus.First(p => p.Name == _status);
                    ticket.TicketStatus = status;
                    ticket.TicketStatusId = status.id;

                    ticket.Project = project;
                    ticket.ProjectId = project.Id;

                    ticket.OwnerUser = creator;
                    ticket.OwnerUserId = creator.Id;

                    ticket.Created = _createdOn;

                    ticket.AssignedToUser = _assignee;
                    ticket.AssignedToUserId = _assignee.Id;

                    //Add Comments
                    if (_comments != null)
                        foreach (var com in _comments)
                        {
                            _context.TicketComments.Add(com);
                            com.Ticket = ticket;
                            com.TicketId = ticket.Id;
                            ticket.TicketComments.Add(com);
                        }


                    //Add Attachements
                    if (_attachements != null)
                        foreach (var att in _attachements)
                        {
                            _context.TicketAttachements.Add(att);
                            att.Ticket = ticket;
                            att.TicketId = ticket.Id;
                            ticket.TicketAttachments.Add(att);
                        }
                }
            }
        }
    }
}
