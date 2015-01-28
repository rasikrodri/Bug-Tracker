using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class BugTrackerModels
    {
        public partial class Project
        {
            public Project()
            {
                this.ProjectUsers = new HashSet<ProjectUser>();
                this.Tickets = new HashSet<Ticket>();
            }

            public int Id { get; set; }
            public string Name { get; set; }

            public virtual ICollection<ProjectUser> ProjectUsers { get; set; }
            public virtual ICollection<Ticket> Tickets { get; set; }
        }
        public partial class ProjectUser
        {
            public int Id { get; set; }
            public int ProjectId { get; set; }
            public int UserId { get; set; }

            public virtual ApplicationUser User { get; set; }
            public virtual Project Project { get; set; }
        }
        public partial class Ticket
        {
            public Ticket()
            {
                this.TicketAttachements = new HashSet<TicketAttachement>();
                this.TicketComments = new HashSet<TicketComment>();
                this.TicketHistories = new HashSet<TicketHistory>();
                this.TicketNotifications = new HashSet<TicketNotification>();
            }

            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public System.DateTimeOffset Created { get; set; }
            public Nullable<System.DateTimeOffset> Updated { get; set; }
            public int ProjectId { get; set; }
            public int TicketTypeId { get; set; }
            public Nullable<int> TicketPriorityId { get; set; }
            public int TicketStatusId { get; set; }
            public int OwnerUserId { get; set; }
            public Nullable<int> AssignedToUserId { get; set; }

            public virtual ApplicationUser OwnerUser { get; set; }
            public virtual ApplicationUser Assigneduser { get; set; }
            public virtual Project Project { get; set; }
            public virtual ICollection<TicketAttachement> TicketAttachements { get; set; }
            public virtual ICollection<TicketComment> TicketComments { get; set; }
            public virtual ICollection<TicketHistory> TicketHistories { get; set; }
            public virtual ICollection<TicketNotification> TicketNotifications { get; set; }
            public virtual TicketPriority TicketPriority { get; set; }
            public virtual TicketStatus TicketStatus { get; set; }
            public virtual TicketType TicketType { get; set; }
        }
        public partial class TicketAttachement
        {
            public int id { get; set; }
            public int TicketId { get; set; }
            public string FilePath { get; set; }
            public string Description { get; set; }
            public System.DateTimeOffset Created { get; set; }
            public int UserId { get; set; }
            public string FileUrl { get; set; }

            public virtual ApplicationUser User { get; set; }
            public virtual Ticket Ticket { get; set; }
        }
        public partial class TicketComment
        {
            public int Id { get; set; }
            public string Comment { get; set; }
            public System.DateTimeOffset Created { get; set; }
            public int TicketId { get; set; }
            public int UserId { get; set; }

            public virtual ApplicationUser User { get; set; }
            public virtual Ticket Ticket { get; set; }
        }
        public partial class TicketHistory
        {
            public int Id { get; set; }
            public int TicketId { get; set; }
            public string Property { get; set; }
            public string OldValue { get; set; }
            public string NewValue { get; set; }
            public Nullable<System.DateTimeOffset> Changed { get; set; }
            public int UserId { get; set; }

            public virtual ApplicationUser User { get; set; }
            public virtual Ticket Ticket { get; set; }
        }
        public partial class TicketNotification
        {
            public int Id { get; set; }
            public int TicketId { get; set; }
            public int UserId { get; set; }

            public virtual ApplicationUser User { get; set; }
            public virtual Ticket Ticket { get; set; }
        }
        public partial class TicketPriority
        {
            public TicketPriority()
            {
                this.Tickets = new HashSet<Ticket>();
            }

            public int Id { get; set; }
            public string Name { get; set; }

            public virtual ICollection<Ticket> Tickets { get; set; }
        }
        public partial class TicketStatus
        {
            public TicketStatus()
            {
                this.Tickets = new HashSet<Ticket>();
            }

            public int id { get; set; }
            public string Name { get; set; }

            public virtual ICollection<Ticket> Tickets { get; set; }
        }
        public partial class TicketType
        {
            public TicketType()
            {
                this.Tickets = new HashSet<Ticket>();
            }

            public int Id { get; set; }
            public string Name { get; set; }

            public virtual ICollection<Ticket> Tickets { get; set; }
        }
    }
}