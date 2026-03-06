using HbtFatura.Api.DTOs.Employees;

namespace HbtFatura.Api.Services;

public interface IEmployeeService
{
    Task<IReadOnlyList<EmployeeListDto>> GetByFirmAsync(CancellationToken ct = default);
    Task<EmployeeListDto> CreateAsync(CreateEmployeeRequest request, CancellationToken ct = default);
    Task<EmployeeListDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<EmployeeListDto> UpdateAsync(Guid id, UpdateEmployeeRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
