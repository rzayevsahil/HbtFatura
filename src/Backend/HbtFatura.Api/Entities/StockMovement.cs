namespace HbtFatura.Api.Entities;

public class StockMovement
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public DateTime Date { get; set; }
    public int Type { get; set; } // 1=Giriş, 2=Çıkış, 3=Transfer
    public decimal Quantity { get; set; }
    public string ReferenceType { get; set; } = string.Empty;
    public Guid? ReferenceId { get; set; }
    public Guid? WarehouseId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Product Product { get; set; } = null!;
}
