namespace HbtFatura.Api.Entities;

public class Product
{
    public Guid Id { get; set; }
    public Guid FirmId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string Unit { get; set; } = "Adet";
    public decimal StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; }

    public Firm Firm { get; set; } = null!;
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
