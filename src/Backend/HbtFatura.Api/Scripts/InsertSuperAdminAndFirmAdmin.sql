-- =============================================================================
-- SuperAdmin ve FirmAdmin hesabı INSERT sorguları
-- =============================================================================
-- ŞİFRE HASH NASIL ALINIR?
-- 1) Uygulamayı çalıştırın (veritabanı boşsa appsettings Bootstrap ile bir
--    SuperAdmin oluşur). SSMS'te: SELECT PasswordHash FROM AspNetUsers WHERE Email = 'admin@hbt.com'
--    Bu değeri kopyalayıp aşağıdaki @PasswordHash değişkenine yapıştırın.
-- 2) Veya API'de geçici bir endpoint açıp şu kodu çalıştırın; dönen değeri kullanın:
--    var hasher = new PasswordHasher<ApplicationUser>();
--    var hash = hasher.HashPassword(null, "Admin123!");
-- =============================================================================

-- Rol Id'leri (roller zaten uygulama açılışında seed ile oluşuyor olabilir; yoksa INSERT et)
DECLARE @RoleSuperAdminId UNIQUEIDENTIFIER = (SELECT Id FROM AspNetRoles WHERE NormalizedName = 'SUPERADMIN');
DECLARE @RoleFirmAdminId  UNIQUEIDENTIFIER = (SELECT Id FROM AspNetRoles WHERE NormalizedName = 'FIRMADMIN');

-- Roller yoksa ekle (uygulama seed'i çalışmışsa atlayın)
IF @RoleSuperAdminId IS NULL
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES
        (NEWID(), 'SuperAdmin', 'SUPERADMIN', NEWID()),
        (NEWID(), 'FirmAdmin', 'FIRMADMIN', NEWID()),
        (NEWID(), 'Employee', 'EMPLOYEE', NEWID());
    SET @RoleSuperAdminId = (SELECT Id FROM AspNetRoles WHERE NormalizedName = 'SUPERADMIN');
    SET @RoleFirmAdminId  = (SELECT Id FROM AspNetRoles WHERE NormalizedName = 'FIRMADMIN');
END

-- Bu hash'i kendi ortamınızda üretip buraya yapıştırın (örn. "Admin123!" için)
-- Örnek: API'de geçici bir endpoint ile HashPassword çıktısını alın
DECLARE @PasswordHash NVARCHAR(MAX) = N'AQAAAAIAAYagAAAAEYourHashHereReplaceWithActualHashFromApp';
DECLARE @Now DATETIME2 = SYSUTCDATETIME();
DECLARE @SecurityStamp NVARCHAR(MAX) = REPLACE(NEWID(), '-', '');

-- =============================================================================
-- 1) SUPER ADMIN
-- =============================================================================
DECLARE @SuperAdminId UNIQUEIDENTIFIER = NEWID();
DECLARE @SuperAdminEmail NVARCHAR(256) = N'superadmin@hbt.com';

INSERT INTO AspNetUsers (
    Id,
    FullName,
    CreatedAt,
    FirmId,
    UserName,
    NormalizedUserName,
    Email,
    NormalizedEmail,
    EmailConfirmed,
    PasswordHash,
    SecurityStamp,
    ConcurrencyStamp,
    PhoneNumber,
    PhoneNumberConfirmed,
    TwoFactorEnabled,
    LockoutEnd,
    LockoutEnabled,
    AccessFailedCount
) VALUES (
    @SuperAdminId,
    N'Super Admin',
    @Now,
    NULL,  -- SuperAdmin'ın firması yok
    @SuperAdminEmail,
    UPPER(@SuperAdminEmail),
    @SuperAdminEmail,
    UPPER(@SuperAdminEmail),
    1,
    @PasswordHash,
    @SecurityStamp,
    NEWID(),
    NULL,
    0,
    0,
    NULL,
    1,
    0
);

INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT @SuperAdminId, Id FROM AspNetRoles WHERE NormalizedName = 'SUPERADMIN';

-- =============================================================================
-- 2) FİRMA ADMIN (önce firma ve CompanySettings gerekir)
-- =============================================================================
DECLARE @FirmId UNIQUEIDENTIFIER = NEWID();
DECLARE @CompanySettingsId UNIQUEIDENTIFIER = NEWID();
DECLARE @FirmAdminId UNIQUEIDENTIFIER = NEWID();
DECLARE @FirmAdminEmail NVARCHAR(256) = N'firmaadmin@firma.com';
DECLARE @FirmName NVARCHAR(256) = N'Örnek Firma';

-- Firma
INSERT INTO Firms (Id, Name, CreatedAt)
VALUES (@FirmId, @FirmName, @Now);

-- Firma için CompanySettings (1:1; boş kayıt)
INSERT INTO CompanySettings (Id, FirmId, CompanyName, TaxOffice, TaxNumber, Address, Phone, Email, IBAN, LogoUrl, CreatedAt, UpdatedAt)
VALUES (@CompanySettingsId, @FirmId, @FirmName, NULL, NULL, NULL, NULL, NULL, NULL, NULL, @Now, NULL);

-- Firma admin kullanıcı (aynı şifre hash'i kullanıyor; isterseniz farklı hash üretip koyun)
SET @SecurityStamp = REPLACE(NEWID(), '-', '');

INSERT INTO AspNetUsers (
    Id,
    FullName,
    CreatedAt,
    FirmId,
    UserName,
    NormalizedUserName,
    Email,
    NormalizedEmail,
    EmailConfirmed,
    PasswordHash,
    SecurityStamp,
    ConcurrencyStamp,
    PhoneNumber,
    PhoneNumberConfirmed,
    TwoFactorEnabled,
    LockoutEnd,
    LockoutEnabled,
    AccessFailedCount
) VALUES (
    @FirmAdminId,
    N'Firma Admin',
    @Now,
    @FirmId,
    @FirmAdminEmail,
    UPPER(@FirmAdminEmail),
    @FirmAdminEmail,
    UPPER(@FirmAdminEmail),
    1,
    @PasswordHash,
    @SecurityStamp,
    NEWID(),
    NULL,
    0,
    0,
    NULL,
    1,
    0
);

INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT @FirmAdminId, Id FROM AspNetRoles WHERE NormalizedName = 'FIRMADMIN';

-- =============================================================================
-- ÖZET
-- SuperAdmin: superadmin@hbt.com  (FirmId = NULL)
-- FirmAdmin:  firmaadmin@firma.com (FirmId = "Örnek Firma" kaydına bağlı)
-- Şifre: @PasswordHash'te kullandığınız şifre (örn. Admin123!) ile giriş yapın.
-- @PasswordHash mutlaka uygulama ile üretilmiş gerçek bir hash olmalıdır.
-- =============================================================================
