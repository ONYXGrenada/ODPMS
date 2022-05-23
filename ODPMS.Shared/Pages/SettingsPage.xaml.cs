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
using System.Collections.ObjectModel;
using System.Reflection;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ODPMS.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public ObservableCollection<UserViewModel> Users { get; set; }
        public SettingsPage()
        {
            this.InitializeComponent();
            GetSettingsData();
        }

        private void GetSettingsData()
        {
            if (App.LoggedInUser.UserType == "admin")
            {
                Users = DatabaseHelper.GetUsers();
            }
            else
            {
                userPanel_sp.Visibility = Visibility.Collapsed;
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

        private async void UpdateUser_Clicked(object sender, RoutedEventArgs e)
        {
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
        }
    }
}
