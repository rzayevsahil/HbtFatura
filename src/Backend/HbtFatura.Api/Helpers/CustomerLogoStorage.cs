using Microsoft.AspNetCore.Hosting;

namespace HbtFatura.Api.Helpers;

/// <summary>Cari kart logosunu şirket ayarlarındaki gibi uploads/logos altına yazar.</summary>
public static class CustomerLogoStorage
{
    public static async Task<string?> ProcessLogoAsync(
        IWebHostEnvironment env,
        string? requestLogoUrl,
        string? existingLogoUrl,
        Guid customerId,
        CancellationToken ct = default)
    {
        if (!string.IsNullOrEmpty(requestLogoUrl) && requestLogoUrl.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
        {
            var webRoot = !string.IsNullOrEmpty(env.WebRootPath)
                ? env.WebRootPath
                : Path.Combine(env.ContentRootPath, "wwwroot");
            var folderPath = Path.Combine(webRoot, "uploads", "logos");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            if (!string.IsNullOrEmpty(existingLogoUrl) && existingLogoUrl.StartsWith("/uploads/", StringComparison.Ordinal))
            {
                try
                {
                    var oldPath = Path.Combine(webRoot, existingLogoUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (File.Exists(oldPath)) File.Delete(oldPath);
                }
                catch { /* ignore */ }
            }

            var extension = ".png";
            if (requestLogoUrl.Contains("image/jpeg", StringComparison.OrdinalIgnoreCase)) extension = ".jpg";
            else if (requestLogoUrl.Contains("image/webp", StringComparison.OrdinalIgnoreCase)) extension = ".webp";

            var fileName = $"c_{customerId:N}_{DateTime.UtcNow.Ticks}{extension}";
            var filePath = Path.Combine(folderPath, fileName);

            var comma = requestLogoUrl.IndexOf(',');
            if (comma < 0) throw new InvalidOperationException("Geçersiz logo verisi.");
            var base64Data = requestLogoUrl[(comma + 1)..].Trim();
            var bytes = Convert.FromBase64String(base64Data);
            await File.WriteAllBytesAsync(filePath, bytes, ct);

            return $"/uploads/logos/{fileName}";
        }

        if (string.IsNullOrEmpty(requestLogoUrl))
        {
            if (!string.IsNullOrEmpty(existingLogoUrl) && existingLogoUrl.StartsWith("/uploads/", StringComparison.Ordinal))
            {
                var webRoot = !string.IsNullOrEmpty(env.WebRootPath)
                    ? env.WebRootPath
                    : Path.Combine(env.ContentRootPath, "wwwroot");
                try
                {
                    var oldPath = Path.Combine(webRoot, existingLogoUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (File.Exists(oldPath)) File.Delete(oldPath);
                }
                catch { /* ignore */ }
            }
            return null;
        }

        return existingLogoUrl;
    }
}
