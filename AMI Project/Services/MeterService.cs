using AMI_Project.Data;
using AMI_Project.DTOs.Meter;
using AMI_Project.Models;
using AMI_Project.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using AMI_Project.Helpers;

namespace AMI_Project.Services
{
    public class MeterService : IMeterService
    {
        private readonly AMIDbContext _context;

        public MeterService(AMIDbContext context)
        {
            _context = context;
        }

        // Get all meters
        public async Task<PagedResult<MeterReadDto>> GetAllMetersAsync(CancellationToken ct)
        {
            var meters = await _context.Meters
                .Include(m => m.Consumer)
                .Select(m => new MeterReadDto
                {
                    MeterSerialNo = m.MeterSerialNo,
                    IpAddress = m.IpAddress,
                    Iccid = m.Iccid,
                    Imsi = m.Imsi,
                    Manufacturer = m.Manufacturer,
                    Firmware = m.Firmware,
                    Category = m.Category,
                    InstallTsUtc = m.InstallTsUtc,
                    Status = m.Status,
                    ConsumerName = m.Consumer != null ? m.Consumer.Name : null
                })
                .ToListAsync(ct);

            return new PagedResult<MeterReadDto>(meters, meters.Count, 1, meters.Count);
        }

        // Get filtered/paged meters
        public async Task<PagedResult<MeterReadDto>> GetMetersAsync(MeterFilterDto filter, CancellationToken ct)
        {
            var query = _context.Meters.Include(m => m.Consumer).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SerialNo))
                query = query.Where(m => m.MeterSerialNo.Contains(filter.SerialNo));
            if (!string.IsNullOrWhiteSpace(filter.Status))
                query = query.Where(m => m.Status == filter.Status);
            if (filter.ConsumerId.HasValue)
                query = query.Where(m => m.ConsumerId == filter.ConsumerId);
            if (filter.FromInstallDate.HasValue)
                query = query.Where(m => m.InstallTsUtc >= filter.FromInstallDate.Value);
            if (filter.ToInstallDate.HasValue)
                query = query.Where(m => m.InstallTsUtc <= filter.ToInstallDate.Value);

            int page = filter.Page <= 0 ? 1 : filter.Page;
            int pageSize = filter.PageSize <= 0 ? 20 : filter.PageSize;

            var total = await query.CountAsync(ct);
            var items = await query
                .OrderBy(m => m.MeterSerialNo)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MeterReadDto
                {
                    MeterSerialNo = m.MeterSerialNo,
                    IpAddress = m.IpAddress,
                    Iccid = m.Iccid,
                    Imsi = m.Imsi,
                    Manufacturer = m.Manufacturer,
                    Firmware = m.Firmware,
                    Category = m.Category,
                    InstallTsUtc = m.InstallTsUtc,
                    Status = m.Status,
                    ConsumerName = m.Consumer != null ? m.Consumer.Name : null
                })
                .ToListAsync(ct);

            return new PagedResult<MeterReadDto>(items, total, page, pageSize);
        }

        public async Task<MeterReadDto?> GetBySerialAsync(string serialNo, CancellationToken ct)
        {
            var meter = await _context.Meters
                .Include(m => m.Consumer)
                .FirstOrDefaultAsync(m => m.MeterSerialNo == serialNo, ct);

            if (meter == null) return null;

            return new MeterReadDto
            {
                MeterSerialNo = meter.MeterSerialNo,
                IpAddress = meter.IpAddress,
                Iccid = meter.Iccid,
                Imsi = meter.Imsi,
                Manufacturer = meter.Manufacturer,
                Firmware = meter.Firmware,
                Category = meter.Category,
                InstallTsUtc = meter.InstallTsUtc,
                Status = meter.Status,
                ConsumerName = meter.Consumer?.Name
            };
        }

        public async Task<MeterReadDto> CreateAsync(MeterCreateDto dto, CancellationToken ct)
        {
            // Find existing consumer by name
            var consumer = await _context.Consumers
                .FirstOrDefaultAsync(c => c.Name == dto.ConsumerName, ct);

            // If consumer doesn't exist, create new
            if (consumer == null)
            {
                consumer = new Consumer { Name = dto.ConsumerName };
                _context.Consumers.Add(consumer);
                await _context.SaveChangesAsync(ct);
            }

            // Create meter
            var meter = new Meter
            {
                MeterSerialNo = dto.MeterSerialNo,
                IpAddress = dto.IpAddress,
                Iccid = dto.Iccid,
                Imsi = dto.Imsi,
                Manufacturer = dto.Manufacturer,
                Firmware = dto.Firmware,
                Category = dto.Category,
                ConsumerId = consumer.ConsumerId, // associate meter to consumer
                InstallTsUtc = DateTime.UtcNow,
                Status = "Active"
            };

            _context.Meters.Add(meter);
            await _context.SaveChangesAsync(ct);

            return await GetBySerialAsync(meter.MeterSerialNo, ct)
                   ?? throw new Exception("Error creating meter");
        }


        public async Task<MeterReadDto> UpdateAsync(string serialNo, MeterUpdateDto dto, CancellationToken ct)
        {
            var meter = await _context.Meters.FirstOrDefaultAsync(m => m.MeterSerialNo == serialNo, ct);
            if (meter == null) throw new KeyNotFoundException("Meter not found");

            meter.Firmware = dto.Firmware ?? meter.Firmware;
            meter.Status = dto.Status ?? meter.Status;

            await _context.SaveChangesAsync(ct);

            return await GetBySerialAsync(serialNo, ct)
                   ?? throw new Exception("Error updating meter");
        }

        public async Task DeleteAsync(string serialNo, CancellationToken ct)
        {
            var meter = await _context.Meters.FirstOrDefaultAsync(m => m.MeterSerialNo == serialNo, ct);
            if (meter == null) return;

            _context.Meters.Remove(meter);
            await _context.SaveChangesAsync(ct);
        }
    }
}