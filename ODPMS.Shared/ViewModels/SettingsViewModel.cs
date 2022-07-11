﻿using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace ODPMS.ViewModels
{
    public partial class SettingsViewModel : BaseViewModel
    {
        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(IsNotUserSelected))]
        bool isUserSelected;

        public bool IsNotUserSelected => !IsUserSelected;

        [ObservableProperty]
        bool isTicketTypeSelected;

        [ObservableProperty]
        bool isVisible;

        [ObservableProperty]
        string firstName;

        [ObservableProperty]
        string lastName;

        [ObservableProperty]
        string companyName;

        [ObservableProperty]
        string companyAddress;

        [ObservableProperty]
        string companyEmail;

        [ObservableProperty]
        string companyPhone;

        [ObservableProperty]
        BitmapImage companyLogo;

        [ObservableProperty]
        User selectedUser;

        [ObservableProperty]
        TicketType selectedTicketType;

        [ObservableProperty]
        string ticketMessage;

        [ObservableProperty]
        string ticketDisclaimer;

        [ObservableProperty]
        bool defaultPrintReceipt;
        
        [ObservableProperty]
        string receiptMessage;

        [ObservableProperty]
        string receiptDisclaimer;

        public ObservableCollection<User> Users { get; } = new();
        public ObservableCollection<TicketType> TicketTypes { get; } = new();
        public static ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
        public static List<string> Statuses { get; } = new() { "Active", "Inactive" };
        public static List<string> UserTypes { get; } = new() { "admin", "user" };

        public SettingsViewModel()
        {
            Title = "Settings";
            Init();
        }

        private async void Init()
        {
            if (App.LoggedInUser.Type == "admin")
            {
                IsVisible = true;
                var users = await User.GetAllUsersDisplay();

                if (Users.Count != 0)
                    Users.Clear();

                foreach (var user in users)
                    Users.Add(user);

                var ticketTypes = await TicketType.GetAllTicketTypesDisplay();

                if (TicketTypes.Count != 0)
                    TicketTypes.Clear();

                foreach (var ticketType in ticketTypes)
                    TicketTypes.Add(ticketType);
            }
            else
            {
                IsVisible = false;
            }
            FirstName = App.LoggedInUser.FirstName;
            LastName = App.LoggedInUser.LastName;

            GetCompanyData();
            GetTicketSettings();
            GetReceiptSettings();
        }

        [ICommand]
        private async void GetUsers()
        {
            await User.GetAllUsers();
        }

        private void GetCompanyData()
        {
            if (LocalSettings.Values["CompanyName"] != null)
                CompanyName = LocalSettings.Values["CompanyName"] as string;

            if (LocalSettings.Values["CompanyAddress"] != null)
                CompanyAddress = LocalSettings.Values["CompanyAddress"] as string;

            if (LocalSettings.Values["CompanyEmail"] != null)
                CompanyEmail = LocalSettings.Values["CompanyEmail"] as string;

            if (LocalSettings.Values["CompanyPhone"] != null)
                CompanyPhone = LocalSettings.Values["CompanyPhone"] as string;

            if (LocalSettings.Values["CompanyLogo"] != null)
            {
                string clogo = LocalSettings.Values["CompanyLogo"] as string;
                if (File.Exists(ApplicationData.Current.LocalFolder.Path + "\\" + clogo))
                {
                    Uri resourceUri = new Uri(ApplicationData.Current.LocalFolder.Path + "\\" + clogo, UriKind.Relative);
                    CompanyLogo = new BitmapImage(resourceUri);
                }
            }

            else
            {
                Uri resourceUri2 = new Uri("ms-appx:///Assets/Images/logo-placeholder.png");
                CompanyLogo = new BitmapImage(resourceUri2);
            }

        }

        [ICommand]
        private void UpdateCompany()
        {
            LocalSettings.Values["CompanyName"] = CompanyName;
            LocalSettings.Values["CompanyAddress"] = CompanyAddress;
            LocalSettings.Values["CompanyEmail"] = CompanyEmail;
            LocalSettings.Values["CompanyPhone"] = CompanyPhone;
        }

        [ICommand]
        async void UpdateUser()
        {
            if (FirstName != App.LoggedInUser.FirstName || LastName != App.LoggedInUser.LastName)
            {
                App.LoggedInUser.FirstName = FirstName;
                App.LoggedInUser.LastName = LastName;
                await User.UpdateUser(App.LoggedInUser);

                var users = await User.GetAllUsersDisplay();

                if (Users.Count != 0)
                    Users.Clear();

                foreach (var user in users)
                    Users.Add(user);
            }
        }

        [ICommand]
        async void ResetPassword(User user)
        {
            if (user == null)
                user = App.LoggedInUser;
            
            // Display the reset password dialog
            if (user.Id == App.LoggedInUser.Id)
            {
                ContentDialog resetPasswordDialog = new ResetPasswordContentDialog(null);
                resetPasswordDialog.XamlRoot = (Application.Current as App)?.Window.Content.XamlRoot;
                await resetPasswordDialog.ShowAsync();
            }
            else
            {
                ContentDialog resetPasswordDialog = new ResetPasswordContentDialog(user.Id);
                resetPasswordDialog.XamlRoot = (Application.Current as App)?.Window.Content.XamlRoot;
                await resetPasswordDialog.ShowAsync();
            }
        }

        [ICommand]
        async void AddUser()
        {
            // Display the new user dialog
            ContentDialog newUserDialog = new NewUserContentDialog();
            newUserDialog.XamlRoot = (Application.Current as App)?.Window.Content.XamlRoot;
            await newUserDialog.ShowAsync();

            var users = await User.GetAllUsersDisplay();

            if (Users.Count != 0)
                Users.Clear();

            foreach (var user in users)
                Users.Add(user);
        }

        [ICommand]
        async void UpdateOtherUser()
        {
            User updatedUser = SelectedUser;
            await User.UpdateUser(updatedUser);

            if (updatedUser.Id == App.LoggedInUser.Id)
            {
                App.LoggedInUser = updatedUser;
                FirstName = App.LoggedInUser.FirstName;
                LastName = App.LoggedInUser.LastName;
            }

            var users = await User.GetAllUsersDisplay();

            if (Users.Count != 0)
                Users.Clear();

            foreach (var user in users)
                Users.Add(user);
        }

        [ICommand]
        async void DeleteUser(User deletedUser)
        {
            await User.DeleteUser(deletedUser);

            var users = await User.GetAllUsersDisplay();

            if (Users.Count != 0)
                Users.Clear();
            
            foreach (var user in users)
                Users.Add(user);
        }

        [ICommand]
        async void AdjustFloat()
        {
            // Display the cash float dialog
            ContentDialog cashFloat = new CashFloatContentDialog();
            cashFloat.XamlRoot = (Application.Current as App)?.Window.Content.XamlRoot;
            await cashFloat.ShowAsync();
        }

        [ICommand]
        async void AddTicketType()
        {
            // Display the new user dialog
            ContentDialog newTicketType = new NewTicketTypeContentDialog();
            newTicketType.XamlRoot = (Application.Current as App)?.Window.Content.XamlRoot;
            await newTicketType.ShowAsync();

            var ticketTypes = await TicketType.GetAllTicketTypesDisplay();

            if (TicketTypes.Count != 0)
                TicketTypes.Clear();

            foreach (var ticketType in ticketTypes)
                TicketTypes.Add(ticketType);
        }

        [ICommand]
        async void DeleteTicketType(TicketType selected)
        {
            await TicketType.DeleteTicketType(selected);

            var ticketTypes = await TicketType.GetAllTicketTypesDisplay();

            if (TicketTypes.Count != 0)
                TicketTypes.Clear();

            foreach (var ticketType in ticketTypes)
                TicketTypes.Add(ticketType);
        }

        [ICommand]
        async void UpdateTicketType()
        {
            TicketType updatedTicket = SelectedTicketType;
            await TicketType.UpdateTicketType(updatedTicket);

            var ticketTypes = await TicketType.GetAllTicketTypesDisplay();

            if (TicketTypes.Count != 0)
                TicketTypes.Clear();

            foreach (var ticketType in ticketTypes)
                TicketTypes.Add(ticketType);
        }

        [ICommand]
        private async void GetCompanyLogo()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".png");

            Window window = (Application.Current as App)?.Window;
            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(openPicker, hwnd);

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                File.Copy(file.Path, ApplicationData.Current.LocalFolder.Path + "\\clogo" + file.FileType, true);
                Uri resourceUri = new Uri(ApplicationData.Current.LocalFolder.Path + "\\clogo" + file.FileType, UriKind.Relative);
                CompanyLogo = new BitmapImage(resourceUri);

                LocalSettings.Values["CompanyLogo"] = "clogo" + file.FileType;
            }
            else
            {
                //OutputTextBlock.Text = "Operation cancelled.";
            }
        }

        private void GetTicketSettings()
        {
            if (LocalSettings.Values["TicketMessage"] != null)
                TicketMessage = LocalSettings.Values["TicketMessage"] as string;

            if (LocalSettings.Values["TicketDisclaimer"] != null)
                TicketDisclaimer = LocalSettings.Values["TicketDisclaimer"] as string;
        }

        [ICommand]
        private void UpdateTicketSettings()
        {
            LocalSettings.Values["TicketMessage"] = TicketMessage;
            LocalSettings.Values["TicketDisclaimer"] = TicketDisclaimer;
        }

        private void GetReceiptSettings()
        {
            if (LocalSettings.Values["DefaultPrintReceipt"] != null)
                DefaultPrintReceipt = (bool)LocalSettings.Values["DefaultPrintReceipt"];
            else
            {
                DefaultPrintReceipt = true;
                LocalSettings.Values["DefaultPrintReceipt"] = DefaultPrintReceipt;
            }

            if (LocalSettings.Values["ReceiptMessage"] != null)
                ReceiptMessage = LocalSettings.Values["ReceiptMessage"] as string;

            if (LocalSettings.Values["ReceiptDisclaimer"] != null)
                ReceiptDisclaimer = LocalSettings.Values["ReceiptDisclaimer"] as string;
        }

        [ICommand]
        private void UpdateReceiptSettings()
        {
            LocalSettings.Values["DefaultPrintReceipt"] = DefaultPrintReceipt;
            LocalSettings.Values["ReceiptMessage"] = ReceiptMessage;
            LocalSettings.Values["ReceiptDisclaimer"] = ReceiptDisclaimer;
        }

        [ICommand]
        void UserSelected()
        {
            if (SelectedUser != null)
                IsUserSelected = true;
            else
                IsUserSelected = false;
        }

        [ICommand]
        void TicketTypeSelected()
        {
            if (SelectedTicketType != null && SelectedTicketType.IsDeletable)
                IsTicketTypeSelected = true;
            else
                IsTicketTypeSelected = false;
        }
    }
}
