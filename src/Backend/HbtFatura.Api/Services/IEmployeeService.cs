using HbtFatura.Api.DTOs.Employees;

namespace HbtFatura.Api.Services;

public interface IEmployeeService
{
    Task<IReadOnlyList<EmployeeListDto>> GetByFirmAsync(CancellationToken ct = default);
    Task<EmployeeListDto> CreateAsync(CreateEmployeeRequest request, CancellationToken ct = default);
}
