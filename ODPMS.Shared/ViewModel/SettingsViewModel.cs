using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Windows.Storage;

namespace ODPMS.ViewModel
{
    public partial class SettingsViewModel : BaseViewModel
    {
        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(IsNotUserSelected))]
        bool isUserSelected;

        public bool IsNotUserSelected => !IsUserSelected;

        [ObservableProperty]
        Visibility visibleState;

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

        public ObservableCollection<UserViewModel> Users { get; } = new();
        public ObservableCollection<TicketTypeViewModel> TicketTypes { get; } = new();
        public static ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        public SettingsViewModel()
        {
            Title = "Settings";
            Init();
        }

        private void Init()
        {
            if (App.LoggedInUser.UserType == "admin")
            {
                VisibleState = Visibility.Visible;
                var users = DatabaseHelper.GetUsers();

                if (Users.Count != 0)
                    Users.Clear();

                foreach (var user in users)
                    Users.Add(user);

                var ticketTypes = DatabaseHelper.GetTicketTypeList("All");

                if (TicketTypes.Count != 0)
                    TicketTypes.Clear();

                foreach (var ticketType in ticketTypes)
                    TicketTypes.Add(ticketType);
            }
            else
            {
                VisibleState = Visibility.Collapsed;
            }
            FirstName = App.LoggedInUser.FirstName;
            LastName = App.LoggedInUser.LastName;

            GetCompanyData();
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
        }
    }
}
