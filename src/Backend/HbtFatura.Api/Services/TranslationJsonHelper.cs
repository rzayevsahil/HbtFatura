using System.Text.Json;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Services;

public static class TranslationJsonHelper
{
    /// <summary>Çeviri JSON (iç içe) düz liste satırlarına dönüştürür.</summary>
    public static List<UiTranslation> Flatten(string culture, string json)
    {
        using var doc = JsonDocument.Parse(json);
        var list = new List<UiTranslation>();
        Walk(string.Empty, doc.RootElement, culture, list);
        return list;
    }

    private static void Walk(string prefix, JsonElement el, string culture, List<UiTranslation> list)
    {
        switch (el.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var p in el.EnumerateObject())
                {
                    var next = string.IsNullOrEmpty(prefix) ? p.Name : $"{prefix}.{p.Name}";
                    Walk(next, p.Value, culture, list);
                }
                break;
            case JsonValueKind.String:
                list.Add(new UiTranslation
                {
                    Id = Guid.NewGuid(),
                    Key = prefix,
                    Culture = culture,
                    Value = el.GetString() ?? string.Empty
                });
                break;
            case JsonValueKind.Number:
                list.Add(new UiTranslation
                {
                    Id = Guid.NewGuid(),
                    Key = prefix,
                    Culture = culture,
                    Value = el.GetRawText()
                });
                break;
            case JsonValueKind.True:
            case JsonValueKind.False:
                list.Add(new UiTranslation
                {
                    Id = Guid.NewGuid(),
                    Key = prefix,
                    Culture = culture,
                    Value = el.GetBoolean() ? "true" : "false"
                });
                break;
        }
    }

    /// <summary>Düz anahtarları ngx-translate uyumlu iç içe sözlüğe çevirir.</summary>
    public static Dictionary<string, object> ToNested(IEnumerable<(string Key, string Value)> pairs)
    {
        var root = new Dictionary<string, object>();
        foreach (var (key, value) in pairs)
        {
            var parts = key.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) continue;
            var cur = root;
            for (var i = 0; i < parts.Length - 1; i++)
            {
                if (!cur.TryGetValue(parts[i], out var nextObj) || nextObj is not Dictionary<string, object> nextDict)
                {
                    nextDict = new Dictionary<string, object>();
                    cur[parts[i]] = nextDict;
                }
                cur = nextDict;
            }
            cur[parts[^1]] = value;
        }
        return root;
    }
}
