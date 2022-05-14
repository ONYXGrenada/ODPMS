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

namespace ODPMS.Pages
{
	public sealed partial class PayTicketContentDialog : ContentDialog
	{
        public PayTicketContentDialog()
		{
			this.InitializeComponent();
		}

        private void PayTicket_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            // Get ticket object from database
            TicketNumberText.Text = "Ticket Number :";
            TicketStartTimeText.Text = "Start Time :";
            TicketEndTimeText.Text = "End Time :";
            TicketDurationText.Text = "Duration :";
            TicketCostText.Text = "Cost :";

        }
        private void PrimaryButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Pay execute the pay function to display change and update ticket in the database
        }

        private void CloseButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //D iscard the new ticket object and cancel payment.
        }

        private void PayAmount_Changed(object sender, RoutedEventArgs e)
        {

        }
    }
}
