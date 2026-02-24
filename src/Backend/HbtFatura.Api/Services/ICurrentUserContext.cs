namespace HbtFatura.Api.Services;

public interface ICurrentUserContext
{
    Guid UserId { get; }
    string Role { get; }
    Guid? FirmId { get; }
    string? FirmName { get; }
    bool IsSuperAdmin { get; }
    bool IsFirmAdmin { get; }
    bool IsEmployee { get; }
}
