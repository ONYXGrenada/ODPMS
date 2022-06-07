using System;
using System.Collections.Generic;
using System.Text;

namespace ODPMS.ViewModel
{
    public partial class SettingsViewModel : BaseViewModel
    {
        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(IsNotUserSelected))]
        bool isUserSelected;

        public bool IsNotUserSelected => !IsUserSelected;

        public SettingsViewModel()
        {
            Title = "Settings";
        }
    }
}
