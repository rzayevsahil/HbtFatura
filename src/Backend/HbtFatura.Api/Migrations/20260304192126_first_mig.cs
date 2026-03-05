using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HbtFatura.Api.Migrations
{
    /// <inheritdoc />
    public partial class first_mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Firms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Firms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Module = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxOffices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxOffices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FirmId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Firms_FirmId",
                        column: x => x.FirmId,
                        principalTable: "Firms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BankAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirmId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Iban = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankAccounts_Firms_FirmId",
                        column: x => x.FirmId,
                        principalTable: "Firms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CashRegisters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirmId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashRegisters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashRegisters_Firms_FirmId",
                        column: x => x.FirmId,
                        principalTable: "Firms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanySettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirmId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaxOffice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IBAN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanySettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanySettings_Firms_FirmId",
                        column: x => x.FirmId,
                        principalTable: "Firms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MainAccountCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirmId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainAccountCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MainAccountCodes_Firms_FirmId",
                        column: x => x.FirmId,
                        principalTable: "Firms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirmId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StockQuantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Firms_FirmId",
                        column: x => x.FirmId,
                        principalTable: "Firms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MainAccountCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaxPayerType = table.Column<int>(type: "int", nullable: false),
                    CardType = table.Column<int>(type: "int", nullable: false),
                    TaxNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedByIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BankTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankTransactions_BankAccounts_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CashTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CashRegisterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashTransactions_CashRegisters_CashRegisterId",
                        column: x => x.CashRegisterId,
                        principalTable: "CashRegisters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockMovements_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountTransactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountTransactions_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChequeOrPromissories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirmId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BankAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChequeOrPromissories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChequeOrPromissories_BankAccounts_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChequeOrPromissories_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChequeOrPromissories_Firms_FirmId",
                        column: x => x.FirmId,
                        principalTable: "Firms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    InvoiceType = table.Column<int>(type: "int", nullable: false),
                    Scenario = table.Column<int>(type: "int", nullable: false),
                    Ettn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerTaxNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalVat = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    IsGibSent = table.Column<bool>(type: "bit", nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OrderType = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    VatRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LineTotalExclVat = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LineVatAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LineTotalInclVat = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceItems_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeliveryNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DeliveryType = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryNotes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryNotes_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryNotes_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    VatRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryNoteItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeliveryNoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    VatRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryNoteItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryNoteItems_DeliveryNotes_DeliveryNoteId",
                        column: x => x.DeliveryNoteId,
                        principalTable: "DeliveryNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliveryNoteItems_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DeliveryNoteItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "TaxOffices",
                columns: new[] { "Id", "City", "CreatedAt", "District", "Name" },
                values: new object[,]
                {
                    { new Guid("00000001-1111-1111-1111-000000000000"), "ADANA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Adana İhtisas Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000002-1111-1111-1111-000000000000"), "ADANA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "5 Ocak Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000003-1111-1111-1111-000000000000"), "ADANA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Yüreğir Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000004-1111-1111-1111-000000000000"), "ADANA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Seyhan Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000005-1111-1111-1111-000000000000"), "ADANA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Ziyapaşa Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000006-1111-1111-1111-000000000000"), "ADANA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Çukurova Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000007-1111-1111-1111-000000000000"), "ADANA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ceyhan", "Ceyhan Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000008-1111-1111-1111-000000000000"), "ADANA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kozan", "Kozan Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000009-1111-1111-1111-000000000000"), "ADANA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Karataş", "Karataş Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000000a-1111-1111-1111-000000000000"), "ADANA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Feke", "Feke Malmüdürlüğü" },
                    { new Guid("0000000b-1111-1111-1111-000000000000"), "ADANA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Karaisalı", "Karaisalı Malmüdürlüğü" },
                    { new Guid("0000000c-1111-1111-1111-000000000000"), "ADANA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Pozantı", "Pozantı Malmüdürlüğü" },
                    { new Guid("0000000d-1111-1111-1111-000000000000"), "ADANA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Saimbeyli", "Saimbeyli Malmüdürlüğü" },
                    { new Guid("0000000e-1111-1111-1111-000000000000"), "ADANA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Tufanbeyli", "Tufanbeyli Malmüdürlüğü" },
                    { new Guid("0000000f-1111-1111-1111-000000000000"), "ADANA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yumurtalık", "Yumurtalık Malmüdürlüğü" },
                    { new Guid("00000010-1111-1111-1111-000000000000"), "ADANA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Aladağ", "Aladağ Malmüdürlüğü" },
                    { new Guid("00000011-1111-1111-1111-000000000000"), "ADANA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İmamoğlu", "İmamoğlu Malmüdürlüğü" },
                    { new Guid("00000012-1111-1111-1111-000000000000"), "ADIYAMAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Adıyaman Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000013-1111-1111-1111-000000000000"), "ADIYAMAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kahta", "Kahta Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000014-1111-1111-1111-000000000000"), "ADIYAMAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Besni", "Besni Malmüdürlüğü" },
                    { new Guid("00000015-1111-1111-1111-000000000000"), "ADIYAMAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çelikhan", "Çelikhan Malmüdürlüğü" },
                    { new Guid("00000016-1111-1111-1111-000000000000"), "ADIYAMAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gerger", "Gerger Malmüdürlüğü" },
                    { new Guid("00000017-1111-1111-1111-000000000000"), "ADIYAMAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gölbaşı", "Gölbaşı Malmüdürlüğü" },
                    { new Guid("00000018-1111-1111-1111-000000000000"), "ADIYAMAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Samsat", "Samsat Malmüdürlüğü" },
                    { new Guid("00000019-1111-1111-1111-000000000000"), "ADIYAMAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sincik", "Sincik Malmüdürlüğü" },
                    { new Guid("0000001a-1111-1111-1111-000000000000"), "ADIYAMAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Tut", "Tut Malmüdürlüğü" },
                    { new Guid("0000001b-1111-1111-1111-000000000000"), "AFYONKARAHİSAR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Tınaztepe Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000001c-1111-1111-1111-000000000000"), "AFYONKARAHİSAR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kocatepe Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000001d-1111-1111-1111-000000000000"), "AFYONKARAHİSAR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Dinar", "Dinar Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000001e-1111-1111-1111-000000000000"), "AFYONKARAHİSAR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bolvadin", "Bolvadin Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000001f-1111-1111-1111-000000000000"), "AFYONKARAHİSAR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Emirdağ", "Emirdağ Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000020-1111-1111-1111-000000000000"), "AFYONKARAHİSAR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sandıklı", "Sandıklı Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000021-1111-1111-1111-000000000000"), "AFYONKARAHİSAR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İscehisar", "İscehisar Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000022-1111-1111-1111-000000000000"), "AFYONKARAHİSAR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çay", "Çay Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000023-1111-1111-1111-000000000000"), "AFYONKARAHİSAR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Dazkırı", "Dazkırı Malmüdürlüğü" },
                    { new Guid("00000024-1111-1111-1111-000000000000"), "AFYONKARAHİSAR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İhsaniye", "İhsaniye Malmüdürlüğü" },
                    { new Guid("00000025-1111-1111-1111-000000000000"), "AFYONKARAHİSAR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sinanpaşa", "Sinanpaşa Malmüdürlüğü" },
                    { new Guid("00000026-1111-1111-1111-000000000000"), "AFYONKARAHİSAR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sultandağı", "Sultandağı Malmüdürlüğü" },
                    { new Guid("00000027-1111-1111-1111-000000000000"), "AFYONKARAHİSAR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Şuhut", "Şuhut Malmüdürlüğü" },
                    { new Guid("00000028-1111-1111-1111-000000000000"), "AFYONKARAHİSAR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Başmakçı", "Başmakçı Malmüdürlüğü" },
                    { new Guid("00000029-1111-1111-1111-000000000000"), "AFYONKARAHİSAR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bayat", "Bayat Malmüdürlüğü" },
                    { new Guid("0000002a-1111-1111-1111-000000000000"), "AFYONKARAHİSAR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çobanlar", "Çobanlar Malmüdürlüğü" },
                    { new Guid("0000002b-1111-1111-1111-000000000000"), "AFYONKARAHİSAR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Evciler", "Evciler Malmüdürlüğü" },
                    { new Guid("0000002c-1111-1111-1111-000000000000"), "AFYONKARAHİSAR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hocalar", "Hocalar Malmüdürlüğü" },
                    { new Guid("0000002d-1111-1111-1111-000000000000"), "AFYONKARAHİSAR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kızılören", "Kızılören Malmüdürlüğü" },
                    { new Guid("0000002e-1111-1111-1111-000000000000"), "AĞRI", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Ağrı Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000002f-1111-1111-1111-000000000000"), "AĞRI", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Doğubeyazıt", "Doğubeyazıt Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000030-1111-1111-1111-000000000000"), "AĞRI", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Patnos", "Patnos Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000031-1111-1111-1111-000000000000"), "AĞRI", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Diyadin", "Diyadin Malmüdürlüğü" },
                    { new Guid("00000032-1111-1111-1111-000000000000"), "AĞRI", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Eleşkirt", "Eleşkirt Malmüdürlüğü" },
                    { new Guid("00000033-1111-1111-1111-000000000000"), "AĞRI", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hamur", "Hamur Malmüdürlüğü" },
                    { new Guid("00000034-1111-1111-1111-000000000000"), "AĞRI", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Taşlıçay", "Taşlıçay Malmüdürlüğü" },
                    { new Guid("00000035-1111-1111-1111-000000000000"), "AĞRI", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Tutak", "Tutak Malmüdürlüğü" },
                    { new Guid("00000036-1111-1111-1111-000000000000"), "AMASYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Amasya Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000037-1111-1111-1111-000000000000"), "AMASYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merzifon", "Merzifon Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000038-1111-1111-1111-000000000000"), "AMASYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gümüşhacıköy", "Gümüşhacıköy Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000039-1111-1111-1111-000000000000"), "AMASYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Taşova", "Taşova Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000003a-1111-1111-1111-000000000000"), "AMASYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Suluova", "Suluova Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000003b-1111-1111-1111-000000000000"), "AMASYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Göynücek", "Göynücek Malmüdürlüğü" },
                    { new Guid("0000003c-1111-1111-1111-000000000000"), "AMASYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hamamözü", "Hamamözü Malmüdürlüğü" },
                    { new Guid("0000003d-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Anadolu İhtisas Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000003e-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Ankara İhtisas Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000003f-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Veraset ve Harçlar Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000040-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kavaklıdere Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000041-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Maltepe Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000042-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Yenimahalle Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000043-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Çankaya Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000044-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kızılbey Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000045-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Mithatpaşa Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000046-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Ulus Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000047-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Yıldırım Beyazıt Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000048-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Seğmenler Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000049-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Dikimevi Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000004a-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Doğanbey Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000004b-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Yeğenbey Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000004c-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Hitit Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000004d-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Yahya Galip Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000004e-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Muhammet Karagüzel Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000004f-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Ostim Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000050-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Gölbaşı Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000051-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Sincan Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000052-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Dışkapı Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000053-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Etimesgut Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000054-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Başkent Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000055-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Cumhuriyet Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000056-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Keçiören Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000057-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kahramankazan Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000058-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Polatlı", "Polatlı Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000059-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Şereflikoçhisar", "Şereflikoçhisar Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000005a-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Beypazarı", "Beypazarı Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000005b-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Çubuk Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000005c-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Haymana", "Haymana Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000005d-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Elmadağ Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000005e-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Ayaş Malmüdürlüğü" },
                    { new Guid("0000005f-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Balâ Malmüdürlüğü" },
                    { new Guid("00000060-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çamlıdere", "Çamlıdere Malmüdürlüğü" },
                    { new Guid("00000061-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Güdül", "Güdül Malmüdürlüğü" },
                    { new Guid("00000062-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kalecik Malmüdürlüğü" },
                    { new Guid("00000063-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kızılcahamam", "Kızılcahamam Malmüdürlüğü" },
                    { new Guid("00000064-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Nallıhan", "Nallıhan Malmüdürlüğü" },
                    { new Guid("00000065-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Akyurt Malmüdürlüğü" },
                    { new Guid("00000066-1111-1111-1111-000000000000"), "ANKARA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Evren", "Evren Malmüdürlüğü" },
                    { new Guid("00000067-1111-1111-1111-000000000000"), "ANTALYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Üçkapılar Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000068-1111-1111-1111-000000000000"), "ANTALYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kalekapı Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000069-1111-1111-1111-000000000000"), "ANTALYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Muratpaşa Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000006a-1111-1111-1111-000000000000"), "ANTALYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Düden Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000006b-1111-1111-1111-000000000000"), "ANTALYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Antalya Kurumlar Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000006c-1111-1111-1111-000000000000"), "ANTALYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Antalya İhtisas Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000006d-1111-1111-1111-000000000000"), "ANTALYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Korkuteli", "Korkuteli Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000006e-1111-1111-1111-000000000000"), "ANTALYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Alanya", "Alanya Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000006f-1111-1111-1111-000000000000"), "ANTALYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Serik", "Serik Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000070-1111-1111-1111-000000000000"), "ANTALYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Manavgat", "Manavgat Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000071-1111-1111-1111-000000000000"), "ANTALYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Elmalı", "Elmalı Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000072-1111-1111-1111-000000000000"), "ANTALYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kemer", "Kemer Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000073-1111-1111-1111-000000000000"), "ANTALYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kumluca", "Kumluca Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000074-1111-1111-1111-000000000000"), "ANTALYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Akseki", "Akseki Malmüdürlüğü" },
                    { new Guid("00000075-1111-1111-1111-000000000000"), "ANTALYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Finike", "Finike Malmüdürlüğü" },
                    { new Guid("00000076-1111-1111-1111-000000000000"), "ANTALYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gazipaşa", "Gazipaşa Malmüdürlüğü" },
                    { new Guid("00000077-1111-1111-1111-000000000000"), "ANTALYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gündoğmuş", "Gündoğmuş Malmüdürlüğü" },
                    { new Guid("00000078-1111-1111-1111-000000000000"), "ANTALYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kaş", "Kaş Malmüdürlüğü" },
                    { new Guid("00000079-1111-1111-1111-000000000000"), "ANTALYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Demre", "Demre Malmüdürlüğü" },
                    { new Guid("0000007a-1111-1111-1111-000000000000"), "ANTALYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İbradı", "İbradı Malmüdürlüğü" },
                    { new Guid("0000007b-1111-1111-1111-000000000000"), "ARTVİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Artvin Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000007c-1111-1111-1111-000000000000"), "ARTVİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hopa", "Hopa Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000007d-1111-1111-1111-000000000000"), "ARTVİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Arhavi", "Arhavi Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000007e-1111-1111-1111-000000000000"), "ARTVİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ardanuç", "Ardanuç Malmüdürlüğü" },
                    { new Guid("0000007f-1111-1111-1111-000000000000"), "ARTVİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Borçka", "Borçka Malmüdürlüğü" },
                    { new Guid("00000080-1111-1111-1111-000000000000"), "ARTVİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Şavşat", "Şavşat Malmüdürlüğü" },
                    { new Guid("00000081-1111-1111-1111-000000000000"), "ARTVİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yusufeli", "Yusufeli Malmüdürlüğü" },
                    { new Guid("00000082-1111-1111-1111-000000000000"), "ARTVİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Murgul", "Murgul Malmüdürlüğü" },
                    { new Guid("00000083-1111-1111-1111-000000000000"), "AYDIN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Efeler Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000084-1111-1111-1111-000000000000"), "AYDIN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Güzelhisar Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000085-1111-1111-1111-000000000000"), "AYDIN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Nazilli", "Nazilli Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000086-1111-1111-1111-000000000000"), "AYDIN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Söke", "Söke Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000087-1111-1111-1111-000000000000"), "AYDIN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çine", "Çine Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000088-1111-1111-1111-000000000000"), "AYDIN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Germencik", "Germencik Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000089-1111-1111-1111-000000000000"), "AYDIN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kuşadası", "Kuşadası Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000008a-1111-1111-1111-000000000000"), "AYDIN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Didim", "Didim Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000008b-1111-1111-1111-000000000000"), "AYDIN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bozdoğan", "Bozdoğan Malmüdürlüğü" },
                    { new Guid("0000008c-1111-1111-1111-000000000000"), "AYDIN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Karacasu", "Karacasu Malmüdürlüğü" },
                    { new Guid("0000008d-1111-1111-1111-000000000000"), "AYDIN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Koçarlı", "Koçarlı Malmüdürlüğü" },
                    { new Guid("0000008e-1111-1111-1111-000000000000"), "AYDIN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kuyucak", "Kuyucak Malmüdürlüğü" },
                    { new Guid("0000008f-1111-1111-1111-000000000000"), "AYDIN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sultanhisar", "Sultanhisar Malmüdürlüğü" },
                    { new Guid("00000090-1111-1111-1111-000000000000"), "AYDIN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yenipazar", "Yenipazar Malmüdürlüğü" },
                    { new Guid("00000091-1111-1111-1111-000000000000"), "AYDIN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Buharkent", "Buharkent Malmüdürlüğü" },
                    { new Guid("00000092-1111-1111-1111-000000000000"), "AYDIN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İncirliova", "İncirliova Malmüdürlüğü" },
                    { new Guid("00000093-1111-1111-1111-000000000000"), "AYDIN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Karpuzlu", "Karpuzlu Malmüdürlüğü" },
                    { new Guid("00000094-1111-1111-1111-000000000000"), "AYDIN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Köşk", "Köşk Malmüdürlüğü" },
                    { new Guid("00000095-1111-1111-1111-000000000000"), "BALIKESİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Karesi Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000096-1111-1111-1111-000000000000"), "BALIKESİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kurtdereli Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000097-1111-1111-1111-000000000000"), "BALIKESİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ayvalık", "Ayvalık Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000098-1111-1111-1111-000000000000"), "BALIKESİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bandırma", "Bandırma Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000099-1111-1111-1111-000000000000"), "BALIKESİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Burhaniye", "Burhaniye Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000009a-1111-1111-1111-000000000000"), "BALIKESİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Edremit", "Edremit Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000009b-1111-1111-1111-000000000000"), "BALIKESİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gönen", "Gönen Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000009c-1111-1111-1111-000000000000"), "BALIKESİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Susurluk", "Susurluk Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000009d-1111-1111-1111-000000000000"), "BALIKESİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Erdek", "Erdek Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000009e-1111-1111-1111-000000000000"), "BALIKESİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bigadiç", "Bigadiç Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000009f-1111-1111-1111-000000000000"), "BALIKESİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sındırgı", "Sındırgı Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000a0-1111-1111-1111-000000000000"), "BALIKESİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Dursunbey", "Dursunbey Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000a1-1111-1111-1111-000000000000"), "BALIKESİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Balya", "Balya Malmüdürlüğü" },
                    { new Guid("000000a2-1111-1111-1111-000000000000"), "BALIKESİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Havran", "Havran Malmüdürlüğü" },
                    { new Guid("000000a3-1111-1111-1111-000000000000"), "BALIKESİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İvrindi", "İvrindi Malmüdürlüğü" },
                    { new Guid("000000a4-1111-1111-1111-000000000000"), "BALIKESİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kepsut", "Kepsut Malmüdürlüğü" },
                    { new Guid("000000a5-1111-1111-1111-000000000000"), "BALIKESİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Manyas", "Manyas Malmüdürlüğü" },
                    { new Guid("000000a6-1111-1111-1111-000000000000"), "BALIKESİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Savaştepe", "Savaştepe Malmüdürlüğü" },
                    { new Guid("000000a7-1111-1111-1111-000000000000"), "BALIKESİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Marmara", "Marmara Malmüdürlüğü" },
                    { new Guid("000000a8-1111-1111-1111-000000000000"), "BALIKESİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gömeç", "Gömeç Malmüdürlüğü" },
                    { new Guid("000000a9-1111-1111-1111-000000000000"), "BİLECİK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Bilecik Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000aa-1111-1111-1111-000000000000"), "BİLECİK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bozüyük", "Bozüyük Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000ab-1111-1111-1111-000000000000"), "BİLECİK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gölpazarı", "Gölpazarı Malmüdürlüğü" },
                    { new Guid("000000ac-1111-1111-1111-000000000000"), "BİLECİK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Osmaneli", "Osmaneli Malmüdürlüğü" },
                    { new Guid("000000ad-1111-1111-1111-000000000000"), "BİLECİK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Pazaryeri", "Pazaryeri Malmüdürlüğü" },
                    { new Guid("000000ae-1111-1111-1111-000000000000"), "BİLECİK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Söğüt", "Söğüt Malmüdürlüğü" },
                    { new Guid("000000af-1111-1111-1111-000000000000"), "BİLECİK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yenipazar", "Yenipazar Malmüdürlüğü" },
                    { new Guid("000000b0-1111-1111-1111-000000000000"), "BİLECİK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İnhisar", "İnhisar Malmüdürlüğü" },
                    { new Guid("000000b1-1111-1111-1111-000000000000"), "BİNGÖL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Bingöl Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000b2-1111-1111-1111-000000000000"), "BİNGÖL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Genç", "Genç Malmüdürlüğü" },
                    { new Guid("000000b3-1111-1111-1111-000000000000"), "BİNGÖL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Karlıova", "Karlıova Malmüdürlüğü" },
                    { new Guid("000000b4-1111-1111-1111-000000000000"), "BİNGÖL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kiğı", "Kiğı Malmüdürlüğü" },
                    { new Guid("000000b5-1111-1111-1111-000000000000"), "BİNGÖL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Solhan", "Solhan Malmüdürlüğü" },
                    { new Guid("000000b6-1111-1111-1111-000000000000"), "BİNGÖL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Adaklı", "Adaklı Malmüdürlüğü" },
                    { new Guid("000000b7-1111-1111-1111-000000000000"), "BİNGÖL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yayladere", "Yayladere Malmüdürlüğü" },
                    { new Guid("000000b8-1111-1111-1111-000000000000"), "BİNGÖL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yedisu", "Yedisu Malmüdürlüğü" },
                    { new Guid("000000b9-1111-1111-1111-000000000000"), "BİTLİS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Bitlis Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000ba-1111-1111-1111-000000000000"), "BİTLİS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Tatvan", "Tatvan Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000bb-1111-1111-1111-000000000000"), "BİTLİS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Adilcevaz", "Adilcevaz Malmüdürlüğü" },
                    { new Guid("000000bc-1111-1111-1111-000000000000"), "BİTLİS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ahlat", "Ahlat Malmüdürlüğü" },
                    { new Guid("000000bd-1111-1111-1111-000000000000"), "BİTLİS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hizan", "Hizan Malmüdürlüğü" },
                    { new Guid("000000be-1111-1111-1111-000000000000"), "BİTLİS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Mutki", "Mutki Malmüdürlüğü" },
                    { new Guid("000000bf-1111-1111-1111-000000000000"), "BİTLİS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Göroymak", "Güroymak Malmüdürlüğü" },
                    { new Guid("000000c0-1111-1111-1111-000000000000"), "BOLU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Bolu Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000c1-1111-1111-1111-000000000000"), "BOLU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gerede", "Gerede Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000c2-1111-1111-1111-000000000000"), "BOLU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Göynük", "Göynük Malmüdürlüğü" },
                    { new Guid("000000c3-1111-1111-1111-000000000000"), "BOLU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kıbrıscık", "Kıbrıscık Malmüdürlüğü" },
                    { new Guid("000000c4-1111-1111-1111-000000000000"), "BOLU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Mengen", "Mengen Malmüdürlüğü" },
                    { new Guid("000000c5-1111-1111-1111-000000000000"), "BOLU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Mudurnu", "Mudurnu Malmüdürlüğü" },
                    { new Guid("000000c6-1111-1111-1111-000000000000"), "BOLU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Seben", "Seben Malmüdürlüğü" },
                    { new Guid("000000c7-1111-1111-1111-000000000000"), "BOLU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Dörtdivan", "Dörtdivan Malmüdürlüğü" },
                    { new Guid("000000c8-1111-1111-1111-000000000000"), "BOLU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yeniçağa", "Yeniçağa Malmüdürlüğü" },
                    { new Guid("000000c9-1111-1111-1111-000000000000"), "BURDUR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Burdur Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000ca-1111-1111-1111-000000000000"), "BURDUR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bucak", "Bucak Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000cb-1111-1111-1111-000000000000"), "BURDUR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ağlasun", "Ağlasun Malmüdürlüğü" },
                    { new Guid("000000cc-1111-1111-1111-000000000000"), "BURDUR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gölhisar", "Gölhisar Malmüdürlüğü" },
                    { new Guid("000000cd-1111-1111-1111-000000000000"), "BURDUR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Tefenni", "Tefenni Malmüdürlüğü" },
                    { new Guid("000000ce-1111-1111-1111-000000000000"), "BURDUR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yeşilova", "Yeşilova Malmüdürlüğü" },
                    { new Guid("000000cf-1111-1111-1111-000000000000"), "BURDUR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Karamanlı", "Karamanlı Malmüdürlüğü" },
                    { new Guid("000000d0-1111-1111-1111-000000000000"), "BURDUR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kemer", "Kemer Malmüdürlüğü" },
                    { new Guid("000000d1-1111-1111-1111-000000000000"), "BURDUR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Altınyayla", "Altınyayla Malmüdürlüğü" },
                    { new Guid("000000d2-1111-1111-1111-000000000000"), "BURDUR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çavdır", "Çavdır Malmüdürlüğü" },
                    { new Guid("000000d3-1111-1111-1111-000000000000"), "BURDUR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çeltikçi", "Çeltikçi Malmüdürlüğü" },
                    { new Guid("000000d4-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Bursa İhtisas Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000d5-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Osmangazi Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000d6-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Yıldırım Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000d7-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Çekirge Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000d8-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Setbaşı Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000d9-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Uludağ Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000da-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Yeşil Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000db-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Nilüfer Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000dc-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Ertuğrulgazi Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000dd-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Gökdere Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000de-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Bursa Veraset ve Harçlar Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000df-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Gemlik Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000e0-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İnegöl", "İnegöl Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000e1-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Karacabey", "Karacabey Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000e2-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Mustafakemalpaşa", "Mustafakemalpaşa Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000e3-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Mudanya Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000e4-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Orhangazi", "Orhangazi Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000e5-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İznik", "İznik Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000e6-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yenişehir", "Yenişehir Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000e7-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Keles", "Keles Malmüdürlüğü" },
                    { new Guid("000000e8-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Orhaneli", "Orhaneli Malmüdürlüğü" },
                    { new Guid("000000e9-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Harmancık", "Harmancık Malmüdürlüğü" },
                    { new Guid("000000ea-1111-1111-1111-000000000000"), "BURSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Büyükorhan", "Büyükorhan Malmüdürlüğü" },
                    { new Guid("000000eb-1111-1111-1111-000000000000"), "ÇANAKKALE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Çanakkale Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000ec-1111-1111-1111-000000000000"), "ÇANAKKALE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Biga", "Biga Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000ed-1111-1111-1111-000000000000"), "ÇANAKKALE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çan", "Çan Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000ee-1111-1111-1111-000000000000"), "ÇANAKKALE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gelibolu", "Gelibolu Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000ef-1111-1111-1111-000000000000"), "ÇANAKKALE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ayvacık", "Ayvacık Malmüdürlüğü" },
                    { new Guid("000000f0-1111-1111-1111-000000000000"), "ÇANAKKALE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bayramiç", "Bayramiç Malmüdürlüğü" },
                    { new Guid("000000f1-1111-1111-1111-000000000000"), "ÇANAKKALE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bozcaada", "Bozcaada Malmüdürlüğü" },
                    { new Guid("000000f2-1111-1111-1111-000000000000"), "ÇANAKKALE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Eceabat", "Eceabat Malmüdürlüğü" },
                    { new Guid("000000f3-1111-1111-1111-000000000000"), "ÇANAKKALE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ezine", "Ezine Malmüdürlüğü" },
                    { new Guid("000000f4-1111-1111-1111-000000000000"), "ÇANAKKALE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gökçeada", "Gökçeada Malmüdürlüğü" },
                    { new Guid("000000f5-1111-1111-1111-000000000000"), "ÇANAKKALE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Lapseki", "Lapseki Malmüdürlüğü" },
                    { new Guid("000000f6-1111-1111-1111-000000000000"), "ÇANAKKALE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yenice", "Yenice Malmüdürlüğü" },
                    { new Guid("000000f7-1111-1111-1111-000000000000"), "ÇANKIRI", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Çankırı Vergi Dairesi Müdürlüğü" },
                    { new Guid("000000f8-1111-1111-1111-000000000000"), "ÇANKIRI", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çerkeş", "Çerkeş Malmüdürlüğü" },
                    { new Guid("000000f9-1111-1111-1111-000000000000"), "ÇANKIRI", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Eldivan", "Eldivan Malmüdürlüğü" },
                    { new Guid("000000fa-1111-1111-1111-000000000000"), "ÇANKIRI", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ilgaz", "Ilgaz Malmüdürlüğü" },
                    { new Guid("000000fb-1111-1111-1111-000000000000"), "ÇANKIRI", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kurşunlu", "Kurşunlu Malmüdürlüğü" },
                    { new Guid("000000fc-1111-1111-1111-000000000000"), "ÇANKIRI", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Orta", "Orta Malmüdürlüğü" },
                    { new Guid("000000fd-1111-1111-1111-000000000000"), "ÇANKIRI", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Şabanözü", "Şabanözü Malmüdürlüğü" },
                    { new Guid("000000fe-1111-1111-1111-000000000000"), "ÇANKIRI", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yapraklı", "Yapraklı Malmüdürlüğü" },
                    { new Guid("000000ff-1111-1111-1111-000000000000"), "ÇANKIRI", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Atkaracalar", "Atkaracalar Malmüdürlüğü" },
                    { new Guid("00000100-1111-1111-1111-000000000000"), "ÇANKIRI", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kızılırmak", "Kızılırmak Malmüdürlüğü" },
                    { new Guid("00000101-1111-1111-1111-000000000000"), "ÇANKIRI", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bayramören", "Bayramören Malmüdürlüğü" },
                    { new Guid("00000102-1111-1111-1111-000000000000"), "ÇANKIRI", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Korgun", "Korgun Malmüdürlüğü" },
                    { new Guid("00000103-1111-1111-1111-000000000000"), "ÇORUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Çorum Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000104-1111-1111-1111-000000000000"), "ÇORUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sungurlu", "Sungurlu Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000105-1111-1111-1111-000000000000"), "ÇORUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Alaca", "Alaca Malmüdürlüğü" },
                    { new Guid("00000106-1111-1111-1111-000000000000"), "ÇORUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bayat", "Bayat Malmüdürlüğü" },
                    { new Guid("00000107-1111-1111-1111-000000000000"), "ÇORUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İskilip", "İskilip Malmüdürlüğü" },
                    { new Guid("00000108-1111-1111-1111-000000000000"), "ÇORUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kargı", "Kargı Malmüdürlüğü" },
                    { new Guid("00000109-1111-1111-1111-000000000000"), "ÇORUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Mecitözü", "Mecitözü Malmüdürlüğü" },
                    { new Guid("0000010a-1111-1111-1111-000000000000"), "ÇORUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ortaköy", "Ortaköy Malmüdürlüğü" },
                    { new Guid("0000010b-1111-1111-1111-000000000000"), "ÇORUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Osmancık", "Osmancık Malmüdürlüğü" },
                    { new Guid("0000010c-1111-1111-1111-000000000000"), "ÇORUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Boğazkale", "Boğazkale Malmüdürlüğü" },
                    { new Guid("0000010d-1111-1111-1111-000000000000"), "ÇORUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Uğurludağ", "Uğurludağ Malmüdürlüğü" },
                    { new Guid("0000010e-1111-1111-1111-000000000000"), "ÇORUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Dodurga", "Dodurga Malmüdürlüğü" },
                    { new Guid("0000010f-1111-1111-1111-000000000000"), "ÇORUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Oğuzlar", "Oğuzlar Malmüdürlüğü" },
                    { new Guid("00000110-1111-1111-1111-000000000000"), "ÇORUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Laçin", "Laçin Malmüdürlüğü" },
                    { new Guid("00000111-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Saraylar Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000112-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Çınar Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000113-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Gökpınar Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000114-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Denizli İhtisas Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000115-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Pamukkale Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000116-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sarayköy", "Sarayköy Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000117-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Acıpayam", "Acıpayam Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000118-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Tavas", "Tavas Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000119-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Buldan", "Buldan Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000011a-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çal", "Çal Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000011b-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çivril", "Çivril Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000011c-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çameli", "Çameli Malmüdürlüğü" },
                    { new Guid("0000011d-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çardak", "Çardak Malmüdürlüğü" },
                    { new Guid("0000011e-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Güney", "Güney Malmüdürlüğü" },
                    { new Guid("0000011f-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kale", "Kale Malmüdürlüğü" },
                    { new Guid("00000120-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Babadağ", "Babadağ Malmüdürlüğü" },
                    { new Guid("00000121-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bekilli", "Bekilli Malmüdürlüğü" },
                    { new Guid("00000122-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Honaz", "Honaz Malmüdürlüğü" },
                    { new Guid("00000123-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Serinhisar", "Serinhisar Malmüdürlüğü" },
                    { new Guid("00000124-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Pamukkale", "Pamukkale Malmüdürlüğü (Akköy)" },
                    { new Guid("00000125-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Baklan", "Baklan Malmüdürlüğü" },
                    { new Guid("00000126-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Beyağaç", "Beyağaç Malmüdürlüğü" },
                    { new Guid("00000127-1111-1111-1111-000000000000"), "DENİZLİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bozkurt", "Bozkurt Malmüdürlüğü" },
                    { new Guid("00000128-1111-1111-1111-000000000000"), "DİYARBAKIR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Gökalp Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000129-1111-1111-1111-000000000000"), "DİYARBAKIR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Süleyman Nazif Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000012a-1111-1111-1111-000000000000"), "DİYARBAKIR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Cahit Sıtkı Tarancı Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000012b-1111-1111-1111-000000000000"), "DİYARBAKIR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ergani", "Ergani Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000012c-1111-1111-1111-000000000000"), "DİYARBAKIR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bismil", "Bismil Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000012d-1111-1111-1111-000000000000"), "DİYARBAKIR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çermik", "Çermik Malmüdürlüğü" },
                    { new Guid("0000012e-1111-1111-1111-000000000000"), "DİYARBAKIR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çınar", "Çınar Malmüdürlüğü" },
                    { new Guid("0000012f-1111-1111-1111-000000000000"), "DİYARBAKIR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çüngüş", "Çüngüş Malmüdürlüğü" },
                    { new Guid("00000130-1111-1111-1111-000000000000"), "DİYARBAKIR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Dicle", "Dicle Malmüdürlüğü" },
                    { new Guid("00000131-1111-1111-1111-000000000000"), "DİYARBAKIR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hani", "Hani Malmüdürlüğü" },
                    { new Guid("00000132-1111-1111-1111-000000000000"), "DİYARBAKIR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hazro", "Hazro Malmüdürlüğü" },
                    { new Guid("00000133-1111-1111-1111-000000000000"), "DİYARBAKIR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kulp", "Kulp Malmüdürlüğü" },
                    { new Guid("00000134-1111-1111-1111-000000000000"), "DİYARBAKIR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Lice", "Lice Malmüdürlüğü" },
                    { new Guid("00000135-1111-1111-1111-000000000000"), "DİYARBAKIR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Silvan", "Silvan Malmüdürlüğü" },
                    { new Guid("00000136-1111-1111-1111-000000000000"), "DİYARBAKIR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Eğil", "Eğil Malmüdürlüğü" },
                    { new Guid("00000137-1111-1111-1111-000000000000"), "DİYARBAKIR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kocaköy", "Kocaköy Malmüdürlüğü" },
                    { new Guid("00000138-1111-1111-1111-000000000000"), "EDİRNE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Arda Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000139-1111-1111-1111-000000000000"), "EDİRNE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kırkpınar Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000013a-1111-1111-1111-000000000000"), "EDİRNE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Keşan", "Keşan Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000013b-1111-1111-1111-000000000000"), "EDİRNE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Uzunköprü", "Uzunköprü Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000013c-1111-1111-1111-000000000000"), "EDİRNE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Havsa", "Havsa Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000013d-1111-1111-1111-000000000000"), "EDİRNE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İpsala", "İpsala Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000013e-1111-1111-1111-000000000000"), "EDİRNE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Enez", "Enez Malmüdürlüğü" },
                    { new Guid("0000013f-1111-1111-1111-000000000000"), "EDİRNE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Lalapaşa", "Lalapaşa Malmüdürlüğü" },
                    { new Guid("00000140-1111-1111-1111-000000000000"), "EDİRNE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Meriç", "Meriç Malmüdürlüğü" },
                    { new Guid("00000141-1111-1111-1111-000000000000"), "EDİRNE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Süloğlu", "Süloğlu Malmüdürlüğü" },
                    { new Guid("00000142-1111-1111-1111-000000000000"), "ELAZIĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Harput Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000143-1111-1111-1111-000000000000"), "ELAZIĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Hazar Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000144-1111-1111-1111-000000000000"), "ELAZIĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ağın", "Ağın Malmüdürlüğü" },
                    { new Guid("00000145-1111-1111-1111-000000000000"), "ELAZIĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Baskil", "Baskil Malmüdürlüğü" },
                    { new Guid("00000146-1111-1111-1111-000000000000"), "ELAZIĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Karakoçan", "Karakoçan Malmüdürlüğü" },
                    { new Guid("00000147-1111-1111-1111-000000000000"), "ELAZIĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Keban", "Keban Malmüdürlüğü" },
                    { new Guid("00000148-1111-1111-1111-000000000000"), "ELAZIĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Maden", "Maden Malmüdürlüğü" },
                    { new Guid("00000149-1111-1111-1111-000000000000"), "ELAZIĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Palu", "Palu Malmüdürlüğü" },
                    { new Guid("0000014a-1111-1111-1111-000000000000"), "ELAZIĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sivrice", "Sivrice Malmüdürlüğü" },
                    { new Guid("0000014b-1111-1111-1111-000000000000"), "ELAZIĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Arıcak", "Arıcak Malmüdürlüğü" },
                    { new Guid("0000014c-1111-1111-1111-000000000000"), "ELAZIĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kovancılar", "Kovancılar Malmüdürlüğü" },
                    { new Guid("0000014d-1111-1111-1111-000000000000"), "ELAZIĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Alacakaya", "Alacakaya Malmüdürlüğü" },
                    { new Guid("0000014e-1111-1111-1111-000000000000"), "ERZİNCAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Fevzipaşa Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000014f-1111-1111-1111-000000000000"), "ERZİNCAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çayırlı", "Çayırlı Malmüdürlüğü" },
                    { new Guid("00000150-1111-1111-1111-000000000000"), "ERZİNCAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İliç", "İliç Malmüdürlüğü" },
                    { new Guid("00000151-1111-1111-1111-000000000000"), "ERZİNCAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kemah", "Kemah Malmüdürlüğü" },
                    { new Guid("00000152-1111-1111-1111-000000000000"), "ERZİNCAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kemaliye", "Kemaliye Malmüdürlüğü" },
                    { new Guid("00000153-1111-1111-1111-000000000000"), "ERZİNCAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Refahiye", "Refahiye Malmüdürlüğü" },
                    { new Guid("00000154-1111-1111-1111-000000000000"), "ERZİNCAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Tercan", "Tercan Malmüdürlüğü" },
                    { new Guid("00000155-1111-1111-1111-000000000000"), "ERZİNCAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Üzümlü", "Üzümlü Malmüdürlüğü" },
                    { new Guid("00000156-1111-1111-1111-000000000000"), "ERZİNCAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Otlukbeli", "Otlukbeli Malmüdürlüğü" },
                    { new Guid("00000157-1111-1111-1111-000000000000"), "ERZURUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Aziziye Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000158-1111-1111-1111-000000000000"), "ERZURUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kazımkarabekir Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000159-1111-1111-1111-000000000000"), "ERZURUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Aşkale", "Aşkale Malmüdürlüğü" },
                    { new Guid("0000015a-1111-1111-1111-000000000000"), "ERZURUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çat", "Çat Malmüdürlüğü" },
                    { new Guid("0000015b-1111-1111-1111-000000000000"), "ERZURUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hınıs", "Hınıs Malmüdürlüğü" },
                    { new Guid("0000015c-1111-1111-1111-000000000000"), "ERZURUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Horasan", "Horasan Malmüdürlüğü" },
                    { new Guid("0000015d-1111-1111-1111-000000000000"), "ERZURUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İspir", "İspir Malmüdürlüğü" },
                    { new Guid("0000015e-1111-1111-1111-000000000000"), "ERZURUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Karayazı", "Karayazı Malmüdürlüğü" },
                    { new Guid("0000015f-1111-1111-1111-000000000000"), "ERZURUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Narman", "Narman Malmüdürlüğü" },
                    { new Guid("00000160-1111-1111-1111-000000000000"), "ERZURUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Oltu", "Oltu Malmüdürlüğü" },
                    { new Guid("00000161-1111-1111-1111-000000000000"), "ERZURUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Olur", "Olur Malmüdürlüğü" },
                    { new Guid("00000162-1111-1111-1111-000000000000"), "ERZURUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Pasinler", "Pasinler Malmüdürlüğü" },
                    { new Guid("00000163-1111-1111-1111-000000000000"), "ERZURUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Şenkaya", "Şenkaya Malmüdürlüğü" },
                    { new Guid("00000164-1111-1111-1111-000000000000"), "ERZURUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Tekman", "Tekman Malmüdürlüğü" },
                    { new Guid("00000165-1111-1111-1111-000000000000"), "ERZURUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Tortum", "Tortum Malmüdürlüğü" },
                    { new Guid("00000166-1111-1111-1111-000000000000"), "ERZURUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Karaçoban", "Karaçoban Malmüdürlüğü" },
                    { new Guid("00000167-1111-1111-1111-000000000000"), "ERZURUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Uzundere", "Uzundere Malmüdürlüğü" },
                    { new Guid("00000168-1111-1111-1111-000000000000"), "ERZURUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Pazaryolu", "Pazaryolu Malmüdürlüğü" },
                    { new Guid("00000169-1111-1111-1111-000000000000"), "ERZURUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Aziziye Malmüdürlüğü  (Ilıca)" },
                    { new Guid("0000016a-1111-1111-1111-000000000000"), "ERZURUM", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Köprüköy", "Köprüköy Malmüdürlüğü" },
                    { new Guid("0000016b-1111-1111-1111-000000000000"), "ESKİŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez Başkanlık", "Eskişehir Vergi Dairesi Başkanlığı (*)" },
                    { new Guid("0000016c-1111-1111-1111-000000000000"), "ESKİŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Mahmudiye", "Mahmudiye Malmüdürlüğü" },
                    { new Guid("0000016d-1111-1111-1111-000000000000"), "ESKİŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Mihalıççık", "Mihalıççık Malmüdürlüğü" },
                    { new Guid("0000016e-1111-1111-1111-000000000000"), "ESKİŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sarıcakaya", "Sarıcakaya Malmüdürlüğü" },
                    { new Guid("0000016f-1111-1111-1111-000000000000"), "ESKİŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Seyitgazi", "Seyitgazi Malmüdürlüğü" },
                    { new Guid("00000170-1111-1111-1111-000000000000"), "ESKİŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Alpu", "Alpu Malmüdürlüğü" },
                    { new Guid("00000171-1111-1111-1111-000000000000"), "ESKİŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Beylikova", "Beylikova Malmüdürlüğü" },
                    { new Guid("00000172-1111-1111-1111-000000000000"), "ESKİŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İnönü", "İnönü Malmüdürlüğü" },
                    { new Guid("00000173-1111-1111-1111-000000000000"), "ESKİŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Günyüzü", "Günyüzü Malmüdürlüğü" },
                    { new Guid("00000174-1111-1111-1111-000000000000"), "ESKİŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Han", "Han Malmüdürlüğü" },
                    { new Guid("00000175-1111-1111-1111-000000000000"), "ESKİŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Mihalgazi", "Mihalgazi Malmüdürlüğü" },
                    { new Guid("00000176-1111-1111-1111-000000000000"), "GAZİANTEP", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Gaziantep İhtisas Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000177-1111-1111-1111-000000000000"), "GAZİANTEP", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Suburcu Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000178-1111-1111-1111-000000000000"), "GAZİANTEP", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Şehitkâmil Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000179-1111-1111-1111-000000000000"), "GAZİANTEP", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Şahinbey Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000017a-1111-1111-1111-000000000000"), "GAZİANTEP", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Gazikent Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000017b-1111-1111-1111-000000000000"), "GAZİANTEP", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kozanlı Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000017c-1111-1111-1111-000000000000"), "GAZİANTEP", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Nizip", "Nizip Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000017d-1111-1111-1111-000000000000"), "GAZİANTEP", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İslahiye", "İslahiye Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000017e-1111-1111-1111-000000000000"), "GAZİANTEP", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Araban", "Araban Malmüdürlüğü" },
                    { new Guid("0000017f-1111-1111-1111-000000000000"), "GAZİANTEP", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Oğuzeli Malmüdürlüğü" },
                    { new Guid("00000180-1111-1111-1111-000000000000"), "GAZİANTEP", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yavuzeli", "Yavuzeli Malmüdürlüğü" },
                    { new Guid("00000181-1111-1111-1111-000000000000"), "GAZİANTEP", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Karkamış", "Karkamış Malmüdürlüğü" },
                    { new Guid("00000182-1111-1111-1111-000000000000"), "GAZİANTEP", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Nurdağı", "Nurdağı Malmüdürlüğü" },
                    { new Guid("00000183-1111-1111-1111-000000000000"), "GİRESUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Giresun Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000184-1111-1111-1111-000000000000"), "GİRESUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bulancak", "Bulancak Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000185-1111-1111-1111-000000000000"), "GİRESUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Alucra", "Alucra Malmüdürlüğü" },
                    { new Guid("00000186-1111-1111-1111-000000000000"), "GİRESUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Dereli", "Dereli Malmüdürlüğü" },
                    { new Guid("00000187-1111-1111-1111-000000000000"), "GİRESUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Espiye", "Espiye Malmüdürlüğü" },
                    { new Guid("00000188-1111-1111-1111-000000000000"), "GİRESUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Eynesil", "Eynesil Malmüdürlüğü" },
                    { new Guid("00000189-1111-1111-1111-000000000000"), "GİRESUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Görele", "Görele Malmüdürlüğü" },
                    { new Guid("0000018a-1111-1111-1111-000000000000"), "GİRESUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Keşap", "Keşap Malmüdürlüğü" },
                    { new Guid("0000018b-1111-1111-1111-000000000000"), "GİRESUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Şebinkarahisar", "Şebinkarahisar Malmüdürlüğü" },
                    { new Guid("0000018c-1111-1111-1111-000000000000"), "GİRESUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Tirebolu", "Tirebolu Malmüdürlüğü" },
                    { new Guid("0000018d-1111-1111-1111-000000000000"), "GİRESUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Piraziz", "Piraziz Malmüdürlüğü" },
                    { new Guid("0000018e-1111-1111-1111-000000000000"), "GİRESUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yağlıdere", "Yağlıdere Malmüdürlüğü" },
                    { new Guid("0000018f-1111-1111-1111-000000000000"), "GİRESUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çanakçı", "Çanakçı Malmüdürlüğü" },
                    { new Guid("00000190-1111-1111-1111-000000000000"), "GİRESUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Güce", "Güce Malmüdürlüğü" },
                    { new Guid("00000191-1111-1111-1111-000000000000"), "GİRESUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çamoluk", "Çamoluk Malmüdürlüğü" },
                    { new Guid("00000192-1111-1111-1111-000000000000"), "GİRESUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Doğankent", "Doğankent Malmüdürlüğü" },
                    { new Guid("00000193-1111-1111-1111-000000000000"), "GÜMÜŞHANE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Gümüşhane Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000194-1111-1111-1111-000000000000"), "GÜMÜŞHANE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kelkit", "Kelkit Malmüdürlüğü" },
                    { new Guid("00000195-1111-1111-1111-000000000000"), "GÜMÜŞHANE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Şiran", "Şiran Malmüdürlüğü" },
                    { new Guid("00000196-1111-1111-1111-000000000000"), "GÜMÜŞHANE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Torul", "Torul Malmüdürlüğü" },
                    { new Guid("00000197-1111-1111-1111-000000000000"), "GÜMÜŞHANE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Köse", "Köse Malmüdürlüğü" },
                    { new Guid("00000198-1111-1111-1111-000000000000"), "GÜMÜŞHANE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kürtün", "Kürtün Malmüdürlüğü" },
                    { new Guid("00000199-1111-1111-1111-000000000000"), "HAKKARİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Hakkari Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000019a-1111-1111-1111-000000000000"), "HAKKARİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yüksekova", "Yüksekova  Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000019b-1111-1111-1111-000000000000"), "HAKKARİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çukurca", "Çukurca Malmüdürlüğü" },
                    { new Guid("0000019c-1111-1111-1111-000000000000"), "HAKKARİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Şemdinli", "Şemdinli Malmüdürlüğü" },
                    { new Guid("0000019d-1111-1111-1111-000000000000"), "HAKKARİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Derecik", "Derecik Malmüdürlüğü" },
                    { new Guid("0000019e-1111-1111-1111-000000000000"), "HATAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "23 Temmuz Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000019f-1111-1111-1111-000000000000"), "HATAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Antakya Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001a0-1111-1111-1111-000000000000"), "HATAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Şükrükanatlı Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001a1-1111-1111-1111-000000000000"), "HATAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İskenderun", "Sahil Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001a2-1111-1111-1111-000000000000"), "HATAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İskenderun", "Akdeniz Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001a3-1111-1111-1111-000000000000"), "HATAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İskenderun", "Asım Gündüz Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001a4-1111-1111-1111-000000000000"), "HATAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Dörtyol", "Dörtyol  Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001a5-1111-1111-1111-000000000000"), "HATAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kırıkhan", "Kırıkhan Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001a6-1111-1111-1111-000000000000"), "HATAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Reyhanlı", "Reyhanlı Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001a7-1111-1111-1111-000000000000"), "HATAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Samandağ", "Samandağ Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001a8-1111-1111-1111-000000000000"), "HATAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Altınözü", "Altınözü Malmüdürlüğü" },
                    { new Guid("000001a9-1111-1111-1111-000000000000"), "HATAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hassa", "Hassa Malmüdürlüğü" },
                    { new Guid("000001aa-1111-1111-1111-000000000000"), "HATAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yayladağı", "Yayladağı Malmüdürlüğü" },
                    { new Guid("000001ab-1111-1111-1111-000000000000"), "HATAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Erzin", "Erzin Malmüdürlüğü" },
                    { new Guid("000001ac-1111-1111-1111-000000000000"), "HATAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Belen", "Belen Malmüdürlüğü" },
                    { new Guid("000001ad-1111-1111-1111-000000000000"), "HATAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kumlu", "Kumlu Malmüdürlüğü" },
                    { new Guid("000001ae-1111-1111-1111-000000000000"), "ISPARTA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Davraz Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001af-1111-1111-1111-000000000000"), "ISPARTA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kaymakkapı Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001b0-1111-1111-1111-000000000000"), "ISPARTA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Eğirdir", "Eğirdir Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001b1-1111-1111-1111-000000000000"), "ISPARTA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yalvaç", "Yalvaç Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001b2-1111-1111-1111-000000000000"), "ISPARTA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Atabey", "Atabey Malmüdürlüğü" },
                    { new Guid("000001b3-1111-1111-1111-000000000000"), "ISPARTA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gelendost", "Gelendost Malmüdürlüğü" },
                    { new Guid("000001b4-1111-1111-1111-000000000000"), "ISPARTA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Keçiborlu", "Keçiborlu Malmüdürlüğü" },
                    { new Guid("000001b5-1111-1111-1111-000000000000"), "ISPARTA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Senirkent", "Senirkent Malmüdürlüğü" },
                    { new Guid("000001b6-1111-1111-1111-000000000000"), "ISPARTA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sütçüler", "Sütçüler Malmüdürlüğü" },
                    { new Guid("000001b7-1111-1111-1111-000000000000"), "ISPARTA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Şarkikaraağaç", "Şarkikaraağaç Malmüdürlüğü" },
                    { new Guid("000001b8-1111-1111-1111-000000000000"), "ISPARTA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Uluborlu", "Uluborlu Malmüdürlüğü" },
                    { new Guid("000001b9-1111-1111-1111-000000000000"), "ISPARTA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Aksu", "Aksu Malmüdürlüğü" },
                    { new Guid("000001ba-1111-1111-1111-000000000000"), "ISPARTA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gönen", "Gönen Malmüdürlüğü" },
                    { new Guid("000001bb-1111-1111-1111-000000000000"), "ISPARTA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yenişarbademli", "Yenişarbademli Malmüdürlüğü" },
                    { new Guid("000001bc-1111-1111-1111-000000000000"), "MERSİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "İstiklâl Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001bd-1111-1111-1111-000000000000"), "MERSİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Uray Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001be-1111-1111-1111-000000000000"), "MERSİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Liman Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001bf-1111-1111-1111-000000000000"), "MERSİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Toros Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001c0-1111-1111-1111-000000000000"), "MERSİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Mersin İhtisas Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001c1-1111-1111-1111-000000000000"), "MERSİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Erdemli", "Erdemli Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001c2-1111-1111-1111-000000000000"), "MERSİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Silifke", "Silifke Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001c3-1111-1111-1111-000000000000"), "MERSİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Anamur", "Anamur Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001c4-1111-1111-1111-000000000000"), "MERSİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Tarsus", "Kızılmurat Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001c5-1111-1111-1111-000000000000"), "MERSİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Tarsus", "Şehitkerim Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001c6-1111-1111-1111-000000000000"), "MERSİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gülnar", "Gülnar Malmüdürlüğü" },
                    { new Guid("000001c7-1111-1111-1111-000000000000"), "MERSİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Mut", "Mut Malmüdürlüğü" },
                    { new Guid("000001c8-1111-1111-1111-000000000000"), "MERSİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Aydıncık", "Aydıncık Malmüdürlüğü" },
                    { new Guid("000001c9-1111-1111-1111-000000000000"), "MERSİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bozyazı", "Bozyazı Malmüdürlüğü" },
                    { new Guid("000001ca-1111-1111-1111-000000000000"), "MERSİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çamlıyayla", "Çamlıyayla Malmüdürlüğü" },
                    { new Guid("000001cb-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Haliç İhtisas Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001cc-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Vatan İhtisas Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001cd-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Çamlıca İhtisas Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001ce-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Alemdağ Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001cf-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Yenikapı Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001d0-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez Başkanlık", "Büyük Mükellefler Vergi Dairesi Başkanlığı (*)" },
                    { new Guid("000001d1-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Boğaziçi Kurumlar Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001d2-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Marmara Kurumlar Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001d3-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Davutpaşa Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001d4-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Esenler Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001d5-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Fatih Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001d6-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Küçükköy Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001d7-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Merter Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001d8-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Sultanbeyli Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001d9-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Anadolu Kurumlar Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001da-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Tuzla Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001db-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kozyatağı Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001dc-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Maslak Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001dd-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Zincirlikuyu Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001de-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "İkitelli Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001df-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Beşiktaş Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001e0-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Ümraniye Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001e1-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Yeditepe Veraset ve Harçlar Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001e2-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kasımpaşa Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001e3-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Erenköy Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001e4-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Hisar Veraset ve Harçlar Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001e5-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Tuna Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001e6-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Rıhtım Veraset ve Harçlar Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001e7-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Güngören Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001e8-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kocasinan Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001e9-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Güneşli Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001ea-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Küçükyalı Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001eb-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Pendik Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001ec-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Bayrampaşa Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001ed-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Beyazıt Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001ee-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Beyoğlu Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001ef-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Gaziosmanpaşa Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001f0-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Göztepe Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001f1-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Hocapaşa Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001f2-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kadıköy Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001f3-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kocamustafapaşa Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001f4-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Mecidiyeköy Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001f5-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Şişli Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001f6-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Üsküdar Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001f7-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Halkalı Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001f8-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kağıthane Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001f9-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Zeytinburnu Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001fa-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Beykoz Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001fb-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Sarıyer Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001fc-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Bakırköy Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001fd-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kartal Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001fe-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Nakil Vasıtaları Vergi Dairesi Müdürlüğü" },
                    { new Guid("000001ff-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Sarıgazi Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000200-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Atışalanı Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000201-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Yakacık Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000202-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Yenibosna Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000203-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Avcılar Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000204-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Adalar Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000205-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Küçükçekmece Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000206-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Beylikdüzü Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000207-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Esenyurt Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000208-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Silivri Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000209-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Büyükçekmece Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000020a-1111-1111-1111-000000000000"), "İSTANBUL", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Şile Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000020b-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "İzmir İhtisas Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000020c-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "9 Eylül Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000020d-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Yamanlar Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000020e-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Belkahve Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000020f-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Karşıyaka Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000210-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kemeraltı Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000211-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Konak Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000212-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kordon Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000213-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Şirinyer Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000214-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kadifekale Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000215-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Taşıtlar Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000216-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Hasan Tahsin Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000217-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Bornova Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000218-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Balçova Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000219-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Gaziemir Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000021a-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Ege Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000021b-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Çiğli Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000021c-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Çakabey İhtisas Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000021d-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bayındır", "Bayındır Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000021e-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bergama", "Bergama Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000021f-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Menemen Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000220-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ödemiş", "Ödemiş Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000221-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Tire", "Tire Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000222-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Torbalı Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000223-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kemalpaşa Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000224-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Urla Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000225-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Selçuk", "Selçuk Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000226-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kınık", "Kınık Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000227-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kiraz", "Kiraz Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000228-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çeşme", "Çeşme Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000229-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Aliağa Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000022a-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Menderes Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000022b-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Dikili", "Dikili Malmüdürlüğü" },
                    { new Guid("0000022c-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Foça Malmüdürlüğü" },
                    { new Guid("0000022d-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Karaburun", "Karaburun Malmüdürlüğü" },
                    { new Guid("0000022e-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Seferihisar Malmüdürlüğü" },
                    { new Guid("0000022f-1111-1111-1111-000000000000"), "İZMİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Beydağ", "Beydağ Malmüdürlüğü" },
                    { new Guid("00000230-1111-1111-1111-000000000000"), "KARS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kars Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000231-1111-1111-1111-000000000000"), "KARS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Arpaçay", "Arpaçay Malmüdürlüğü" },
                    { new Guid("00000232-1111-1111-1111-000000000000"), "KARS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Digor", "Digor Malmüdürlüğü" },
                    { new Guid("00000233-1111-1111-1111-000000000000"), "KARS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kağızman", "Kağızman Malmüdürlüğü" },
                    { new Guid("00000234-1111-1111-1111-000000000000"), "KARS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sarıkamış", "Sarıkamış Malmüdürlüğü" },
                    { new Guid("00000235-1111-1111-1111-000000000000"), "KARS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Selim", "Selim Malmüdürlüğü" },
                    { new Guid("00000236-1111-1111-1111-000000000000"), "KARS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Susuz", "Susuz Malmüdürlüğü" },
                    { new Guid("00000237-1111-1111-1111-000000000000"), "KARS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Akyaka", "Akyaka Malmüdürlüğü" },
                    { new Guid("00000238-1111-1111-1111-000000000000"), "KASTAMONU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kastamonu Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000239-1111-1111-1111-000000000000"), "KASTAMONU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Tosya", "Tosya Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000023a-1111-1111-1111-000000000000"), "KASTAMONU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Taşköprü", "Taşköprü Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000023b-1111-1111-1111-000000000000"), "KASTAMONU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Araç", "Araç Malmüdürlüğü" },
                    { new Guid("0000023c-1111-1111-1111-000000000000"), "KASTAMONU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Azdavay", "Azdavay Malmüdürlüğü" },
                    { new Guid("0000023d-1111-1111-1111-000000000000"), "KASTAMONU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bozkurt", "Bozkurt Malmüdürlüğü" },
                    { new Guid("0000023e-1111-1111-1111-000000000000"), "KASTAMONU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Cide", "Cide Malmüdürlüğü" },
                    { new Guid("0000023f-1111-1111-1111-000000000000"), "KASTAMONU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çatalzeytin", "Çatalzeytin Malmüdürlüğü" },
                    { new Guid("00000240-1111-1111-1111-000000000000"), "KASTAMONU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Daday", "Daday Malmüdürlüğü" },
                    { new Guid("00000241-1111-1111-1111-000000000000"), "KASTAMONU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Devrekani", "Devrekani Malmüdürlüğü" },
                    { new Guid("00000242-1111-1111-1111-000000000000"), "KASTAMONU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İnebolu", "İnebolu Malmüdürlüğü" },
                    { new Guid("00000243-1111-1111-1111-000000000000"), "KASTAMONU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Küre", "Küre Malmüdürlüğü" },
                    { new Guid("00000244-1111-1111-1111-000000000000"), "KASTAMONU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Abana", "Abana Malmüdürlüğü" },
                    { new Guid("00000245-1111-1111-1111-000000000000"), "KASTAMONU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İhsangazi", "İhsangazi Malmüdürlüğü" },
                    { new Guid("00000246-1111-1111-1111-000000000000"), "KASTAMONU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Pınarbaşı", "Pınarbaşı Malmüdürlüğü" },
                    { new Guid("00000247-1111-1111-1111-000000000000"), "KASTAMONU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Şenpazar", "Şenpazar Malmüdürlüğü" },
                    { new Guid("00000248-1111-1111-1111-000000000000"), "KASTAMONU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ağlı", "Ağlı Malmüdürlüğü" },
                    { new Guid("00000249-1111-1111-1111-000000000000"), "KASTAMONU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Doğanyurt", "Doğanyurt Malmüdürlüğü" },
                    { new Guid("0000024a-1111-1111-1111-000000000000"), "KASTAMONU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hanönü", "Hanönü Malmüdürlüğü" },
                    { new Guid("0000024b-1111-1111-1111-000000000000"), "KASTAMONU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Seydiler", "Seydiler Malmüdürlüğü" },
                    { new Guid("0000024c-1111-1111-1111-000000000000"), "KAYSERİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kayseri İhtisas Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000024d-1111-1111-1111-000000000000"), "KAYSERİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Mimar Sinan Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000024e-1111-1111-1111-000000000000"), "KAYSERİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Erciyes Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000024f-1111-1111-1111-000000000000"), "KAYSERİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kaleönü Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000250-1111-1111-1111-000000000000"), "KAYSERİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Gevher Nesibe Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000251-1111-1111-1111-000000000000"), "KAYSERİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Develi", "Develi Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000252-1111-1111-1111-000000000000"), "KAYSERİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Pınarbaşı", "Pınarbaşı Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000253-1111-1111-1111-000000000000"), "KAYSERİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bünyan", "Bünyan Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000254-1111-1111-1111-000000000000"), "KAYSERİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Felahiye", "Felahiye Malmüdürlüğü" },
                    { new Guid("00000255-1111-1111-1111-000000000000"), "KAYSERİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İncesu", "İncesu Malmüdürlüğü" },
                    { new Guid("00000256-1111-1111-1111-000000000000"), "KAYSERİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sarıoğlan", "Sarıoğlan Malmüdürlüğü" },
                    { new Guid("00000257-1111-1111-1111-000000000000"), "KAYSERİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sarız", "Sarız Malmüdürlüğü" },
                    { new Guid("00000258-1111-1111-1111-000000000000"), "KAYSERİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Tomarza", "Tomarza Malmüdürlüğü" },
                    { new Guid("00000259-1111-1111-1111-000000000000"), "KAYSERİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yahyalı", "Yahyalı Malmüdürlüğü" },
                    { new Guid("0000025a-1111-1111-1111-000000000000"), "KAYSERİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yeşilhisar", "Yeşilhisar Malmüdürlüğü" },
                    { new Guid("0000025b-1111-1111-1111-000000000000"), "KAYSERİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Akkışla", "Akkışla Malmüdürlüğü" },
                    { new Guid("0000025c-1111-1111-1111-000000000000"), "KAYSERİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Hacılar Malmüdürlüğü" },
                    { new Guid("0000025d-1111-1111-1111-000000000000"), "KAYSERİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Özvatan", "Özvatan Malmüdürlüğü" },
                    { new Guid("0000025e-1111-1111-1111-000000000000"), "KIRKLARELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kırklareli Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000025f-1111-1111-1111-000000000000"), "KIRKLARELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Lüleburgaz", "Lüleburgaz Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000260-1111-1111-1111-000000000000"), "KIRKLARELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Babaeski", "Babaeski Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000261-1111-1111-1111-000000000000"), "KIRKLARELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Demirköy", "Demirköy Malmüdürlüğü" },
                    { new Guid("00000262-1111-1111-1111-000000000000"), "KIRKLARELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kofçaz", "Kofçaz Malmüdürlüğü" },
                    { new Guid("00000263-1111-1111-1111-000000000000"), "KIRKLARELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Pehlivanköy", "Pehlivanköy Malmüdürlüğü" },
                    { new Guid("00000264-1111-1111-1111-000000000000"), "KIRKLARELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Pınarhisar", "Pınarhisar Malmüdürlüğü" },
                    { new Guid("00000265-1111-1111-1111-000000000000"), "KIRKLARELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Vize", "Vize Malmüdürlüğü" },
                    { new Guid("00000266-1111-1111-1111-000000000000"), "KIRŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kırşehir Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000267-1111-1111-1111-000000000000"), "KIRŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kaman", "Kaman Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000268-1111-1111-1111-000000000000"), "KIRŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çiçekdağı", "Çiçekdağı Malmüdürlüğü" },
                    { new Guid("00000269-1111-1111-1111-000000000000"), "KIRŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Mucur", "Mucur Malmüdürlüğü" },
                    { new Guid("0000026a-1111-1111-1111-000000000000"), "KIRŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Akpınar", "Akpınar Malmüdürlüğü" },
                    { new Guid("0000026b-1111-1111-1111-000000000000"), "KIRŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Akçakent", "Akçakent Malmüdürlüğü" },
                    { new Guid("0000026c-1111-1111-1111-000000000000"), "KIRŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Boztepe", "Boztepe Malmüdürlüğü" },
                    { new Guid("0000026d-1111-1111-1111-000000000000"), "KOCAELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kocaeli İhtisas Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000026e-1111-1111-1111-000000000000"), "KOCAELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Tepecik Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000026f-1111-1111-1111-000000000000"), "KOCAELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Alemdar Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000270-1111-1111-1111-000000000000"), "KOCAELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez Gebze", "Gebze İhtisas Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000271-1111-1111-1111-000000000000"), "KOCAELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Acısu Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000272-1111-1111-1111-000000000000"), "KOCAELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Uluçınar Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000273-1111-1111-1111-000000000000"), "KOCAELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "İlyasbey Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000274-1111-1111-1111-000000000000"), "KOCAELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Gölcük Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000275-1111-1111-1111-000000000000"), "KOCAELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Karamürsel Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000276-1111-1111-1111-000000000000"), "KOCAELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Körfez Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000277-1111-1111-1111-000000000000"), "KOCAELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Derince Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000278-1111-1111-1111-000000000000"), "KOCAELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kandıra Malmüdürlüğü" },
                    { new Guid("00000279-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Konya İhtisas Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000027a-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Selçuk Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000027b-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Mevlana Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000027c-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Meram Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000027d-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Alaaddin Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000027e-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Akşehir", "Akşehir Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000027f-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ereğli", "Ereğli Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000280-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Beyşehir", "Beyşehir Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000281-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Cihanbeyli", "Cihanbeyli Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000282-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çumra", "Çumra Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000283-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Seydişehir", "Seydişehir Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000284-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ilgın", "Ilgın Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000285-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kulu", "Kulu Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000286-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Karapınar", "Karapınar Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000287-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bozkır", "Bozkır Malmüdürlüğü" },
                    { new Guid("00000288-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Doğanhisar", "Doğanhisar Malmüdürlüğü" },
                    { new Guid("00000289-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hadim", "Hadim Malmüdürlüğü" },
                    { new Guid("0000028a-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kadınhanı", "Kadınhanı Malmüdürlüğü" },
                    { new Guid("0000028b-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sarayönü", "Sarayönü Malmüdürlüğü" },
                    { new Guid("0000028c-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yunak", "Yunak Malmüdürlüğü" },
                    { new Guid("0000028d-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Akören", "Akören Malmüdürlüğü" },
                    { new Guid("0000028e-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Altınekin", "Altınekin Malmüdürlüğü" },
                    { new Guid("0000028f-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Derebucak", "Derebucak Malmüdürlüğü" },
                    { new Guid("00000290-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hüyük", "Hüyük Malmüdürlüğü" },
                    { new Guid("00000291-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Taşkent", "Taşkent Malmüdürlüğü" },
                    { new Guid("00000292-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ahırlı", "Ahırlı Malmüdürlüğü" },
                    { new Guid("00000293-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çeltik", "Çeltik Malmüdürlüğü" },
                    { new Guid("00000294-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Derbent", "Derbent Malmüdürlüğü" },
                    { new Guid("00000295-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Emirgazi", "Emirgazi Malmüdürlüğü" },
                    { new Guid("00000296-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Güneysınır", "Güneysınır Malmüdürlüğü" },
                    { new Guid("00000297-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Halkapınar", "Halkapınar Malmüdürlüğü" },
                    { new Guid("00000298-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Tuzlukçu", "Tuzlukçu Malmüdürlüğü" },
                    { new Guid("00000299-1111-1111-1111-000000000000"), "KONYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yalıhüyük", "Yalıhüyük Malmüdürlüğü" },
                    { new Guid("0000029a-1111-1111-1111-000000000000"), "KÜTAHYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "30 Ağustos Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000029b-1111-1111-1111-000000000000"), "KÜTAHYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Çinili Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000029c-1111-1111-1111-000000000000"), "KÜTAHYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gediz", "Gediz Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000029d-1111-1111-1111-000000000000"), "KÜTAHYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Simav", "Simav Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000029e-1111-1111-1111-000000000000"), "KÜTAHYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Tavşanlı", "Tavşanlı Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000029f-1111-1111-1111-000000000000"), "KÜTAHYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Emet", "Emet Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002a0-1111-1111-1111-000000000000"), "KÜTAHYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Altıntaş", "Altıntaş Malmüdürlüğü" },
                    { new Guid("000002a1-1111-1111-1111-000000000000"), "KÜTAHYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Domaniç", "Domaniç Malmüdürlüğü" },
                    { new Guid("000002a2-1111-1111-1111-000000000000"), "KÜTAHYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Aslanapa", "Aslanapa Malmüdürlüğü" },
                    { new Guid("000002a3-1111-1111-1111-000000000000"), "KÜTAHYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Dumlupınar", "Dumlupınar Malmüdürlüğü" },
                    { new Guid("000002a4-1111-1111-1111-000000000000"), "KÜTAHYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hisarcık", "Hisarcık Malmüdürlüğü" },
                    { new Guid("000002a5-1111-1111-1111-000000000000"), "KÜTAHYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Şaphane", "Şaphane Malmüdürlüğü" },
                    { new Guid("000002a6-1111-1111-1111-000000000000"), "KÜTAHYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çavdarhisar", "Çavdarhisar Malmüdürlüğü" },
                    { new Guid("000002a7-1111-1111-1111-000000000000"), "KÜTAHYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Pazarlar", "Pazarlar Malmüdürlüğü" },
                    { new Guid("000002a8-1111-1111-1111-000000000000"), "MALATYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Fırat Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002a9-1111-1111-1111-000000000000"), "MALATYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Beydağı Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002aa-1111-1111-1111-000000000000"), "MALATYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Akçadağ", "Akçadağ Malmüdürlüğü" },
                    { new Guid("000002ab-1111-1111-1111-000000000000"), "MALATYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Arapgir", "Arapgir Malmüdürlüğü" },
                    { new Guid("000002ac-1111-1111-1111-000000000000"), "MALATYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Arguvan", "Arguvan Malmüdürlüğü" },
                    { new Guid("000002ad-1111-1111-1111-000000000000"), "MALATYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Darende", "Darende Malmüdürlüğü" },
                    { new Guid("000002ae-1111-1111-1111-000000000000"), "MALATYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Doğanşehir", "Doğanşehir Malmüdürlüğü" },
                    { new Guid("000002af-1111-1111-1111-000000000000"), "MALATYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hekimhan", "Hekimhan Malmüdürlüğü" },
                    { new Guid("000002b0-1111-1111-1111-000000000000"), "MALATYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Pütürge", "Pütürge Malmüdürlüğü" },
                    { new Guid("000002b1-1111-1111-1111-000000000000"), "MALATYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yeşilyurt", "Yeşilyurt Malmüdürlüğü" },
                    { new Guid("000002b2-1111-1111-1111-000000000000"), "MALATYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Battalgazi", "Battalgazi Malmüdürlüğü" },
                    { new Guid("000002b3-1111-1111-1111-000000000000"), "MALATYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Doğanyol", "Doğanyol Malmüdürlüğü" },
                    { new Guid("000002b4-1111-1111-1111-000000000000"), "MALATYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kale", "Kale Malmüdürlüğü" },
                    { new Guid("000002b5-1111-1111-1111-000000000000"), "MALATYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kuluncak", "Kuluncak Malmüdürlüğü" },
                    { new Guid("000002b6-1111-1111-1111-000000000000"), "MALATYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yazıhan", "Yazıhan Malmüdürlüğü" },
                    { new Guid("000002b7-1111-1111-1111-000000000000"), "MANİSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Manisa İhtisas Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002b8-1111-1111-1111-000000000000"), "MANİSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Şehit Cihan Güneş Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002b9-1111-1111-1111-000000000000"), "MANİSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Mesir Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002ba-1111-1111-1111-000000000000"), "MANİSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Akhisar", "Akhisar Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002bb-1111-1111-1111-000000000000"), "MANİSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Alaşehir", "Alaşehir Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002bc-1111-1111-1111-000000000000"), "MANİSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Demirci", "Demirci Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002bd-1111-1111-1111-000000000000"), "MANİSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kırkağaç", "Kırkağaç Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002be-1111-1111-1111-000000000000"), "MANİSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Salihli", "Salihli Adil Oral Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002bf-1111-1111-1111-000000000000"), "MANİSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sarıgöl", "Sarıgöl Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002c0-1111-1111-1111-000000000000"), "MANİSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Saruhanlı", "Saruhanlı Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002c1-1111-1111-1111-000000000000"), "MANİSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Soma", "Soma Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002c2-1111-1111-1111-000000000000"), "MANİSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Turgutlu", "Turgutlu Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002c3-1111-1111-1111-000000000000"), "MANİSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gördes", "Gördes Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002c4-1111-1111-1111-000000000000"), "MANİSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kula", "Kula Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002c5-1111-1111-1111-000000000000"), "MANİSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Selendi", "Selendi Malmüdürlüğü" },
                    { new Guid("000002c6-1111-1111-1111-000000000000"), "MANİSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ahmetli", "Ahmetli Malmüdürlüğü" },
                    { new Guid("000002c7-1111-1111-1111-000000000000"), "MANİSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gölmarmara", "Gölmarmara Malmüdürlüğü" },
                    { new Guid("000002c8-1111-1111-1111-000000000000"), "MANİSA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Köprübaşı", "Köprübaşı Malmüdürlüğü" },
                    { new Guid("000002c9-1111-1111-1111-000000000000"), "KAHRAMANMARAŞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Aslanbey Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002ca-1111-1111-1111-000000000000"), "KAHRAMANMARAŞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Aksu Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002cb-1111-1111-1111-000000000000"), "KAHRAMANMARAŞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Elbistan", "Elbistan Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002cc-1111-1111-1111-000000000000"), "KAHRAMANMARAŞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Afşin", "Afşin Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002cd-1111-1111-1111-000000000000"), "KAHRAMANMARAŞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Pazarcık", "Pazarcık Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002ce-1111-1111-1111-000000000000"), "KAHRAMANMARAŞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Andırın", "Andırın Malmüdürlüğü" },
                    { new Guid("000002cf-1111-1111-1111-000000000000"), "KAHRAMANMARAŞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Göksun", "Göksun Malmüdürlüğü" },
                    { new Guid("000002d0-1111-1111-1111-000000000000"), "KAHRAMANMARAŞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Türkoğlu", "Türkoğlu Malmüdürlüğü" },
                    { new Guid("000002d1-1111-1111-1111-000000000000"), "KAHRAMANMARAŞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çağlayancerit", "Çağlayancerit Malmüdürlüğü" },
                    { new Guid("000002d2-1111-1111-1111-000000000000"), "KAHRAMANMARAŞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ekinözü", "Ekinözü Malmüdürlüğü" },
                    { new Guid("000002d3-1111-1111-1111-000000000000"), "KAHRAMANMARAŞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Nurhak", "Nurhak Malmüdürlüğü" },
                    { new Guid("000002d4-1111-1111-1111-000000000000"), "MARDİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Mardin Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002d5-1111-1111-1111-000000000000"), "MARDİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kızıltepe", "Kızıltepe Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002d6-1111-1111-1111-000000000000"), "MARDİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Nusaybin", "Nusaybin Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002d7-1111-1111-1111-000000000000"), "MARDİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Derik", "Derik Malmüdürlüğü" },
                    { new Guid("000002d8-1111-1111-1111-000000000000"), "MARDİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Mazıdağı", "Mazıdağı Malmüdürlüğü" },
                    { new Guid("000002d9-1111-1111-1111-000000000000"), "MARDİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Midyat", "Midyat Malmüdürlüğü" },
                    { new Guid("000002da-1111-1111-1111-000000000000"), "MARDİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ömerli", "Ömerli Malmüdürlüğü" },
                    { new Guid("000002db-1111-1111-1111-000000000000"), "MARDİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Savur", "Savur Malmüdürlüğü" },
                    { new Guid("000002dc-1111-1111-1111-000000000000"), "MARDİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Dargeçit", "Dargeçit Malmüdürlüğü" },
                    { new Guid("000002dd-1111-1111-1111-000000000000"), "MARDİN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yeşilli", "Yeşilli Malmüdürlüğü" },
                    { new Guid("000002de-1111-1111-1111-000000000000"), "MUĞLA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Muğla Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002df-1111-1111-1111-000000000000"), "MUĞLA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bodrum", "Bodrum Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002e0-1111-1111-1111-000000000000"), "MUĞLA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Fethiye", "Fethiye Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002e1-1111-1111-1111-000000000000"), "MUĞLA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Köyceğiz", "Köyceğiz Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002e2-1111-1111-1111-000000000000"), "MUĞLA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Milas", "Milas Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002e3-1111-1111-1111-000000000000"), "MUĞLA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Marmaris", "Marmaris Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002e4-1111-1111-1111-000000000000"), "MUĞLA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yatağan", "Yatağan Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002e5-1111-1111-1111-000000000000"), "MUĞLA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Datça", "Datça Malmüdürlüğü" },
                    { new Guid("000002e6-1111-1111-1111-000000000000"), "MUĞLA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ula", "Ula Malmüdürlüğü" },
                    { new Guid("000002e7-1111-1111-1111-000000000000"), "MUĞLA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Dalaman", "Dalaman Malmüdürlüğü" },
                    { new Guid("000002e8-1111-1111-1111-000000000000"), "MUĞLA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ortaca", "Ortaca Malmüdürlüğü" },
                    { new Guid("000002e9-1111-1111-1111-000000000000"), "MUĞLA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kavaklıdere", "Kavaklıdere Malmüdürlüğü" },
                    { new Guid("000002ea-1111-1111-1111-000000000000"), "MUĞLA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Seydiemer", "Seydikemer Malmüdürlüğü" },
                    { new Guid("000002eb-1111-1111-1111-000000000000"), "MUŞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Muş Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002ec-1111-1111-1111-000000000000"), "MUŞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bulanık", "Bulanık Malmüdürlüğü" },
                    { new Guid("000002ed-1111-1111-1111-000000000000"), "MUŞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Malazgirt", "Malazgirt Malmüdürlüğü" },
                    { new Guid("000002ee-1111-1111-1111-000000000000"), "MUŞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Varto", "Varto Malmüdürlüğü" },
                    { new Guid("000002ef-1111-1111-1111-000000000000"), "MUŞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hasköy", "Hasköy Malmüdürlüğü" },
                    { new Guid("000002f0-1111-1111-1111-000000000000"), "MUŞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Korkut", "Korkut Malmüdürlüğü" },
                    { new Guid("000002f1-1111-1111-1111-000000000000"), "NEVŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Nevşehir Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002f2-1111-1111-1111-000000000000"), "NEVŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Avanos", "Avanos Malmüdürlüğü" },
                    { new Guid("000002f3-1111-1111-1111-000000000000"), "NEVŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Derinkuyu", "Derinkuyu Malmüdürlüğü" },
                    { new Guid("000002f4-1111-1111-1111-000000000000"), "NEVŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gülşehir", "Gülşehir Malmüdürlüğü" },
                    { new Guid("000002f5-1111-1111-1111-000000000000"), "NEVŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hacıbektaş", "Hacıbektaş Malmüdürlüğü" },
                    { new Guid("000002f6-1111-1111-1111-000000000000"), "NEVŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kozaklı", "Kozaklı Malmüdürlüğü" },
                    { new Guid("000002f7-1111-1111-1111-000000000000"), "NEVŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ürgüp", "Ürgüp Malmüdürlüğü" },
                    { new Guid("000002f8-1111-1111-1111-000000000000"), "NEVŞEHİR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Acıgöl", "Acıgöl Malmüdürlüğü" },
                    { new Guid("000002f9-1111-1111-1111-000000000000"), "NİĞDE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Niğde Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002fa-1111-1111-1111-000000000000"), "NİĞDE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bor", "Bor Vergi Dairesi Müdürlüğü" },
                    { new Guid("000002fb-1111-1111-1111-000000000000"), "NİĞDE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çamardı", "Çamardı Malmüdürlüğü" },
                    { new Guid("000002fc-1111-1111-1111-000000000000"), "NİĞDE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ulukışla", "Ulukışla Malmüdürlüğü" },
                    { new Guid("000002fd-1111-1111-1111-000000000000"), "NİĞDE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Altunhisar", "Altunhisar Malmüdürlüğü" },
                    { new Guid("000002fe-1111-1111-1111-000000000000"), "NİĞDE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çiftlik", "Çiftlik Malmüdürlüğü" },
                    { new Guid("000002ff-1111-1111-1111-000000000000"), "ORDU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Boztepe Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000300-1111-1111-1111-000000000000"), "ORDU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Köprübaşı Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000301-1111-1111-1111-000000000000"), "ORDU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Fatsa", "Fatsa Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000302-1111-1111-1111-000000000000"), "ORDU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ünye", "Ünye Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000303-1111-1111-1111-000000000000"), "ORDU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Akkuş", "Akkuş Malmüdürlüğü" },
                    { new Guid("00000304-1111-1111-1111-000000000000"), "ORDU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Aybastı", "Aybastı Malmüdürlüğü" },
                    { new Guid("00000305-1111-1111-1111-000000000000"), "ORDU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gölköy", "Gölköy Malmüdürlüğü" },
                    { new Guid("00000306-1111-1111-1111-000000000000"), "ORDU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Korgan", "Korgan Malmüdürlüğü" },
                    { new Guid("00000307-1111-1111-1111-000000000000"), "ORDU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kumru", "Kumru Malmüdürlüğü" },
                    { new Guid("00000308-1111-1111-1111-000000000000"), "ORDU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Mesudiye", "Mesudiye Malmüdürlüğü" },
                    { new Guid("00000309-1111-1111-1111-000000000000"), "ORDU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Perşembe", "Perşembe Malmüdürlüğü" },
                    { new Guid("0000030a-1111-1111-1111-000000000000"), "ORDU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ulubey", "Ulubey Malmüdürlüğü" },
                    { new Guid("0000030b-1111-1111-1111-000000000000"), "ORDU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gülyalı", "Gülyalı Malmüdürlüğü" },
                    { new Guid("0000030c-1111-1111-1111-000000000000"), "ORDU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gürgentepe", "Gürgentepe Malmüdürlüğü" },
                    { new Guid("0000030d-1111-1111-1111-000000000000"), "ORDU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çamaş", "Çamaş Malmüdürlüğü" },
                    { new Guid("0000030e-1111-1111-1111-000000000000"), "ORDU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çatalpınar", "Çatalpınar Malmüdürlüğü" },
                    { new Guid("0000030f-1111-1111-1111-000000000000"), "ORDU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çaybaşı", "Çaybaşı Malmüdürlüğü" },
                    { new Guid("00000310-1111-1111-1111-000000000000"), "ORDU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İkizce", "İkizce Malmüdürlüğü" },
                    { new Guid("00000311-1111-1111-1111-000000000000"), "ORDU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kabadüz", "Kabadüz Malmüdürlüğü" },
                    { new Guid("00000312-1111-1111-1111-000000000000"), "ORDU", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kabataş", "Kabataş Malmüdürlüğü" },
                    { new Guid("00000313-1111-1111-1111-000000000000"), "RİZE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kaçkar Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000314-1111-1111-1111-000000000000"), "RİZE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Yeşilçay Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000315-1111-1111-1111-000000000000"), "RİZE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çayeli", "Çayeli Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000316-1111-1111-1111-000000000000"), "RİZE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Pazar", "Pazar Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000317-1111-1111-1111-000000000000"), "RİZE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ardeşen", "Ardeşen Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000318-1111-1111-1111-000000000000"), "RİZE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çamlıhemşin", "Çamlıhemşin Malmüdürlüğü" },
                    { new Guid("00000319-1111-1111-1111-000000000000"), "RİZE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Fındıklı", "Fındıklı Malmüdürlüğü" },
                    { new Guid("0000031a-1111-1111-1111-000000000000"), "RİZE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İkizdere", "İkizdere Malmüdürlüğü" },
                    { new Guid("0000031b-1111-1111-1111-000000000000"), "RİZE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kalkandere", "Kalkandere Malmüdürlüğü" },
                    { new Guid("0000031c-1111-1111-1111-000000000000"), "RİZE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Güneysu", "Güneysu Malmüdürlüğü" },
                    { new Guid("0000031d-1111-1111-1111-000000000000"), "RİZE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Derepazarı", "Derepazarı Malmüdürlüğü" },
                    { new Guid("0000031e-1111-1111-1111-000000000000"), "RİZE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hemşin", "Hemşin Malmüdürlüğü" },
                    { new Guid("0000031f-1111-1111-1111-000000000000"), "RİZE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İyidere", "İyidere Malmüdürlüğü" },
                    { new Guid("00000320-1111-1111-1111-000000000000"), "SAKARYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Sakarya İhtisas Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000321-1111-1111-1111-000000000000"), "SAKARYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Gümrükönü Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000322-1111-1111-1111-000000000000"), "SAKARYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Ali Fuat Cebesoy Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000323-1111-1111-1111-000000000000"), "SAKARYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Sapanca Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000324-1111-1111-1111-000000000000"), "SAKARYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Akyazı Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000325-1111-1111-1111-000000000000"), "SAKARYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Geyve", "Geyve Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000326-1111-1111-1111-000000000000"), "SAKARYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Hendek Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000327-1111-1111-1111-000000000000"), "SAKARYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Karasu", "Karasu Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000328-1111-1111-1111-000000000000"), "SAKARYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kaynarca", "Kaynarca Malmüdürlüğü" },
                    { new Guid("00000329-1111-1111-1111-000000000000"), "SAKARYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kocaali", "Kocaali Malmüdürlüğü" },
                    { new Guid("0000032a-1111-1111-1111-000000000000"), "SAKARYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Pamukova", "Pamukova Malmüdürlüğü" },
                    { new Guid("0000032b-1111-1111-1111-000000000000"), "SAKARYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Taraklı", "Taraklı Malmüdürlüğü" },
                    { new Guid("0000032c-1111-1111-1111-000000000000"), "SAKARYA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Karapürçek Malmüdürlüğü" },
                    { new Guid("0000032d-1111-1111-1111-000000000000"), "SAMSUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "19 Mayıs Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000032e-1111-1111-1111-000000000000"), "SAMSUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Gaziler Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000032f-1111-1111-1111-000000000000"), "SAMSUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Zafer Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000330-1111-1111-1111-000000000000"), "SAMSUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bafra", "Bafra Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000331-1111-1111-1111-000000000000"), "SAMSUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çarşamba", "Çarşamba Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000332-1111-1111-1111-000000000000"), "SAMSUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Terme", "Terme Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000333-1111-1111-1111-000000000000"), "SAMSUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Havza", "Havza Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000334-1111-1111-1111-000000000000"), "SAMSUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Alaçam", "Alaçam Malmüdürlüğü" },
                    { new Guid("00000335-1111-1111-1111-000000000000"), "SAMSUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kavak", "Kavak Malmüdürlüğü" },
                    { new Guid("00000336-1111-1111-1111-000000000000"), "SAMSUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ladik", "Ladik Malmüdürlüğü" },
                    { new Guid("00000337-1111-1111-1111-000000000000"), "SAMSUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Vezirköprü", "Vezirköprü Malmüdürlüğü" },
                    { new Guid("00000338-1111-1111-1111-000000000000"), "SAMSUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Asarcık", "Asarcık Malmüdürlüğü" },
                    { new Guid("00000339-1111-1111-1111-000000000000"), "SAMSUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ondokuz Mayıs", "Ondokuz Mayıs Malmüdürlüğü" },
                    { new Guid("0000033a-1111-1111-1111-000000000000"), "SAMSUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Salıpazarı", "Salıpazarı Malmüdürlüğü" },
                    { new Guid("0000033b-1111-1111-1111-000000000000"), "SAMSUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Tekkeköy Malmüdürlüğü" },
                    { new Guid("0000033c-1111-1111-1111-000000000000"), "SAMSUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ayvacık", "Ayvacık Malmüdürlüğü" },
                    { new Guid("0000033d-1111-1111-1111-000000000000"), "SAMSUN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yakakent", "Yakakent Malmüdürlüğü" },
                    { new Guid("0000033e-1111-1111-1111-000000000000"), "SİİRT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Siirt Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000033f-1111-1111-1111-000000000000"), "SİİRT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Baykan", "Baykan Malmüdürlüğü" },
                    { new Guid("00000340-1111-1111-1111-000000000000"), "SİİRT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Eruh", "Eruh Malmüdürlüğü" },
                    { new Guid("00000341-1111-1111-1111-000000000000"), "SİİRT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kurtalan", "Kurtalan Malmüdürlüğü" },
                    { new Guid("00000342-1111-1111-1111-000000000000"), "SİİRT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Pervari", "Pervari Malmüdürlüğü" },
                    { new Guid("00000343-1111-1111-1111-000000000000"), "SİİRT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Şirvan", "Şirvan Malmüdürlüğü" },
                    { new Guid("00000344-1111-1111-1111-000000000000"), "SİNOP", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Sinop Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000345-1111-1111-1111-000000000000"), "SİNOP", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Boyabat", "Boyabat Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000346-1111-1111-1111-000000000000"), "SİNOP", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ayancık", "Ayancık Malmüdürlüğü" },
                    { new Guid("00000347-1111-1111-1111-000000000000"), "SİNOP", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Durağan", "Durağan Malmüdürlüğü" },
                    { new Guid("00000348-1111-1111-1111-000000000000"), "SİNOP", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Erfelek", "Erfelek Malmüdürlüğü" },
                    { new Guid("00000349-1111-1111-1111-000000000000"), "SİNOP", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gerze", "Gerze Malmüdürlüğü" },
                    { new Guid("0000034a-1111-1111-1111-000000000000"), "SİNOP", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Türkeli", "Türkeli Malmüdürlüğü" },
                    { new Guid("0000034b-1111-1111-1111-000000000000"), "SİNOP", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Dikmen", "Dikmen Malmüdürlüğü" },
                    { new Guid("0000034c-1111-1111-1111-000000000000"), "SİNOP", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Saraydüzü", "Saraydüzü Malmüdürlüğü" },
                    { new Guid("0000034d-1111-1111-1111-000000000000"), "SİVAS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kale Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000034e-1111-1111-1111-000000000000"), "SİVAS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Site Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000034f-1111-1111-1111-000000000000"), "SİVAS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Şarkışla", "Şarkışla Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000350-1111-1111-1111-000000000000"), "SİVAS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Divriği", "Divriği Malmüdürlüğü" },
                    { new Guid("00000351-1111-1111-1111-000000000000"), "SİVAS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gemerek", "Gemerek Malmüdürlüğü" },
                    { new Guid("00000352-1111-1111-1111-000000000000"), "SİVAS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gürün", "Gürün Malmüdürlüğü" },
                    { new Guid("00000353-1111-1111-1111-000000000000"), "SİVAS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hafik", "Hafik Malmüdürlüğü" },
                    { new Guid("00000354-1111-1111-1111-000000000000"), "SİVAS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İmranlı", "İmranlı Malmüdürlüğü" },
                    { new Guid("00000355-1111-1111-1111-000000000000"), "SİVAS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kangal", "Kangal Malmüdürlüğü" },
                    { new Guid("00000356-1111-1111-1111-000000000000"), "SİVAS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Koyulhisar", "Koyulhisar Malmüdürlüğü" },
                    { new Guid("00000357-1111-1111-1111-000000000000"), "SİVAS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Suşehri", "Suşehri Malmüdürlüğü" },
                    { new Guid("00000358-1111-1111-1111-000000000000"), "SİVAS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yıldızeli", "Yıldızeli Malmüdürlüğü" },
                    { new Guid("00000359-1111-1111-1111-000000000000"), "SİVAS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Zara", "Zara Malmüdürlüğü" },
                    { new Guid("0000035a-1111-1111-1111-000000000000"), "SİVAS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Akıncılar", "Akıncılar Malmüdürlüğü" },
                    { new Guid("0000035b-1111-1111-1111-000000000000"), "SİVAS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Altınyayla", "Altınyayla Malmüdürlüğü" },
                    { new Guid("0000035c-1111-1111-1111-000000000000"), "SİVAS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Doğanşar", "Doğanşar Malmüdürlüğü" },
                    { new Guid("0000035d-1111-1111-1111-000000000000"), "SİVAS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gölova", "Gölova Malmüdürlüğü" },
                    { new Guid("0000035e-1111-1111-1111-000000000000"), "SİVAS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ulaş", "Ulaş Malmüdürlüğü" },
                    { new Guid("0000035f-1111-1111-1111-000000000000"), "TEKİRDAĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Süleymanpaşa Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000360-1111-1111-1111-000000000000"), "TEKİRDAĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Namık Kemal Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000361-1111-1111-1111-000000000000"), "TEKİRDAĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çorlu", "Çorlu İhtisas Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000362-1111-1111-1111-000000000000"), "TEKİRDAĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çerkezköy", "Çerkezköy Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000363-1111-1111-1111-000000000000"), "TEKİRDAĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çorlu", "Çorlu Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000364-1111-1111-1111-000000000000"), "TEKİRDAĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hayrabolu", "Hayrabolu Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000365-1111-1111-1111-000000000000"), "TEKİRDAĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Malkara", "Malkara Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000366-1111-1111-1111-000000000000"), "TEKİRDAĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Muratlı", "Muratlı Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000367-1111-1111-1111-000000000000"), "TEKİRDAĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Şarköy", "Şarköy Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000368-1111-1111-1111-000000000000"), "TEKİRDAĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kapaklı", "Kapaklı Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000369-1111-1111-1111-000000000000"), "TEKİRDAĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Saray", "Saray Malmüdürlüğü" },
                    { new Guid("0000036a-1111-1111-1111-000000000000"), "TEKİRDAĞ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Marmara Ereğlisi", "Marmara Ereğlisi Malmüdürlüğü" },
                    { new Guid("0000036b-1111-1111-1111-000000000000"), "TOKAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Tokat Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000036c-1111-1111-1111-000000000000"), "TOKAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Erbaa", "Erbaa Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000036d-1111-1111-1111-000000000000"), "TOKAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Niksar", "Niksar Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000036e-1111-1111-1111-000000000000"), "TOKAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Turhal", "Turhal Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000036f-1111-1111-1111-000000000000"), "TOKAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Zile", "Zile Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000370-1111-1111-1111-000000000000"), "TOKAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Almus", "Almus Malmüdürlüğü" },
                    { new Guid("00000371-1111-1111-1111-000000000000"), "TOKAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Artova", "Artova Malmüdürlüğü" },
                    { new Guid("00000372-1111-1111-1111-000000000000"), "TOKAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Reşadiye", "Reşadiye Malmüdürlüğü" },
                    { new Guid("00000373-1111-1111-1111-000000000000"), "TOKAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Pazar", "Pazar Malmüdürlüğü" },
                    { new Guid("00000374-1111-1111-1111-000000000000"), "TOKAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yeşilyurt", "Yeşilyurt Malmüdürlüğü" },
                    { new Guid("00000375-1111-1111-1111-000000000000"), "TOKAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Başçiftlik", "Başçiftlik Malmüdürlüğü" },
                    { new Guid("00000376-1111-1111-1111-000000000000"), "TOKAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sulusaray", "Sulusaray Malmüdürlüğü" },
                    { new Guid("00000377-1111-1111-1111-000000000000"), "TRABZON", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Hızırbey Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000378-1111-1111-1111-000000000000"), "TRABZON", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Karadeniz Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000379-1111-1111-1111-000000000000"), "TRABZON", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Akçaabat", "Akçaabat Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000037a-1111-1111-1111-000000000000"), "TRABZON", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Of", "Of Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000037b-1111-1111-1111-000000000000"), "TRABZON", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Vakfıkebir", "Vakfıkebir Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000037c-1111-1111-1111-000000000000"), "TRABZON", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Araklı", "Araklı Malmüdürlüğü" },
                    { new Guid("0000037d-1111-1111-1111-000000000000"), "TRABZON", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Arsin", "Arsin Malmüdürlüğü" },
                    { new Guid("0000037e-1111-1111-1111-000000000000"), "TRABZON", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çaykara", "Çaykara Malmüdürlüğü" },
                    { new Guid("0000037f-1111-1111-1111-000000000000"), "TRABZON", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Maçka", "Maçka Malmüdürlüğü" },
                    { new Guid("00000380-1111-1111-1111-000000000000"), "TRABZON", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sürmene", "Sürmene Malmüdürlüğü" },
                    { new Guid("00000381-1111-1111-1111-000000000000"), "TRABZON", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Tonya", "Tonya Malmüdürlüğü" },
                    { new Guid("00000382-1111-1111-1111-000000000000"), "TRABZON", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yomra", "Yomra Malmüdürlüğü" },
                    { new Guid("00000383-1111-1111-1111-000000000000"), "TRABZON", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Beşikdüzü", "Beşikdüzü Malmüdürlüğü" },
                    { new Guid("00000384-1111-1111-1111-000000000000"), "TRABZON", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Şalpazarı", "Şalpazarı Malmüdürlüğü" },
                    { new Guid("00000385-1111-1111-1111-000000000000"), "TRABZON", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çarşıbaşı", "Çarşıbaşı Malmüdürlüğü" },
                    { new Guid("00000386-1111-1111-1111-000000000000"), "TRABZON", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Dernekpazarı", "Dernekpazarı Malmüdürlüğü" },
                    { new Guid("00000387-1111-1111-1111-000000000000"), "TRABZON", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Düzköy", "Düzköy Malmüdürlüğü" },
                    { new Guid("00000388-1111-1111-1111-000000000000"), "TRABZON", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hayrat", "Hayrat Malmüdürlüğü" },
                    { new Guid("00000389-1111-1111-1111-000000000000"), "TRABZON", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Köprübaşı", "Köprübaşı Malmüdürlüğü" },
                    { new Guid("0000038a-1111-1111-1111-000000000000"), "TUNCELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Tunceli Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000038b-1111-1111-1111-000000000000"), "TUNCELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çemişgezek", "Çemişgezek Malmüdürlüğü" },
                    { new Guid("0000038c-1111-1111-1111-000000000000"), "TUNCELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hozat", "Hozat Malmüdürlüğü" },
                    { new Guid("0000038d-1111-1111-1111-000000000000"), "TUNCELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Mazgirt", "Mazgirt Malmüdürlüğü" },
                    { new Guid("0000038e-1111-1111-1111-000000000000"), "TUNCELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Nazimiye", "Nazimiye Malmüdürlüğü" },
                    { new Guid("0000038f-1111-1111-1111-000000000000"), "TUNCELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ovacık", "Ovacık Malmüdürlüğü" },
                    { new Guid("00000390-1111-1111-1111-000000000000"), "TUNCELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Pertek", "Pertek Malmüdürlüğü" },
                    { new Guid("00000391-1111-1111-1111-000000000000"), "TUNCELİ", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Pülümür", "Pülümür Malmüdürlüğü" },
                    { new Guid("00000392-1111-1111-1111-000000000000"), "ŞANLIURFA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Şehitlik Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000393-1111-1111-1111-000000000000"), "ŞANLIURFA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Topçu Meydanı Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000394-1111-1111-1111-000000000000"), "ŞANLIURFA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Göbeklitepe Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000395-1111-1111-1111-000000000000"), "ŞANLIURFA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Siverek", "Siverek Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000396-1111-1111-1111-000000000000"), "ŞANLIURFA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Viranşehir", "Viranşehir Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000397-1111-1111-1111-000000000000"), "ŞANLIURFA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Birecik", "Birecik Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000398-1111-1111-1111-000000000000"), "ŞANLIURFA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Suruç", "Suruç  Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000399-1111-1111-1111-000000000000"), "ŞANLIURFA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Akçakale", "Akçakale Malmüdürlüğü" },
                    { new Guid("0000039a-1111-1111-1111-000000000000"), "ŞANLIURFA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bozova", "Bozova Malmüdürlüğü" },
                    { new Guid("0000039b-1111-1111-1111-000000000000"), "ŞANLIURFA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Halfeti", "Halfeti Malmüdürlüğü" },
                    { new Guid("0000039c-1111-1111-1111-000000000000"), "ŞANLIURFA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hilvan", "Hilvan Malmüdürlüğü" },
                    { new Guid("0000039d-1111-1111-1111-000000000000"), "ŞANLIURFA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ceylanpınar", "Ceylanpınar Malmüdürlüğü" },
                    { new Guid("0000039e-1111-1111-1111-000000000000"), "ŞANLIURFA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Harran", "Harran Malmüdürlüğü" },
                    { new Guid("0000039f-1111-1111-1111-000000000000"), "UŞAK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Uşak Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003a0-1111-1111-1111-000000000000"), "UŞAK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Banaz", "Banaz Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003a1-1111-1111-1111-000000000000"), "UŞAK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Eşme", "Eşme Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003a2-1111-1111-1111-000000000000"), "UŞAK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Karahallı", "Karahallı Malmüdürlüğü" },
                    { new Guid("000003a3-1111-1111-1111-000000000000"), "UŞAK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ulubey", "Ulubey Malmüdürlüğü" },
                    { new Guid("000003a4-1111-1111-1111-000000000000"), "UŞAK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sivaslı", "Sivaslı Malmüdürlüğü" },
                    { new Guid("000003a5-1111-1111-1111-000000000000"), "VAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Van Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003a6-1111-1111-1111-000000000000"), "VAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Erciş", "Erciş Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003a7-1111-1111-1111-000000000000"), "VAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Başkale", "Başkale Malmüdürlüğü" },
                    { new Guid("000003a8-1111-1111-1111-000000000000"), "VAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çatak", "Çatak Malmüdürlüğü" },
                    { new Guid("000003a9-1111-1111-1111-000000000000"), "VAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gevaş", "Gevaş Malmüdürlüğü" },
                    { new Guid("000003aa-1111-1111-1111-000000000000"), "VAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gürpınar", "Gürpınar Malmüdürlüğü" },
                    { new Guid("000003ab-1111-1111-1111-000000000000"), "VAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Muradiye", "Muradiye Malmüdürlüğü" },
                    { new Guid("000003ac-1111-1111-1111-000000000000"), "VAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Özalp", "Özalp Malmüdürlüğü" },
                    { new Guid("000003ad-1111-1111-1111-000000000000"), "VAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bahçesaray", "Bahçesaray Malmüdürlüğü" },
                    { new Guid("000003ae-1111-1111-1111-000000000000"), "VAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çaldıran", "Çaldıran Malmüdürlüğü" },
                    { new Guid("000003af-1111-1111-1111-000000000000"), "VAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Edremit", "Edremit Malmüdürlüğü" },
                    { new Guid("000003b0-1111-1111-1111-000000000000"), "VAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Saray", "Saray Malmüdürlüğü" },
                    { new Guid("000003b1-1111-1111-1111-000000000000"), "YOZGAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Yozgat Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003b2-1111-1111-1111-000000000000"), "YOZGAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Boğazlıyan", "Boğazlıyan Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003b3-1111-1111-1111-000000000000"), "YOZGAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sorgun", "Sorgun Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003b4-1111-1111-1111-000000000000"), "YOZGAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yerköy", "Yerköy Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003b5-1111-1111-1111-000000000000"), "YOZGAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Akdağmadeni", "Akdağmadeni Malmüdürlüğü" },
                    { new Guid("000003b6-1111-1111-1111-000000000000"), "YOZGAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çayıralan", "Çayıralan Malmüdürlüğü" },
                    { new Guid("000003b7-1111-1111-1111-000000000000"), "YOZGAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çekerek", "Çekerek Malmüdürlüğü" },
                    { new Guid("000003b8-1111-1111-1111-000000000000"), "YOZGAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sarıkaya", "Sarıkaya Malmüdürlüğü" },
                    { new Guid("000003b9-1111-1111-1111-000000000000"), "YOZGAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Şefaatli", "Şefaatli Malmüdürlüğü" },
                    { new Guid("000003ba-1111-1111-1111-000000000000"), "YOZGAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Aydıncık", "Aydıncık Malmüdürlüğü" },
                    { new Guid("000003bb-1111-1111-1111-000000000000"), "YOZGAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çandır", "Çandır Malmüdürlüğü" },
                    { new Guid("000003bc-1111-1111-1111-000000000000"), "YOZGAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kadışehri", "Kadışehri Malmüdürlüğü" },
                    { new Guid("000003bd-1111-1111-1111-000000000000"), "YOZGAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Saraykent", "Saraykent Malmüdürlüğü" },
                    { new Guid("000003be-1111-1111-1111-000000000000"), "YOZGAT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yenifakılı", "Yenifakılı Malmüdürlüğü" },
                    { new Guid("000003bf-1111-1111-1111-000000000000"), "ZONGULDAK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Uzunmehmet Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003c0-1111-1111-1111-000000000000"), "ZONGULDAK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kara Elmas Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003c1-1111-1111-1111-000000000000"), "ZONGULDAK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ereğli", "Ereğli Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003c2-1111-1111-1111-000000000000"), "ZONGULDAK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çaycuma", "Çaycuma Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003c3-1111-1111-1111-000000000000"), "ZONGULDAK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Devrek", "Devrek Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003c4-1111-1111-1111-000000000000"), "ZONGULDAK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Alaplı", "Alaplı Malmüdürlüğü" },
                    { new Guid("000003c5-1111-1111-1111-000000000000"), "ZONGULDAK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gökçebey", "Gökçebey Malmüdürlüğü" },
                    { new Guid("000003c6-1111-1111-1111-000000000000"), "AKSARAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Aksaray Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003c7-1111-1111-1111-000000000000"), "AKSARAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ağaçören", "Ağaçören Malmüdürlüğü" },
                    { new Guid("000003c8-1111-1111-1111-000000000000"), "AKSARAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Güzelyurt", "Güzelyurt Malmüdürlüğü" },
                    { new Guid("000003c9-1111-1111-1111-000000000000"), "AKSARAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ortaköy", "Ortaköy Malmüdürlüğü" },
                    { new Guid("000003ca-1111-1111-1111-000000000000"), "AKSARAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sarıyahşi", "Sarıyahşi Malmüdürlüğü" },
                    { new Guid("000003cb-1111-1111-1111-000000000000"), "AKSARAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Eskil", "Eskil Malmüdürlüğü" },
                    { new Guid("000003cc-1111-1111-1111-000000000000"), "AKSARAY", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gülağaç", "Gülağaç Malmüdürlüğü" },
                    { new Guid("000003cd-1111-1111-1111-000000000000"), "BAYBURT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Bayburt Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003ce-1111-1111-1111-000000000000"), "BAYBURT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Aydıntepe", "Aydıntepe Malmüdürlüğü" },
                    { new Guid("000003cf-1111-1111-1111-000000000000"), "BAYBURT", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Demirözü", "Demirözü Malmüdürlüğü" },
                    { new Guid("000003d0-1111-1111-1111-000000000000"), "KARAMAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Karaman Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003d1-1111-1111-1111-000000000000"), "KARAMAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ayrancı", "Ayrancı Malmüdürlüğü" },
                    { new Guid("000003d2-1111-1111-1111-000000000000"), "KARAMAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ermenek", "Ermenek Malmüdürlüğü" },
                    { new Guid("000003d3-1111-1111-1111-000000000000"), "KARAMAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kazım Karabekir", "Kazım Karabekir Malmüdürlüğü" },
                    { new Guid("000003d4-1111-1111-1111-000000000000"), "KARAMAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Başyayla", "Başyayla Malmüdürlüğü" },
                    { new Guid("000003d5-1111-1111-1111-000000000000"), "KARAMAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sarıveliler", "Sarıveliler Malmüdürlüğü" },
                    { new Guid("000003d6-1111-1111-1111-000000000000"), "KIRIKKALE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Irmak Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003d7-1111-1111-1111-000000000000"), "KIRIKKALE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kaletepe Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003d8-1111-1111-1111-000000000000"), "KIRIKKALE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Delice", "Delice Malmüdürlüğü" },
                    { new Guid("000003d9-1111-1111-1111-000000000000"), "KIRIKKALE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Keskin", "Keskin Malmüdürlüğü" },
                    { new Guid("000003da-1111-1111-1111-000000000000"), "KIRIKKALE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sulakyurt", "Sulakyurt Malmüdürlüğü" },
                    { new Guid("000003db-1111-1111-1111-000000000000"), "KIRIKKALE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Balışeyh", "Balışeyh Malmüdürlüğü" },
                    { new Guid("000003dc-1111-1111-1111-000000000000"), "KIRIKKALE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çelebi", "Çelebi Malmüdürlüğü" },
                    { new Guid("000003dd-1111-1111-1111-000000000000"), "KIRIKKALE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Karakeçili", "Karakeçili Malmüdürlüğü" },
                    { new Guid("000003de-1111-1111-1111-000000000000"), "BATMAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Batman Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003df-1111-1111-1111-000000000000"), "BATMAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Beşiri", "Beşiri Malmüdürlüğü" },
                    { new Guid("000003e0-1111-1111-1111-000000000000"), "BATMAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gercüş", "Gercüş Malmüdürlüğü" },
                    { new Guid("000003e1-1111-1111-1111-000000000000"), "BATMAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hasankeyf", "Hasankeyf Malmüdürlüğü" },
                    { new Guid("000003e2-1111-1111-1111-000000000000"), "BATMAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kozluk", "Kozluk Malmüdürlüğü" },
                    { new Guid("000003e3-1111-1111-1111-000000000000"), "BATMAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sason", "Sason Malmüdürlüğü" },
                    { new Guid("000003e4-1111-1111-1111-000000000000"), "ŞIRNAK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Şırnak Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003e5-1111-1111-1111-000000000000"), "ŞIRNAK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Cizre", "Cizre Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003e6-1111-1111-1111-000000000000"), "ŞIRNAK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Silopi", "Silopi Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003e7-1111-1111-1111-000000000000"), "ŞIRNAK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Beytüşşebap", "Beytüşşebap Malmüdürlüğü" },
                    { new Guid("000003e8-1111-1111-1111-000000000000"), "ŞIRNAK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Güçlükonak", "Güçlükonak Malmüdürlüğü" },
                    { new Guid("000003e9-1111-1111-1111-000000000000"), "ŞIRNAK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "İdil", "İdil Malmüdürlüğü" },
                    { new Guid("000003ea-1111-1111-1111-000000000000"), "ŞIRNAK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Uludere", "Uludere Malmüdürlüğü" },
                    { new Guid("000003eb-1111-1111-1111-000000000000"), "BARTIN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Bartın Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003ec-1111-1111-1111-000000000000"), "BARTIN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Amasra", "Amasra Malmüdürlüğü" },
                    { new Guid("000003ed-1111-1111-1111-000000000000"), "BARTIN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kurucaşile", "Kurucaşile Malmüdürlüğü" },
                    { new Guid("000003ee-1111-1111-1111-000000000000"), "BARTIN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ulus", "Ulus Malmüdürlüğü" },
                    { new Guid("000003ef-1111-1111-1111-000000000000"), "ARDAHAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Ardahan Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003f0-1111-1111-1111-000000000000"), "ARDAHAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çıldır", "Çıldır Malmüdürlüğü" },
                    { new Guid("000003f1-1111-1111-1111-000000000000"), "ARDAHAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Damal", "Damal Malmüdürlüğü" },
                    { new Guid("000003f2-1111-1111-1111-000000000000"), "ARDAHAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Göle", "Göle Malmüdürlüğü" },
                    { new Guid("000003f3-1111-1111-1111-000000000000"), "ARDAHAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hanak", "Hanak Malmüdürlüğü" },
                    { new Guid("000003f4-1111-1111-1111-000000000000"), "ARDAHAN", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Posof", "Posof Malmüdürlüğü" },
                    { new Guid("000003f5-1111-1111-1111-000000000000"), "IĞDIR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Iğdır Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003f6-1111-1111-1111-000000000000"), "IĞDIR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Aralık", "Aralık Malmüdürlüğü" },
                    { new Guid("000003f7-1111-1111-1111-000000000000"), "IĞDIR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Karakoyunlu", "Karakoyunlu Malmüdürlüğü" },
                    { new Guid("000003f8-1111-1111-1111-000000000000"), "IĞDIR", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Tuzluca", "Tuzluca Malmüdürlüğü" },
                    { new Guid("000003f9-1111-1111-1111-000000000000"), "YALOVA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Yalova Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003fa-1111-1111-1111-000000000000"), "YALOVA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Altınova", "Altınova Malmüdürlüğü" },
                    { new Guid("000003fb-1111-1111-1111-000000000000"), "YALOVA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Armutlu", "Armutlu Malmüdürlüğü" },
                    { new Guid("000003fc-1111-1111-1111-000000000000"), "YALOVA", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çınarcık", "Çınarcık Malmüdürlüğü" },
                    { new Guid("000003fd-1111-1111-1111-000000000000"), "KARABÜK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Karabük Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003fe-1111-1111-1111-000000000000"), "KARABÜK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Safranbolu", "Safranbolu Vergi Dairesi Müdürlüğü" },
                    { new Guid("000003ff-1111-1111-1111-000000000000"), "KARABÜK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Eflani", "Eflani Malmüdürlüğü" },
                    { new Guid("00000400-1111-1111-1111-000000000000"), "KARABÜK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Eskipazar", "Eskipazar Malmüdürlüğü" },
                    { new Guid("00000401-1111-1111-1111-000000000000"), "KARABÜK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Ovacık", "Ovacık Malmüdürlüğü" },
                    { new Guid("00000402-1111-1111-1111-000000000000"), "KARABÜK", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yenice", "Yenice Malmüdürlüğü" },
                    { new Guid("00000403-1111-1111-1111-000000000000"), "KİLİS", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Kilis Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000404-1111-1111-1111-000000000000"), "OSMANİYE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Osmaniye Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000405-1111-1111-1111-000000000000"), "OSMANİYE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kadirli", "Kadirli Vergi Dairesi Müdürlüğü" },
                    { new Guid("00000406-1111-1111-1111-000000000000"), "OSMANİYE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Bahçe", "Bahçe  Malmüdürlüğü" },
                    { new Guid("00000407-1111-1111-1111-000000000000"), "OSMANİYE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Düziçi", "Düziçi Malmüdürlüğü" },
                    { new Guid("00000408-1111-1111-1111-000000000000"), "OSMANİYE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Hasanbeyli", "Hasanbeyli Malmüdürlüğü" },
                    { new Guid("00000409-1111-1111-1111-000000000000"), "OSMANİYE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Sumbas", "Sumbas Malmüdürlüğü" },
                    { new Guid("0000040a-1111-1111-1111-000000000000"), "OSMANİYE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Toprakkale", "Toprakkale Malmüdürlüğü" },
                    { new Guid("0000040b-1111-1111-1111-000000000000"), "DÜZCE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Merkez", "Düzce Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000040c-1111-1111-1111-000000000000"), "DÜZCE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Akçakoca", "Akçakoca Vergi Dairesi Müdürlüğü" },
                    { new Guid("0000040d-1111-1111-1111-000000000000"), "DÜZCE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Cumayeri", "Cumayeri Malmüdürlüğü" },
                    { new Guid("0000040e-1111-1111-1111-000000000000"), "DÜZCE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Çilimli", "Çilimli Malmüdürlüğü" },
                    { new Guid("0000040f-1111-1111-1111-000000000000"), "DÜZCE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gölyaka", "Gölyaka Malmüdürlüğü" },
                    { new Guid("00000410-1111-1111-1111-000000000000"), "DÜZCE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Gümüşova", "Gümüşova Malmüdürlüğü" },
                    { new Guid("00000411-1111-1111-1111-000000000000"), "DÜZCE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Kaynaşlı", "Kaynaşlı Malmüdürlüğü" },
                    { new Guid("00000412-1111-1111-1111-000000000000"), "DÜZCE", new DateTime(2026, 3, 4, 19, 21, 24, 959, DateTimeKind.Utc).AddTicks(3090), "Yığılca", "Yığılca Malmüdürlüğü" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransactions_CustomerId",
                table: "AccountTransactions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransactions_CustomerId_Date",
                table: "AccountTransactions",
                columns: new[] { "CustomerId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransactions_UserId",
                table: "AccountTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FirmId",
                table: "AspNetUsers",
                column: "FirmId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_FirmId",
                table: "BankAccounts",
                column: "FirmId");

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_BankAccountId",
                table: "BankTransactions",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_BankAccountId_Date",
                table: "BankTransactions",
                columns: new[] { "BankAccountId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_CashRegisters_FirmId",
                table: "CashRegisters",
                column: "FirmId");

            migrationBuilder.CreateIndex(
                name: "IX_CashTransactions_CashRegisterId",
                table: "CashTransactions",
                column: "CashRegisterId");

            migrationBuilder.CreateIndex(
                name: "IX_CashTransactions_CashRegisterId_Date",
                table: "CashTransactions",
                columns: new[] { "CashRegisterId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_ChequeOrPromissories_BankAccountId",
                table: "ChequeOrPromissories",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ChequeOrPromissories_CustomerId",
                table: "ChequeOrPromissories",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ChequeOrPromissories_FirmId",
                table: "ChequeOrPromissories",
                column: "FirmId");

            migrationBuilder.CreateIndex(
                name: "IX_ChequeOrPromissories_FirmId_DueDate",
                table: "ChequeOrPromissories",
                columns: new[] { "FirmId", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CompanySettings_FirmId",
                table: "CompanySettings",
                column: "FirmId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId",
                table: "Customers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId_Code",
                table: "Customers",
                columns: new[] { "UserId", "Code" },
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryNoteItems_DeliveryNoteId",
                table: "DeliveryNoteItems",
                column: "DeliveryNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryNoteItems_OrderItemId",
                table: "DeliveryNoteItems",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryNoteItems_ProductId",
                table: "DeliveryNoteItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryNotes_CustomerId",
                table: "DeliveryNotes",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryNotes_OrderId",
                table: "DeliveryNotes",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryNotes_UserId_DeliveryNumber",
                table: "DeliveryNotes",
                columns: new[] { "UserId", "DeliveryNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Firms_Name",
                table: "Firms",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_InvoiceId",
                table: "InvoiceItems",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_ProductId",
                table: "InvoiceItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CustomerId",
                table: "Invoices",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_UserId_InvoiceNumber",
                table: "Invoices",
                columns: new[] { "UserId", "InvoiceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_Level",
                table: "LogEntries",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_Module",
                table: "LogEntries",
                column: "Module");

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_Timestamp",
                table: "LogEntries",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_MainAccountCodes_Code",
                table: "MainAccountCodes",
                column: "Code",
                unique: true,
                filter: "[FirmId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MainAccountCodes_FirmId",
                table: "MainAccountCodes",
                column: "FirmId");

            migrationBuilder.CreateIndex(
                name: "IX_MainAccountCodes_FirmId_Code",
                table: "MainAccountCodes",
                columns: new[] { "FirmId", "Code" },
                unique: true,
                filter: "[FirmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId_OrderNumber",
                table: "Orders",
                columns: new[] { "UserId", "OrderNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_FirmId",
                table: "Products",
                column: "FirmId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_FirmId_Code",
                table: "Products",
                columns: new[] { "FirmId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ProductId",
                table: "StockMovements",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ProductId_Date",
                table: "StockMovements",
                columns: new[] { "ProductId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_TaxOffices_City",
                table: "TaxOffices",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_TaxOffices_District",
                table: "TaxOffices",
                column: "District");

            migrationBuilder.CreateIndex(
                name: "IX_TaxOffices_Name",
                table: "TaxOffices",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountTransactions");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BankTransactions");

            migrationBuilder.DropTable(
                name: "CashTransactions");

            migrationBuilder.DropTable(
                name: "ChequeOrPromissories");

            migrationBuilder.DropTable(
                name: "CompanySettings");

            migrationBuilder.DropTable(
                name: "DeliveryNoteItems");

            migrationBuilder.DropTable(
                name: "InvoiceItems");

            migrationBuilder.DropTable(
                name: "LogEntries");

            migrationBuilder.DropTable(
                name: "MainAccountCodes");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "StockMovements");

            migrationBuilder.DropTable(
                name: "TaxOffices");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "CashRegisters");

            migrationBuilder.DropTable(
                name: "BankAccounts");

            migrationBuilder.DropTable(
                name: "DeliveryNotes");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Firms");
        }
    }
}
