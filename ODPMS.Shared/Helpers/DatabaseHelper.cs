﻿using System;
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
                    "Tickets (Id INTEGER PRIMARY KEY, " +
                    "Number INTEGER," +
                    "Type NVCARCHAR(50)," +
                    "Description NVARCHAR(50)," +
                    "Created DATETIME," +
                    "Closed DATETIME," +
                    "Status NVARCHAR(50)," +
                    "Rate FLOAT," +
                    "Cost FLOAT," +
                    "Balance FLOAT," +
                    "User NVARCHAR(10))";
    
                SqliteCommand createTable = new SqliteCommand(tableCommand, dbconn);
                createTable.ExecuteReader();

                //Users Table
                tableCommand =
                    "CREATE TABLE IF NOT EXISTS " +
                    "Users (Id INTEGER PRIMARY KEY AUTO_INCREMENT, "  +
                    "Username NVARCHAR(25) NOT NULL, " +
                    "Password NVARCHAR(255), " +
                    "Salt NVARCHAR(25), " + 
                    "FirstName NVARCHAR(25), " +
                    "LastName NVARCHAR(25), " +
                    "UserType NVARCHAR(25), " +
                    "LastLogin DATETIME);";

                createTable = new SqliteCommand(tableCommand, dbconn);
                createTable.ExecuteReader();

                dbconn.Close();
            }
            //AddData();
        }

        public static void AddData()
        {
            var cultureInfo = new CultureInfo("en-US");

            int id = 1;
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
                insertCommand.Parameters.AddWithValue("@Id", id);
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
                insertCommand.CommandText = "INSERT INTO Tickets VALUES (@Id, @Number, @Type, @Description, @Created, @Closed, @Status, @Rate, @Cost, @Balance, @User);";
                insertCommand.Parameters.AddWithValue("@Id", ticket.Id);
                insertCommand.Parameters.AddWithValue("@Number", ticket.Number);
                insertCommand.Parameters.AddWithValue("@Type", ticket.Type);
                insertCommand.Parameters.AddWithValue("@Description", ticket.Description);
                insertCommand.Parameters.AddWithValue("@Created", ticket.Created);
                insertCommand.Parameters.AddWithValue("@Closed", ticket.Closed);
                insertCommand.Parameters.AddWithValue("@Status", ticket.Status);
                insertCommand.Parameters.AddWithValue("@Rate", ticket.Rate);
                insertCommand.Parameters.AddWithValue("@Cost", ticket.Cost);
                insertCommand.Parameters.AddWithValue("@Balance", ticket.Balance);
                insertCommand.Parameters.AddWithValue("@User", ticket.User);

                insertCommand.ExecuteReader();

                dbconn.Close();
            }

        }

        public static void PayTicket(Ticket ticket)
        {
            //var cultureInfo = new CultureInfo("en-US");

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                SqliteCommand updateCommand = new SqliteCommand();
                updateCommand.Connection = dbconn;

                // Use parameterized query to prevent SQL injection attacks
                updateCommand.CommandText = "UPDATE Tickets SET (Closed = @Closed, Status = @Status, Cost = @Cost, Balance = @Balance, User = @User) WHERE (Number = @Number);";
                //updateCommand.Parameters.AddWithValue("@Id", ticket.Id);
                updateCommand.Parameters.AddWithValue("@Closed", ticket.Closed);
                updateCommand.Parameters.AddWithValue("@Status", ticket.Status);
                updateCommand.Parameters.AddWithValue("@Cost", ticket.Cost);
                updateCommand.Parameters.AddWithValue("@Balance", ticket.Balance);
                updateCommand.Parameters.AddWithValue("@User", ticket.User);
                updateCommand.Parameters.AddWithValue("@Number", ticket.Number);

                updateCommand.ExecuteReader();

                dbconn.Close();
            }
        }

        public static ObservableCollection<Ticket> GetTicketListViewData()
        {
            ObservableCollection<Ticket> tickets = new ObservableCollection<Ticket>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "odpms_data.db");
            using (SqliteConnection dbconn = new SqliteConnection($"Filename={dbpath}"))
            {
                dbconn.Open();

                string selectCMD = "SELECT * from Tickets";
                SqliteCommand selectCommand = new SqliteCommand(selectCMD, dbconn);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    tickets.Add(new Ticket(Int32.Parse(query.GetString(0)), Int32.Parse(query.GetString(1)), query.GetString(2),
                        query.GetString(3), DateTime.Parse(query.GetString(4)), DateTime.Parse(query.GetString(5)), query.GetString(6),
                        float.Parse(query.GetString(7)), float.Parse(query.GetString(8)), float.Parse(query.GetString(8)), query.GetString(10)));
                    //tickets.Add(new Ticket(query.GetString(0)));
                }

                dbconn.Close();
            }

            return tickets;
        }
    }
}
