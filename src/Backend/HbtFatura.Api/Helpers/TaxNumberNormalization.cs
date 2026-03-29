namespace HbtFatura.Api.Helpers;

public static class TaxNumberNormalization
{
    /// <summary>Removes whitespace; returns empty if invalid length for VKN (10) or TC (11).</summary>
    public static string Normalize(string? taxNumber)
    {
        if (string.IsNullOrWhiteSpace(taxNumber))
            return string.Empty;
        var digits = new string(taxNumber.Where(char.IsDigit).ToArray());
        if (digits.Length is 10 or 11)
            return digits;
        return string.Empty;
    }
}
