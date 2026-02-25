namespace HbtFatura.Api.DTOs.Customers;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string? MainAccountCode { get; set; }
    public string? Code { get; set; }
    public string Title { get; set; } = string.Empty;
    public int TaxPayerType { get; set; }
    public int CardType { get; set; }
    public string? TaxNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public decimal Balance { get; set; }
}

public class CustomerListDto : CustomerDto
{
    public DateTime CreatedAt { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
}

public class CreateCustomerRequest
{
    public string? MainAccountCode { get; set; }
    public string? Code { get; set; }
    public string Title { get; set; } = string.Empty;
    public int TaxPayerType { get; set; } = 2;
    public int CardType { get; set; } = 1;
    public string? TaxNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public class AccountTransactionDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Type { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string ReferenceType { get; set; } = string.Empty;
    public Guid? ReferenceId { get; set; }
    public decimal RunningBalance { get; set; }
}

public class UpdateCustomerRequest : CreateCustomerRequest { }

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
