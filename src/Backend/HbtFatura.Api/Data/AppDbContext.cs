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
            e.HasQueryFilter(x => !x.IsDeleted);
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
            e.Property(x => x.Quantity).HasPrecision(18, 4);
            e.Property(x => x.UnitPrice).HasPrecision(18, 2);
            e.Property(x => x.VatRate).HasPrecision(5, 2);
            e.Property(x => x.LineTotalExclVat).HasPrecision(18, 2);
            e.Property(x => x.LineVatAmount).HasPrecision(18, 2);
            e.Property(x => x.LineTotalInclVat).HasPrecision(18, 2);
        });
    }
}
