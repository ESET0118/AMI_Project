using AMI_Project.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AMI_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillController : ControllerBase
    {
        private readonly IBillingService _billingService;

        public BillController(IBillingService billingService)
        {
            _billingService = billingService;
        }

        [HttpGet("{meterSerialNo}")]
        public IActionResult GetBill(string meterSerialNo)
        {
            var billAmount = _billingService.GenerateBill(meterSerialNo);

            if (billAmount == null)
                return NotFound("Meter, Tariff, or Slabs not found.");

            return Ok(new
            {
                MeterSerialNo = meterSerialNo,
                BillAmount = billAmount
            });
        }
    }
}
