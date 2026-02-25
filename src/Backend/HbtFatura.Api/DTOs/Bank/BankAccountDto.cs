namespace HbtFatura.Api.DTOs.Bank;

public class BankAccountDto
{
    public Guid Id { get; set; }
    public Guid FirmId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Iban { get; set; }
    public string? BankName { get; set; }
    public string Currency { get; set; } = "TRY";
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal Balance { get; set; }
}

public class CreateBankAccountRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Iban { get; set; }
    public string? BankName { get; set; }
    public string Currency { get; set; } = "TRY";
    public Guid? FirmId { get; set; }
}

public class UpdateBankAccountRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Iban { get; set; }
    public string? BankName { get; set; }
    public bool IsActive { get; set; }
}

public class BankTransactionDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public int Type { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ReferenceType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateBankTransactionRequest
{
    public DateTime Date { get; set; }
    public int Type { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}
