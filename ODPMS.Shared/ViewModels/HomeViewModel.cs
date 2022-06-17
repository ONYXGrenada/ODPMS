using System;
using System.Collections.Generic;
using System.Text;

namespace ODPMS.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        public ObservableCollection<Ticket> TicketList { get; } = new();
        public ObservableCollection<Ticket> OtherTicketList { get; } = new();

        [ObservableProperty]
        string welcomeMessage;

        [ObservableProperty]
        string validTicketMessage;

        [ObservableProperty]
        string ticketNumber;

        [ObservableProperty]
        Ticket selectedTicket;

        public HomeViewModel()
        {
            Title = "Home";
            Init();
        }

        async void Init()
        {
            //var tickets = DatabaseHelper.GetTicketListViewData(null);
            var tickets = await Ticket.GetAllTickets();

            if (TicketList.Count != 0)
                TicketList.Clear();
            if (OtherTicketList.Count != 0)
                OtherTicketList.Clear();

            foreach (var ticket in tickets)
            {
                if (ticket.Type == "Hourly" && ticket.Status == "Open")
                    TicketList.Add(ticket);
                else if (ticket.Type != "Hourly" && ticket.Closed >= DateTime.Now)
                    OtherTicketList.Add(ticket);
            }

            WelcomeMessage = String.Format("Welcome {0}!", App.LoggedInUser.FirstName);
        }

        [ICommand]
        async void NewTicket()
        {
            // Display the new ticket dialog
            ContentDialog ticketDialog = new NewTicketContentDialog();
            ticketDialog.XamlRoot = (Application.Current as App)?.Window.Content.XamlRoot;
            await ticketDialog.ShowAsync();
            //var tickets = DatabaseHelper.GetTicketListViewData("Open");
            var tickets = await Ticket.GetTicketsByStatus("Open");

            if (TicketList.Count != 0)
                TicketList.Clear();

            foreach (var ticket in tickets)
                TicketList.Add(ticket);
        }

        [ICommand]
        async void OtherTickets()
        {
            // Display the new ticket dialog
            ContentDialog ticketDialog = new OtherTicketsContentDialog();
            ticketDialog.XamlRoot = (Application.Current as App)?.Window.Content.XamlRoot;
            await ticketDialog.ShowAsync();
            //var tickets = DatabaseHelper.GetTicketListViewData("Open");
            var tickets = await Ticket.GetTicketsByStatus("Open");

            if (TicketList.Count != 0)
                TicketList.Clear();

            foreach (var ticket in tickets)
                TicketList.Add(ticket);
        }

        [ICommand]
        async void PayTicket()
        {
            // Display the pay ticket dialog
            ValidTicketMessage = "";
            int ticketNumber;
            if (Int32.TryParse(TicketNumber, out ticketNumber))
            {
                ticketNumber = Int32.Parse(TicketNumber);
                if (DatabaseHelper.CheckTicket(ticketNumber))
                {
                    ContentDialog payDialog = new PayTicketContentDialog(ticketNumber);
                    payDialog.XamlRoot = (Application.Current as App)?.Window.Content.XamlRoot;
                    await payDialog.ShowAsync();

                    //var tickets = DatabaseHelper.GetTicketListViewData("Open");
                    var tickets = await Ticket.GetTicketsByStatus("Open");

                    if (TicketList.Count != 0)
                        TicketList.Clear();

                    foreach (var ticket in tickets)
                        TicketList.Add(ticket);

                    TicketNumber = "";
                    SelectedTicket = null;
                }
                else
                {
                    ValidTicketMessage = string.Format("The ticket number you entered does not exist or is not open.");
                }
            }
            else
            {
                ValidTicketMessage = string.Format("That was not a valid ticket number.");
            }
        }

        [ICommand]
        void SelectTicket()
        {
            if (IsBusy)
                return;

            if (SelectedTicket == null)
                return;

            try
            {
                IsBusy = true;
                int ticketNumber = (int)SelectedTicket.Id;
                TicketNumber = ticketNumber.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to select the ticket: {ex.Message}");
                //await Shell.Current.DisplayAlert("Error", $"Unable to select the ticket: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                //IsRefreshing = false;
            }
        }
    }
}
