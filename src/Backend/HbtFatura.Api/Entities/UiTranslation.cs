namespace HbtFatura.Api.Entities;

/// <summary>İstenilen dil (ngx-translate) metinleri — noktalı anahtar, örn. menu.dashboard</summary>
public class UiTranslation
{
    public Guid Id { get; set; }

    /// <summary>Nokta notasyonu, örn. common.save</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>Kültür kodu: tr, en</summary>
    public string Culture { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
}
