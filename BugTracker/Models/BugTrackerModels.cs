using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace BugTracker.Models
{
    [NotMapped]
    public class Search
    {
        public MultiSelectList searchFilters { get; set; }

        public string[] selectedSearchFilters { get; set; }
    }

    public class UserRole
    {
        public IdentityRole Role { get; set; }

        public MultiSelectList subscribedUsers { get; set; }

        public string[] selectedSubscribedUsersId { get; set; }

        public MultiSelectList noneSubscribedUsers { get; set; }

        public string[] selecteNoneSubscribedUsers { get; set; }
    }

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

        [NotMapped]
        public MultiSelectList subscribedUsers { get; set; }

        [NotMapped]
        public string[] selectedSubscribedUsersId { get; set; }

        [NotMapped]
        public MultiSelectList noneSubscribedUsers { get; set; }

        [NotMapped]
        public string[] selecteNoneSubscribedUsers { get; set; }  
    }
    public partial class ProjectUser
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Project Project { get; set; }
    }

    public partial class Ticket    
    {
        [NotMapped]
        ApplicationDbContext db = new ApplicationDbContext();
        public Ticket()
        {
            this.TicketAttachments = new HashSet<TicketAttachement>();
            this.TicketComments = new HashSet<TicketComment>();
            //this.Histories = new HashSet<GenericHistory>();
            //this.Notifications = new HashSet<GenericNotification>();
        }

        public int Id { get; set; }
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Created")]
        public System.DateTimeOffset Created { get; set; }
        public Nullable<System.DateTimeOffset> Updated { get; set; }

        
        public int ProjectId { get; set; }
        public int TicketTypeId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketStatusId { get; set; }

        public string OwnerUserId { get; set; }

        
        [Required]
        [AllowHtml]
        [Display(Name = "Project")]
        public string Description { get; set; }

        [Display(Name = "Project")]
        public virtual Project Project { get; set; }

        [Display(Name = "Ticket Type")]
        public virtual TicketType TicketType { get; set; }

        [Display(Name = "Priority")]
        public virtual TicketPriority TicketPriority { get; set; }

        [Display(Name = "Status")]
        public virtual TicketStatus TicketStatus { get; set; }

        [Display(Name = "Ticket Creator")]
        public virtual ApplicationUser OwnerUser { get; set; }

        [Display(Name = "Asigned To")]
        public virtual ApplicationUser AssignedToUser { get; set; }

        public string AssignedToUserId { get; set; }

        public virtual ICollection<TicketAttachement> TicketAttachments { get; set; }
        public virtual ICollection<TicketComment> TicketComments { get; set; }
        //public virtual ICollection<GenericHistory> Histories { get; set; }
        //public virtual ICollection<GenericNotification> Notifications { get; set; }        
    }


    public partial class TicketAttachement 
    {
        public int id { get; set; }        
        public int TicketId { get; set; }

        [AllowHtml]
        public string Description { get; set; }   
        public System.DateTimeOffset Created { get; set; }
        public string UserId { get; set; }

        public string FilesPath { get; set; }
        public string FilesUrl { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
    public partial class TicketComment
    {
        public int Id { get; set; }
       
        public System.DateTimeOffset Created { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }

        [Required]
        [AllowHtml]
        public string Description { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
    public partial class GenericHistory
    {
        public int Id { get; set; }
        public string WhoMadeChangesId { get; set; }
        public virtual ApplicationUser WhoMadeChanges { get; set; }

        public byte[] Values { get; set; }

        

        /// <summary>
        /// Is going to contain the foreign key of the object
        /// </summary>
        public string ObjectType { get; set; }
        public int ObjectId { get; set; }
        public Nullable<System.DateTimeOffset> Changed { get; set; }             
    }

    /// <summary>
    /// Use this model for any object that needs notification
    /// </summary>
    public partial class GenericNotification
    {
        public int Id { get; set; }
        public int ObjectId { get; set; }
        public string ObjectType { get; set; }

        [Required]
        public string Title { get; set; }

        public string LinkToObjectDetails { get; set; }

        public string FromUserId { get; set; }
        public virtual ApplicationUser FromUser { get; set; }

        public System.DateTimeOffset Created { get; set; }


        public string ToUserId { get; set; }
        /// <summary>
        /// To flag if the notification was allready read
        /// The notification Icon will show the amount of notifications that have not been read
        /// </summary>
        public bool Read { get; set; }

        [Required]
        [AllowHtml]
        public string Message { get; set; }
        public virtual ApplicationUser ToUser { get; set; }
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