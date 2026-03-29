using HbtFatura.Api.DTOs.Notifications;

namespace HbtFatura.Api.Services;

public interface IUserNotificationService
{
    Task<IReadOnlyList<UserNotificationDto>> GetMineAsync(int take, CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(CancellationToken ct = default);
    Task MarkReadAsync(Guid id, CancellationToken ct = default);
    Task MarkAllReadAsync(CancellationToken ct = default);
    Task NotifyUsersInFirmAsync(Guid firmId, string type, string title, string body, string? referenceType, Guid? referenceId, CancellationToken ct = default);
}
