using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using ODPMS.Models;
using Windows.Storage;

namespace ODPMS.Helpers
{
    class DatabaseHelper
    {
        public DatabaseHelper()
        {
            InitializeDatabase();
        }

        public async static void InitializeDatabase()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("odpms_data.db", CreationCollisionOption.OpenIfExists);
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                //Tickets Table
                String tableCommand = 
                    "CREATE TABLE IF NOT EXISTS " +
                    "Tickets (Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "Number INTEGER," +
                    "Type NVCARCHAR(50)," +
                    "Description NVARCHAR(50)," +
                    "Created DATETIME," +
                    "Closed DATETIME," +
                    "Status NVARCHAR(50)," +
                    "Rate FLOAT," +
                    "Cost FLOAT," +
                    "PayAmount FLOAT," +
                    "Balance FLOAT," +
                    "User NVARCHAR(10))";
    
                SqliteCommand createTable = new SqliteCommand(tableCommand, dbconn);
                createTable.ExecuteReader();

                //Users Table
                tableCommand =
                    "CREATE TABLE IF NOT EXISTS " +
                    "Users (Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "Username NVARCHAR(25) NOT NULL, " +
                    "Password NVARCHAR(255), " +
                    "Salt NVARCHAR(25), " + 
                    "FirstName NVARCHAR(25), " +
                    "LastName NVARCHAR(25), " +
                    "UserType NVARCHAR(25), " +
                    "Status NVARCHAR(25), " +
                    "LastLogin DATETIME DEFAULT NULL);";

                createTable = new SqliteCommand(tableCommand, dbconn);
                createTable.ExecuteReader();

                //Ticket Type Table
                tableCommand =
                    "CREATE TABLE IF NOT EXISTS " +
                    "TicketType (Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "TicketType NVARCHAR(25) NOT NULL, " +
                    "Description NVARCHAR(25), " +
                    "UnitCost REAL, " +
                    "Status NVARCHAR(25), " +
                    "Username NVARCHAR(25), " +
                    "ActivityDate DATETIME);";

                createTable = new SqliteCommand(tableCommand, dbconn);
                createTable.ExecuteReader();

                //Prepaid Customers Table
                tableCommand =
                    "CREATE TABLE IF NOT EXISTS " +
                    "Customers (NumberPlate NVARCHAR(7) PRIMARY KEY, " +
                    "FirstName NVCARCHAR(50)," +
                    "LastName NVCARCHAR(50)," +
                    "Telephone NVARCHAR(15)," +
                    "Email NVCARCHAR(50)," +
                    "Created DATETIME," +
                    "User NVARCHAR(10))";

                createTable = new SqliteCommand(tableCommand, dbconn);
                createTable.ExecuteReader();

                //Prepaid Transactions Table
                tableCommand =
                    "CREATE TABLE IF NOT EXISTS " +
                    "PrepaidTransactions (Id INTEGER PRIMARY KEY, " +
                    "NumberPlate NVARCHAR(7)," +
                    "Type NVCARCHAR(50)," +
                    "Description NVARCHAR(50)," +
                    "IssueDate DATETIME," +
                    "ExpiryDate DATETIME," +
                    "Status NVARCHAR(50)," +
                    "Cost FLOAT," +
                    "User NVARCHAR(10))";

                createTable = new SqliteCommand(tableCommand, dbconn);
                createTable.ExecuteReader();

                //Float Table
                tableCommand =
                    "CREATE TABLE IF NOT EXISTS " +
                    "Float (Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "Amount FLOAT," +
                    "User NVCARCHAR(10)," +
                    "Date DATETIME," +
                    "UpdatedBy NVCARCHAR(10)," +
                    "UpdateDate DATETIME);";

                createTable = new SqliteCommand(tableCommand, dbconn);
                createTable.ExecuteReader();

                dbconn.Close();
            }

            //Add data only if admin user does not exist
            if (!FindUser(1))
                AddData();
        }

        public static void AddData()
        {
            //This function creates initial database state (Default admin User & 1 ticket)
            var cultureInfo = new CultureInfo("en-US");

            //Default Ticket
            //int id = 1;
            int number = 1;
            string type = "Hourly";
            string description = "Hourly Ticket";
            var created = DateTime.Parse("05-12-2022 16:01:01", cultureInfo, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
            //var closed = default(DateTime);
            string status = "Paid";
            double rate = 3.50;
            double cost = 10.00;
            double payAmount = 10.00;
            double balance = 0.0;
            string user = "test";

            //Admin User
            //int userId = 1;
            string username = "admin";
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string password = BCrypt.Net.BCrypt.HashPassword("Password1", salt);
            string firstName = "Admin";
            string lastName = "User";
            string userType = "admin";
            string userStatus = "Active";
            //DateTime lastLogin = DateTime.Now;

            //Ticket Type
            int[] ttId = { 1, 2, 3, 4 };
            string[] ticketType = { "Hourly", "Daily", "Weekly", "Monthly" };
            string[] ttDescription = { "Hourly Ticket", "Daily Ticket", "Weekly Customer", "Monthly Customer" };
            double[] unitCost = { 2.50, 10.00, 50.00, 200.00 };
            string[] ttStatus = { "Active", "Active", "Active", "Active" };
            string[] ttUsername = { "admin", "admin", "admin", "admin" };
            DateTime[] activityDate = { DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now };

            

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                //Default Ticket
                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = dbconn;

                // Use parameterized query to prevent SQL injection attacks
                //insertCommand.CommandText = "INSERT INTO Tickets VALUES (@Id, @Number, @Type, @Description, @Created, NULL, @Status, @Rate, @Cost, @Balance, @User);";
                insertCommand.CommandText = "INSERT INTO Tickets VALUES (NULL, @Number, @Type, @Description, @Created, NULL, @Status, @Rate, @Cost, @PayAmount, @Balance, @User);";
                //insertCommand.Parameters.AddWithValue("@Id", id);
                insertCommand.Parameters.AddWithValue("@Number", number);
                insertCommand.Parameters.AddWithValue("@Type", type);
                insertCommand.Parameters.AddWithValue("@Description", description);
                insertCommand.Parameters.AddWithValue("@Created", created);
                //insertCommand.Parameters.AddWithValue("@Closed", closed);
                insertCommand.Parameters.AddWithValue("@Status", status);
                insertCommand.Parameters.AddWithValue("@Rate", rate);
                insertCommand.Parameters.AddWithValue("@Cost", cost);
                insertCommand.Parameters.AddWithValue("@PayAmount", payAmount);
                insertCommand.Parameters.AddWithValue("@Balance", balance);
                insertCommand.Parameters.AddWithValue("@User", user);

                insertCommand.ExecuteReader();

                //Admin User
                SqliteCommand insertUserCommand = new SqliteCommand();
                insertUserCommand.Connection = dbconn;

                // Use parameterized query to prevent SQL injection attacks
                //insertUserCommand.CommandText = "INSERT INTO Users VALUES (@Id, @Username, @Password, @Salt, @FirstName, @LastName, @UserType, @Status, @LastLogin);";
                insertUserCommand.CommandText = "INSERT INTO Users VALUES (NULL, @Username, @Password, @Salt, @FirstName, @LastName, @UserType, @Status, NULL);";
                //insertUserCommand.Parameters.AddWithValue("@Id", userId);
                insertUserCommand.Parameters.AddWithValue("@Username", username);
                insertUserCommand.Parameters.AddWithValue("@Password", password);
                insertUserCommand.Parameters.AddWithValue("@Salt", salt);
                insertUserCommand.Parameters.AddWithValue("@FirstName", firstName);
                insertUserCommand.Parameters.AddWithValue("@LastName", lastName);
                insertUserCommand.Parameters.AddWithValue("@UserType", userType);
                insertUserCommand.Parameters.AddWithValue("@Status", userStatus);
                //insertUserCommand.Parameters.AddWithValue("@LastLogin", lastLogin);

                insertUserCommand.ExecuteReader();
              

                //Ticket Type
                SqliteCommand insertTypeCommand = new SqliteCommand();
                insertTypeCommand.Connection = dbconn;

                for (int i = 0; i < ttId.Length; i++)
                {
                    // Use parameterized query to prevent SQL injection attacks
                    insertTypeCommand.CommandText = "INSERT INTO TicketType VALUES (@Id, @TicketType, @Description, @UnitCost, @Status, @Username, @ActivityDate);";
                    insertTypeCommand.Parameters.AddWithValue("@Id", ttId[i]);
                    insertTypeCommand.Parameters.AddWithValue("@TicketType", ticketType[i]);
                    insertTypeCommand.Parameters.AddWithValue("@Description", ttDescription[i]);
                    insertTypeCommand.Parameters.AddWithValue("@UnitCost", unitCost[i]);
                    insertTypeCommand.Parameters.AddWithValue("@Status", ttStatus[i]);
                    insertTypeCommand.Parameters.AddWithValue("@Username", ttUsername[i]);
                    insertTypeCommand.Parameters.AddWithValue("@ActivityDate", activityDate[i]);              
                    insertTypeCommand.ExecuteReader();
                    insertTypeCommand.Parameters.Clear();
                    insertTypeCommand.Dispose();
                              
                }
                
                dbconn.Close();
                
            }

        }

        public static List<String> GetData()
        {
            List<String> entries = new List<string>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                string selectCMD = "SELECT User, Number from Tickets";
                SqliteCommand selectCommand = new SqliteCommand(selectCMD, dbconn);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    entries.Add(query.GetString(1));
                }

                dbconn.Close();
            }

            return entries;
        }

        public static void AddTicket(Ticket ticket)
        {
            //var cultureInfo = new CultureInfo("en-US");

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = dbconn;

                // Use parameterized query to prevent SQL injection attacks
                //insertCommand.CommandText = "INSERT INTO Tickets VALUES (@Id, @Number, @Type, @Description, @Created, @Closed, @Status, @Rate, @Cost, @Balance, @User);";
                insertCommand.CommandText = "INSERT INTO Tickets VALUES (NULL, @Number, @Type, @Description, @Created, NULL, @Status, @Rate, @Cost, @PayAmount, @Balance, @User);";
                //insertCommand.Parameters.AddWithValue("@Id", ticket.Id);
                insertCommand.Parameters.AddWithValue("@Number", ticket.Number);
                insertCommand.Parameters.AddWithValue("@Type", ticket.Type);
                insertCommand.Parameters.AddWithValue("@Description", ticket.Description);
                insertCommand.Parameters.AddWithValue("@Created", ticket.Created);
                //insertCommand.Parameters.AddWithValue("@Closed", ticket.Closed);
                insertCommand.Parameters.AddWithValue("@Status", ticket.Status);
                insertCommand.Parameters.AddWithValue("@Rate", ticket.Rate);
                insertCommand.Parameters.AddWithValue("@Cost", ticket.Cost);
                insertCommand.Parameters.AddWithValue("@PayAmount", ticket.PayAmount);
                insertCommand.Parameters.AddWithValue("@Balance", ticket.Balance);
                insertCommand.Parameters.AddWithValue("@User", ticket.User);

                insertCommand.ExecuteReader();

                dbconn.Close();
            }

        }

        public static Ticket CreateTicket(string ticketType)
        {
            Ticket ticket;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = dbconn;

                selectCommand.CommandText = "SELECT Id FROM Tickets ORDER BY Id DESC LIMIT 1;";
                SqliteDataReader query = selectCommand.ExecuteReader();

                query.Read();
                int ticketNumber = Int32.Parse(query.GetString(0));

                ticket = new Ticket(null, ticketNumber+1, ticketType, "Hourly Ticket", DateTime.Now, null, "Open", float.Parse("2.5"), float.Parse("0.0"), float.Parse("0.0"), float.Parse("0.0"), App.LoggedInUser.Username);

                dbconn.Close();
            }

            return ticket;
        }

        public static void PayTicket(Ticket ticket)
        {
            //var cultureInfo = new CultureInfo("en-US");
            ticket.Status = "Paid";

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                SqliteCommand updateCommand = new SqliteCommand();
                updateCommand.Connection = dbconn;

                // Use parameterized query to prevent SQL injection attacks
                updateCommand.CommandText = "UPDATE Tickets SET Closed = @Closed, Status = @Status, Cost = @Cost, PayAmount = @PayAmount WHERE Number = @Number;";
                //updateCommand.Parameters.AddWithValue("@Id", ticket.Id);
                updateCommand.Parameters.AddWithValue("@Closed", ticket.Closed);
                updateCommand.Parameters.AddWithValue("@Status", ticket.Status);
                updateCommand.Parameters.AddWithValue("@Cost", ticket.Cost);
                updateCommand.Parameters.AddWithValue("@PayAmount", ticket.PayAmount);
                updateCommand.Parameters.AddWithValue("@Number", ticket.Number);

                updateCommand.ExecuteReader();

                dbconn.Close();
            }
        }

        public static Ticket FindTicket(int number)
        {
            Ticket ticket = null;
            int gracePeriod = 5;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = dbconn;

                // Use parameterized query to prevent SQL injection attacks
                selectCommand.CommandText = "SELECT * FROM Tickets WHERE Number = @Number LIMIT 1;";
                selectCommand.Parameters.AddWithValue("@Number", number);

                SqliteDataReader query = selectCommand.ExecuteReader();

                query.Read();
                if (query.HasRows)
                {
                    if (query[5].GetType() == typeof(DBNull))
                    {
                        ticket = new Ticket(Int32.Parse(query.GetString(0)), Int32.Parse(query.GetString(1)), query.GetString(2),
                            query.GetString(3), DateTime.Parse(query.GetString(4)), null, query.GetString(6), float.Parse(query.GetString(7)), 
                            float.Parse(query.GetString(8)), float.Parse(query.GetString(9)), float.Parse(query.GetString(10)), query.GetString(11));
                    } else
                    {
                        ticket = new Ticket(Int32.Parse(query.GetString(0)), Int32.Parse(query.GetString(1)), query.GetString(2),
                            query.GetString(3), DateTime.Parse(query.GetString(4)), DateTime.Parse(query.GetString(5)), query.GetString(6),
                            float.Parse(query.GetString(7)), float.Parse(query.GetString(8)), float.Parse(query.GetString(9)), 
                            float.Parse(query.GetString(10)), query.GetString(11));
                    }

                    ticket.Closed = DateTime.Now;
                    TimeSpan ts = (DateTime)ticket.Closed - ticket.Created;

                    if (ts.TotalMinutes % 60 >= gracePeriod)
                    {
                        ticket.Cost = ticket.Rate * Math.Ceiling(ts.TotalHours);
                    }
                    else
                    {
                        ticket.Cost = ticket.Rate * Math.Floor(ts.TotalHours);
                    }
                    //dbconn.Close();
                    //return ticket;
                }
                dbconn.Close();
            }
            //return null;
            return ticket;
        }

        public static bool CheckTicket(int number)
        {
            bool isTicket;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = dbconn;

                // Use parameterized query to prevent SQL injection attacks
                selectCommand.CommandText = "SELECT * FROM Tickets WHERE Number = @Number AND Status = @Status LIMIT 1;";
                selectCommand.Parameters.AddWithValue("@Number", number);
                selectCommand.Parameters.AddWithValue("@Status", "Open");

                SqliteDataReader query = selectCommand.ExecuteReader();

                query.Read();
                if (query.HasRows)
                {
                    isTicket = true;
                }
                else
                {
                    isTicket = false;
                }
                dbconn.Close();
            }
            return isTicket;
        }

        public static ObservableCollection<TicketViewModel> GetTicketListViewData(string? status)
        {
            ObservableCollection<TicketViewModel> tickets = new ObservableCollection<TicketViewModel>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = dbconn;

                if (status != null)
                {
                    selectCommand.CommandText = "SELECT * FROM Tickets WHERE Status = @Status";
                    selectCommand.Parameters.AddWithValue("@Status", status);
                } else
                {
                    selectCommand.CommandText = "SELECT * FROM Tickets";
                }

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    if (query[5].GetType() == typeof(DBNull))
                    {
                        tickets.Add(new TicketViewModel(Int32.Parse(query.GetString(1)), query.GetString(2), query.GetString(3), 
                            DateTime.Parse(query.GetString(4)), null, query.GetString(6), float.Parse(query.GetString(7)), 
                            float.Parse(query.GetString(8)), float.Parse(query.GetString(9)), float.Parse(query.GetString(10)), 
                            query.GetString(11)));
                    }
                    else
                    {
                        tickets.Add(new TicketViewModel(Int32.Parse(query.GetString(1)), query.GetString(2), query.GetString(3), 
                            DateTime.Parse(query.GetString(4)), DateTime.Parse(query.GetString(5)), query.GetString(6),
                            float.Parse(query.GetString(7)), float.Parse(query.GetString(8)), float.Parse(query.GetString(9)), 
                            float.Parse(query.GetString(10)), query.GetString(11)));
                    }
                }

                dbconn.Close();
            }

            return tickets;
        }

        public static ObservableCollection<Ticket> GetTicketListRange(DateTime fromDate, DateTime toDate, string status)
        {
            ObservableCollection<Ticket> tickets = new ObservableCollection<Ticket>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = dbconn;

                if (status == "All")
                    selectCommand.CommandText = "SELECT * FROM Tickets WHERE Created >= @CreatedFrom AND Created <= @CreatedTo;";                    
                else
                {
                    selectCommand.CommandText = "SELECT * FROM Tickets WHERE Created >= @CreatedFrom AND Created <= @CreatedTo AND Status = @Status;";
                    selectCommand.Parameters.AddWithValue("@Status", status);
                }
                selectCommand.Parameters.AddWithValue("@CreatedFrom", fromDate);
                selectCommand.Parameters.AddWithValue("@CreatedTo", toDate);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    if (query[5].GetType() == typeof(DBNull))
                    {
                        tickets.Add(new Ticket(Int32.Parse(query.GetString(0)), Int32.Parse(query.GetString(1)), query.GetString(2),
                            query.GetString(3), DateTime.Parse(query.GetString(4)), null, query.GetString(6), float.Parse(query.GetString(7)),
                            float.Parse(query.GetString(8)), float.Parse(query.GetString(9)), float.Parse(query.GetString(10)), query.GetString(11)));
                    }
                    else
                    {
                        tickets.Add(new Ticket(Int32.Parse(query.GetString(0)), Int32.Parse(query.GetString(1)), query.GetString(2),
                            query.GetString(3), DateTime.Parse(query.GetString(4)), DateTime.Parse(query.GetString(5)), query.GetString(6),
                            float.Parse(query.GetString(7)), float.Parse(query.GetString(8)), float.Parse(query.GetString(9)), 
                            float.Parse(query.GetString(10)), query.GetString(11)));
                    }
                }

                dbconn.Close();
            }

            return tickets;
        }

        public static void AddUser(User newUser)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = dbconn;

                // Use parameterized query to prevent SQL injection attacks
                //insertCommand.CommandText = "INSERT INTO Users VALUES (@Id, @Username, @Password, @Salt, @FirstName, @LastName, @UserType, @Status, @LastLogin);";
                insertCommand.CommandText = "INSERT INTO Users VALUES (NULL, @Username, @Password, @Salt, @FirstName, @LastName, @UserType, @Status, NULL);";
                //insertCommand.Parameters.AddWithValue("@Id", newUser.Id);
                insertCommand.Parameters.AddWithValue("@Username", newUser.Username);
                insertCommand.Parameters.AddWithValue("@Password", newUser.Password);
                insertCommand.Parameters.AddWithValue("@Salt", newUser.Salt);
                insertCommand.Parameters.AddWithValue("@FirstName", newUser.FirstName);
                insertCommand.Parameters.AddWithValue("@LastName", newUser.LastName);
                insertCommand.Parameters.AddWithValue("@UserType", newUser.UserType);
                insertCommand.Parameters.AddWithValue("@Status", newUser.Status);
                //insertCommand.Parameters.AddWithValue("@LastLogin", newUser.LastLogin);

                insertCommand.ExecuteReader();

                dbconn.Close();
            }
        }

        public static bool CreateUser(string username, string password, string firstName, string lastName, string userType)
        {
            User user;
            string salt = string.Empty;
            string status = "Active";
            bool functionStatus = false;

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = dbconn;

                selectCommand.CommandText = "SELECT Id FROM Users WHERE Username = @Username;";
                selectCommand.Parameters.AddWithValue("@Username", username);
                SqliteDataReader query = selectCommand.ExecuteReader();

                query.Read();
                

                if (!query.HasRows) 
                {
                    selectCommand.CommandText = "SELECT Id FROM Users ORDER BY Id DESC LIMIT 1;";
                    query = selectCommand.ExecuteReader();

                    query.Read();
                    int userId = Int32.Parse(query.GetString(0));

                    user = new User(userId + 1, username, password, salt, firstName, lastName, userType, status, default(DateTime));
                    AddUser(user);

                    functionStatus = true;
                }

                else
                {
                    functionStatus = false;
                }                

                dbconn.Close();
            }

            return functionStatus;
        }

        public static void UpdateUser(User updateUser)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                SqliteCommand updateCommand = new SqliteCommand();
                updateCommand.Connection = dbconn;

                // Use parameterized query to prevent SQL injection attacks
                updateCommand.CommandText = "UPDATE Users SET Username=@Username, Password=@Password, Salt=@Salt, FirstName=@FirstName, LastName=@LastName, UserType=@UserType, LastLogin=@LastLogin WHERE Id=@Id;";
                updateCommand.Parameters.AddWithValue("@Username", updateUser.Username);
                updateCommand.Parameters.AddWithValue("@Password", updateUser.Password);
                updateCommand.Parameters.AddWithValue("@Salt", updateUser.Salt);
                updateCommand.Parameters.AddWithValue("@FirstName", updateUser.FirstName);
                updateCommand.Parameters.AddWithValue("@LastName", updateUser.LastName);
                updateCommand.Parameters.AddWithValue("@UserType", updateUser.UserType);
                updateCommand.Parameters.AddWithValue("@Status", updateUser.Status);
                updateCommand.Parameters.AddWithValue("@LastLogin", updateUser.LastLogin);
                updateCommand.Parameters.AddWithValue("@Id", updateUser.Id);

                updateCommand.ExecuteReader();

                dbconn.Close();
            }
        }

        public static bool FindUser(int id)
        {
            bool userFound;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = dbconn;

                selectCommand.CommandText = "SELECT * FROM Users WHERE Id=@Id LIMIT 1;";
                selectCommand.Parameters.AddWithValue("@Id", id);

                SqliteDataReader query = selectCommand.ExecuteReader();

                if (query.HasRows)
                    userFound = true;
                else
                    userFound = false;
                dbconn.Close();
            }
            return userFound;
        }

        public static List<User> UserLogin(string username, string password)
        {
            List<User> users = new List<User>();
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = dbconn;

                selectCommand.CommandText = "SELECT * FROM Users WHERE Username=@Username AND Status=@Status;";
                selectCommand.Parameters.AddWithValue("@Username", username);
                selectCommand.Parameters.AddWithValue("@Status", "Active");

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    if (query[8].GetType() == typeof(DBNull))
                    {
                        users.Add(new User(Int32.Parse(query.GetString(0)), query.GetString(1), query.GetString(2),
                            query.GetString(3), query.GetString(4), query.GetString(5), query.GetString(6), query.GetString(7),
                            null));
                    }
                    else
                    {
                        users.Add(new User(Int32.Parse(query.GetString(0)), query.GetString(1), query.GetString(2),
                            query.GetString(3), query.GetString(4), query.GetString(5), query.GetString(6),
                            query.GetString(7), DateTime.Parse(query.GetString(8))));
                    }
                }

                dbconn.Close();
            }
            //if (users.Count > 0 && BCrypt.Net.BCrypt.Verify(BCrypt.Net.BCrypt.HashPassword(password, users[0].Salt), users[0].Password))
            if (users.Count > 0 && BCrypt.Net.BCrypt.HashPassword(password, users[0].Salt) == users[0].Password)
            {
                users[0].LastLogin = DateTime.Now;
                UpdateUser(users[0]);
                return users;
            }
            else
            {
                return users = new List<User>();
            }
            //return users;
        }
        public static ObservableCollection<UserViewModel> GetUsers()
        {
            ObservableCollection<UserViewModel> users = new ObservableCollection<UserViewModel>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                string selectCMD = "SELECT * from Users";
                SqliteCommand selectCommand = new SqliteCommand(selectCMD, dbconn);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    if (query[8].GetType() == typeof(DBNull))
                    {
                        users.Add(new UserViewModel(Int32.Parse(query.GetString(0)), query.GetString(1),
                            query.GetString(4), query.GetString(5), query.GetString(6),
                            query.GetString(7), null));
                    }
                    else
                    {
                        users.Add(new UserViewModel(Int32.Parse(query.GetString(0)), query.GetString(1),
                            query.GetString(4), query.GetString(5), query.GetString(6),
                            query.GetString(7), DateTime.Parse(query.GetString(8))));
                    }
                }

                dbconn.Close();
            }

            return users;
        }
    }
}
