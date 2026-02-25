using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.DTOs.Product;

namespace HbtFatura.Api.Services;

public interface IProductService
{
    Task<PagedResult<ProductListDto>> GetPagedAsync(int page, int pageSize, string? search, Guid? firmId, CancellationToken ct = default);
    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken ct = default);
    Task<ProductDto?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<StockMovementDto>> GetMovementsAsync(Guid productId, int page, int pageSize, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default);
    Task<StockMovementDto> AddMovementAsync(Guid productId, CreateStockMovementRequest request, CancellationToken ct = default);
    Task<List<ProductDto>> GetListForDropdownAsync(Guid? firmId, CancellationToken ct = default);
}
