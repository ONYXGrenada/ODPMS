// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

using System.Linq;

namespace ODPMS.Dialogs
{
    public sealed partial class CashFloatUpdateContentDialog : ContentDialog
    {
        public CashFloat userFloat;
        public CashFloatUpdateContentDialog()
        {
            this.InitializeComponent();

            
        }

        private void TextBox_OnBeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        public CashFloatUpdateContentDialog(string user, float amount)
        {
            this.InitializeComponent();

            this.lbl_info.Text = String.Format("{0} has existing cash float of {1}",user,amount.ToString("C2"));


        }

        public CashFloatUpdateContentDialog(CashFloat cashFloat)
        {
            this.InitializeComponent();
            
            userFloat = cashFloat;

            this.lbl_info.Text = String.Format("{0} has existing cash float of {1}", cashFloat.User, cashFloat.Amount.ToString("C2"));


        }

        private async void CashFloatUpdateContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            float newAmount;
            if (cashFloatUpdate_txt.Text == String.Empty)
            {
                newAmount = 0;
            }
            else
            {
                newAmount = float.Parse(cashFloatUpdate_txt.Text);
            }

            userFloat.Amount = newAmount;
            userFloat.UpdatedBy = App.LoggedInUser.Username;
            userFloat.Updated = DateTime.Now;

            await CashFloat.UpdateCashFloat(userFloat);

        }

        
    }
}
