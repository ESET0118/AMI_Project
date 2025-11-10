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
                    long? consumerId = null;

                    if (record.ConsumerId != null)
                    {
                        if (validConsumerIds.Contains(record.ConsumerId.Value))
                        {
                            consumerId = record.ConsumerId;
                        }
                        else
                        {
                            warnings.Add($"Meter '{record.MeterSerialNo}': ConsumerId {record.ConsumerId} does not exist. Setting ConsumerId = null.");
                        }
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
                        ConsumerId = consumerId
                    };

                    meters.Add(meter);
                }
            }

            // Save to database
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
