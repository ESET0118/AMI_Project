using AMI_Project.DTOs.Consumers;
using AMI_Project.Models;
using AMI_Project.Repositories.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Services
{
    public class ConsumerService : IConsumerService
    {
        private readonly IConsumerRepository _repo;
        private readonly IMapper _mapper;

        public ConsumerService(IConsumerRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // GET ALL -> returns DTO list
        public async Task<IEnumerable<ConsumerReadDto>> GetAllAsync(CancellationToken ct)
        {
            var consumers = await _repo.GetAllAsync(ct);

            var list = consumers.Select(c =>
            {
                // safe extraction of meter count
                var meterCount = c.Meters?.Count ?? 0;

                return new ConsumerReadDto
                {
                    ConsumerId = c.ConsumerId,
                    Name = c.Name,
                    Address = c.Address,
                    Phone = c.Phone,
                    Email = c.Email,
                    Status = c.Status,
                    Lat = c.Lat,
                    Lon = c.Lon,
                    CreatedAt = c.CreatedAt,
                    MeterCount = meterCount,
                    OrgUnitName = c.OrgUnit?.Name,
                    TariffName = c.Tariff?.Name
                };
            });

            return list;
        }

        // GET BY ID
        public async Task<ConsumerReadDto?> GetByIdAsync(long id, CancellationToken ct)
        {
            var c = await _repo.GetByIdAsync(id, ct);
            if (c == null) return null;

            var meterCount = c.Meters?.Count ?? 0;

            return new ConsumerReadDto
            {
                ConsumerId = c.ConsumerId,
                Name = c.Name,
                Address = c.Address,
                Phone = c.Phone,
                Email = c.Email,
                Status = c.Status,
                Lat = c.Lat,
                Lon = c.Lon,
                CreatedAt = c.CreatedAt,
                MeterCount = meterCount,
                OrgUnitName = c.OrgUnit?.Name,
                TariffName = c.Tariff?.Name
            };
        }

        // CREATE
        public async Task<ConsumerReadDto> CreateAsync(ConsumerCreateDto dto, string createdBy, CancellationToken ct)
        {
            var entity = new Consumer
            {
                Name = dto.Name,
                Address = dto.Address,
                Phone = dto.Phone,
                Email = dto.Email,
                OrgUnitId = dto.OrgUnitId,
                TariffId = dto.TariffId,
                Lat = dto.Lat,
                Lon = dto.Lon,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow,
                Status = "Active"
            };

            var created = await _repo.AddAsync(entity, ct);

            return new ConsumerReadDto
            {
                ConsumerId = created.ConsumerId,
                Name = created.Name,
                Address = created.Address,
                Phone = created.Phone,
                Email = created.Email,
                Status = created.Status,
                Lat = created.Lat,
                Lon = created.Lon,
                CreatedAt = created.CreatedAt,
                MeterCount = created.Meters?.Count ?? 0,
                OrgUnitName = created.OrgUnit?.Name,
                TariffName = created.Tariff?.Name
            };
        }

        // UPDATE
        public async Task<ConsumerReadDto?> UpdateAsync(long id, ConsumerUpdateDto dto, string updatedBy, CancellationToken ct)
        {
            var existing = await _repo.GetByIdAsync(id, ct);
            if (existing == null) return null;

            // Only update fields provided in DTO (null means no change)
            if (!string.IsNullOrWhiteSpace(dto.Address)) existing.Address = dto.Address;
            if (!string.IsNullOrWhiteSpace(dto.Phone)) existing.Phone = dto.Phone;
            if (!string.IsNullOrWhiteSpace(dto.Email)) existing.Email = dto.Email;
            if (!string.IsNullOrWhiteSpace(dto.Status)) existing.Status = dto.Status;
            if (dto.Lat.HasValue) existing.Lat = dto.Lat;
            if (dto.Lon.HasValue) existing.Lon = dto.Lon;
            if (dto.OrgUnitId.HasValue) existing.OrgUnitId = dto.OrgUnitId.Value;
            if (dto.TariffId.HasValue) existing.TariffId = dto.TariffId.Value;

            existing.UpdatedBy = updatedBy;
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await _repo.UpdateAsync(existing, ct);
            if (updated == null) return null;

            return new ConsumerReadDto
            {
                ConsumerId = updated.ConsumerId,
                Name = updated.Name,
                Address = updated.Address,
                Phone = updated.Phone,
                Email = updated.Email,
                Status = updated.Status,
                Lat = updated.Lat,
                Lon = updated.Lon,
                CreatedAt = updated.CreatedAt,
                MeterCount = updated.Meters?.Count ?? 0,
                OrgUnitName = updated.OrgUnit?.Name,
                TariffName = updated.Tariff?.Name
            };
        }

        // DELETE
        public async Task DeleteAsync(long id, CancellationToken ct)
        {
            await _repo.DeleteAsync(id, ct);
        }
    }
}
