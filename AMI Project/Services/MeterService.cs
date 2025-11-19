using AMI_Project.Data;
using AMI_Project.DTOs.Meter;
using AMI_Project.Helpers;
using AMI_Project.Data.Models;
using AMI_Project.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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
                    ICCID = m.ICCID,
                    IMSI = m.IMSI,
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
                    ICCID = m.ICCID,
                    IMSI = m.IMSI,
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
                ICCID = meter.ICCID,
                IMSI = meter.IMSI,
                Manufacturer = meter.Manufacturer,
                Firmware = meter.Firmware,
                Category = meter.Category,
                InstallTsUtc = meter.InstallTsUtc,
                Status = meter.Status,
                ConsumerName = meter.Consumer?.Name
            };
        }
        public async Task<IEnumerable<MeterReadDto>> GetByConsumerNameAsync(string consumerName, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(consumerName))
                return Enumerable.Empty<MeterReadDto>();

            return await _context.Meters
                .Include(m => m.Consumer)
                .Where(m => m.Consumer != null && m.Consumer.Name.Contains(consumerName))
                .Select(m => new MeterReadDto
                {
                    MeterSerialNo = m.MeterSerialNo,
                    ConsumerName = m.Consumer!.Name,
                    IpAddress = m.IpAddress,
                    Manufacturer = m.Manufacturer,
                    Firmware = m.Firmware,
                    Category = m.Category,
                    Status = m.Status,
                    InstallTsUtc = m.InstallTsUtc
                })
                .ToListAsync(ct);
        }


        // Example in your MeterService.cs
        public async Task CreateAsync(MeterCreateDto dto, CancellationToken ct)
        {
            // Find consumer by name (case-insensitive)
            var consumer = await _context.Consumers
                .FirstOrDefaultAsync(c => c.Name.ToLower() == dto.ConsumerName.Trim().ToLower(), ct);

            if (consumer == null)
                throw new ApplicationException("Consumer does not exist.");

            if (string.IsNullOrWhiteSpace(dto.MeterSerialNo))
                throw new ApplicationException("Meter Serial Number is required.");
            var meter = new Meter
            {
                MeterSerialNo = dto.MeterSerialNo,
                IpAddress = dto.IpAddress,
                ICCID = dto.ICCID,
                IMSI = dto.IMSI,
                Manufacturer = dto.Manufacturer,
                Firmware = dto.Firmware,
                Category = dto.Category,
                ConsumerId = consumer.ConsumerId, // use found consumer's ID
                Status = "Active",
                InstallTsUtc = DateTime.UtcNow
            };



            _context.Meters.Add(meter);

            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                {
                    if (sqlEx.Message.Contains("PK__Meter__") || sqlEx.Message.Contains("MeterSerialNo"))
                        throw new ApplicationException("Serial number already exists.");
                    if (sqlEx.Message.Contains("IX_Meter_IpAddress") || sqlEx.Message.Contains("IpAddress"))
                        throw new ApplicationException("IP Address must be unique.");
                }
                throw;
            }
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