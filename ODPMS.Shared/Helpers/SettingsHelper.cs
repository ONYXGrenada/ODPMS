using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace ODPMS.Helpers
{
    internal class SettingsHelper
    {
        public static ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        public SettingsHelper()
        {
            LocalSettings.Values["CompanyName"] = "Generic Company Ltd.";
            LocalSettings.Values["CompanyAddress"] = "St. George, Grenada";
            LocalSettings.Values["CompanyEmail"] = "info@genericcompany.com";
            LocalSettings.Values["CompanyPhone"] = "+1 473 440-1234";
        }

    }


}