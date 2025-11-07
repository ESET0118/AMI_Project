using AMI_Project.DTOs.MeterReadings;
using AMI_Project.Models;
using AMI_Project.Repositories.Interfaces;
using AMI_Project.Services.Interfaces;
using AutoMapper;

namespace AMI_Project.Services
{
    public class MeterReadingService : IMeterReadingService
    {
        private readonly IMeterReadingRepository _repository;
        private readonly IMapper _mapper;

        public MeterReadingService(IMeterReadingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MeterReadingReadDto>> GetAllAsync(CancellationToken ct)
        {
            var readings = await _repository.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<MeterReadingReadDto>>(readings);
        }

        public async Task<MeterReadingReadDto?> GetByIdAsync(long id, CancellationToken ct)
        {
            var reading = await _repository.GetByIdAsync(id, ct);
            return _mapper.Map<MeterReadingReadDto?>(reading);
        }

        public async Task<IEnumerable<MeterReadingReadDto>> GetByMeterSerialNoAsync(string meterSerialNo, CancellationToken ct)
        {
            var readings = await _repository.GetByMeterSerialNoAsync(meterSerialNo, ct);
            return _mapper.Map<IEnumerable<MeterReadingReadDto>>(readings);
        }

        public async Task<MeterReadingReadDto> CreateAsync(MeterReadingCreateDto dto, CancellationToken ct)
        {
            var entity = _mapper.Map<MeterReading>(dto);
            entity.CreatedAt = DateTime.UtcNow;

            var created = await _repository.AddAsync(entity, ct);
            return _mapper.Map<MeterReadingReadDto>(created);
        }

        public async Task<MeterReadingReadDto?> UpdateAsync(long id, MeterReadingUpdateDto dto, CancellationToken ct)
        {
            var existing = await _repository.GetByIdAsync(id, ct);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            var updated = await _repository.UpdateAsync(existing, ct);
            return _mapper.Map<MeterReadingReadDto?>(updated);
        }

        public async Task DeleteAsync(long id, CancellationToken ct)
        {
            await _repository.DeleteAsync(id, ct);
        }
    }
}
