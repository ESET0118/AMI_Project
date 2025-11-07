using AMI_Project.DTOs.OrgUnits;

namespace AMI_Project.Services.Interfaces
{
    public interface IOrgUnitService
    {
        Task<IEnumerable<OrgUnitReadDto>> GetAllAsync(CancellationToken ct);
        Task<OrgUnitReadDto?> GetByIdAsync(int id, CancellationToken ct);
        Task<OrgUnitReadDto> CreateAsync(OrgUnitCreateDto dto, CancellationToken ct);
        Task<OrgUnitReadDto?> UpdateAsync(int id, OrgUnitUpdateDto dto, CancellationToken ct);
        Task DeleteAsync(int id, CancellationToken ct);
    }
}
