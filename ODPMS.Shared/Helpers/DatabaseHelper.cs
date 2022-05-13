using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
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

                String tableCommand = 
                    "CREATE TABLE IF NOT EXISTS " +
                    "Tickets (Id INTEGER PRIMARY KEY, " +
                             "Number INTEGER," +
                             "Type NVCARCHAR(50)," +
                             "Description NVARCHAR(50)," +
                             "Created DateTime," +
                             "Closed DateTime," +
                             "Status NVARCHAR(50)," +
                             "Rate FLOAT," +
                             "Cost FLOAT," +
                             "Balance FLOAT," +
                             "User NVARCHAR(10))";
    
                SqliteCommand createTable = new SqliteCommand(tableCommand, dbconn);

                createTable.ExecuteReader();
                dbconn.Close();
            }
        }

        public static void AddData()
        {
            var cultureInfo = new CultureInfo("en-US");            

            int number = 1;
            string type = "Hour";
            string description = "Hourly Ticket";
            var created = DateTime.Parse("05-11-2022 16:01:01", cultureInfo, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
            var closed = DateTime.Parse("05-11-2022 18:25:01", cultureInfo, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
            string status = "Closed";
            double rate = 3.50;
            double cost = 10.00;
            double balance = 0.0;
            string user = "test";
            
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = dbconn;

                // Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "INSERT INTO Tickets VALUES (@Id, @Number, @Type, @Description, @Created, @Closed, @Status, @Rate, @Cost, @Balance, @User);";
                insertCommand.Parameters.AddWithValue("@Id", number);
                insertCommand.Parameters.AddWithValue("@Number", number);
                insertCommand.Parameters.AddWithValue("@Type", type);
                insertCommand.Parameters.AddWithValue("@Description", description);
                insertCommand.Parameters.AddWithValue("@Created", created);
                insertCommand.Parameters.AddWithValue("@Closed", closed);
                insertCommand.Parameters.AddWithValue("@Status", status);
                insertCommand.Parameters.AddWithValue("@Rate", rate);
                insertCommand.Parameters.AddWithValue("@Cost", cost);
                insertCommand.Parameters.AddWithValue("@Balance", balance);
                insertCommand.Parameters.AddWithValue("@User", user);

                insertCommand.ExecuteReader();

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

                string selectCMD = "SELECT User from Tickets";
                SqliteCommand selectCommand = new SqliteCommand(selectCMD, dbconn);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    entries.Add(query.GetString(0));
                }

                dbconn.Close();
            }

            return entries;
        }
    }
}
