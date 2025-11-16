using AMI_Project.Data.Models;
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
            return reading == null ? null : _mapper.Map<MeterReadingReadDto>(reading);
        }

        public async Task<IEnumerable<MeterReadingReadDto>> GetByMeterSerialNoAsync(string serialNo, CancellationToken ct)
        {
            var readings = await _repository.GetByMeterSerialNoAsync(serialNo, ct);
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
            return _mapper.Map<MeterReadingReadDto>(updated);
        }

        public async Task DeleteAsync(long id, CancellationToken ct)
        {
            await _repository.DeleteAsync(id, ct);
        }
        // New: get calendar for month
        public async Task<MeterReadingCalendarDto> GetCalendarForMonthAsync(string serialNo, int year, int month, CancellationToken ct)
        {
            // get existing daily readings for month
            var existing = (await _repository.GetByMeterSerialNoForMonthAsync(serialNo, year, month, ct)).ToList();

            // build calendar days
            var start = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var days = new List<DailyMeterReadingDto>(daysInMonth);

            for (int d = 1; d <= daysInMonth; d++)
            {
                var date = start.AddDays(d - 1);
                var found = existing.FirstOrDefault(r => r.ReadingDateTime.Date == date.Date);
                days.Add(new DailyMeterReadingDto
                {
                    Date = date,
                    MeterReadingId = found?.MeterReadingId,
                    ConsumptionKwh = found?.ConsumptionKwh,
                    Voltage = found?.Voltage,
                    Ampere = found?.Ampere,
                    PowerFactor = found?.PowerFactor,
                    Frequency = found?.Frequency
                });
            }

            // compute monthly total from existing (sum of consumptions)
            var monthlyTotal = existing.Sum(r => r.ConsumptionKwh);

            // try fetch monthly record
            var monthly = await _repository.GetMonthlyAsync(serialNo, year, month, ct);

            return new MeterReadingCalendarDto
            {
                MeterSerialNo = serialNo,
                Year = year,
                Month = month,
                Days = days,
                MonthlyTotalKwh = monthly?.TotalConsumptionKwh ?? monthlyTotal,
                HasMonthlyRecord = monthly != null
            };
        }

        // Create or update a bulk of daily readings; returns created/updated read dtos
        public async Task<IEnumerable<MeterReadingReadDto>> CreateBulkAsync(BulkMeterReadingCreateDto dto, CancellationToken ct)
        {
            // validate month range (defensive)
            var start = new DateTime(dto.Year, dto.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = start.AddMonths(1);

            // get existing for the month
            var existing = (await _repository.GetByMeterSerialNoForMonthAsync(dto.MeterSerialNo, dto.Year, dto.Month, ct)).ToList();

            // prepare added/updated entities
            var toAdd = new List<MeterReading>();
            var toUpdate = new List<MeterReading>();

            foreach (var r in dto.Readings)
            {
                // normalize date to UTC date part
                var dateUtc = new DateTime(r.Date.Year, r.Date.Month, r.Date.Day, 0, 0, 0, DateTimeKind.Utc);

                var exist = existing.FirstOrDefault(e => e.ReadingDateTime.Date == dateUtc.Date);

                if (exist == null)
                {
                    var entity = new MeterReading
                    {
                        MeterSerialNo = dto.MeterSerialNo,
                        ReadingDateTime = dateUtc,
                        ConsumptionKwh = r.ConsumptionKwh,
                        Voltage = r.Voltage,
                        Ampere = r.Ampere,
                        PowerFactor = r.PowerFactor,
                        Frequency = r.Frequency,
                        CreatedAt = DateTime.UtcNow
                    };
                    toAdd.Add(entity);
                }
                else
                {
                    // update fields
                    exist.ConsumptionKwh = r.ConsumptionKwh;
                    exist.Voltage = r.Voltage;
                    exist.Ampere = r.Ampere;
                    exist.PowerFactor = r.PowerFactor;
                    exist.Frequency = r.Frequency;
                    toUpdate.Add(exist);
                }
            }

            var created = new List<MeterReading>();
            if (toAdd.Count > 0)
            {
                var added = await _repository.AddRangeAsync(toAdd, ct);
                created.AddRange(added);
            }

            if (toUpdate.Count > 0)
            {
                var updated = await _repository.UpdateRangeAsync(toUpdate, ct);
                created.AddRange(updated);
            }

            // compute new monthly total (sum of all readings for month)
            var allReadingsForMonth = (await _repository.GetByMeterSerialNoForMonthAsync(dto.MeterSerialNo, dto.Year, dto.Month, ct)).ToList();
            var monthlyTotal = allReadingsForMonth.Sum(x => x.ConsumptionKwh);

            // upsert monthly record
            var monthlyEntity = new MonthlyMeterReading
            {
                MeterSerialNo = dto.MeterSerialNo,
                Year = dto.Year,
                Month = dto.Month,
                TotalConsumptionKwh = monthlyTotal
            };
            await _repository.UpsertMonthlyAsync(monthlyEntity, ct);

            return _mapper.Map<IEnumerable<MeterReadingReadDto>>(created);
        }

        public async Task<MonthlyMeterReadingDto> GetMonthlyAsync(string serialNo, int year, int month, CancellationToken ct)
        {
            var monthly = await _repository.GetMonthlyAsync(serialNo, year, month, ct);
            if (monthly == null)
            {
                // if no monthly entity exists, compute total from daily
                var daily = await _repository.GetByMeterSerialNoForMonthAsync(serialNo, year, month, ct);
                var total = daily.Sum(d => d.ConsumptionKwh);
                return new MonthlyMeterReadingDto
                {
                    MonthlyMeterReadingId = 0,
                    MeterSerialNo = serialNo,
                    Year = year,
                    Month = month,
                    TotalConsumptionKwh = total
                };
            }

            return new MonthlyMeterReadingDto
            {
                MonthlyMeterReadingId = monthly.MonthlyMeterReadingId,
                MeterSerialNo = monthly.MeterSerialNo,
                Year = monthly.Year,
                Month = monthly.Month,
                TotalConsumptionKwh = monthly.TotalConsumptionKwh
            };
        }
    }
}
