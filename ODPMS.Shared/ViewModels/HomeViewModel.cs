
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
        List<string> suggestionList;

        [ObservableProperty]
        AutoSuggestBox chosenSuggestionTxt;

        public HomeViewModel()
        {
            Title = "Home";
            Init();
        }

        private void Init()
        {
            // Check float for first daily login
            if (App.LoggedInUser.Username != "admin")
                CheckForFloat(App.LoggedInUser.Username);
            
            WelcomeMessage = $"Welcome {App.LoggedInUser.FirstName}!";

            Refresh();
        }

        private async void Refresh()
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
                        SearchList.Add($"{ticket.Id.ToString()} - {ticket.Registration.ToString()}");
                }
            }

            SearchList.Reverse();

            if (TicketList.Count != 0)
                VisibleTicketList = Visibility.Visible;
            else
                VisibleTicketList = Visibility.Collapsed;

            if (OtherTicketList.Count != 0)
                VisibleOtherTicketList = Visibility.Visible;
            else
                VisibleOtherTicketList = Visibility.Collapsed;

            DailyTicketsHeader = $"Daily Tickets ({TicketList.Count})";
            OtherTicketsHeader = $"Daily Tickets ({OtherTicketList.Count})";
        }

        private async void CheckForFloat(string userId)
        {

            var today = DateTime.Now.ToString("yyyy-MM-dd");
            await App.Database.Init();
            var query = App.Database.Current.Table<CashFloat>().Where(v => v.User.Equals(userId) && v.Created.Equals(today));
            var StatusMessage = string.Format("{0} record(s) found in the ticket table)", await query.CountAsync());
            Debug.WriteLine(StatusMessage);

            if (await query.CountAsync() == 0)
            {
                ContentDialog floatDialog = new CashFloatContentDialog();
                floatDialog.XamlRoot = (Application.Current as App)?.Window.Content.XamlRoot;
                await floatDialog.ShowAsync();
            }
        }        

        [RelayCommand]
        private async void NewTicket()
        {
            // Display the new ticket dialog
            ContentDialog ticketDialog = new NewTicketContentDialog();
            ticketDialog.XamlRoot = (Application.Current as App)?.Window.Content.XamlRoot;
            await ticketDialog.ShowAsync();
            Refresh();
        }

        [RelayCommand]
        private async void OtherTickets()
        {
            // Display the new ticket dialog
            ContentDialog ticketDialog = new OtherTicketsContentDialog();
            ticketDialog.XamlRoot = (Application.Current as App)?.Window.Content.XamlRoot;
            await ticketDialog.ShowAsync();
            Refresh();
        }

        [RelayCommand]
        private async void PayTicket()
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
                        ValidTicketMessage = $"This ticket has already been paid.";
                    else if (checkTicket.Status == "Closed")
                        ValidTicketMessage = $"This ticket is no longer valid.";

                    else
                    {
                        ContentDialog payDialog = new PayTicketContentDialog(ticketNumber);
                        payDialog.XamlRoot = (Application.Current as App)?.Window.Content.XamlRoot;
                        await payDialog.ShowAsync();

                        Refresh();

                        TicketNumber = "";
                        SelectedTicket = null;
                    }
                }
                else
                {
                    ValidTicketMessage = $"The ticket number you entered does not exist or is not open.";
                }
            }
            else
            {
                ValidTicketMessage = $"That was not a valid ticket number.";
            }
        }

        [RelayCommand]
        private async void DeleteTicket(Ticket ticket)
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
                Refresh();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to select the ticket: {ex.Message}");
                //await Shell.Current.DisplayAlert("Error", $"Unable to select the ticket: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async void SelectTicket()
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

        [RelayCommand]            
        private void TextChanged()
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

        [RelayCommand]
        private void SearchSuggestionChosen(AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            TicketNumber = args.SelectedItem.ToString();
        }

        [RelayCommand]
        private async void SearchQuerySubmitted()
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

                    Refresh();

                    TicketNumber = "";
                    SelectedTicket = null;
                }
            }
        }
    }
}
