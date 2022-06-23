using CommunityToolkit.WinUI.UI.Controls;

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

        [ObservableProperty]
        Visibility visibleTicketList;

        [ObservableProperty]
        Visibility visibleOtherTicketList;

        [ObservableProperty]
        bool canDelete;

        public HomeViewModel()
        {
            Title = "Home";
            Init();
        }

        async void Init()
        {
            var tickets = await Ticket.GetAllTickets();

            if (TicketList.Count != 0)
                TicketList.Clear();
            if (OtherTicketList.Count != 0)
                OtherTicketList.Clear();

            foreach (var ticket in tickets)
            {
                if (ticket.Type == "Hourly" && ticket.Status == "Open")
                {
                    ticket.UpdateCost();
                    ticket.UpdateDeletable();
                    TicketList.Add(ticket);
                }
                else if (ticket.Type != "Hourly" && ticket.Status != "Delete" && ticket.Closed >= DateTime.Now)
                    OtherTicketList.Add(ticket);
            }

            if (TicketList.Count != 0)
                VisibleTicketList = Visibility.Visible;
            else
                VisibleTicketList = Visibility.Collapsed;
            
            if (OtherTicketList.Count != 0)
                VisibleOtherTicketList = Visibility.Visible;
            else
                VisibleOtherTicketList = Visibility.Collapsed;

            WelcomeMessage = String.Format("Welcome {0}!", App.LoggedInUser.FirstName);
        }

        [ICommand]
        async void NewTicket()
        {
            // Display the new ticket dialog
            ContentDialog ticketDialog = new NewTicketContentDialog();
            ticketDialog.XamlRoot = (Application.Current as App)?.Window.Content.XamlRoot;
            await ticketDialog.ShowAsync();
            Init();
        }

        [ICommand]
        async void OtherTickets()
        {
            // Display the new ticket dialog
            ContentDialog ticketDialog = new OtherTicketsContentDialog();
            ticketDialog.XamlRoot = (Application.Current as App)?.Window.Content.XamlRoot;
            await ticketDialog.ShowAsync();
            Init();
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
                Ticket checkTicket = await Ticket.GetTicket(ticketNumber);
                if (checkTicket.Id > 0)
                {
                    if (checkTicket.Status == "Paid")
                        ValidTicketMessage = string.Format("This ticket has already been paid.");
                    else if (checkTicket.Status == "Closed")
                        ValidTicketMessage = string.Format("This ticket is no longer valid.");

                    else
                    {
                        ContentDialog payDialog = new PayTicketContentDialog(ticketNumber);
                        payDialog.XamlRoot = (Application.Current as App)?.Window.Content.XamlRoot;
                        await payDialog.ShowAsync();

                        Init();

                        TicketNumber = "";
                        SelectedTicket = null;
                    }
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
        async void DeleteTicket(Ticket ticket)
        {
            if (IsBusy)
                return;
            
            if (ticket == null)
                return;

            try
            {
                IsBusy = true;
                TimeSpan ts = DateTime.Now - ticket.Created;
                if (ts.TotalMinutes <= 5)
                {
                    ticket.IsDeletable = false;
                    ticket.Updated = DateTime.Now;
                    ticket.UpdatedBy = App.LoggedInUser.Username;
                    await Ticket.DeleteTicket(ticket);
                }
                TicketNumber = "";
                Init();
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

        [ICommand]
        async void SelectTicket()
        {
            if (IsBusy)
                return;

            if (SelectedTicket == null)
                return;

            try
            {
                IsBusy = true;
                int ticketNumber = SelectedTicket.Id;
                TicketNumber = ticketNumber.ToString();

                // Allow ticket to be deleted if within 5 minutes
                TimeSpan ts = DateTime.Now - SelectedTicket.Created;
                if (ts.TotalMinutes > 5)
                {
                    SelectedTicket.IsDeletable = false;
                    await Ticket.UpdateTicket(SelectedTicket);
                }
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
