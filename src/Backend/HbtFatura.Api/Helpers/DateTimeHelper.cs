namespace HbtFatura.Api.Helpers;

/// <summary>
/// Shared date/time normalization for API (e.g. order date, delivery note date, invoice date).
/// Ensures values are valid and stored without timezone ambiguity (Unspecified).
/// </summary>
public static class DateTimeHelper
{
    /// <summary>
    /// Normalizes a DateTime for persistence: default becomes UtcNow, other values are converted to Unspecified kind.
    /// </summary>
    public static DateTime NormalizeForStorage(DateTime value)
    {
        if (value == default)
            value = DateTime.UtcNow;
        if (value.Kind != DateTimeKind.Unspecified)
            return DateTime.SpecifyKind(value, DateTimeKind.Unspecified);
        return value;
    }
}
