using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using ODPMS.Models;
using ODPMS.Helpers;
using ODPMS.Dialogs;
using System.Collections.ObjectModel;
using System.Reflection;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
using Microsoft.UI.Xaml.Media.Imaging;
using CommunityToolkit.WinUI.UI.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ODPMS.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public ObservableCollection<UserViewModel> Users { get; } = new();
        public ObservableCollection<TicketTypeViewModel> TicketTypes { get; } = new();
        public static ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
        public static StorageFile CLogoFile { get; set; }
        SettingsViewModel viewModel = null;

        public SettingsPage()
        {
            this.InitializeComponent();
            viewModel = new SettingsViewModel();
            DataContext = viewModel;
            GetSettingsData();
            GetCompanyData();
        }

        private void GetSettingsData()
        {
            if (App.LoggedInUser.UserType == "admin")
            {
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
                usersAdminPanel_sp.Visibility = Visibility.Collapsed;
                companyAdminPanel_sp.Visibility = Visibility.Collapsed;
                ticketsAdminPanel_sp.Visibility = Visibility.Collapsed;
                receiptsAdminPanel_sp.Visibility = Visibility.Collapsed;
            }
            this.firstName_txt.Text = App.LoggedInUser.FirstName;
            this.lastName_txt.Text = App.LoggedInUser.LastName;
        }

        private void GetCompanyData()
        {
            if (LocalSettings.Values["CompanyName"] != null)
                this.companyName_txt.Text = LocalSettings.Values["CompanyName"] as string;

            if (LocalSettings.Values["CompanyAddress"] != null)
                this.companyAddress_txt.Text = LocalSettings.Values["CompanyAddress"] as string;

            if (LocalSettings.Values["CompanyEmail"] != null)
                this.companyEmail_txt.Text = LocalSettings.Values["CompanyEmail"] as string;

            if (LocalSettings.Values["CompanyPhone"] != null)
                this.companyPhone_txt.Text = LocalSettings.Values["CompanyPhone"] as string;

            if (LocalSettings.Values["CompanyLogo"] != null)
            {
                string clogo = LocalSettings.Values["CompanyLogo"] as string;
                if (File.Exists(ApplicationData.Current.LocalFolder.Path + "\\" + clogo))
                {
                    Uri resourceUri = new Uri(ApplicationData.Current.LocalFolder.Path + "\\" + clogo, UriKind.Relative);
                    this.companyLogo_img.Source = new BitmapImage(resourceUri);
                }
            }
        }

        private void OnThemeRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            var selectedTheme = ((RadioButton)sender)?.Tag?.ToString();
            if (selectedTheme != null)
            {
                ((sender as RadioButton).XamlRoot.Content as Frame).RequestedTheme = GetEnum<ElementTheme>(selectedTheme);
            }
        }

        private void OnThemeRadioButtonKeyDown(object sender, KeyRoutedEventArgs e)
        {
            var selectedTheme = ((RadioButton)sender)?.Tag?.ToString();
            if (selectedTheme != null)
            {
                ((sender as RadioButton).XamlRoot.Content as Frame).RequestedTheme = GetEnum<ElementTheme>(selectedTheme);
            }
        }

        private TEnum GetEnum<TEnum>(string text) where TEnum : struct
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
            {
                throw new InvalidOperationException("Generic parameter 'TEnum' must be an enum.");
            }
            return (TEnum)Enum.Parse(typeof(TEnum), text);
        }

        private async void ResetPassword_Clicked(object sender, RoutedEventArgs e)
        {
            // Display the reset password dialog
            ContentDialog resetPasswordDialog = new ResetPasswordContentDialog(null);
            resetPasswordDialog.XamlRoot = this.XamlRoot;
            await resetPasswordDialog.ShowAsync();
        }

        private async void ResetUserPassword_Clicked(object sender, RoutedEventArgs e)
        {
            // Display the reset password dialog
            if (user_dataGrid.SelectedItem != null)
            {
                ContentDialog resetPasswordDialog = new ResetPasswordContentDialog((user_dataGrid.SelectedItem as UserViewModel).Id);
                resetPasswordDialog.XamlRoot = this.XamlRoot;
                await resetPasswordDialog.ShowAsync();
            }
        }

        private void UpdateUser_Clicked(object sender, RoutedEventArgs e)
        {
            if (this.firstName_txt.Text != App.LoggedInUser.FirstName || this.lastName_txt.Text != App.LoggedInUser.LastName)
            {
                App.LoggedInUser.FirstName = this.firstName_txt.Text;
                App.LoggedInUser.LastName = this.lastName_txt.Text;
                DatabaseHelper.UpdateUser(App.LoggedInUser);

                var users = DatabaseHelper.GetUsers();

                if (Users.Count != 0)
                    Users.Clear();

                foreach (var user in users)
                    Users.Add(user);
            }

            // Example FileSavePicker as reference
            //FileSavePicker picker = new();
            //picker.SuggestedStartLocation = PickerLocationId.Downloads;
            //picker.FileTypeChoices.Add("Csv", new List<string>() { ".csv" });
            //picker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            //picker.SuggestedFileName = String.Format("Ticket Report - {0}", DateTime.Now.ToString("d"));
            //picker.SettingsIdentifier = "settingsIdentifier";
            //picker.DefaultFileExtension = ".csv";

            //Window window = (Application.Current as App)?.Window;
            //var hWnd = WindowNative.GetWindowHandle(window);
            //InitializeWithWindow.Initialize(picker, hWnd);

            //StorageFile file = await picker.PickSaveFileAsync();
        }

        private async void AddUser_Clicked(object sender, RoutedEventArgs e)
        {
            // Display the new user dialog
            ContentDialog newUserDialog = new NewUserContentDialog();
            newUserDialog.XamlRoot = this.XamlRoot;
            await newUserDialog.ShowAsync();

            var users = DatabaseHelper.GetUsers();

            if (Users.Count != 0)
                Users.Clear();

            foreach (var user in users)
                Users.Add(user);
        }

        private void UpdateCompany_Clicked(object sender, RoutedEventArgs e)
        {
            LocalSettings.Values["CompanyName"] = this.companyName_txt.Text;
            LocalSettings.Values["CompanyAddress"] = this.companyAddress_txt.Text;
            LocalSettings.Values["CompanyEmail"] = this.companyEmail_txt.Text;
            LocalSettings.Values["CompanyPhone"] = this.companyPhone_txt.Text;
        }

        private async void CompanyLogo_Clicked(object sender, RoutedEventArgs e)
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
                // Application now has read/write access to the picked file
                //OutputTextBlock.Text = "Picked photo: " + file.Name;
                //await ApplicationData.Current.LocalFolder. OpenStreamForWriteAsync(file.Name, CreationCollisionOption.ReplaceExisting);
                //CLogoFile = file;
                //this.TestDisplay_txt.Text = CLogoFile.Path;

                File.Copy(file.Path, ApplicationData.Current.LocalFolder.Path + "\\clogo" + file.FileType, true);
                Uri resourceUri = new Uri(ApplicationData.Current.LocalFolder.Path + "\\clogo" + file.FileType, UriKind.Relative);
                this.companyLogo_img.Source = new BitmapImage(resourceUri);

                LocalSettings.Values["CompanyLogo"] = "clogo" + file.FileType;
            }
            else
            {
                //OutputTextBlock.Text = "Operation cancelled.";
            }
        }

        private async void AddTicketType_Clicked(object sender, RoutedEventArgs e)
        {
            // Display the new user dialog
            ContentDialog newTicketType = new NewTicketTypeContentDialog();
            newTicketType.XamlRoot = this.XamlRoot;
            await newTicketType.ShowAsync();

            var ticketTypes = DatabaseHelper.GetTicketTypeList("All");

            if (TicketTypes.Count != 0)
                TicketTypes.Clear();

            foreach (var ticketType in ticketTypes)
                TicketTypes.Add(ticketType);
        }

        private void UserDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() == "FirstName")
            {
                e.Column.Header = "First Name";
            }
            if (e.Column.Header.ToString() == "LastName")
            {
                e.Column.Header = "Last Name";
            }
            if (e.Column.Header.ToString() == "UserType")
            {
                e.Column.Header = "Type";
            }
            if (e.Column.Header.ToString() == "LastLogin")
            {
                e.Column.Header = "Last Login";
            }
        }

        private void UserDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //IsUserSelected = true;
            viewModel.IsUserSelected = true;
        }
    }
}
