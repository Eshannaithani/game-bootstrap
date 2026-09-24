// Consent.cs
// Persisted consent and install stamp (SOP section 10). Both live in PlayerPrefs, not in the save file:
// a player who wipes their progress is not a new install, and consent must survive a save reset.
// Permission-style properties (notifications) are read live from the OS on every resume, never cached here.
using System;
using System.Globalization;
using UnityEngine;

namespace __NAMESPACE__
{
    public static class Consent
    {
        const string ConsentKey = "consent_v1";
        const string InstallKey = "install_utc";

        public static bool Granted => PlayerPrefs.GetInt(ConsentKey, 0) == 1;

        public static void Set(bool granted)
        {
            PlayerPrefs.SetInt(ConsentKey, granted ? 1 : 0);
            PlayerPrefs.Save();
        }

        public static DateTime InstallUtc
        {
            get
            {
                if (!PlayerPrefs.HasKey(InstallKey))
                {
                    PlayerPrefs.SetString(InstallKey, DateTime.UtcNow.ToString("o"));
                    PlayerPrefs.Save();
                }
                return DateTime.Parse(PlayerPrefs.GetString(InstallKey), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }
        }

        public static int InstallAgeDays => (int)(DateTime.UtcNow - InstallUtc).TotalDays;
    }
}
