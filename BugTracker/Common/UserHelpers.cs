using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Security.Claims;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using System.Text;
using Blog;
using SendGrid;
using System.Net;
using System.Configuration;
using System.Diagnostics;

namespace BugTracker.Models
{
    public class UserHelper
    {
        /// <summary>
        /// Is the way to access Roles
        /// </summary>
        private UserManager<ApplicationUser> manager =
            new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(
                    new ApplicationDbContext()));

        ApplicationDbContext db = new ApplicationDbContext();

        public static string GetUserDisplayName(string userId)
        {
            var db = new ApplicationDbContext();
            return db.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId).DisplayName;
        }

        public bool IsUserInRole(string userId, string userRole)
        {
            return manager.IsInRole(userId, userRole);
        }

        public static string GetUserName(string userId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var user = db.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return "User does Not Exist";
            else
                return user.DisplayName;
        }

        public IList<string> UserRoles(string userId)
        {
           return manager.GetRoles(userId);
        }

        public bool AddUserToRole(string userId, string roleName)
        {            
            var result = manager.AddToRole(userId, roleName);
            return result.Succeeded;
        }

        public bool RemoveUserFromRole(string userId, string roleName)
        {
            var result = manager.RemoveFromRole(userId, roleName);
            return result.Succeeded;
        }

        public IList<ApplicationUser> UsersInRole(string roleName)
        {
            var db = new ApplicationDbContext(); 
            //or
            var resultList = new List<ApplicationUser>();
            foreach (var user in db.Users)
            {
                if(IsUserInRole(user.Id, roleName))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }

        public IList<ApplicationUser> UsersNotInRole(string roleName)
        {
            var db = new ApplicationDbContext(); 
            //or
            //var userList = manager.Users;//Gives error

            var resultList = new List<ApplicationUser>();
            foreach (var user in db.Users)
            {
                if (!IsUserInRole(user.Id, roleName))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }
    }

    public class UserProjectsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsOnProject(string userId, int projectId)
        {
            if (db.ProjectUsers.Any(p => p.ProjectId == projectId && p.UserId == userId))
                return true;

            return false;
        }

        public List<Project> ListUserProjects(string userId)
        {
           return db.Projects.Where(p => p.ProjectUsers.Any(u => u.UserId == userId)).ToList();
        }
        //public async Task<List<Project>> ListUserProjects(string userId)
        //{
        //    var results = new List<Project>();
        //    foreach (Project p in db.Projects)
        //        if (await IsOnProject(userId, p.Id))
        //            results.Add(p);

        //    return results;
        //}

        public bool AddUserToProject(string userId, int projectId)
        {
            if(!this.IsOnProject(userId, projectId))
            {
                var pu = new ProjectUser
                {
                    ProjectId = projectId,
                    UserId = userId,
                };
                db.ProjectUsers.Add(pu);
                db.SaveChanges();
                return true;
            }
            return false;
        }

        public bool RemoveProjectUserFromProject(string userId, int projectId)
        {
            if(this.IsOnProject(userId, projectId))
            {
                var pu = db.ProjectUsers.SingleAsync(p => p.ProjectId == projectId && p.UserId == userId);
                db.ProjectUsers.Remove(pu.Result);
                db.SaveChanges();
                return true;
            }
            return false;
        }

        public List<ApplicationUser> UsersOnProject(int projectId)
        {
            var results = new List<ApplicationUser>();

            foreach (var user in db.Users.ToList())//I had to set it to list because when published it causer the Mars Error
                if (IsOnProject(user.Id, projectId))
                    results.Add(user);

            return results;
        }
        /// <summary>
        /// Gets a list of ApplicationUser
        /// </summary>
        /// <param name="projectId">The integer Id of the specified project.</param>
        /// <param name="roleName">The Role</param>
        /// <returns>list of ApplicationUser that have the specified Role Name</returns>
        public List<ApplicationUser> UsersOnProject(int projectId, string roleName)
        {
            var results = new List<ApplicationUser>();
            var rolesHelper = new UserHelper();

            foreach (var user in db.Users)
                if (IsOnProject(user.Id, projectId) && 
                    rolesHelper.IsUserInRole(user.Id, roleName))
                    results.Add(user);

            return results;
        }

        public List<ApplicationUser> UsersNotOnProject(int projectId)
        {
            var results = new List<ApplicationUser>();

            foreach (var user in db.Users.ToList())//I had to set it to list because when published it causer the Mars Error
                if (!IsOnProject(user.Id, projectId))
                    results.Add(user);

            return results;
        }
        public List<ApplicationUser> UsersNotOnProject(int projectId, string roleName)
        {
            var results = new List<ApplicationUser>();
            var rolesHelper = new UserHelper();

            foreach (var user in db.Users)
                if (!IsOnProject(user.Id, projectId) &&
                    rolesHelper.IsUserInRole(user.Id, roleName))
                    results.Add(user);

            return results;
        }
    }

    public class UserNotificationsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        /// <summary>
        /// Returns the amount of notifications that the
        /// user has not read
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Dictionary<string, int> CountUnreadNotifications(string userId)
        {
            var db = new ApplicationDbContext();

            var allMessages = db.Notifications.AsNoTracking().Where(n => n.ToUserId == userId);
            var read = allMessages.Where(r => r.Read == true);
            var counts = new Dictionary<string, int>();
            counts.Add("all", allMessages.Count());
            counts.Add("read", read.Count());
            counts.Add("unread", allMessages.Count() - read.Count());

            return counts;
        }        

        public static void Notify_CreatedTicket(string userId, int ticketId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var notification = new GenericNotification();

            notification.Title = "Ticket Created";
            notification.Created = DateTimeOffset.Now;
            notification.Read = false;
            notification.LinkToObjectDetails = "~/Tickets/Details/" + ticketId.ToString();


            var tableEntry = db.Tickets.AsNoTracking().First(t => t.Id == ticketId);

            notification.ObjectType = db.Tickets.AsNoTracking().First(t => t.Id == ticketId).GetType().Name;
            notification.ObjectId = ticketId;
            notification.ToUserId = userId;
            notification.ToUser = db.Users.First(u => u.Id == userId);

            notification.Message = "You have Created Ticket#: " + ticketId + " " + 
                tableEntry.Title + ".";            

            db.Notifications.Add(notification);
            db.SaveChanges();
        }
        public static void Notify_ChangedTicket(string userId, int ticketId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var notification = new GenericNotification();

            notification.Title = "Ticket Changed";
            notification.Created = DateTimeOffset.Now;
            notification.Read = false;
            notification.LinkToObjectDetails = "~/Tickets/Details/" + ticketId.ToString();

            var tableEntry = db.Tickets.AsNoTracking().First(t => t.Id == ticketId);

            notification.ObjectType = db.Tickets.AsNoTracking().First(t => t.Id == ticketId).GetType().Name;
            notification.ObjectId = ticketId;
            notification.ToUserId = userId;
            notification.ToUser = db.Users.First(u => u.Id == userId);

            notification.Message = "Some changes were made in Ticket#: " + ticketId + " " +
                tableEntry.Title + ".";
            
            db.Notifications.Add(notification);
            db.SaveChanges();
        }
        public static void Notify_AsignedToTicket(string userId, int ticketId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var notification = new GenericNotification();

            notification.Title = "Asigned to Ticket";
            notification.Created = DateTimeOffset.Now;
            notification.Read = false;
            notification.LinkToObjectDetails = "~/Tickets/Details/" + ticketId.ToString();

            var tableEntry = db.Tickets.AsNoTracking().First(t => t.Id == ticketId);

            notification.ObjectType = db.Tickets.AsNoTracking().First(t => t.Id == ticketId).GetType().Name;
            notification.ObjectId = ticketId;
            notification.ToUserId = userId;
            notification.ToUser = db.Users.First(u => u.Id == userId);

            notification.Message = "You have been asigned to Ticket#: " + ticketId + " " +
                tableEntry.Title + ".";
            
            db.Notifications.Add(notification);
            db.SaveChanges();
        }
        public static void Notify_UnasignedFromTicket(string userId, int ticketId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var notification = new GenericNotification();

            notification.Title = "Unasigned from Ticket";
            notification.Created = DateTimeOffset.Now;
            notification.Read = false;
            notification.LinkToObjectDetails = "~/Tickets/Details/" + ticketId.ToString();

            var tableEntry = db.Tickets.AsNoTracking().First(t => t.Id == ticketId);

            notification.ObjectType = db.Tickets.AsNoTracking().First(t => t.Id == ticketId).GetType().Name;
            notification.ObjectId = ticketId;
            notification.ToUserId = userId;
            notification.ToUser = db.Users.First(u => u.Id == userId);

            notification.Message = "You have been unasigned from Ticket#: " + ticketId + " " + 
                tableEntry.Title + ".";
            
            db.Notifications.Add(notification);
            db.SaveChanges();
        }
        public static void Notify_AsignedToProject(string userId, int projectId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var notification = new GenericNotification();

            notification.Title = "Asigned to Project";
            notification.Created = DateTimeOffset.Now;
            notification.Read = false;
            notification.LinkToObjectDetails = "~/Projects/Details/" + projectId.ToString();

            var tableEntry = db.Projects.AsNoTracking().First(t => t.Id == projectId);

            notification.ObjectType = db.Projects.AsNoTracking().First(t => t.Id == projectId).GetType().Name;
            notification.ObjectId = projectId;
            notification.ToUserId = userId;
            notification.ToUser = db.Users.First(u => u.Id == userId);

            notification.Message = "You have been asigned to Project# " + projectId + "-" + tableEntry.Name + ".";

            db.Notifications.Add(notification);
            db.SaveChanges();
        }
        public static void Notify_UnasignedFromProject(string userId, int projectId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var notification = new GenericNotification();

            notification.Title = "Unasigned from Project";
            notification.Created = DateTimeOffset.Now;
            notification.Read = false;
            notification.LinkToObjectDetails = "~/Projects/Details/" + projectId.ToString();

            var tableEntry = db.Projects.First(t => t.Id == projectId);

            notification.ObjectType = db.Projects.AsNoTracking().First(t => t.Id == projectId).GetType().Name;
            notification.ObjectId = projectId;
            notification.ToUserId = userId;
            notification.ToUser = db.Users.First(u => u.Id == userId);

            notification.Message = "You have been unasigned from Project# " + projectId + "-" + tableEntry.Name + ".";

            db.Notifications.Add(notification);
            db.SaveChanges();
        }
        public static void Notify_AsignedToRole(string userId, string _roleName)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var notification = new GenericNotification();

            notification.Title = "Asigned to Role";
            notification.Created = DateTimeOffset.Now;
            notification.Read = false;

            var tableEntry = db.Roles.AsNoTracking().First(r => r.Name == _roleName);

            notification.ObjectType = tableEntry.GetType().Name;
            //notification.ObjectId = tableEntry.Id;
            notification.ToUserId = userId;
            notification.ToUser = db.Users.First(u => u.Id == userId);
            

            notification.Message = "You have been asigned to Role: " + _roleName + ".";

            db.Notifications.Add(notification);
            db.SaveChanges();
        }
        public static void Notify_UnasignedFromRole(string userId, string _roleName)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var notification = new GenericNotification();

            notification.Title = "Unasigned from Role";
            notification.Created = DateTimeOffset.Now;
            notification.Read = false;

            var tableEntry = db.Roles.AsNoTracking().First(r => r.Name == _roleName);

            notification.ObjectType = tableEntry.GetType().Name;
            //notification.ObjectId = -1;
            //notification.ObjectId = tableEntry.Id;
            notification.ToUserId = userId;
            notification.ToUser = db.Users.First(u => u.Id == userId);            

            notification.Message = "You have been unasigned from Role: " + _roleName + ".";

            db.Notifications.Add(notification);
            db.SaveChanges();
        }
        public static void Notify_CustomMessage(string userId, string title, string message)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var notification = new GenericNotification();

            notification.Title = title;
            notification.Created = DateTimeOffset.Now;
            notification.Read = false;

            notification.ToUserId = userId;
            notification.ToUser = db.Users.AsNoTracking().First(u => u.Id == userId);

            notification.Message = message;

            db.Notifications.Add(notification);
            db.SaveChanges();
        }
    }

    public class HistoryHelper
    {
        public static void SaveTicketToHistory(Ticket newTicket)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == newTicket.Id);

            string[] exclude = new string[]{
            "Id", "Created", "Updated", "OwnerUserId", "Project","TicketType",
            "TicketPriority", "TicketStatus", "OwnerUser", "AssignedToUser",
            "TicketAttachments", "TicketComments", "Histories", "Notifications"
            };

            

            SaveToHistory(newTicket, oldTicket, exclude);
        }
        public static List<Ticket> TicketGetFullHistory(int? id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var ticket = db.Tickets.AsNoTracking().First(t => t.Id == id);

            var list = new List<Ticket>();
            string ticketType = ticket.GetType().Name;
            Dictionary<string, object[]> result = new Dictionary<string, object[]>();
            var entries = db.GenericHistories.AsNoTracking().Where(h => h.ObjectId == ticket.Id
                    && h.ObjectType == ticketType).OrderByDescending(h => h.Changed);
            foreach (var item in entries)
            {
                try
                {
                    Dictionary<string, object> values = ByteArrayToObject(item.Values) as Dictionary<string, object>;
                    list.Add(ConvertDictionaryToObject<Ticket>(values));
                }
                catch
                {
                    //In the case of an error create a fake history ticket with the explanation as the description
                    Dictionary<string, object> dict = new Dictionary<string,object>();
                    list.Add(new Ticket() {Updated = item.Changed,  Description = "An error has ocurred when retriving this history item" });
                }
            }

            var tempEntries = entries.ToList();
            int count = 0;
            foreach (var history in tempEntries)
            {
                var item = list[count];
                item.OwnerUserId = history.WhoMadeChangesId;
                item.OwnerUser = history.WhoMadeChanges;
                item.Created = (DateTimeOffset)history.Changed;

                if (item.ProjectId != 0) { item.Project = db.Projects.AsNoTracking().FirstOrDefault(i => i.Id == item.ProjectId); }

                if (item.TicketTypeId != 0) { item.TicketType = db.TicketTypes.AsNoTracking().FirstOrDefault(i => i.Id == item.TicketTypeId); }

                if (item.TicketPriorityId != 0) { item.TicketPriority = db.TicketPriorities.AsNoTracking().FirstOrDefault(i => i.Id == item.TicketPriorityId); }

                if (item.TicketStatusId != 0) { item.TicketStatus = db.TicketStatus.AsNoTracking().FirstOrDefault(i => i.id == item.TicketStatusId); }

                if (item.AssignedToUserId != null) { item.AssignedToUser = db.Users.AsNoTracking().FirstOrDefault(i => i.Id == item.AssignedToUserId); }

                count += 1;
            }

            return list;
        }
        public static Dictionary<string, object[]> GetTicketLastHistoryForEveryProperty(Ticket ticket)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Dictionary<string,object[]> dict = null;
            try
            {
                dict = GetTheLatestTicketChangesToAllTheProperties(ticket);
            }
            catch
            {
                dict = new Dictionary<string, object[]>();
                dict.Add("Description", new object[2] { "An error has ocurred when retrieving last history for every property", DateTimeOffset.Now });
            }
            
            var result = new Dictionary<string, object[]>();
            string stringId;
            int Id;
            foreach(var item in dict)
            {
                switch(item.Key)
                {
                    case "ProjectId":
                        Id = (int)item.Value[0];
                        result.Add(item.Key, new object[3] { item.Value[0], CleanDateTimeOffset(item.Value[1]), db.Projects.AsNoTracking().First(p => p.Id == Id).Name });
                        break;
                    case "TicketTypeId":
                        Id = (int)item.Value[0];
                        result.Add(item.Key, new object[3] { item.Value[0], CleanDateTimeOffset(item.Value[1]), db.TicketTypes.AsNoTracking().First(p => p.Id == Id).Name });
                        break;
                    case "TicketPriorityId":
                        Id = (int)item.Value[0];
                        result.Add(item.Key, new object[3] { item.Value[0], CleanDateTimeOffset(item.Value[1]), db.TicketPriorities.AsNoTracking().First(p => p.Id == Id).Name });
                        break;
                    case "TicketStatusId":
                        Id = (int)item.Value[0];
                        result.Add(item.Key, new object[3] { item.Value[0], CleanDateTimeOffset(item.Value[1]), db.TicketStatus.AsNoTracking().First(p => p.id == Id).Name });
                        break;
                    case "AssignedToUserId":
                        stringId =  (string)item.Value[0];
                        result.Add(item.Key, new object[3] { item.Value[0], CleanDateTimeOffset(item.Value[1]), db.Users.AsNoTracking().First(p => p.Id == stringId).DisplayName });
                        break;
                    case "Description":
                        string htmlCleaned = TextParsing.GetWordsFromHTML_EqualToCharAmmount_Regex((string)item.Value[0], 150).ToString();
                        result.Add(item.Key, new object[3] { item.Value[0], CleanDateTimeOffset(item.Value[1]), htmlCleaned });
                        break;
                    default:
                        result.Add(item.Key, new object[2] { item.Value[0], CleanDateTimeOffset(item.Value[1])});
                        break;
                        
                }
            }

            return result;
        }
        public static string CleanDateTimeOffset(object time)
        { return "'" + ((DateTimeOffset)time).ToLocalTime().ToString("dd MMM yyyy h:mm tt") + "'"; }

        /// <summary>
        /// It has to be called before the new changes are saved
        /// </summary>
        /// <param name="ticketId"></param>
        public static void SaveToHistory(object newObj, object oldObj, string[] exclude)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var oldDict = ConvertObjectToDictionary(exclude, oldObj);
            var newDict = ConvertObjectToDictionary(exclude, newObj);
            var difference = oldDict.Except(newDict).ToDictionary(x => x.Key, x => x.Value);

            if (difference.Count() > 0)
            {
                var history = new GenericHistory();
                history.WhoMadeChangesId = HttpContext.Current.User.Identity.GetUserId();
                history.Changed = DateTimeOffset.Now;
                history.ObjectType = oldObj.GetType().Name;
                history.ObjectId = (newObj as dynamic).Id;
                history.Changed = DateTimeOffset.Now;
                history.WhoMadeChangesId = HttpContext.Current.User.Identity.GetUserId();

                try
                {
                    history.Values = ObjectToByteArray(difference);
                }
                catch (Exception e)
                {
                    //Create a new dictionary containing the error
                    var errorDict = new Dictionary<string, string>();
                    errorDict.Add("Description", "Error while saving ticket history!  \n" + e.Message);
                    history.Values = ObjectToByteArray(errorDict);
                }
                

                //var b = db.GenericHistories.AsNoTracking().FirstOrDefault(h => h.Id == 2);
                //var reversed = ByteArrayToObject(b.Values);

                db.GenericHistories.Add(history);
                db.SaveChanges();
            }            
        }
        /// <summary>
        /// It goes back in time looking for the latest canges of all the properties
        /// in an object and returns them in a dictionary
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, object[]> GetTheLatestTicketChangesToAllTheProperties(Ticket ticket)
        {
            string ticketType = ticket.GetType().Name;
            Dictionary<string, object[]> result = new Dictionary<string, object[]>();
            ApplicationDbContext db = new ApplicationDbContext();
            var entries = db.GenericHistories.AsNoTracking().Where(h => h.ObjectId == ticket.Id
                    && h.ObjectType == ticketType).OrderByDescending(h => h.Changed);
            foreach(var item in entries)
            {
                try
                {
                    Dictionary<string, object> values = ByteArrayToObject(item.Values) as Dictionary<string, object>;
                    foreach (var keypair in values)
                    {
                        if (!result.ContainsKey(keypair.Key))
                        {
                            result.Add(keypair.Key, new object[2] { keypair.Value, item.Changed });
                        }
                    }
                }
                catch
                {
                    //If there is an error when converting Byte array to object or getting any property
                    //Just skip it
                    throw;
                }
            }
            return result;
        }
        
        public static string GetTimeOfUpdate(string propertyName, Dictionary<string, Object[]> HistoryDictionary)
        {
            return "'" + ((DateTimeOffset)HistoryDictionary[propertyName][1]).ToLocalTime().ToString("dd MMM yyyy h:mm tt") + "'";
        }
        public static string GetOldMessage(Dictionary<string, Object[]> HistoryDictionary)
        {
            string message = (string)HistoryDictionary["Description"][0];
            return TextParsing.GetWordsFromHTML_EqualToCharAmmount_Regex(message, 150).ToString();
        }
        public static string GetUserName(string propertyName, Dictionary<string, Object[]> HistoryDictionary)
        { 
            ApplicationDbContext db = new ApplicationDbContext();
            string userid = HistoryDictionary[propertyName][0] as string;
            var user = db.Users.AsNoTracking().FirstOrDefault(u => u.Id == userid);
            if (user == null)
                return "User Was Excomulgated from Users List";
            else
                return user.DisplayName;
        }
        private static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;

            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            try
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
            finally
            {
                ms.Dispose();
            }
            

        }
        private static Object ByteArrayToObject(byte[] arrBytes)
        {
            var memStream = new MemoryStream();
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);

            try
            {
                Object obj = (Object)binForm.Deserialize(memStream);
                return obj;
            }
            finally
            {
                memStream.Dispose();                
            }
        }
        /// <summary>
        /// Give me an object and I will turn it into a dicionary
        /// </summary>
        /// <param name="exlcudeNames"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ConvertObjectToDictionary(string[] exlcudeNames, object obj)
        {
            var newDictionary = new Dictionary<string, object>();
            var allProperties = obj.GetType().GetProperties();
            if (exlcudeNames.GetType().Name == "String[]")
            {
                foreach (PropertyInfo info in allProperties)
                {
                    if (!exlcudeNames.Contains(info.Name))
                    {
                        var value = info.GetValue(obj);
                        newDictionary.Add(info.Name, value);
                    }
                }
            }            
            return newDictionary;
        }
        /// <summary>
        /// Give me a dictionary and the type of the object it was created from 
        /// and I will build the object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static T ConvertDictionaryToObject<T>(IDictionary<string, object> dict)
        {
            Type type = typeof(T);
            T result = (T)Activator.CreateInstance(type);
            foreach (var entry in dict)
            {
                type.GetProperty(entry.Key).SetValue(result, entry.Value, null);
            }
            return result;
        }
    }

    public class SavingHelper
    {
        /// <summary>
        /// each entry will be separatd by a ':' dowble dots
        /// Because filename cannot have that character,
        /// there fore because we are going to save file names
        /// there will be no conflict
        /// </summary>
        /// <param name="_list"></param>
        /// <returns></returns>
        public static string ListOfStringsToString(List<string> _list)
        {
            return string.Join(":", _list.ToArray());
        }
        /// <summary>
        /// each entry is separatd by a ':' dowble dots
        /// </summary>
        /// <param name="_list"></param>
        /// <returns></returns>
        public static List<string> StringToListOfStrings(string _list)
        {
            return _list.Split(':').ToList();
        }

        public static string GetCleanFileNameOfAttachement(string _filePath)
        {
            string result = System.IO.Path.GetFileName(_filePath);
            result = result.Substring(result.IndexOf('_')+1);
            return result;
        }
    }


    //public class EmailService : IIdentityMessageService
    //{
    //    public async Task SendAsync(IdentityMessage message)
    //    {
    //        await configSendGridasync(message);
    //    }

    //    // Use NuGet to install SendGrid (Basic C# client lib) 
    //    private async Task configSendGridasync(IdentityMessage message)
    //    {
    //        var myMessage = new SendGridMessage();
    //        myMessage.AddTo(message.Destination);
    //        myMessage.From = new System.Net.Mail.MailAddress(
    //                            "Joe@contoso.com", "Joe S.");
    //        myMessage.Subject = message.Subject;
    //        myMessage.Text = message.Body;
    //        myMessage.Html = message.Body;

    //        var credentials = new NetworkCredential(
    //                   ConfigurationManager.AppSettings["mailAccount"],
    //                   ConfigurationManager.AppSettings["mailPassword"]
    //                   );

    //        // Create a Web transport for sending email.
    //        var transportWeb = new Web(credentials);

    //        // Send the email.
    //        if (transportWeb != null)
    //        {
    //            await transportWeb.DeliverAsync(myMessage);
    //        }
    //        else
    //        {
    //            Trace.TraceError("Failed to create Web transport.");
    //            await Task.FromResult(0);
    //        }
    //    }
    //}
}