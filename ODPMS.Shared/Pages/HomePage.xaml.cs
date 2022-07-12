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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ODPMS.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
            DataContext = new HomeViewModel();

            checkForFloat(App.LoggedInUser.Username);
        }

        public async void checkForFloat(string userId)
        {

            var today = DateTime.Now.ToString("yyyy-MM-dd");
            await App.Database.Init();
            var query = App.Database.Current.Table<CashFloat>().Where(v => v.User.Equals(userId) && v.Created.Equals(today));
            var StatusMessage = string.Format("{0} record(s) found in the ticket table)", await query.CountAsync());

            //return await query.ToListAsync();
            if (await query.CountAsync() == 0)
            {
                ContentDialog floatDialog = new CashFloatContentDialog();
                floatDialog.XamlRoot = (Application.Current as App)?.Window.Content.XamlRoot;
                await floatDialog.ShowAsync();
            }


        }
    }
}
