using AMI_Project.Data;
using AMI_Project.DTOs.Meter;
using AMI_Project.Helpers;
using AMI_Project.Models;
using AMI_Project.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Http;


namespace AMI_Project.Services
{
    public class MeterService : IMeterService
    {
        private readonly AMIDbContext _context;

        public MeterService(AMIDbContext context)
        {
            _context = context;
        }

        // ✅ GET all meters with optional filters
        public async Task<PagedResult<MeterReadDto>> GetMetersAsync(MeterFilterDto filter, CancellationToken ct)
        {
            var query = _context.Meters
                .Include(m => m.Consumer)
                .AsQueryable();

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

        // ✅ GET by serial number
        public async Task<MeterReadDto?> GetBySerialAsync(string serialNo, CancellationToken ct)
        {
            var meter = await _context.Meters
                .Include(m => m.Consumer)
                .FirstOrDefaultAsync(m => m.MeterSerialNo == serialNo, ct);

            if (meter == null)
                return null;

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

        // ✅ CREATE
        public async Task<MeterReadDto> CreateAsync(MeterCreateDto dto, CancellationToken ct)
        {
            var entity = new Meter
            {
                MeterSerialNo = dto.MeterSerialNo,
                IpAddress = dto.IpAddress,
                Iccid = dto.Iccid,
                Imsi = dto.Imsi,
                Manufacturer = dto.Manufacturer,
                Firmware = dto.Firmware,
                Category = dto.Category,
                ConsumerId = dto.ConsumerId,
                InstallTsUtc = DateTime.UtcNow,
                Status = "Active"
            };

            _context.Meters.Add(entity);
            await _context.SaveChangesAsync(ct);

            return await GetBySerialAsync(entity.MeterSerialNo, ct)
                   ?? throw new Exception("Error creating meter");
        }

        // ✅ UPDATE
        public async Task<MeterReadDto> UpdateAsync(string serialNo, MeterUpdateDto dto, CancellationToken ct)
        {
            var entity = await _context.Meters.FirstOrDefaultAsync(m => m.MeterSerialNo == serialNo, ct);
            if (entity == null)
                throw new KeyNotFoundException("Meter not found");

            if (!string.IsNullOrEmpty(dto.Firmware))
                entity.Firmware = dto.Firmware;

            if (!string.IsNullOrEmpty(dto.Status))
                entity.Status = dto.Status;

            await _context.SaveChangesAsync(ct);
            return await GetBySerialAsync(serialNo, ct)
                ?? throw new Exception("Error updating meter");
        }

        // ✅ DELETE
        public async Task DeleteAsync(string serialNo, CancellationToken ct)
        {
            var entity = await _context.Meters.FirstOrDefaultAsync(m => m.MeterSerialNo == serialNo, ct);
            if (entity == null) return;

            _context.Meters.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }

        // ✅ NEW: Upload CSV for bulk add/update
        public async Task<object> UploadCsvAsync(IFormFile file, CancellationToken ct)
        {
            using var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            var lines = new List<string>();

            while (!stream.EndOfStream)
            {
                var line = await stream.ReadLineAsync();
                if (!string.IsNullOrWhiteSpace(line))
                    lines.Add(line.Trim());
            }

            if (lines.Count <= 1)
                throw new Exception("CSV file must contain a header and at least one record.");

            var headers = lines[0].Split(',').Select(h => h.Trim().ToLower()).ToArray();

            var requiredHeaders = new[] { "meterserialno", "ipaddress", "iccid", "imsi", "manufacturer", "category" };
            foreach (var header in requiredHeaders)
            {
                if (!headers.Contains(header))
                    throw new Exception($"Missing required header: {header}");
            }

            var added = 0;
            var updated = 0;

            for (int i = 1; i < lines.Count; i++)
            {
                var cols = lines[i].Split(',').Select(c => c.Trim()).ToArray();
                if (cols.Length < headers.Length) continue;

                var dict = headers.Zip(cols, (h, c) => new { h, c }).ToDictionary(x => x.h, x => x.c);

                var serialNo = dict["meterserialno"];
                if (string.IsNullOrEmpty(serialNo)) continue;

                var existing = await _context.Meters.FirstOrDefaultAsync(m => m.MeterSerialNo == serialNo, ct);

                if (existing == null)
                {
                    var newMeter = new Meter
                    {
                        MeterSerialNo = serialNo,
                        IpAddress = dict["ipaddress"],
                        Iccid = dict["iccid"],
                        Imsi = dict["imsi"],
                        Manufacturer = dict["manufacturer"],
                        Firmware = dict.ContainsKey("firmware") ? dict["firmware"] : null,
                        Category = dict["category"],
                        InstallTsUtc = DateTime.UtcNow,
                        Status = "Active"
                    };
                    _context.Meters.Add(newMeter);
                    added++;
                }
                else
                {
                    existing.IpAddress = dict["ipaddress"];
                    existing.Iccid = dict["iccid"];
                    existing.Imsi = dict["imsi"];
                    existing.Manufacturer = dict["manufacturer"];
                    existing.Category = dict["category"];
                    if (dict.ContainsKey("firmware"))
                        existing.Firmware = dict["firmware"];
                    updated++;
                }
            }

            await _context.SaveChangesAsync(ct);

            return new { message = "CSV processed successfully", added, updated };
        }
    }
}
