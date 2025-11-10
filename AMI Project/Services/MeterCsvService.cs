using AMI_Project.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text;
using AMI_Project.Models;
public class MeterCsvService : IMeterCsvService
{
    private readonly AMIDbContext _context;

    public MeterCsvService(AMIDbContext context)
    {
        _context = context;
    }

    public async Task<MeterUploadResultDto> UploadCsvAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("CSV file is required");

        int added = 0, updated = 0;
        using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
        var lines = new List<string>();
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (!string.IsNullOrWhiteSpace(line))
                lines.Add(line.Trim());
        }

        if (lines.Count <= 1) throw new Exception("CSV must have header + data rows");

        var headers = lines[0].Split(',').Select(h => h.Trim().ToLower()).ToArray();
        for (int i = 1; i < lines.Count; i++)
        {
            var cols = lines[i].Split(',').Select(c => c.Trim()).ToArray();
            var dict = headers.Zip(cols, (h, c) => new { h, c }).ToDictionary(x => x.h, x => x.c);

            var serial = dict["meterserialno"];
            if (string.IsNullOrEmpty(serial)) continue;

            var existing = await _context.Meters.FirstOrDefaultAsync(m => m.MeterSerialNo == serial);
            if (existing == null)
            {
                _context.Meters.Add(new Meter
                {
                    MeterSerialNo = serial,
                    IpAddress = dict["ipaddress"],
                    Iccid = dict["iccid"],
                    Imsi = dict["imsi"],
                    Manufacturer = dict["manufacturer"],
                    Category = dict["category"],
                    Firmware = dict.ContainsKey("firmware") ? dict["firmware"] : null,
                    Status = "Active"
                });
                added++;
            }
            else
            {
                existing.IpAddress = dict["ipaddress"];
                existing.Iccid = dict["iccid"];
                existing.Imsi = dict["imsi"];
                existing.Manufacturer = dict["manufacturer"];
                existing.Category = dict["category"];
                if (dict.ContainsKey("firmware")) existing.Firmware = dict["firmware"];
                updated++;
            }
        }

        await _context.SaveChangesAsync();

        return new MeterUploadResultDto
        {
            Message = "CSV processed successfully",
            Added = added,
            Updated = updated
        };
    }
}
