using RimWorld;
using Verse;

namespace RimBank.Core
{
    [DefOf]
    public static class CoreDefOf
    {
        public static JobDef UseBankTerminal;
        public static ThingDef RimBankBuildingTerminal;

        static CoreDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(CoreDefOf));
        }
    }
}