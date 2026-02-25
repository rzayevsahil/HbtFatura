namespace HbtFatura.Api.DTOs.Cash;

public class CashRegisterDto
{
    public Guid Id { get; set; }
    public Guid FirmId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Currency { get; set; } = "TRY";
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal Balance { get; set; }
}

public class CreateCashRegisterRequest
{
    public string Name { get; set; } = string.Empty;
    public string Currency { get; set; } = "TRY";
    public Guid? FirmId { get; set; }
}

public class UpdateCashRegisterRequest
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CashTransactionDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public int Type { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ReferenceType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateCashTransactionRequest
{
    public DateTime Date { get; set; }
    public int Type { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}
