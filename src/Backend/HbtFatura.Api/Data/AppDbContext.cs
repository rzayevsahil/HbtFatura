using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Firm> Firms => Set<Firm>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<CompanySettings> CompanySettings => Set<CompanySettings>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<AccountTransaction> AccountTransactions => Set<AccountTransaction>();
    public DbSet<CashRegister> CashRegisters => Set<CashRegister>();
    public DbSet<CashTransaction> CashTransactions => Set<CashTransaction>();
    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<BankTransaction> BankTransactions => Set<BankTransaction>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<ChequeOrPromissory> ChequeOrPromissories => Set<ChequeOrPromissory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Firm>(e =>
        {
            e.HasIndex(x => x.Name);
        });

        modelBuilder.Entity<ApplicationUser>(e =>
        {
            e.HasOne(x => x.Firm).WithMany(x => x.Users).HasForeignKey(x => x.FirmId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => x.FirmId);
        });

        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.HasOne(x => x.User).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.Token);
        });

        modelBuilder.Entity<CompanySettings>(e =>
        {
            e.HasOne(x => x.Firm).WithOne(x => x.CompanySettings).HasForeignKey<CompanySettings>(x => x.FirmId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.FirmId).IsUnique();
        });

        modelBuilder.Entity<Customer>(e =>
        {
            e.HasOne(x => x.User).WithMany(x => x.Customers).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.UserId);
            e.HasIndex(x => new { x.UserId, x.Code }).IsUnique().HasFilter("[Code] IS NOT NULL");
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<AccountTransaction>(e =>
        {
            e.HasOne(x => x.Customer).WithMany(x => x.AccountTransactions).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => x.CustomerId);
            e.HasIndex(x => new { x.CustomerId, x.Date });
            e.Property(x => x.Amount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<CashRegister>(e =>
        {
            e.HasOne(x => x.Firm).WithMany(x => x.CashRegisters).HasForeignKey(x => x.FirmId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => x.FirmId);
        });

        modelBuilder.Entity<CashTransaction>(e =>
        {
            e.HasOne(x => x.CashRegister).WithMany(x => x.Transactions).HasForeignKey(x => x.CashRegisterId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.CashRegisterId);
            e.HasIndex(x => new { x.CashRegisterId, x.Date });
            e.Property(x => x.Amount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<BankAccount>(e =>
        {
            e.HasOne(x => x.Firm).WithMany(x => x.BankAccounts).HasForeignKey(x => x.FirmId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => x.FirmId);
        });

        modelBuilder.Entity<BankTransaction>(e =>
        {
            e.HasOne(x => x.BankAccount).WithMany(x => x.Transactions).HasForeignKey(x => x.BankAccountId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.BankAccountId);
            e.HasIndex(x => new { x.BankAccountId, x.Date });
            e.Property(x => x.Amount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Product>(e =>
        {
            e.HasOne(x => x.Firm).WithMany(x => x.Products).HasForeignKey(x => x.FirmId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => x.FirmId);
            e.HasIndex(x => new { x.FirmId, x.Code }).IsUnique();
            e.Property(x => x.MinStock).HasPrecision(18, 4);
            e.Property(x => x.MaxStock).HasPrecision(18, 4);
        });

        modelBuilder.Entity<StockMovement>(e =>
        {
            e.HasOne(x => x.Product).WithMany(x => x.StockMovements).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.ProductId);
            e.HasIndex(x => new { x.ProductId, x.Date });
            e.Property(x => x.Quantity).HasPrecision(18, 4);
        });

        modelBuilder.Entity<ChequeOrPromissory>(e =>
        {
            e.HasOne(x => x.Firm).WithMany(x => x.ChequeOrPromissories).HasForeignKey(x => x.FirmId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.BankAccount).WithMany().HasForeignKey(x => x.BankAccountId).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(x => x.FirmId);
            e.HasIndex(x => new { x.FirmId, x.DueDate });
            e.Property(x => x.Amount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Invoice>(e =>
        {
            e.HasOne(x => x.User).WithMany(x => x.Invoices).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Customer).WithMany(x => x.Invoices).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(x => new { x.UserId, x.InvoiceNumber }).IsUnique();
            e.Property(x => x.RowVersion).IsRowVersion();
            e.Property(x => x.SubTotal).HasPrecision(18, 2);
            e.Property(x => x.TotalVat).HasPrecision(18, 2);
            e.Property(x => x.GrandTotal).HasPrecision(18, 2);
            e.Property(x => x.ExchangeRate).HasPrecision(18, 6);
        });

        modelBuilder.Entity<InvoiceItem>(e =>
        {
            e.HasOne(x => x.Invoice).WithMany(x => x.Items).HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.SetNull);
            e.Property(x => x.Quantity).HasPrecision(18, 4);
            e.Property(x => x.UnitPrice).HasPrecision(18, 2);
            e.Property(x => x.VatRate).HasPrecision(5, 2);
            e.Property(x => x.LineTotalExclVat).HasPrecision(18, 2);
            e.Property(x => x.LineVatAmount).HasPrecision(18, 2);
            e.Property(x => x.LineTotalInclVat).HasPrecision(18, 2);
        });
    }
}
