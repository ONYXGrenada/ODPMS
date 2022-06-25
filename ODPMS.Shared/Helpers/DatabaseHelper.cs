

namespace ODPMS.Helpers
{
    public class DatabaseHelper
    {
        private readonly string _databasePath;
        private SQLiteAsyncConnection _database;

        public SQLiteAsyncConnection Current { get { return _database; } }

        public DatabaseHelper(string dbPath)
        {
            _databasePath = dbPath;
        }
        
        public async Task Init()
        {
            Type[] tables = {
                typeof(Ticket),
                typeof(User),
                typeof(TicketType),
                typeof(Receipt),
                typeof(CashFloat)
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
            admin.IsDeletable = false;

            var queryUser = _database.Table<User>().Where(v => v.Username == "admin");
            if (await queryUser.CountAsync() == 0)
                await _database.InsertAsync(admin);

            List<TicketType> ticketTypes = new();
            TicketType ticketTypeH = new TicketType();
            ticketTypeH.Type = "Hourly";
            ticketTypeH.Description = "Hourly Ticket";
            ticketTypeH.Period = 1;
            ticketTypeH.Rate = 2.50;
            ticketTypeH.Status = "Active";
            ticketTypeH.User = "admin";
            ticketTypeH.Created = DateTime.Now;
            ticketTypeH.Updated = DateTime.Now;
            ticketTypeH.UpdatedBy = "admin";
            ticketTypes.Add(ticketTypeH);

            TicketType ticketTypeD = new TicketType();
            ticketTypeD.Type = "Daily";
            ticketTypeD.Description = "Daily Ticket";
            ticketTypeD.Period = 1;
            ticketTypeD.Rate = 10.00;
            ticketTypeD.Status = "Active";
            ticketTypeD.User = "admin";
            ticketTypeD.Created = DateTime.Now;
            ticketTypeD.Updated = DateTime.Now;
            ticketTypeD.UpdatedBy = "admin";
            ticketTypes.Add(ticketTypeD);

            TicketType ticketTypeW = new TicketType();
            ticketTypeW.Type = "Weekly";
            ticketTypeW.Description = "Weekly Ticket";
            ticketTypeW.Period = 1;
            ticketTypeW.Rate = 50.00;
            ticketTypeW.Status = "Active";
            ticketTypeW.User = "admin";
            ticketTypeW.Created = DateTime.Now;
            ticketTypeW.Updated = DateTime.Now;
            ticketTypeW.UpdatedBy = "admin";
            ticketTypes.Add(ticketTypeW);

            TicketType ticketTypeM = new TicketType();
            ticketTypeM.Type = "Monthly";
            ticketTypeM.Description = "Monthly Ticket";
            ticketTypeM.Period = 1;
            ticketTypeM.Rate = 200.00;
            ticketTypeM.Status = "Active";
            ticketTypeM.User = "admin";
            ticketTypeM.Created = DateTime.Now;
            ticketTypeM.Updated = DateTime.Now;
            ticketTypeM.UpdatedBy = "admin";
            ticketTypes.Add(ticketTypeM);

            foreach (var ticketType in ticketTypes)
            {
                var queryTicketType = _database.Table<TicketType>().Where(v => v.Type == ticketType.Type);
                if (await queryTicketType.CountAsync() == 0)
                    await _database.InsertAsync(ticketType);
            }
        }
    }
}
