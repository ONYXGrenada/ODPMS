
using ODPMS.Models;
using System.Linq;

namespace ODPMS.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        public ObservableCollection<Ticket> TicketList { get; } = new();
        public ObservableCollection<Ticket> OtherTicketList { get; } = new();
        List<string> SearchList;

        [ObservableProperty]
        string welcomeMessage;
        
        [ObservableProperty]
        string validTicketMessage;
        
        [ObservableProperty]
        string ticketNumber;

        [ObservableProperty]
        string dailyTicketsHeader;

        [ObservableProperty]
        string otherTicketsHeader;

        [ObservableProperty]
        Ticket selectedTicket;

        [ObservableProperty]
        Visibility visibleTicketList;

        [ObservableProperty]
        Visibility visibleOtherTicketList;

        [ObservableProperty]
        bool canDelete;

        [ObservableProperty]
        List<string> suggestionList;

        [ObservableProperty]
        AutoSuggestBox chosenSuggestionTxt;

        public HomeViewModel()
        {
            Title = "Home";
            Init();
        }

        async void Init()
        {
            var tickets = await Ticket.GetAllTickets();
            SearchList = new List<string>();

            if (TicketList.Count != 0)
                TicketList.Clear();
            if (OtherTicketList.Count != 0)
                OtherTicketList.Clear();

            foreach (var ticket in tickets)
            {
                ticket.UpdateDeletable();
                if (ticket.Type == "Hourly" && ticket.Status == "Open")
                {
                    ticket.UpdateCost();
                    TicketList.Add(ticket);
                }
                else if (ticket.Type != "Hourly" && ticket.Status != "Delete" && ticket.Closed >= DateTime.Now)
                {
                    OtherTicketList.Add(ticket);
                }

                //Creates a list of tickets that can be searched
                //  This includes closed tickets and tickets that are no longer valid
                if (ticket.Status != "Delete")
                {
                    if (ticket.Type == "Hourly")
                        SearchList.Add(ticket.Id.ToString());
                    else
                        SearchList.Add( $"{ticket.Id.ToString()} - {ticket.Registration.ToString()}");
                }
            }

            if (TicketList.Count != 0)
                VisibleTicketList = Visibility.Visible;
            else
                VisibleTicketList = Visibility.Collapsed;
            
            if (OtherTicketList.Count != 0)
                VisibleOtherTicketList = Visibility.Visible;
            else
                VisibleOtherTicketList = Visibility.Collapsed;

            DailyTicketsHeader = String.Format("Daily Tickets ({0})", TicketList.Count);
            OtherTicketsHeader = String.Format("Other Tickets ({0})", OtherTicketList.Count);

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

        [ICommand]            
        public void TextChanged()
        {   
            SuggestionList = new List<string>();
            var splitText = TicketNumber.ToLower().Split(" ");
            foreach (var cat in SearchList)
            {
                var found = splitText.All((key) =>
                {
                    return cat.ToLower().Contains(key);
                });
                if (found)
                {
                    SuggestionList.Add(cat);
                }
            }
            if (SuggestionList.Count == 0)
            {
                SuggestionList.Add("No results found");
            }
            //sender.ItemsSource = SuggestionList;       

        }

        [ICommand]
        public void SearchSuggestionChosen(AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            TicketNumber = args.SelectedItem.ToString();
        }

        [ICommand]
        async void SearchQuerySubmitted()
        {
            ValidTicketMessage = "";
            int ticketNumber = 0;
            if (TicketNumber != null)
            {
                //Start if the testing area

                // Display the pay ticket dialog
                if (Int32.TryParse(TicketNumber, out ticketNumber))
                {
                    ticketNumber = Int32.Parse(TicketNumber);
                    Ticket checkTicket = await Ticket.GetTicket(ticketNumber);
                    if (checkTicket.Id > 0)
                    {
                        //If its a valid ticket
                    }
                    else
                    {
                        ValidTicketMessage = string.Format("The ticket number you entered is invalid.");
                    }
                }
                else
                {
                    foreach (Ticket ticket in OtherTicketList)
                    {
                        string[] parts = TicketNumber.Split('-');
                        Int32.TryParse(parts[0].Replace(" ", ""), out ticketNumber);
                        //if (ticket.Registration == TicketNumber)
                        //{
                        //    ticketNumber = ticket.Id;
                        //}
                    }

                   
                }

                //End of the testing area

                if (ticketNumber == 0)
                {
                    ValidTicketMessage = string.Format("The ticket number you entered is invalid.");
                }
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
        }
    }
}
