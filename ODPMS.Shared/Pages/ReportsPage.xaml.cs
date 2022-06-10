using System;
using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using System.Globalization;
using ODPMS.Models;
using ODPMS.Helpers;
using Windows.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using WinRT.Interop;
using System.Linq;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ODPMS.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReportsPage : Page
    {
        ReportsViewModel viewModel = null;
        public ReportsPage()
        {
            this.InitializeComponent();
            viewModel = new ReportsViewModel();
            DataContext = viewModel;
        }   
    }
}
