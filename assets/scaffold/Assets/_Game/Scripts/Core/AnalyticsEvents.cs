// AnalyticsEvents.cs
// The declared event schema (SOP section 10). Every event name the game can emit is a constant here,
// and adding one is a two-file change: this constant AND the SDK's allow-list settings asset.
// Why: three SDKs, four events and three naming styles left one game with no funnel at all, and one SDK
// silently drops any name not on its allow-list. The wrapper refuses names that are not in All.
namespace __NAMESPACE__
{
    public static class AnalyticsEvents
    {
        public const string TutorialStart = "tutorial_start";
        public const string TutorialStep = "tutorial_step";          // property: step index
        public const string TutorialComplete = "tutorial_complete";
        public const string TutorialAbandon = "tutorial_abandon";
        public const string LevelStart = "level_start";
        public const string LevelComplete = "level_complete";
        public const string LevelFail = "level_fail";
        public const string Purchase = "purchase";                  // properties: product, localized price, ISO currency, store (per platform)
        public const string FirstPurchase = "first_purchase";
        public const string AdRequest = "ad_request";
        public const string AdFill = "ad_fill";
        public const string AdImpression = "ad_impression";
        public const string AdComplete = "ad_complete";
        public const string AdClick = "ad_click";
        public const string AdFail = "ad_fail";
        public const string CurrencySource = "currency_source";     // batched by reason, flushed on a boundary
        public const string CurrencySink = "currency_sink";
        public const string SessionEnd = "session_end";

        public static readonly string[] All =
        {
            TutorialStart, TutorialStep, TutorialComplete, TutorialAbandon,
            LevelStart, LevelComplete, LevelFail,
            Purchase, FirstPurchase,
            AdRequest, AdFill, AdImpression, AdComplete, AdClick, AdFail,
            CurrencySource, CurrencySink, SessionEnd,
        };

        // User properties: breakdowns, not new events. Set after the SDK initialises.
        public const string PropNotificationPermission = "notification_permission";
        public const string PropInstallAgeDays = "install_age_days";
        public const string PropPayer = "payer";
    }
}
