using AMI_Project.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AMI_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillingController : ControllerBase
    {
        private readonly IBillingService _billingService;

        public BillingController(IBillingService billingService)
        {
            _billingService = billingService;
        }

        // GET: api/Billing
        [HttpGet]
        public IActionResult GetAllBills()
        {
            var bills = _billingService.GetAllBills();
            return Ok(bills);
        }

        // POST: api/Billing/calculate?meterId=123&fromDate=2024-01-01&toDate=2024-01-31
        [HttpPost("calculate")]
        public IActionResult Calculate(string meterId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var bill = _billingService.GenerateBill(meterId);

                if (bill == null)
                    return BadRequest("Bill generation returned null. Meter, tariff, slabs, or readings missing.");

                return Ok(bill);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Billing Error: " + ex.Message);
                return BadRequest("Exception: " + ex.Message);
            }
        }



        // GET: api/Billing/{meterSerial}
        [HttpGet("{meterSerialNo}")]
        public IActionResult GetBillByMeter(string meterSerialNo)
        {
            var bill = _billingService.GenerateBill(meterSerialNo);

            if (bill == null)
                return NotFound(new { message = "Meter, Tariff, or Slabs not found." });

            return Ok(bill);
        }
    }
}
