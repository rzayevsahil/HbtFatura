namespace HbtFatura.Api.DTOs.Translation;

public record UiTranslationAdminDto(Guid Id, string Key, string Culture, string Value);

public record UiTranslationAdminListResponse(IReadOnlyList<UiTranslationAdminDto> Items, int Total);

/// <summary>Admin grid: tek satırda aynı anahtar için TR ve EN.</summary>
public record UiTranslationPairAdminDto(string Key, Guid? TrId, string ValueTr, Guid? EnId, string ValueEn);

public record UiTranslationPairListResponse(IReadOnlyList<UiTranslationPairAdminDto> Items, int Total);

public class UpdateUiTranslationRequest
{
    public string Value { get; set; } = string.Empty;
}

public class UpdateUiTranslationPairRequest
{
    public string Key { get; set; } = string.Empty;
    public string ValueTr { get; set; } = string.Empty;
    public string ValueEn { get; set; } = string.Empty;
}
