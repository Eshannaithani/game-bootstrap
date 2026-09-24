// GameConstants.cs
// The single source of truth for every player-visible number (SOP section 2).
// Rule: no price, bonus, reward, duration or percentage is ever a literal in UI copy or gameplay code.
// Why: a purchase screen once promised $1,000,000 against a $500,000 grant because the figure was
// copy-pasted from the other pack. UI builds its numbers from here:
//   "+" + Mathf.RoundToInt(IncomeBonus * 100) + "% income"   never   "+25% income"
// Grep for hardcoded figures before every release; literals reappear.
namespace __NAMESPACE__
{
    public static class GameConstants
    {
        public const string SaveFileName = "save.json";
        public const int SaveSchemaVersion = 1;           // bump when a field's default becomes ambiguous (SOP section 5)
        public const int TargetFrameRate = 60;

        // Economy placeholders. Replace with the game's own values, keep the pattern.
        public const long StartingCoins = 0;
        public const double UpgradePriceBase = 100;
        public const double UpgradePriceGrowth = 1.15;
        public const long PriceCeiling = 1_000_000_000_000;  // well inside long: a curve must never overflow to a negative price
        public const float RewardedAdMinutesOfIncome = 15f;   // rewards scale with production rate, never with balance (SOP section 2c)

        // Sort-order ladder (SOP section 7). Every modal registers with a coordinator so nothing opens underneath another.
        public const int SortHud = 6;
        public const int SortPopup = 20;
        public const int SortToast = 30;

        // Exponential curves compute in double, return long, clamp to a ceiling and floor at a minimum,
        // so no reachable level can ever be free or negative. Sweep the whole level range in a test.
        public static long Price(double baseValue, double growth, int level, long ceiling = PriceCeiling, long floor = 1)
        {
            double v = baseValue * System.Math.Pow(growth, level);
            if (double.IsNaN(v) || double.IsInfinity(v) || v > ceiling) return ceiling;
            long r = (long)System.Math.Round(v);
            return r < floor ? floor : r;
        }
    }
}
