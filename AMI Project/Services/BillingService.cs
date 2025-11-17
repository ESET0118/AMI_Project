using AMI_Project.Data;
using AMI_Project.Models;
using AMI_Project.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Services
{
    public class BillingService : IBillingService
    {
        private readonly AMIDbContext _context;

        public BillingService(AMIDbContext context)
        {
            _context = context;
        }

        // ----------------------
        // Generate bill (DTO only)
        // ----------------------
        public BillDto? GenerateBill(string meterSerialNo)
        {
            var meter = _context.Meters
                .Include(m => m.MeterReadings)
                .Include(m => m.Consumer)
                    .ThenInclude(c => c!.Tariff)
                        .ThenInclude(t => t.TariffSlabs)
                .FirstOrDefault(m => m.MeterSerialNo == meterSerialNo);

            if (meter == null || meter.Consumer?.Tariff == null)
                return null;

            var tariff = meter.Consumer.Tariff;
            var slabs = tariff.TariffSlabs.OrderBy(s => s.FromKwh).ToList();

            if (!slabs.Any())
                return null;

            decimal totalConsumption = meter.MeterReadings.Sum(r => r.ConsumptionKwh);
            decimal remaining = totalConsumption;
            decimal billAmount = 0;
            var slabsApplied = new List<string>();

            foreach (var slab in slabs)
            {
                if (remaining <= 0) break;

                decimal slabRange = slab.ToKwh - slab.FromKwh;
                if (slabRange <= 0) continue;

                decimal units = Math.Min(remaining, slabRange);
                billAmount += units * slab.RatePerKwh;
                slabsApplied.Add($"{units} kWh @ {slab.RatePerKwh}/kWh");
                remaining -= units;
            }

            billAmount += tariff.BaseRate;
            billAmount += billAmount * tariff.TaxRate / 100;

            return new BillDto
            {
                MeterSerialNo = meterSerialNo,
                BillAmount = Math.Round(billAmount, 2),
                TotalUnits = totalConsumption,
                BaseRate = tariff.BaseRate,
                TaxRate = tariff.TaxRate,
                SlabsApplied = string.Join(", ", slabsApplied),
                QrCodeUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=120x120&data=Bill-{meterSerialNo}"
            };
        }

        // ----------------------
        // Save bill to database
        // ----------------------
        public Bill SaveBill(string meterSerialNo, decimal amount, long consumerId)
        {
            var bill = new Bill
            {
                MeterSerialNo = meterSerialNo,
                ConsumerId = consumerId,
                TotalAmount = amount,
                BillGeneratedAt = DateTime.UtcNow
            };

            _context.Bills.Add(bill);
            _context.SaveChanges();

            return bill;
        }

        // ----------------------
        // Get all bills
        // ----------------------
        public List<Bill> GetAllBills()
        {
            return _context.Bills
                .OrderByDescending(b => b.BillGeneratedAt)
                .ToList();
        }

        // ----------------------
        // Get bills for meter
        // ----------------------
        public List<Bill> GetBillsByMeter(string meterSerialNo)
        {
            return _context.Bills
                .Where(b => b.MeterSerialNo == meterSerialNo)
                .OrderByDescending(b => b.BillGeneratedAt)
                .ToList();
        }

        // ----------------------
        // Get bills for consumer
        // ----------------------
        public List<Bill> GetBillsByConsumer(long consumerId)
        {
            return _context.Bills
                .Where(b => b.ConsumerId == consumerId)
                .OrderByDescending(b => b.BillGeneratedAt)
                .ToList();
        }
    }

    public class BillDto
    {
        public string MeterSerialNo { get; set; } = "";
        public decimal BillAmount { get; set; }
        public decimal TotalUnits { get; set; }
        public decimal BaseRate { get; set; }
        public decimal TaxRate { get; set; }
        public string SlabsApplied { get; set; } = "";
        public string? QrCodeUrl { get; set; }
    }
}
