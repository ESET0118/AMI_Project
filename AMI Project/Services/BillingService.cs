using AMI_Project.Data;
using AMI_Project.Models;
using AMI_Project.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AMI_Project.Services
{
    public class BillingService : IBillingService
    {
        private readonly AMIDbContext _context;

        public BillingService(AMIDbContext context)
        {
            _context = context;
        }

        public decimal? GenerateBill(string meterSerialNo)
        {
            // 1️⃣ Fetch meter with readings and related consumer + tariff + slabs

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
                return null; // no slabs defined

            // 2️⃣ Calculate total consumption from all readings
            decimal totalConsumption = meter.MeterReadings.Sum(r => r.ConsumptionKwh);

            // 3️⃣ Calculate bill using slabs
            decimal remainingConsumption = totalConsumption;
            decimal billAmount = 0;

            foreach (var slab in slabs)
            {
                if (remainingConsumption <= 0) break;

                decimal slabStart = slab.FromKwh;
                decimal slabEnd = slab.ToKwh;

                decimal slabRange = slabEnd - slabStart;
                if (slabRange <= 0) continue; // skip invalid slab

                // Units applicable in this slab
                decimal unitsInSlab = Math.Min(remainingConsumption, slabRange);
                billAmount += unitsInSlab * slab.RatePerKwh;

                remainingConsumption -= unitsInSlab;
            }

            // 4️⃣ Add base rate and tax
            billAmount += tariff.BaseRate;
            billAmount += billAmount * tariff.TaxRate / 100;

            return Math.Round(billAmount, 2);
        }
    }
}
