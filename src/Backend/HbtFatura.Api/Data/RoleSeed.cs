using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Constants;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Data;

public static class RoleSeed
{
    public static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager, AppDbContext db, CancellationToken ct = default)
    {
        var rolesToSeed = new (string Name, string Display)[]
        {
            (Roles.SuperAdmin, "Super Admin"),
            (Roles.FirmAdmin, "Firma Yöneticisi"),
            (Roles.Employee, "Çalışan")
        };

        foreach (var (name, display) in rolesToSeed)
        {
            var role = await roleManager.FindByNameAsync(name);
            if (role == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(name) { DisplayName = display, IsSystem = true });
            }
            else
            {
                var needsSave = false;
                if (string.IsNullOrEmpty(role.DisplayName))
                {
                    role.DisplayName = display;
                    needsSave = true;
                }
                if (!role.IsSystem)
                {
                    role.IsSystem = true;
                    needsSave = true;
                }
                if (needsSave)
                    await roleManager.UpdateAsync(role);
            }
        }

        await SeedPermissionsAndMenusAsync(roleManager, db, ct);
    }

    private static async Task SeedPermissionsAndMenusAsync(RoleManager<ApplicationRole> roleManager, AppDbContext db, CancellationToken ct)
    {
        // 1. Permissions
        var permissions = new List<Permission>
        {
            new() { Id = Guid.NewGuid(), Group = "Sistem", Code = "Dashboard.View", Name = "Dashboard Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Sistem", Code = "Logs.View", Name = "Logları Görüntüle" },

            new() { Id = Guid.NewGuid(), Group = "Faturalar", Code = "Invoices.View", Name = "Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Faturalar", Code = "Invoices.Create", Name = "Oluştur" },
            new() { Id = Guid.NewGuid(), Group = "Faturalar", Code = "Invoices.Edit", Name = "Düzenle" },
            new() { Id = Guid.NewGuid(), Group = "Faturalar", Code = "Invoices.Delete", Name = "Sil" },

            new() { Id = Guid.NewGuid(), Group = "GİB Simülasyonu", Code = "GibSimulation.ViewInbox", Name = "Kutu görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "GİB Simülasyonu", Code = "GibSimulation.Accept", Name = "Onayla / reddet" },
            
            new() { Id = Guid.NewGuid(), Group = "Siparişler", Code = "Orders.View", Name = "Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Siparişler", Code = "Orders.Create", Name = "Oluştur" },
            new() { Id = Guid.NewGuid(), Group = "Siparişler", Code = "Orders.Edit", Name = "Düzenle" },
            new() { Id = Guid.NewGuid(), Group = "Siparişler", Code = "Orders.Delete", Name = "Sil" },
            
            new() { Id = Guid.NewGuid(), Group = "İrsaliyeler", Code = "DeliveryNotes.View", Name = "Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "İrsaliyeler", Code = "DeliveryNotes.Create", Name = "Oluştur" },
            new() { Id = Guid.NewGuid(), Group = "İrsaliyeler", Code = "DeliveryNotes.Edit", Name = "Düzenle" },
            new() { Id = Guid.NewGuid(), Group = "İrsaliyeler", Code = "DeliveryNotes.Delete", Name = "Sil" },
            
            new() { Id = Guid.NewGuid(), Group = "Ürünler", Code = "Products.View", Name = "Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Ürünler", Code = "Products.Edit", Name = "Düzenle" },
            
            new() { Id = Guid.NewGuid(), Group = "Tanımlamalar", Code = "Lookups.View", Name = "Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Tanımlamalar", Code = "Lookups.Edit", Name = "Düzenle" },
            
            new() { Id = Guid.NewGuid(), Group = "Yetkilendirme", Code = "Roles.View", Name = "Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Yetkilendirme", Code = "Roles.Edit", Name = "Düzenle" },
            
            new() { Id = Guid.NewGuid(), Group = "Menü Yönetimi", Code = "Menus.View", Name = "Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Menü Yönetimi", Code = "Menus.Edit", Name = "Düzenle" },

            new() { Id = Guid.NewGuid(), Group = "Sistem", Code = "MaterialIcons.View", Name = "Material ikon listesi" },
            new() { Id = Guid.NewGuid(), Group = "Sistem", Code = "MaterialIcons.Edit", Name = "Material ikonları düzenle" },

            new() { Id = Guid.NewGuid(), Group = "Sistem", Code = "Translations.View", Name = "Arayüz çevirileri (liste)" },
            new() { Id = Guid.NewGuid(), Group = "Sistem", Code = "Translations.Edit", Name = "Arayüz çevirileri (düzenle)" },
            
            new() { Id = Guid.NewGuid(), Group = "Firma Yönetimi", Code = "Firms.View", Name = "Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Firma Yönetimi", Code = "Firms.Edit", Name = "Düzenle" },
            
            new() { Id = Guid.NewGuid(), Group = "Personel Yönetimi", Code = "Employees.View", Name = "Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Personel Yönetimi", Code = "Employees.Edit", Name = "Düzenle" },
            
            new() { Id = Guid.NewGuid(), Group = "Cari Yönetimi", Code = "Customers.View", Name = "Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Cari Yönetimi", Code = "Customers.Edit", Name = "Düzenle" },

            new() { Id = Guid.NewGuid(), Group = "Hesap Kodları", Code = "MainAccountCodes.View", Name = "Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Hesap Kodları", Code = "MainAccountCodes.Edit", Name = "Düzenle" },
            
            new() { Id = Guid.NewGuid(), Group = "Banka Yönetimi", Code = "Banking.View", Name = "Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Banka Yönetimi", Code = "Banking.Edit", Name = "Düzenle" },

            new() { Id = Guid.NewGuid(), Group = "Kasa Yönetimi", Code = "Cash.View", Name = "Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Kasa Yönetimi", Code = "Cash.Edit", Name = "Düzenle" },

            new() { Id = Guid.NewGuid(), Group = "Tahsilat/Ödemeleri Yönetimi", Code = "Payments.View", Name = "Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Tahsilat/Ödemeleri Yönetimi", Code = "Payments.Create", Name = "Tahsilat/Ödeme girişi" },

            new() { Id = Guid.NewGuid(), Group = "Çek/Senet Yönetimi", Code = "Cheques.View", Name = "Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Çek/Senet Yönetimi", Code = "Cheques.Edit", Name = "Düzenle" },
            
            new() { Id = Guid.NewGuid(), Group = "Raporlar", Code = "Reports.View", Name = "Görüntüle" },
            
            new() { Id = Guid.NewGuid(), Group = "Şirket Profili", Code = "CompanyProfile.View", Name = "Görüntüle" },
            new() { Id = Guid.NewGuid(), Group = "Şirket Profili", Code = "CompanyProfile.Edit", Name = "Düzenle" }
        };

        foreach (var p in permissions)
            p.IsSystem = true;

        foreach (var p in permissions)
        {
            if (!await db.Permissions.AnyAsync(x => x.Code == p.Code, ct))
                db.Permissions.Add(p);
        }
        await db.SaveChangesAsync(ct);

        var seededCodes = permissions.ConvertAll(x => x.Code);
        var needFlag = await db.Permissions.Where(x => seededCodes.Contains(x.Code) && !x.IsSystem).ToListAsync(ct);
        foreach (var p in needFlag)
            p.IsSystem = true;
        if (needFlag.Count > 0)
            await db.SaveChangesAsync(ct);

        // 2. Role Permissions
        var allPerms = await db.Permissions.ToListAsync(ct);
        var superAdminRole = await roleManager.FindByNameAsync(Roles.SuperAdmin);
        if (superAdminRole != null)
        {
            // SuperAdmin: sistem + firma yönetimi; Employees.* firma detayından çalışan düzenleme ve API (genel /employees listesi menü ve servis tarafında kısıtlı).
            var adminPermCodes = new[] { "Dashboard.View", "Lookups.", "Roles.", "Menus.", "MaterialIcons.", "Translations.", "Logs.", "Firms.", "CompanyProfile.Edit", "Employees." };
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
            var firmPermCodes = new[] 
            { 
                "Dashboard.View", "Invoices.", "GibSimulation.", "Orders.", "DeliveryNotes.", "Products.", 
                "Employees.", "Customers.", "MainAccountCodes.", "Banking.", "Cash.", "Payments.", "Cheques.", 
                "Reports.View", "CompanyProfile."
            };
            var firmPerms = allPerms.Where(x => firmPermCodes.Any(code => x.Code.StartsWith(code))).ToList();

            foreach (var p in firmPerms)
            {
                if (!await db.RolePermissions.AnyAsync(x => x.RoleId == firmAdminRole.Id && x.PermissionId == p.Id, ct))
                    db.RolePermissions.Add(new RolePermission { RoleId = firmAdminRole.Id, PermissionId = p.Id });
            }

            // Remove unwanted permissions if they exist (like Firms.Edit)
            var currentFirmPerms = await db.RolePermissions.Where(x => x.RoleId == firmAdminRole.Id).ToListAsync(ct);
            foreach (var rp in currentFirmPerms)
            {
                var perm = allPerms.FirstOrDefault(p => p.Id == rp.PermissionId);
                if (perm == null || !firmPermCodes.Any(code => perm.Code.StartsWith(code)))
                {
                    db.RolePermissions.Remove(rp);
                }
            }
        }

        var employeeRole = await roleManager.FindByNameAsync(Roles.Employee);
        if (employeeRole != null)
        {
            var empPermCodes = new[] 
            { 
                "Dashboard.View", "Invoices.", "GibSimulation.", "Orders.", "DeliveryNotes.", "Products.", 
                "Customers.", "MainAccountCodes.", "Banking.", "Cash.", "Payments.", "Cheques.", 
                "Reports.View", "CompanyProfile.View" // Note: ONLY View for CompanyProfile
            };
            var empPerms = allPerms.Where(x => empPermCodes.Any(code => x.Code.StartsWith(code))).ToList();

            foreach (var p in empPerms)
            {
                if (!await db.RolePermissions.AnyAsync(x => x.RoleId == employeeRole.Id && x.PermissionId == p.Id, ct))
                    db.RolePermissions.Add(new RolePermission { RoleId = employeeRole.Id, PermissionId = p.Id });
            }

            // Cleanup any unwanted permissions
            var currentEmpPerms = await db.RolePermissions.Where(x => x.RoleId == employeeRole.Id).ToListAsync(ct);
            foreach (var rp in currentEmpPerms)
            {
                var perm = allPerms.FirstOrDefault(p => p.Id == rp.PermissionId);
                // Specifically ensure NO .Edit for CompanyProfile even if it starts with "CompanyProfile."
                if (perm == null || !empPermCodes.Any(code => perm.Code.StartsWith(code)) || perm.Code == "CompanyProfile.Edit")
                {
                    db.RolePermissions.Remove(rp);
                }
            }
        }
        await db.SaveChangesAsync(ct);

        // 3. Menus
        var menus = new List<Menu>
        {
            new() { Label = "Dashboard", Icon = "dashboard", RouterLink = "/dashboard", SortOrder = 1, RequiredPermissionCode = "Dashboard.View" },
            new() { Label = "Faturalar", Icon = "receipt", RouterLink = "/invoices", SortOrder = 2, RequiredPermissionCode = "Invoices.View" },
            new() { Label = "GİB Kutusu (simülasyon)", Icon = "description", RouterLink = "/gib-simulation/inbox", SortOrder = 25, RequiredPermissionCode = "GibSimulation.ViewInbox" },
            new() { Label = "Siparişler", Icon = "shopping_cart", RouterLink = "/orders", SortOrder = 3, RequiredPermissionCode = "Orders.View" },
            new() { Label = "İrsaliyeler", Icon = "local_shipping", RouterLink = "/delivery-notes", SortOrder = 4, RequiredPermissionCode = "DeliveryNotes.View" },
            new() { Label = "Ürünler", Icon = "inventory_2", RouterLink = "/products", SortOrder = 5, RequiredPermissionCode = "Products.View" },
            new() { Label = "Cari Kartlar", Icon = "people", RouterLink = "/customers", SortOrder = 6, RequiredPermissionCode = "Customers.View" },
            new() { Label = "Hesap Kodları", Icon = "inventory", RouterLink = "/main-account-codes", SortOrder = 6, RequiredPermissionCode = "MainAccountCodes.View" },
            new() { Label = "Tahsilat / Ödeme", Icon = "account_balance_wallet", RouterLink = "/payments", SortOrder = 7, RequiredPermissionCode = "Payments.View" },
            new() { Label = "Kasa Yönetimi", Icon = "point_of_sale", RouterLink = "/cash-registers", SortOrder = 8, RequiredPermissionCode = "Cash.View" },
            new() { Label = "Banka Yönetimi", Icon = "account_balance", RouterLink = "/bank-accounts", SortOrder = 9, RequiredPermissionCode = "Banking.View" },
            new() { Label = "Çek / Senet", Icon = "history_edu", RouterLink = "/cheques", SortOrder = 10, RequiredPermissionCode = "Cheques.View" },
            new() { Label = "Raporlar", Icon = "analytics", RouterLink = "/reports", SortOrder = 100, RequiredPermissionCode = "Reports.View" },
            new() { Label = "Sistem Tanımları", Icon = "settings", RouterLink = "/lookups", SortOrder = 101, RequiredPermissionCode = "Lookups.View" },
            new() { Label = "Rol ve Yetki Yönetimi", Icon = "security", RouterLink = "/permissions", SortOrder = 102, RequiredPermissionCode = "Roles.View" },
            new() { Label = "Menü Yönetimi", Icon = "menu", RouterLink = "/menus", SortOrder = 103, RequiredPermissionCode = "Menus.View" },
            new() { Label = "Material İkonları", Icon = "palette", RouterLink = "/material-icons", SortOrder = 104, RequiredPermissionCode = "MaterialIcons.View" },
            new() { Label = "Firma Yönetimi", Icon = "business", RouterLink = "/firms", SortOrder = 105, RequiredPermissionCode = "Firms.View" },
            new() { Label = "Personel Yönetimi", Icon = "badge", RouterLink = "/employees", SortOrder = 106, RequiredPermissionCode = "Employees.View" },
            new() { Label = "Sistem Logları", Icon = "history", RouterLink = "/logs", SortOrder = 107, RequiredPermissionCode = "Logs.View" },
            new() { Label = "Arayüz Çevirileri", Icon = "translate", RouterLink = "/translations-admin", SortOrder = 108, RequiredPermissionCode = "Translations.View" },
            new() { Label = "Şirket Profili", Icon = "business", RouterLink = "/company/profile", SortOrder = 11, RequiredPermissionCode = "CompanyProfile.View" }
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
