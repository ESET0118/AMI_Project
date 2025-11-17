using AMI_Project.DTOs.Meters;
using AMI_Project.Models;
using AMI_Project.Repositories.Interfaces;
using AMI_Project.Services.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace AMI_Project.Services
{
    public class MeterCsvService : IMeterCsvService
    {
        private readonly IMeterRepository _meterRepository;
        private readonly IConsumerRepository _consumerRepository;

        public MeterCsvService(IMeterRepository meterRepository, IConsumerRepository consumerRepository)
        {
            _meterRepository = meterRepository;
            _consumerRepository = consumerRepository;
        }

        public async Task<MeterCsvUploadResult> UploadAndImportAsync(MeterUploadResultDto dto, CancellationToken ct)
        {
            if (dto.CsvFile == null || dto.CsvFile.Length == 0)
                throw new ArgumentException("Invalid CSV file.");

            var result = new MeterCsvUploadResult();
            var meters = new List<Meter>();
            var warnings = new List<string>();

            // Get all valid ConsumerIds from DB
            var validConsumerIds = (await _consumerRepository.GetAllAsync(ct))
                .Select(c => c.ConsumerId)
                .ToHashSet();

            using (var stream = new StreamReader(dto.CsvFile.OpenReadStream()))
            using (var csv = new CsvReader(stream, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null,
                HeaderValidated = null
            }))
            {
                var records = csv.GetRecordsAsync<MeterCsvRecord>();

                await foreach (var record in records.WithCancellation(ct))
                {
                    // Skip if MeterSerialNo is missing
                    if (string.IsNullOrWhiteSpace(record.MeterSerialNo))
                    {
                        warnings.Add($"Meter with IP '{record.IpAddress}' or ICCID '{record.Iccid}' could not be added: missing MeterSerialNo.");
                        continue;
                    }

                    // Skip if ConsumerId is missing or invalid
                    if (!record.ConsumerId.HasValue || !validConsumerIds.Contains(record.ConsumerId.Value))
                    {
                        warnings.Add($"Meter '{record.MeterSerialNo}' could not be added: ConsumerId is missing or does not exist.");
                        continue;
                    }

                    var meter = new Meter
                    {
                        MeterSerialNo = record.MeterSerialNo,
                        IpAddress = record.IpAddress,
                        Iccid = record.Iccid,
                        Imsi = record.Imsi,
                        Manufacturer = record.Manufacturer,
                        Firmware = record.Firmware,
                        Category = record.Category,
                        ConsumerId = record.ConsumerId
                    };

                    meters.Add(meter);
                }
            }

            // Save valid meters to database
            foreach (var m in meters)
                await _meterRepository.AddAsync(m, ct);

            await _meterRepository.SaveChangesAsync(ct);

            result.ImportedMeters = meters;
            result.Warnings = warnings;

            return result;
        }

        // CSV mapping class
        private class MeterCsvRecord
        {
            public string MeterSerialNo { get; set; } = string.Empty;
            public string IpAddress { get; set; } = string.Empty;
            public string Iccid { get; set; } = string.Empty;
            public string Imsi { get; set; } = string.Empty;
            public string Manufacturer { get; set; } = string.Empty;
            public string? Firmware { get; set; }
            public string Category { get; set; } = string.Empty;
            public long? ConsumerId { get; set; }
        }
    }
}