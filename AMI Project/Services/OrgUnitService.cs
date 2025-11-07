using AMI_Project.DTOs.OrgUnits;
using AMI_Project.Models;
using AMI_Project.Repositories.Interfaces;
using AMI_Project.Services.Interfaces;
using AutoMapper;

namespace AMI_Project.Services
{
    public class OrgUnitService : IOrgUnitService
    {
        private readonly IOrgUnitRepository _repo;
        private readonly IMapper _mapper;

        public OrgUnitService(IOrgUnitRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrgUnitReadDto>> GetAllAsync(CancellationToken ct)
        {
            var orgUnits = await _repo.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<OrgUnitReadDto>>(orgUnits);
        }

        public async Task<OrgUnitReadDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            var orgUnit = await _repo.GetByIdAsync(id, ct);
            return orgUnit == null ? null : _mapper.Map<OrgUnitReadDto>(orgUnit);
        }

        public async Task<OrgUnitReadDto> CreateAsync(OrgUnitCreateDto dto, CancellationToken ct)
        {
            var orgUnit = _mapper.Map<OrgUnit>(dto);
            var created = await _repo.AddAsync(orgUnit, ct);
            return _mapper.Map<OrgUnitReadDto>(created);
        }

        public async Task<OrgUnitReadDto?> UpdateAsync(int id, OrgUnitUpdateDto dto, CancellationToken ct)
        {
            var existing = await _repo.GetByIdAsync(id, ct);
            if (existing == null) return null;

            _mapper.Map(dto, existing); // map only provided fields

            var updated = await _repo.UpdateAsync(existing, ct);
            return updated == null ? null : _mapper.Map<OrgUnitReadDto>(updated);
        }

        public async Task DeleteAsync(int id, CancellationToken ct)
        {
            await _repo.DeleteAsync(id, ct);
        }
    }
}
