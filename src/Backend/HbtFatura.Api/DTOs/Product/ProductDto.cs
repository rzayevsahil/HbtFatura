namespace HbtFatura.Api.DTOs.Product;

public class ProductDto
{
    public Guid Id { get; set; }
    public Guid FirmId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string StockType { get; set; } = "ticari mal";
    public string Unit { get; set; } = "Adet";
    public decimal StockQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = "TRY";
    public DateTime CreatedAt { get; set; }
}

public class ProductListDto : ProductDto { }

public class CreateProductRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string StockType { get; set; } = "ticari mal";
    public string Unit { get; set; } = "Adet";
    public decimal StockQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = "TRY";
    public Guid? FirmId { get; set; }
}

public class UpdateProductRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string StockType { get; set; } = "ticari mal";
    public string Unit { get; set; } = "Adet";
    public decimal StockQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = "TRY";
}

public class StockMovementDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public int Type { get; set; }
    public decimal Quantity { get; set; }
    public string ReferenceType { get; set; } = string.Empty;
    public Guid? ReferenceId { get; set; }
    public Guid? InvoiceId { get; set; }
    public string? InvoiceNumber { get; set; }
    public Guid? DeliveryNoteId { get; set; }
    public string? DeliveryNumber { get; set; }
    public Guid? OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateStockMovementRequest
{
    public DateTime Date { get; set; }
    public int Type { get; set; } // 1=Giris, 2=Cikis
    public decimal Quantity { get; set; }
    public string Description { get; set; } = string.Empty;
}

/// <summary>Ürün detay sayfasında "bu üründen ne zaman ne kadar satılmış" listesi için satır.</summary>
public class ProductSaleRowDto
{
    public DateTime Date { get; set; }
    public decimal Quantity { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid? InvoiceId { get; set; }
    public string? OrderNumber { get; set; }
    public Guid? OrderId { get; set; }
    public string? DeliveryNumber { get; set; }
    public Guid? DeliveryNoteId { get; set; }
}
