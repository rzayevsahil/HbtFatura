using HbtFatura.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HbtFatura.Api.Data;

public static class LookupSeed
{
    public static async Task SeedIfEmptyAsync(AppDbContext db)
    {
        // 1. Ensure Groups exist
        var groupNames = new[] { "OrderType", "OrderStatus", "InvoiceStatus", "DeliveryNoteStatus", "InvoiceType", "DeliveryNoteType", "ChequeStatus", "Currency" };
        var existingGroups = await db.LookupGroups.ToListAsync();
        
        var groupsToCreate = new List<LookupGroup>();
        if (!existingGroups.Any(x => x.Name == "OrderType"))
            groupsToCreate.Add(new LookupGroup { Id = Guid.NewGuid(), Name = "OrderType", DisplayName = "Sipariş Tipi", IsSystemGroup = true });
        
        if (!existingGroups.Any(x => x.Name == "OrderStatus"))
            groupsToCreate.Add(new LookupGroup { Id = Guid.NewGuid(), Name = "OrderStatus", DisplayName = "Sipariş Durumu", IsSystemGroup = true });
        
        if (!existingGroups.Any(x => x.Name == "InvoiceStatus"))
            groupsToCreate.Add(new LookupGroup { Id = Guid.NewGuid(), Name = "InvoiceStatus", DisplayName = "Fatura Durumu", IsSystemGroup = true });
        
        if (!existingGroups.Any(x => x.Name == "DeliveryNoteStatus"))
            groupsToCreate.Add(new LookupGroup { Id = Guid.NewGuid(), Name = "DeliveryNoteStatus", DisplayName = "İrsaliye Durumu", IsSystemGroup = true });

        if (!existingGroups.Any(x => x.Name == "InvoiceType"))
            groupsToCreate.Add(new LookupGroup { Id = Guid.NewGuid(), Name = "InvoiceType", DisplayName = "Fatura Tipi", IsSystemGroup = true });

        if (!existingGroups.Any(x => x.Name == "DeliveryNoteType"))
            groupsToCreate.Add(new LookupGroup { Id = Guid.NewGuid(), Name = "DeliveryNoteType", DisplayName = "İrsaliye Tipi", IsSystemGroup = true });

        if (!existingGroups.Any(x => x.Name == "ChequeStatus"))
            groupsToCreate.Add(new LookupGroup { Id = Guid.NewGuid(), Name = "ChequeStatus", DisplayName = "Çek/Senet Durumu", IsSystemGroup = true });

        if (!existingGroups.Any(x => x.Name == "Currency"))
            groupsToCreate.Add(new LookupGroup { Id = Guid.NewGuid(), Name = "Currency", DisplayName = "Para Birimi", IsSystemGroup = true });

        if (!existingGroups.Any(x => x.Name == "VatRate"))
            groupsToCreate.Add(new LookupGroup { Id = Guid.NewGuid(), Name = "VatRate", DisplayName = "KDV Oranı (%)", IsSystemGroup = true });

        if (groupsToCreate.Any())
        {
            await db.LookupGroups.AddRangeAsync(groupsToCreate);
            await db.SaveChangesAsync();
            existingGroups.AddRange(groupsToCreate);
        }

        // 2. Pre-calculate IDs to avoid expression tree issues
        var orderTypeId = existingGroups.First(x => x.Name == "OrderType").Id;
        var orderStatusId = existingGroups.First(x => x.Name == "OrderStatus").Id;
        var invoiceStatusId = existingGroups.First(x => x.Name == "InvoiceStatus").Id;
        var deliveryNoteStatusId = existingGroups.First(x => x.Name == "DeliveryNoteStatus").Id;
        var invoiceTypeId = existingGroups.First(x => x.Name == "InvoiceType").Id;
        var deliveryNoteTypeId = existingGroups.First(x => x.Name == "DeliveryNoteType").Id;
        var chequeStatusId = existingGroups.First(x => x.Name == "ChequeStatus").Id;
        var currencyGroupId = existingGroups.First(x => x.Name == "Currency").Id;
        var vatRateGroupId = existingGroups.First(x => x.Name == "VatRate").Id;

        var lookups = new List<Lookup>();

        // OrderType
        if (!await db.Lookups.AnyAsync(x => x.LookupGroupId == orderTypeId))
        {
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = orderTypeId, Code = "0", Name = "Alınan", Color = "#0d9488", SortOrder = 1 });
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = orderTypeId, Code = "1", Name = "Verilen", Color = "#3b82f6", SortOrder = 2 });
        }

        // OrderStatus
        if (!await db.Lookups.AnyAsync(x => x.LookupGroupId == orderStatusId))
        {
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = orderStatusId, Code = "0", Name = "Bekliyor", Color = "#ffc107", SortOrder = 1 });
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = orderStatusId, Code = "1", Name = "Tamamı Teslim", Color = "#28a745", SortOrder = 2 });
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = orderStatusId, Code = "2", Name = "İptal", Color = "#dc3545", SortOrder = 3 });
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = orderStatusId, Code = "3", Name = "Onaylandı", Color = "#007bff", SortOrder = 4 });
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = orderStatusId, Code = "4", Name = "Kısmi Teslim", Color = "#17a2b8", SortOrder = 5 });
        }

        // InvoiceStatus
        if (!await db.Lookups.AnyAsync(x => x.LookupGroupId == invoiceStatusId))
        {
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = invoiceStatusId, Code = "0", Name = "Taslak", Color = "#6c757d", SortOrder = 1 });
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = invoiceStatusId, Code = "1", Name = "Onaylandı", Color = "#007bff", SortOrder = 2 });
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = invoiceStatusId, Code = "2", Name = "Ödendi", Color = "#28a745", SortOrder = 3 });
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = invoiceStatusId, Code = "3", Name = "İptal", Color = "#dc3545", SortOrder = 4 });
        }

        // DeliveryNoteStatus
        if (!await db.Lookups.AnyAsync(x => x.LookupGroupId == deliveryNoteStatusId))
        {
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = deliveryNoteStatusId, Code = "0", Name = "Taslak", Color = "#6c757d", SortOrder = 1 });
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = deliveryNoteStatusId, Code = "1", Name = "Onaylandı", Color = "#007bff", SortOrder = 2 });
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = deliveryNoteStatusId, Code = "2", Name = "İptal", Color = "#dc3545", SortOrder = 3 });
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = deliveryNoteStatusId, Code = "3", Name = "Faturalandı", Color = "#28a745", SortOrder = 4 });
        }

        // InvoiceType
        if (!await db.Lookups.AnyAsync(x => x.LookupGroupId == invoiceTypeId))
        {
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = invoiceTypeId, Code = "0", Name = "Satış", Color = "#28a745", SortOrder = 1 });
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = invoiceTypeId, Code = "1", Name = "Alış", Color = "#dc3545", SortOrder = 2 });
        }

        // DeliveryNoteType
        if (!await db.Lookups.AnyAsync(x => x.LookupGroupId == deliveryNoteTypeId))
        {
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = deliveryNoteTypeId, Code = "0", Name = "Satış", Color = "#28a745", SortOrder = 1 });
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = deliveryNoteTypeId, Code = "1", Name = "Alış", Color = "#dc3545", SortOrder = 2 });
        }

        // ChequeStatus (Çek/Senet durumu)
        if (!await db.Lookups.AnyAsync(x => x.LookupGroupId == chequeStatusId))
        {
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = chequeStatusId, Code = "0", Name = "Portföyde", Color = "#6c757d", SortOrder = 1 });
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = chequeStatusId, Code = "1", Name = "Tahsil edildi", Color = "#28a745", SortOrder = 2 });
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = chequeStatusId, Code = "2", Name = "Ödendi", Color = "#007bff", SortOrder = 3 });
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = chequeStatusId, Code = "3", Name = "Reddedildi", Color = "#dc3545", SortOrder = 4 });
        }

        // Currency (Para birimi) - TRY, USD, EUR
        if (!await db.Lookups.AnyAsync(x => x.LookupGroupId == currencyGroupId))
        {
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = currencyGroupId, Code = "TRY", Name = "Türk Lirası", Color = "#0ca678", SortOrder = 1 });
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = currencyGroupId, Code = "USD", Name = "Amerikan Doları", Color = "#2563eb", SortOrder = 2 });
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = currencyGroupId, Code = "EUR", Name = "Euro", Color = "#f59e0b", SortOrder = 3 });
        }

        // KDV oranları (Code = yüzde değeri; varsayılan satır App:DefaultVatRate ile uyumlu olmalı)
        if (!await db.Lookups.AnyAsync(x => x.LookupGroupId == vatRateGroupId))
        {
            lookups.Add(new Lookup { Id = Guid.NewGuid(), LookupGroupId = vatRateGroupId, Code = "20", Name = "%20", Color = "#0d9488", SortOrder = 1 });
        }

        if (lookups.Any())
        {
            await db.Lookups.AddRangeAsync(lookups);
            await db.SaveChangesAsync();
        }
    }
}
