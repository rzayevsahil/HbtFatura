using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.Notifications;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Services;

public class UserNotificationService : IUserNotificationService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserContext _currentUser;

    public UserNotificationService(AppDbContext db, ICurrentUserContext currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<UserNotificationDto>> GetMineAsync(int take, CancellationToken ct = default)
    {
        take = Math.Clamp(take, 1, 100);
        var uid = _currentUser.UserId;
        return await _db.UserNotifications.AsNoTracking()
            .Where(n => n.UserId == uid)
            .OrderByDescending(n => n.CreatedAt)
            .Take(take)
            .Select(n => new UserNotificationDto
            {
                Id = n.Id,
                Type = n.Type,
                Title = n.Title,
                Body = n.Body,
                ReferenceType = n.ReferenceType,
                ReferenceId = n.ReferenceId,
                ReadAt = n.ReadAt,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync(ct);
    }

    public async Task<int> GetUnreadCountAsync(CancellationToken ct = default)
    {
        var uid = _currentUser.UserId;
        return await _db.UserNotifications.CountAsync(n => n.UserId == uid && n.ReadAt == null, ct);
    }

    public async Task MarkReadAsync(Guid id, CancellationToken ct = default)
    {
        var uid = _currentUser.UserId;
        var n = await _db.UserNotifications.FirstOrDefaultAsync(x => x.Id == id && x.UserId == uid, ct);
        if (n == null || n.ReadAt != null) return;
        n.ReadAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task MarkAllReadAsync(CancellationToken ct = default)
    {
        var uid = _currentUser.UserId;
        var now = DateTime.UtcNow;
        await _db.UserNotifications
            .Where(x => x.UserId == uid && x.ReadAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.ReadAt, now), ct);
    }

    public async Task NotifyUsersInFirmAsync(Guid firmId, string type, string title, string body, string? referenceType, Guid? referenceId, CancellationToken ct = default)
    {
        var userIds = await _db.Users.AsNoTracking()
            .Where(u => u.FirmId == firmId)
            .Select(u => u.Id)
            .ToListAsync(ct);
        if (userIds.Count == 0) return;

        var now = DateTime.UtcNow;
        foreach (var userId in userIds)
        {
            _db.UserNotifications.Add(new UserNotification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FirmId = firmId,
                Type = type,
                Title = title,
                Body = body,
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                CreatedAt = now
            });
        }

        await _db.SaveChangesAsync(ct);
    }
}
