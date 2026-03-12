namespace HbtFatura.Api.Helpers;

/// <summary>
/// Resolves and normalizes 3-character document serial prefixes for invoice and delivery note numbers (GIB-style).
/// Priority: configured prefix → derived from company name → default.
/// Seri her zaman 3 harf (rakam yok). Çakışmada firma adından başka harf kullanılır (örn. ABC alındıysa ABC Corp → ABO).
/// </summary>
public static class DocumentSerialPrefixHelper
{
    private static readonly (string From, string To)[] TurkishToAscii = new[]
    {
        ("Ç", "C"), ("ç", "C"), ("Ğ", "G"), ("ğ", "G"), ("İ", "I"), ("ı", "I"),
        ("Ö", "O"), ("ö", "O"), ("Ş", "S"), ("ş", "S"), ("Ü", "U"), ("ü", "U")
    };

    /// <summary>
    /// Replaces Turkish characters with ASCII equivalents (e.g. Ş→S, İ→I). Used for prefix normalization.
    /// </summary>
    public static string NormalizeTurkishToAscii(string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        var result = value;
        foreach (var (from, to) in TurkishToAscii)
            result = result.Replace(from, to);
        return result;
    }

    /// <summary>
    /// Resolves the 3-character prefix: uses configuredPrefix if valid, else derives from companyName, else defaultPrefix.
    /// </summary>
    /// <param name="configuredPrefix">Firma ayarlarından gelen seri (3 karakter, opsiyonel).</param>
    /// <param name="companyName">Şirket adı (configured boşsa ilk 3 harf türetilir).</param>
    /// <param name="defaultPrefix">Varsayılan (örn: FTR, IRS).</param>
    public static string GetPrefix(string? configuredPrefix, string? companyName, string defaultPrefix)
    {
        var valid = GetValidThreeCharPrefix(configuredPrefix);
        if (!string.IsNullOrEmpty(valid))
            return valid;

        var fromName = DerivePrefixFromCompanyName(companyName);
        if (!string.IsNullOrEmpty(fromName))
            return fromName;

        return string.IsNullOrEmpty(defaultPrefix) ? "XXX" : GetValidThreeCharPrefix(defaultPrefix) ?? defaultPrefix;
    }

    /// <summary>
    /// Returns a valid 3-char prefix (A-Z, 0-9 only, Turkish normalized), or null if invalid/empty.
    /// </summary>
    public static string? GetValidThreeCharPrefix(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var normalized = NormalizeTurkishToAscii(value.Trim().ToUpperInvariant());
        var filtered = new string(normalized.Where(c => (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9')).ToArray());
        if (filtered.Length < 3) return null;
        return filtered.Length > 3 ? filtered.Substring(0, 3) : filtered;
    }

    /// <summary>
    /// Derives a 3-character prefix from company name: first 3 letters, Turkish normalized, padded with 'X' if shorter.
    /// </summary>
    public static string? DerivePrefixFromCompanyName(string? companyName)
    {
        if (string.IsNullOrWhiteSpace(companyName)) return null;
        var normalized = NormalizeTurkishToAscii(companyName.Trim().ToUpperInvariant());
        var letters = new string(normalized.Where(c => (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9')).ToArray());
        if (letters.Length == 0) return null;
        return letters.Length >= 3 ? letters.Substring(0, 3) : letters + new string('X', 3 - letters.Length);
    }

    /// <summary>
    /// Returns a 3-character prefix that is not in otherPrefixes. Seri hep harf (rakam yok).
    /// Çakışmada ilk 2 harf + A..Z ile dener.
    /// </summary>
    public static string MakeUniqueAmong(string prefix, IEnumerable<string> otherPrefixes)
    {
        var set = otherPrefixes?.ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrEmpty(prefix)) return "XXX";
        var p = prefix.Trim().ToUpperInvariant();
        if (p.Length > 3) p = p.Substring(0, 3);
        // Sadece harf kalsın (rakam varsa XXX sayılır)
        p = new string(p.Where(c => c >= 'A' && c <= 'Z').ToArray());
        if (p.Length < 3) p = (p + "XXX").Substring(0, 3);
        if (!set.Contains(p)) return p;
        var base2 = p.Length >= 2 ? p.Substring(0, 2) : p.Substring(0, 1) + "X";
        foreach (var c in "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
        {
            var candidate = base2 + c;
            if (candidate.Length == 3 && !set.Contains(candidate))
                return candidate;
        }
        return base2 + "Z";
    }

    /// <summary>
    /// Firma adındaki harfleri (sırayla, Türkçe normalize) döndürür. Sadece A-Z.
    /// </summary>
    public static string GetLettersFromCompanyName(string? companyName)
    {
        if (string.IsNullOrWhiteSpace(companyName)) return string.Empty;
        var normalized = NormalizeTurkishToAscii(companyName.Trim().ToUpperInvariant());
        return new string(normalized.Where(c => c >= 'A' && c <= 'Z').ToArray());
    }

    /// <summary>
    /// Firma adından türetilebilecek 3 harfli seri adaylarını döndürür: önce ilk 3 harf, sonra aynı isimdeki
    /// diğer harflerle oluşturulan alternatifler (örn. ABC Corp → ABC, ABO, ABR, ABP, ACO, ...).
    /// Hepsi sadece harf; çakışmada "ABC Corp" için ABO gibi kullanılabilir.
    /// </summary>
    public static IEnumerable<string> GetPrefixCandidatesFromCompanyName(string? companyName)
    {
        var letters = GetLettersFromCompanyName(companyName);
        if (letters.Length < 3)
        {
            var first = letters + new string('X', 3 - letters.Length);
            yield return first;
            yield break;
        }
        var first3 = letters.Substring(0, 3);
        yield return first3;
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { first3 };
        // Önce 3. harfi değiştir (ABO, ABR, ABP...), sonra 2., sonra 1.
        for (var pos = 2; pos >= 0; pos--)
        {
            for (var i = 0; i < letters.Length; i++)
            {
                if (i == pos) continue;
                var c = letters[i];
                var candidate = first3.Remove(pos, 1).Insert(pos, c.ToString());
                if (seen.Add(candidate)) yield return candidate;
            }
        }
        // Kalan tüm 3'lü permütasyonlar (isimdeki harflerden)
        for (var i = 0; i < letters.Length; i++)
            for (var j = 0; j < letters.Length; j++)
                for (var k = 0; k < letters.Length; k++)
                {
                    if (i == j || i == k || j == k) continue;
                    var candidate = new string(new[] { letters[i], letters[j], letters[k] });
                    if (seen.Add(candidate)) yield return candidate;
                }
    }

    /// <summary>
    /// Firma adından, diğer firmalarla çakışmayan 3 harfli seri seçer. İlk 3 harf doluysa isimdeki başka harfi kullanır (örn. ABC Corp → ABO).
    /// </summary>
    public static string GetUniquePrefixFromCompanyName(string? companyName, IEnumerable<string> otherPrefixes)
    {
        var set = otherPrefixes?.ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var candidate in GetPrefixCandidatesFromCompanyName(companyName))
        {
            if (!set.Contains(candidate)) return candidate;
        }
        return DerivePrefixFromCompanyName(companyName) ?? "XXX";
    }
}
