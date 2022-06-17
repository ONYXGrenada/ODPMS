using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using ODPMS.Models;
using SQLite;
using Windows.Storage;

namespace ODPMS.Helpers
{
    public class DatabaseHelper
    {
        private readonly string _databasePath;
        private SQLiteAsyncConnection _database;

        public SQLiteAsyncConnection Current { get { return _database; } }
        //private SQLiteAsyncConnection conn;
        //private string _dbPath;
        //public string StatusMessage { get; set; }

        //private string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "opms.db3");

        //public DatabaseHelper()
        //{
        //    //InitializeDatabase();
        //    Init();
        //}

        public DatabaseHelper(string dbPath)
        {
            _databasePath = dbPath;
        }

        public async void Init()
        {
            Type[] tables = {
                typeof(Ticket),
                typeof(User),
                typeof(TicketType),
                typeof(Receipt)
            };
            if (_database != null)
            {
                return;
            }
            _database = new SQLiteAsyncConnection(_databasePath);
            await _database.CreateTablesAsync(CreateFlags.None, tables);

            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            User admin = new();
            admin.Username = "admin";
            admin.Salt = salt;
            admin.Password = BCrypt.Net.BCrypt.HashPassword("Password1", salt);
            admin.FirstName = "Admin";
            admin.LastName = "User";
            admin.Type = "admin";
            admin.Status = "Active";

            var queryUser = _database.Table<User>().Where(v => v.Username == "admin");
            if (await queryUser.CountAsync() == 0)
                await _database.InsertAsync(admin);

            List<TicketType> ticketTypes = new();
            TicketType ticketTypeH = new TicketType();
            ticketTypeH.Type = "Hourly";
            ticketTypeH.Description = "Hourly Ticket";
            ticketTypeH.Quantity = 1;
            ticketTypeH.Rate = 2.50;
            ticketTypeH.Status = "Active";
            ticketTypeH.User = "admin";
            ticketTypeH.ActivityDate = DateTime.Now;
            ticketTypes.Add(ticketTypeH);

            TicketType ticketTypeD = new TicketType();
            ticketTypeD.Type = "Daily";
            ticketTypeD.Description = "Daily Ticket";
            ticketTypeD.Quantity = 1;
            ticketTypeD.Rate = 10.00;
            ticketTypeD.Status = "Active";
            ticketTypeD.User = "admin";
            ticketTypeD.ActivityDate = DateTime.Now;
            ticketTypes.Add(ticketTypeD);

            TicketType ticketTypeW = new TicketType();
            ticketTypeW.Type = "Weekly";
            ticketTypeW.Description = "Weekly Ticket";
            ticketTypeW.Quantity = 1;
            ticketTypeW.Rate = 50.00;
            ticketTypeW.Status = "Active";
            ticketTypeW.User = "admin";
            ticketTypeW.ActivityDate = DateTime.Now;
            ticketTypes.Add(ticketTypeW);

            TicketType ticketTypeM = new TicketType();
            ticketTypeM.Type = "Monthly";
            ticketTypeM.Description = "Monthly Ticket";
            ticketTypeM.Quantity = 1;
            ticketTypeM.Rate = 200.00;
            ticketTypeM.Status = "Active";
            ticketTypeM.User = "admin";
            ticketTypeM.ActivityDate = DateTime.Now;
            ticketTypes.Add(ticketTypeM);

            foreach (var ticketType in ticketTypes)
            {
                var queryTicketType = _database.Table<TicketType>().Where(v => v.Type == ticketType.Type);
                if (await queryTicketType.CountAsync() == 0)
                    await _database.InsertAsync(ticketType);
            }
        }



        //private async Task Init()
        //{
        //    _dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "opms.db3");
        //    List<Ticket> ticketList = new();
        //    var cultureInfo = new CultureInfo("en-US");
        //    if (conn != null)
        //        return;

        //    conn = new SQLiteAsyncConnection(_dbPath);

        //    await conn.CreateTableAsync<Ticket>();

        //    //using var stream = await FileSystem.OpenAppPackageFileAsync("ticketdata.json");
        //    //using var reader = new StreamReader(stream);
        //    //var contents = await reader.ReadToEndAsync();
        //    //ticketList = JsonSerializer.Deserialize<List<Ticket>>(contents);

        //    ticketList.Add(new Ticket(null, 1, "Hourly", "Hourly Ticket", DateTime.Parse("05-12-2022 16:01:01", cultureInfo, System.Globalization.DateTimeStyles.NoCurrentDateDefault),
        //        null, "Open", 0.00, 0.00, 0.00, 0.00, "admin"));

        //    //foreach (Ticket ticket in ticketList)
        //    //    await AddNewTicket(ticket);
        //}

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
                    "Type NVCARCHAR(50)," +
                    "Description NVARCHAR(50)," +
                    "Created DATETIME," +
                    "Closed DATETIME," +
                    "Status NVARCHAR(50)," +
                    "CustomerId INTERGER," +
                    "Registration NVARCHAR(50)," +
                    "Quantity INTEGER," +
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
                    "Type NVARCHAR(25), " +
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
                    "Quantity INTEGER, " +
                    "Rate REAL, " +
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
            string type = "Hourly";
            string description = "Hourly Ticket";
            var created = DateTime.Parse("05-12-2022 16:01:01", cultureInfo, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
            //var closed = default(DateTime);
            string status = "Paid";
            int customerId = 1;
            string registration = "P1234";
            int quantity = 0;
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
            int[] ttQuantity = { 1, 2, 1, 1 };
            double[] ttRate = { 2.50, 10.00, 50.00, 200.00 };
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
                insertCommand.CommandText = "INSERT INTO Tickets VALUES (NULL, @Type, @Description, @Created, NULL, @Status, @CustomerId, @Registration, @Quantity, @Rate, @Cost, @PayAmount, @Balance, @User);";
                //insertCommand.Parameters.AddWithValue("@Id", id);                
                insertCommand.Parameters.AddWithValue("@Type", type);
                insertCommand.Parameters.AddWithValue("@Description", description);
                insertCommand.Parameters.AddWithValue("@Created", created);
                //insertCommand.Parameters.AddWithValue("@Closed", closed);
                insertCommand.Parameters.AddWithValue("@Status", status);
                insertCommand.Parameters.AddWithValue("@CustomerId", customerId);
                insertCommand.Parameters.AddWithValue("@Registration", registration);
                insertCommand.Parameters.AddWithValue("@Quantity", quantity);
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
                insertUserCommand.CommandText = "INSERT INTO Users VALUES (NULL, @Username, @Password, @Salt, @FirstName, @LastName, @Type, @Status, NULL);";
                //insertUserCommand.Parameters.AddWithValue("@Id", userId);
                insertUserCommand.Parameters.AddWithValue("@Username", username);
                insertUserCommand.Parameters.AddWithValue("@Password", password);
                insertUserCommand.Parameters.AddWithValue("@Salt", salt);
                insertUserCommand.Parameters.AddWithValue("@FirstName", firstName);
                insertUserCommand.Parameters.AddWithValue("@LastName", lastName);
                insertUserCommand.Parameters.AddWithValue("@Type", userType);
                insertUserCommand.Parameters.AddWithValue("@Status", userStatus);
                //insertUserCommand.Parameters.AddWithValue("@LastLogin", lastLogin);

                insertUserCommand.ExecuteReader();
              

                //Ticket Type
                SqliteCommand insertTypeCommand = new SqliteCommand();
                insertTypeCommand.Connection = dbconn;

                for (int i = 0; i < ttId.Length; i++)
                {
                    // Use parameterized query to prevent SQL injection attacks
                    insertTypeCommand.CommandText = "INSERT INTO TicketType VALUES (@Id, @TicketType, @Description, @Quantity, @Rate, @Status, @Username, @ActivityDate);";
                    insertTypeCommand.Parameters.AddWithValue("@Id", ttId[i]);
                    insertTypeCommand.Parameters.AddWithValue("@TicketType", ticketType[i]);
                    insertTypeCommand.Parameters.AddWithValue("@Description", ttDescription[i]);
                    insertTypeCommand.Parameters.AddWithValue("@Quantity", ttQuantity[i]);
                    insertTypeCommand.Parameters.AddWithValue("@Rate", ttRate[i]);
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

                string selectCMD = "SELECT User, Id from Tickets";
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
                if (ticket.Type == "Hourly")
                    insertCommand.CommandText = "INSERT INTO Tickets VALUES (NULL, @Type, @Description, @Created, NULL, @Status, @CustomerId, @Registration, @Quantity, @Rate, @Cost, @PayAmount, @Balance, @User);";
                else
                    insertCommand.CommandText = "INSERT INTO Tickets VALUES (NULL, @Type, @Description, @Created, @Closed, @Status, @CustomerId, @Registration, @Quantity, @Rate, @Cost, @PayAmount, @Balance, @User);";
                //insertCommand.Parameters.AddWithValue("@Id", ticket.Id);                
                insertCommand.Parameters.AddWithValue("@Type", ticket.Type);
                insertCommand.Parameters.AddWithValue("@Description", ticket.Description);
                insertCommand.Parameters.AddWithValue("@Created", ticket.Created);
                insertCommand.Parameters.AddWithValue("@Closed", ticket.Closed);
                insertCommand.Parameters.AddWithValue("@Status", ticket.Status);
                insertCommand.Parameters.AddWithValue("@CustomerId", ticket.CustomerId);
                insertCommand.Parameters.AddWithValue("@Registration", ticket.Registration);
                insertCommand.Parameters.AddWithValue("@Quantity", ticket.Quantity);
                insertCommand.Parameters.AddWithValue("@Rate", ticket.Rate);
                insertCommand.Parameters.AddWithValue("@Cost", ticket.Cost);
                insertCommand.Parameters.AddWithValue("@PayAmount", ticket.PayAmount);
                insertCommand.Parameters.AddWithValue("@Balance", ticket.Balance);
                insertCommand.Parameters.AddWithValue("@User", ticket.User);

                insertCommand.ExecuteReader();

                dbconn.Close();
            }

        }

        public static Ticket CreateTicket(string? ticketDescription, int customer, string reg)
        {
            Ticket ticket;
            TicketType ticketType;
            int customerId = customer;
            string registration = reg;

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                //Find the relevant ticket type to determine the releveant details
                SqliteCommand selectTypeCommand = new SqliteCommand();
                selectTypeCommand.Connection = dbconn;

                selectTypeCommand.CommandText = "SELECT * FROM TicketType WHERE Description = @Description AND Status = @Status ORDER BY Id ASC LIMIT 1;";
                selectTypeCommand.Parameters.AddWithValue("@Description", ticketDescription);
                selectTypeCommand.Parameters.AddWithValue("@Status", "Active");
                SqliteDataReader query1 = selectTypeCommand.ExecuteReader();

                query1.Read();
                ticketType = new TicketType(Int32.Parse(query1.GetString(0)), query1.GetString(1), query1.GetString(2), Int32.Parse(query1.GetString(3)),
                    Double.Parse(query1.GetString(4)), query1.GetString(5), query1.GetString(6), DateTime.Parse(query1.GetString(7)));

                
                
                //Find the last created ticket number of the specified type to create new ticket
                SqliteCommand selectTicketCommand = new SqliteCommand();
                selectTicketCommand.Connection = dbconn;

                selectTicketCommand.CommandText = "SELECT Id FROM Tickets ORDER BY Id DESC LIMIT 1;";
                //selectTicketCommand.Parameters.AddWithValue("@Type", ticketType.Type);
                SqliteDataReader query2 = selectTicketCommand.ExecuteReader();

                query2.Read();
                int ticketId = Int32.Parse(query2.GetString(0)) + 1;

                if (ticketType.Type == "Hourly")
                {
                   ticket = new Ticket(ticketId, ticketType.Type, ticketType.Description, DateTime.Now, null, "Open", 0, "0", ticketType.Quantity, 
                       ticketType.Rate, float.Parse("0.0"), float.Parse("0.0"), float.Parse("0.0"), App.LoggedInUser.Username);
                }
                else
                {
                    //DateTime closed = ticketType.GetEndDate();
                    ticket = new Ticket(ticketId, ticketType.Type, ticketType.Description, DateTime.Now, null, "Open", customerId, registration,
                        ticketType.Quantity, ticketType.Rate, ticketType.Rate, float.Parse("0.0"), ticketType.Rate, App.LoggedInUser.Username);
                }               

                dbconn.Close();
            }

            return ticket;
        }

        public static void PayTicket(Ticket ticket)
        {
            //var cultureInfo = new CultureInfo("en-US");
            //ticket.Status = "Paid";

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                SqliteCommand updateCommand = new SqliteCommand();
                updateCommand.Connection = dbconn;

                // Use parameterized query to prevent SQL injection attacks
                updateCommand.CommandText = "UPDATE Tickets SET Closed = @Closed, Status = @Status, Cost = @Cost, PayAmount = @PayAmount, Balance = @Balance WHERE Id = @Id;";
                //updateCommand.Parameters.AddWithValue("@Id", ticket.Id);
                updateCommand.Parameters.AddWithValue("@Closed", ticket.Closed);
                updateCommand.Parameters.AddWithValue("@Status", ticket.Status);
                updateCommand.Parameters.AddWithValue("@Cost", ticket.Cost);
                updateCommand.Parameters.AddWithValue("@PayAmount", ticket.PayAmount);
                updateCommand.Parameters.AddWithValue("@Balance", ticket.Balance);
                updateCommand.Parameters.AddWithValue("@Number", ticket.Id);

                updateCommand.ExecuteReader();

                dbconn.Close();
            }
        }

        public static Ticket FindTicket(int id)
        {
            Ticket ticket = null;
            //int gracePeriod = 5;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = dbconn;

                // Use parameterized query to prevent SQL injection attacks
                selectCommand.CommandText = "SELECT * FROM Tickets WHERE Id = @Id LIMIT 1;";
                selectCommand.Parameters.AddWithValue("@Id", id);

                SqliteDataReader query = selectCommand.ExecuteReader();

                query.Read();
                if (query.HasRows)
                {
                    if (query[4].GetType() == typeof(DBNull))
                    {
                        ticket = new Ticket(Int32.Parse(query.GetString(0)), query.GetString(1), query.GetString(2), DateTime.Parse(query.GetString(3)), null, query.GetString(5), 
                            Int32.Parse(query.GetString(6)), query.GetString(7), Int32.Parse(query.GetString(8)), float.Parse(query.GetString(9)), float.Parse(query.GetString(10)), 
                            float.Parse(query.GetString(11)), float.Parse(query.GetString(12)), query.GetString(13));
                    } else
                    {
                        ticket = new Ticket(Int32.Parse(query.GetString(0)), query.GetString(1), query.GetString(2), DateTime.Parse(query.GetString(3)), DateTime.Parse(query.GetString(4)), 
                            query.GetString(5), Int32.Parse(query.GetString(6)), query.GetString(7), Int32.Parse(query.GetString(8)), float.Parse(query.GetString(9)), 
                            float.Parse(query.GetString(10)), float.Parse(query.GetString(11)), float.Parse(query.GetString(12)), query.GetString(13));
                    }
                }
                dbconn.Close();
            }
            return ticket;
        }

        public static bool CheckTicket(int id)
        {
            bool isTicket;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = dbconn;

                // Use parameterized query to prevent SQL injection attacks
                selectCommand.CommandText = "SELECT * FROM Tickets WHERE Id = @Id AND Status = @Status LIMIT 1;";
                selectCommand.Parameters.AddWithValue("@Id", id);
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
                    if (query[4].GetType() == typeof(DBNull))
                    {
                        tickets.Add(new TicketViewModel(Int32.Parse(query.GetString(0)), query.GetString(1), query.GetString(2), 
                            DateTime.Parse(query.GetString(3)), null, query.GetString(5), query.GetString(7), float.Parse(query.GetString(10)), 
                            float.Parse(query.GetString(12)), query.GetString(13)));
                    }
                    else
                    {
                        tickets.Add(new TicketViewModel(Int32.Parse(query.GetString(0)), query.GetString(1), query.GetString(2),
                            DateTime.Parse(query.GetString(3)), DateTime.Parse(query.GetString(4)), query.GetString(5), query.GetString(7), 
                            float.Parse(query.GetString(10)), float.Parse(query.GetString(12)), query.GetString(13)));
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
                    if (query[4].GetType() == typeof(DBNull))
                    {
                        tickets.Add(new Ticket(Int32.Parse(query.GetString(0)), query.GetString(1), query.GetString(2), DateTime.Parse(query.GetString(3)), null, query.GetString(5),
                            Int32.Parse(query.GetString(6)), query.GetString(7), Int32.Parse(query.GetString(8)), float.Parse(query.GetString(9)), float.Parse(query.GetString(10)),
                            float.Parse(query.GetString(11)), float.Parse(query.GetString(12)), query.GetString(13)));
                    }
                    else
                    {
                        tickets.Add(new Ticket(Int32.Parse(query.GetString(0)), query.GetString(1), query.GetString(2), DateTime.Parse(query.GetString(3)), DateTime.Parse(query.GetString(4)),
                            query.GetString(5), Int32.Parse(query.GetString(6)), query.GetString(7), Int32.Parse(query.GetString(8)), float.Parse(query.GetString(9)),
                            float.Parse(query.GetString(10)), float.Parse(query.GetString(11)), float.Parse(query.GetString(12)), query.GetString(13)));
                    }
                }

                dbconn.Close();
            }

            return tickets;
        }

        public static Ticket GetTicketByReg(string registration)
        {
            Ticket ticket = null;
            //int gracePeriod = 5;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = dbconn;

                // Use parameterized query to prevent SQL injection attacks
                selectCommand.CommandText = "SELECT * FROM Tickets WHERE Registration = @registration LIMIT 1;";
                selectCommand.Parameters.AddWithValue("@Registration", registration);

                SqliteDataReader query = selectCommand.ExecuteReader();

                query.Read();
                if (query.HasRows)
                {
                    if (query[4].GetType() == typeof(DBNull))
                    {
                        ticket = new Ticket(Int32.Parse(query.GetString(0)), query.GetString(1), query.GetString(2), DateTime.Parse(query.GetString(3)), null, query.GetString(5),
                            Int32.Parse(query.GetString(6)), query.GetString(7), Int32.Parse(query.GetString(8)), float.Parse(query.GetString(9)), float.Parse(query.GetString(10)),
                            float.Parse(query.GetString(11)), float.Parse(query.GetString(12)), query.GetString(13));
                    }
                    else
                    {
                        ticket = new Ticket(Int32.Parse(query.GetString(0)), query.GetString(1), query.GetString(2), DateTime.Parse(query.GetString(3)), DateTime.Parse(query.GetString(4)),
                            query.GetString(5), Int32.Parse(query.GetString(6)), query.GetString(7), Int32.Parse(query.GetString(8)), float.Parse(query.GetString(9)),
                            float.Parse(query.GetString(10)), float.Parse(query.GetString(11)), float.Parse(query.GetString(12)), query.GetString(13));
                    }
                }
                dbconn.Close();
            }
            return ticket;
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
                insertCommand.CommandText = "INSERT INTO Users VALUES (NULL, @Username, @Password, @Salt, @FirstName, @LastName, @Type, @Status, NULL);";
                //insertCommand.Parameters.AddWithValue("@Id", newUser.Id);
                insertCommand.Parameters.AddWithValue("@Username", newUser.Username);
                insertCommand.Parameters.AddWithValue("@Password", newUser.Password);
                insertCommand.Parameters.AddWithValue("@Salt", newUser.Salt);
                insertCommand.Parameters.AddWithValue("@FirstName", newUser.FirstName);
                insertCommand.Parameters.AddWithValue("@LastName", newUser.LastName);
                insertCommand.Parameters.AddWithValue("@Type", newUser.Type);
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
                updateCommand.CommandText = "UPDATE Users SET Username=@Username, Password=@Password, Salt=@Salt, FirstName=@FirstName, LastName=@LastName, Type=@Type, LastLogin=@LastLogin WHERE Id=@Id;";
                updateCommand.Parameters.AddWithValue("@Username", updateUser.Username);
                updateCommand.Parameters.AddWithValue("@Password", updateUser.Password);
                updateCommand.Parameters.AddWithValue("@Salt", updateUser.Salt);
                updateCommand.Parameters.AddWithValue("@FirstName", updateUser.FirstName);
                updateCommand.Parameters.AddWithValue("@LastName", updateUser.LastName);
                updateCommand.Parameters.AddWithValue("@Type", updateUser.Type);
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

        public static User GetUser(int userId)
        {
            User user;

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = dbconn;

                selectCommand.CommandText = "SELECT * from Users WHERE Id=@Id";
                selectCommand.Parameters.AddWithValue("@Id", userId);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    if (query[8].GetType() == typeof(DBNull))
                    {
                        user = new User(Int32.Parse(query.GetString(0)), query.GetString(1), query.GetString(2),
                            query.GetString(3), query.GetString(4), query.GetString(5), query.GetString(6),
                            query.GetString(7), null);
                        dbconn.Close();
                        return user;
                    }
                    else
                    {
                        user = new User(Int32.Parse(query.GetString(0)), query.GetString(1), query.GetString(2),
                            query.GetString(3), query.GetString(4), query.GetString(5), query.GetString(6),
                            query.GetString(7), DateTime.Parse(query.GetString(8)));
                        dbconn.Close();
                        return user;
                    }
                }
                dbconn.Close();
            }
            return null;
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

        public static ObservableCollection<TicketTypeViewModel> GetTicketTypeList(string status)
        {
            ObservableCollection<TicketTypeViewModel> ticketTypes = new ObservableCollection<TicketTypeViewModel>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = dbconn;

                if (status == "All")
                    selectCommand.CommandText = "SELECT * FROM TicketType;";
                else
                {
                    selectCommand.CommandText = "SELECT * FROM TicketType WHERE Status = @Status;";
                    selectCommand.Parameters.AddWithValue("@Status", status);
                }


                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    ticketTypes.Add(new TicketTypeViewModel(Int32.Parse(query.GetString(0)), query.GetString(1), query.GetString(2),
                        Int32.Parse(query.GetString(3)), float.Parse(query.GetString(4)), query.GetString(5), query.GetString(6), DateTime.Parse(query.GetString(7))));
                }

                dbconn.Close();
            }

            return ticketTypes;
        }        

        public static void AddTicketType(TicketType ticketType)
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
                insertCommand.CommandText = "INSERT INTO TicketType VALUES (NULL, @Type, @Description, @Quantity, @Rate, @Status, @User, @ActivityDate);";
                //insertCommand.Parameters.AddWithValue("@Id", ticket.Id);
                insertCommand.Parameters.AddWithValue("@Type", ticketType.Type);
                insertCommand.Parameters.AddWithValue("@Description", ticketType.Description);
                insertCommand.Parameters.AddWithValue("@Quantity", ticketType.Quantity);
                insertCommand.Parameters.AddWithValue("@UnitCost", ticketType.Rate);
                insertCommand.Parameters.AddWithValue("@Status", ticketType.Status);
                insertCommand.Parameters.AddWithValue("@User", ticketType.User);
                insertCommand.Parameters.AddWithValue("@ActivityDate", ticketType.ActivityDate);                

                insertCommand.ExecuteReader();

                dbconn.Close();
            }

        }
    }
}
