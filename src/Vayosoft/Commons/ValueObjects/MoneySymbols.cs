﻿using Vayosoft.Commons.Enums;

namespace Vayosoft.Commons.ValueObjects
{
    public static class MoneySymbols
    {
        private static readonly Dictionary<MoneyUnit, string> Symbols = new()
        {
            { MoneyUnit.UnSpecified, string.Empty },
            { MoneyUnit.Dollar, "$" },
            { MoneyUnit.Euro, "€" },
            { MoneyUnit.Shekel, "₪" }
        };

        public static string GetSymbol(MoneyUnit moneyUnit)
        {
            return Symbols[moneyUnit];
        }
    }
}
