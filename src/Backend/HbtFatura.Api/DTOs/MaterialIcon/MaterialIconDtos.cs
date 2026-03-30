namespace HbtFatura.Api.DTOs.MaterialIcon;

public record MaterialIconDto(Guid Id, string LigatureName, int SortOrder, bool IsActive);

public record CreateMaterialIconRequest(string LigatureName, int SortOrder, bool IsActive = true);

public record UpdateMaterialIconRequest(string LigatureName, int SortOrder, bool IsActive);
