using UnityEngine;

public static class CurrencyFormatter
{
    /// <summary>
    /// Formata valores de moeda:
    /// - Abrevia (K, M, B) para números grandes
    /// - Remove zeros desnecessários em decimais
    /// </summary>
    public static string FormatCurrency(double value, int casasDecimais = 2)
    {
        string format = "0." + new string('#', casasDecimais);

        if (value >= 1_000_000_000)
            return (value / 1_000_000_000d).ToString(format) + "B"; // Bilhões
        if (value >= 1_000_000)
            return (value / 1_000_000d).ToString(format) + "M";     // Milhões
        if (value >= 1_000)
            return (value / 1_000d).ToString(format) + "K";         // Milhares

        return value.ToString(format); // Valor normal
    }
}