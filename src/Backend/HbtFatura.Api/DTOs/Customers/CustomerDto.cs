namespace HbtFatura.Api.DTOs.Customers;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TaxNumber { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public class CustomerListDto : CustomerDto
{
    public DateTime CreatedAt { get; set; }
}

public class CreateCustomerRequest
{
    public string Title { get; set; } = string.Empty;
    public string? TaxNumber { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public class UpdateCustomerRequest : CreateCustomerRequest { }

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
