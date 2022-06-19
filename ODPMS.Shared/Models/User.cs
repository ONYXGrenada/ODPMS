using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;

namespace ODPMS.Models
{
    [Table("users")]
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsReset { get; set; }

        [Ignore]
        public static string StatusMessage { get; set; }

        public override string ToString()
        {
            return Status;
        }

        #region Database Functions
        public static async Task<List<User>> GetAllUsers()
        {
            try
            {
                //await Init();
                var query = App.Database.Current.Table<User>();
                StatusMessage = string.Format("{0} record(s) found in the ticket table)", await query.CountAsync());

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }
            return new List<User>();
        }

        public static async Task<User> GetUser(int id)
        {
            try
            {
                var query = await App.Database.Current.GetAsync<User>(id);
                StatusMessage = string.Format("{0} record(s) found in the ticket table)", query);

                return query;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }
            return new User();
        }

        public static async Task UpdateUser(User user)
        {
            int result = 0;
            try
            {
                //await Init();
                result = await App.Database.Current.UpdateAsync(user);
                StatusMessage = string.Format("{0} record(s) found in the ticket table)", result);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }
        }

        public static async Task<User> Login(string username, string password)
        {
            try
            {
                //await Init();
                var query = await App.Database.Current.Table<User>().Where(v => v.Username == username).FirstOrDefaultAsync();
                if (query == null)
                    return null;
                if (query.Status == "Active" && 
                    BCrypt.Net.BCrypt.HashPassword(password, query.Salt) == query.Password)
                {
                    query.LastLogin = DateTime.Now;
                    await UpdateUser(query);
                    StatusMessage = string.Format("{0} record(s) found in the ticket table)", query);
                    return query;
                } else 
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }
            return null;
        }

        public static async Task CreateUser(User user)
        {
            int result = 0;
            try
            {
                //await App.Database.Init();
                result = await App.Database.Current.InsertAsync(user);

                StatusMessage = string.Format("{0} record(s) added [Ticket: {1})", result, user.Id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", user.Id, ex.Message);
            }
        }

        public static async Task DeleteUser(int id)
        {
            int result = 0;
            try
            {
                result = await App.Database.Current.DeleteAsync<User>(id);

                StatusMessage = string.Format("{0} record(s) deleted [Ticket: {1})", result, id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete {0}. Error: {1}", id, ex.Message);
            }
        }
        #endregion
    }
}
