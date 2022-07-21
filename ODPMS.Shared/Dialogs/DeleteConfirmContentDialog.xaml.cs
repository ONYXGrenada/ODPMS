// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
using System.Linq;
using ODPMS.ViewModels;

namespace ODPMS.Dialogs
{
    public sealed partial class DeleteConfirmContentDialog : ContentDialog
    {
        public User userToDelete;
        public TicketType ticketTypeToDelete;
        public DeleteConfirmContentDialog()
        {
            this.InitializeComponent();
        }
        public DeleteConfirmContentDialog(User deletedUser)
        {
            this.InitializeComponent();
            this.userToDelete = deletedUser;
            this.lblDeleteMsg.Text = "Are you sure that you want to delete user " + deletedUser.Username + "?";
        }
        public DeleteConfirmContentDialog(TicketType deletedTicketType)
        {
            this.InitializeComponent();
            this.ticketTypeToDelete = deletedTicketType;
            this.lblDeleteMsg.Text = "Are you sure that you want to delete ticket type " + deletedTicketType.Id + "?";
        }

        private void DeleteConfirmContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (userToDelete != null)
            {
                deleteUser(userToDelete);
            }
            else if(ticketTypeToDelete != null )
            {
                deleteTicketType(ticketTypeToDelete);
            }
        }
        private void DeleteConfirmContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }


        public async void deleteUser(User deletedUser)
        {
            await User.DeleteUser(deletedUser);
           
        }
        public async void deleteTicketType(TicketType deletedTicketType)
        {
            await TicketType.DeleteTicketType(deletedTicketType);

        }


    }
}
