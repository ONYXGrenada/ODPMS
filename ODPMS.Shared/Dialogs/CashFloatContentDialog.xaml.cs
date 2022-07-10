using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace ODPMS.Dialogs
{
	public sealed partial class CashFloatContentDialog : ContentDialog
	{
        private CashFloat cashFloat;
        public CashFloatContentDialog()
        {
            this.InitializeComponent();
        }

        private void TextBox_OnBeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            cashFloat = new();
            cashFloat.User = App.LoggedInUser.Username;
            if (cashFloat_txt.Text == "")
            {
                cashFloat.Amount = 0;
            }
            else
            {
                cashFloat.Amount = float.Parse(cashFloat_txt.Text);
            }
            cashFloat.Created = DateTime.Now.ToString("yyyy-MM-dd");

            if (await CashFloat.FloatExists("admin"))
            {
               /* ContentDialog floatAlertDialog = new CashFloatAlertContentDialog();
                floatAlertDialog.XamlRoot = (Application.Current as App)?.Window.Content.XamlRoot;
                await floatAlertDialog.ShowAsync();

                */


            }
            else
            {
                await CashFloat.CreateCashFloat(cashFloat);
            }

        }



        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
