using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Constants;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Data;

public static class RoleSeed
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager, AppDbContext db, CancellationToken ct = default)
    {
        foreach (var roleName in new[] { Roles.SuperAdmin, Roles.FirmAdmin, Roles.Employee })
        {
            if (await roleManager.RoleExistsAsync(roleName))
                continue;
            await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }

        await SeedPermissionsAndMenusAsync(roleManager, db, ct);
    }

    private static async Task SeedPermissionsAndMenusAsync(RoleManager<IdentityRole<Guid>> roleManager, AppDbContext db, CancellationToken ct)
    {
        // 1. Permissions
        var permissions = new List<Permission>
        {
            new() { Id = Guid.NewGuid(), Group = "Sistem", Code = "Dashboard.View", Name = "Panel Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Faturalar", Code = "Invoices.View", Name = "Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Faturalar", Code = "Invoices.Create", Name = "Oluştur" },
            new() { Id = Guid.NewGuid(), Group = "Faturalar", Code = "Invoices.Edit", Name = "Düzenle" },
            new() { Id = Guid.NewGuid(), Group = "Faturalar", Code = "Invoices.Delete", Name = "Sil" },
            
            new() { Id = Guid.NewGuid(), Group = "Siparişler", Code = "Orders.View", Name = "Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Siparişler", Code = "Orders.Create", Name = "Oluştur" },
            
            new() { Id = Guid.NewGuid(), Group = "Ürünler", Code = "Products.View", Name = "Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Ürünler", Code = "Products.Edit", Name = "Düzenle" },
            
            new() { Id = Guid.NewGuid(), Group = "Tanımlamalar", Code = "Lookups.View", Name = "Sistem Tanımları Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Tanımlamalar", Code = "Lookups.Edit", Name = "Sistem Tanımları Düzenle" },
            
            new() { Id = Guid.NewGuid(), Group = "Yetkilendirme", Code = "Roles.View", Name = "Rolleri Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Yetkilendirme", Code = "Roles.Edit", Name = "Rolleri Düzenle" },
            
            new() { Id = Guid.NewGuid(), Group = "Menü Yönetimi", Code = "Menus.View", Name = "Menüleri Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Menü Yönetimi", Code = "Menus.Edit", Name = "Menüleri Düzenle" },
            
            new() { Id = Guid.NewGuid(), Group = "Firma Yönetimi", Code = "Firms.View", Name = "Firmaları Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Firma Yönetimi", Code = "Firms.Edit", Name = "Firmaları Düzenle" },
            
            new() { Id = Guid.NewGuid(), Group = "Personel Yönetimi", Code = "Employees.View", Name = "Personelleri Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Personel Yönetimi", Code = "Employees.Edit", Name = "Personelleri Düzenle" },
            
            new() { Id = Guid.NewGuid(), Group = "Cari Yönetimi", Code = "Customers.View", Name = "Carileri Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Cari Yönetimi", Code = "Customers.Edit", Name = "Carileri Düzenle" },
            
            new() { Id = Guid.NewGuid(), Group = "Finans", Code = "Banking.View", Name = "Banka İşlemleri Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Finans", Code = "Banking.Edit", Name = "Banka İşlemleri Düzenle" },
            new() { Id = Guid.NewGuid(), Group = "Finans", Code = "Cash.View", Name = "Kasa İşlemleri Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Finans", Code = "Cash.Edit", Name = "Kasa İşlemleri Düzenle" },
            new() { Id = Guid.NewGuid(), Group = "Finans", Code = "Payments.View", Name = "Tahsilat/Ödemeleri Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Finans", Code = "Cheques.View", Name = "Çek/Senet Yönetimi" },
            new() { Id = Guid.NewGuid(), Group = "Finans", Code = "Cheques.Edit", Name = "Çek/Senet Düzenle" },
            
            new() { Id = Guid.NewGuid(), Group = "Raporlar", Code = "Reports.View", Name = "Raporları Görüntüle" },
            
            new() { Id = Guid.NewGuid(), Group = "Sistem", Code = "Logs.View", Name = "Logları Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Ayarlar", Code = "Settings.View", Name = "Ayarları Görüntüle" }
        };

        foreach (var p in permissions)
        {
            if (!await db.Permissions.AnyAsync(x => x.Code == p.Code, ct))
                db.Permissions.Add(p);
        }
        await db.SaveChangesAsync(ct);

        // 2. Role Permissions
        var allPerms = await db.Permissions.ToListAsync(ct);
        var superAdminRole = await roleManager.FindByNameAsync(Roles.SuperAdmin);
        if (superAdminRole != null)
        {
            var adminPermCodes = new[] { "Dashboard.View", "Lookups.", "Roles.", "Menus.", "Logs.", "Firms." };
            var adminPerms = allPerms.Where(x => adminPermCodes.Any(code => x.Code.StartsWith(code))).ToList();

            // Add missing permissions
            foreach (var p in adminPerms)
            {
                if (!await db.RolePermissions.AnyAsync(x => x.RoleId == superAdminRole.Id && x.PermissionId == p.Id, ct))
                    db.RolePermissions.Add(new RolePermission { RoleId = superAdminRole.Id, PermissionId = p.Id });
            }

            // Remove unwanted permissions if they exist
            var currentAdminPerms = await db.RolePermissions
                .Where(x => x.RoleId == superAdminRole.Id)
                .ToListAsync(ct);

            foreach (var rp in currentAdminPerms)
            {
                var perm = allPerms.FirstOrDefault(p => p.Id == rp.PermissionId);
                if (perm == null || !adminPermCodes.Any(code => perm.Code.StartsWith(code)))
                {
                    db.RolePermissions.Remove(rp);
                }
            }
        }

        var firmAdminRole = await roleManager.FindByNameAsync(Roles.FirmAdmin);
        if (firmAdminRole != null)
        {
            var firmPerms = allPerms.Where(x => 
                x.Code == "Dashboard.View" || 
                x.Code.StartsWith("Invoices") || 
                x.Code.StartsWith("Orders") || 
                x.Code.StartsWith("Products") || 
                x.Code.StartsWith("Employees") || 
                x.Code.StartsWith("Customers") || 
                x.Code.StartsWith("Banking") || 
                x.Code.StartsWith("Cash") || 
                x.Code.StartsWith("Payments") || 
                x.Code.StartsWith("Cheques") || 
                x.Code == "Reports.View" || 
                x.Code == "Settings.View" || 
                x.Code == "Firms.Edit").ToList();
            foreach (var p in firmPerms)
            {
                if (!await db.RolePermissions.AnyAsync(x => x.RoleId == firmAdminRole.Id && x.PermissionId == p.Id, ct))
                    db.RolePermissions.Add(new RolePermission { RoleId = firmAdminRole.Id, PermissionId = p.Id });
            }
        }

        var employeeRole = await roleManager.FindByNameAsync(Roles.Employee);
        if (employeeRole != null)
        {
            var empPerms = allPerms.Where(x => x.Code == "Dashboard.View" || x.Code == "Invoices.View" || x.Code == "Orders.View" || x.Code == "Products.View" || x.Code == "Settings.View").ToList();
            foreach (var p in empPerms)
            {
                if (!await db.RolePermissions.AnyAsync(x => x.RoleId == employeeRole.Id && x.PermissionId == p.Id, ct))
                    db.RolePermissions.Add(new RolePermission { RoleId = employeeRole.Id, PermissionId = p.Id });
            }
        }
        await db.SaveChangesAsync(ct);

        // 3. Menus
        var menus = new List<Menu>
        {
            new() { Label = "Dashboard", Icon = "dashboard", RouterLink = "/dashboard", SortOrder = 1, RequiredPermissionCode = "Dashboard.View" },
            new() { Label = "Faturalar", Icon = "receipt", RouterLink = "/invoices", SortOrder = 2, RequiredPermissionCode = "Invoices.View" },
            new() { Label = "Siparişler", Icon = "shopping_cart", RouterLink = "/orders", SortOrder = 3, RequiredPermissionCode = "Orders.View" },
            new() { Label = "İrsaliyeler", Icon = "local_shipping", RouterLink = "/delivery-notes", SortOrder = 4, RequiredPermissionCode = "Invoices.View" },
            new() { Label = "Ürünler", Icon = "inventory_2", RouterLink = "/products", SortOrder = 5, RequiredPermissionCode = "Products.View" },
            new() { Label = "Cari Kartlar", Icon = "people", RouterLink = "/customers", SortOrder = 6, RequiredPermissionCode = "Customers.View" },
            new() { Label = "Tahsilat / Ödeme", Icon = "account_balance_wallet", RouterLink = "/payments", SortOrder = 7, RequiredPermissionCode = "Payments.View" },
            new() { Label = "Kasa Yönetimi", Icon = "point_of_sale", RouterLink = "/cash-registers", SortOrder = 8, RequiredPermissionCode = "Cash.View" },
            new() { Label = "Banka Yönetimi", Icon = "account_balance", RouterLink = "/bank-accounts", SortOrder = 9, RequiredPermissionCode = "Banking.View" },
            new() { Label = "Çek / Senet", Icon = "history_edu", RouterLink = "/cheques", SortOrder = 10, RequiredPermissionCode = "Cheques.View" },
            new() { Label = "Raporlar", Icon = "analytics", RouterLink = "/reports", SortOrder = 100, RequiredPermissionCode = "Reports.View" },
            new() { Label = "Sistem Tanımları", Icon = "settings", RouterLink = "/lookups", SortOrder = 101, RequiredPermissionCode = "Lookups.View" },
            new() { Label = "Rol ve Yetki Yönetimi", Icon = "security", RouterLink = "/permissions", SortOrder = 102, RequiredPermissionCode = "Roles.View" },
            new() { Label = "Menü Yönetimi", Icon = "menu", RouterLink = "/menus", SortOrder = 103, RequiredPermissionCode = "Menus.View" },
            new() { Label = "Firma Yönetimi", Icon = "business", RouterLink = "/firms", SortOrder = 104, RequiredPermissionCode = "Firms.View" },
            new() { Label = "Personel Yönetimi", Icon = "badge", RouterLink = "/employees", SortOrder = 105, RequiredPermissionCode = "Employees.View" },
            new() { Label = "Sistem Logları", Icon = "history", RouterLink = "/logs", SortOrder = 106, RequiredPermissionCode = "Logs.View" },
            new() { Label = "Firma Bilgileri", Icon = "business", RouterLink = "/settings", SortOrder = 11, RequiredPermissionCode = "Settings.View" }
        };

        foreach (var m in menus)
        {
            if (!await db.Menus.AnyAsync(x => x.Label == m.Label, ct))
                db.Menus.Add(m);
        }
        await db.SaveChangesAsync(ct);
    }

    public static async Task BootstrapSuperAdminIfEmptyAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration config,
        CancellationToken ct = default)
    {
        if (await userManager.Users.AnyAsync(ct))
            return;

        var email = config["Bootstrap:AdminEmail"];
        var password = config["Bootstrap:AdminPassword"];
        var fullName = config["Bootstrap:AdminFullName"] ?? "Super Admin";
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return;

        email = email.Trim().ToLowerInvariant();
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            FullName = fullName.Trim(),
            CreatedAt = DateTime.UtcNow,
            FirmId = null
        };
        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return;
        await userManager.AddToRoleAsync(user, Roles.SuperAdmin);
    }
}
