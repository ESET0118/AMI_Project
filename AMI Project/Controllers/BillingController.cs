using AMI_Project.DTOs.Billing;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BillReadDto>>> GetAll()
        {
            var bills = await _billingService.GetAllBillsAsync();
            return Ok(bills);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<BillReadDto>> GetById(long id)
        {
            var bill = await _billingService.GetBillByIdAsync(id);
            if (bill == null) return NotFound();
            return Ok(bill);
        }

        [HttpPost]
        public async Task<ActionResult<BillReadDto>> Create([FromBody] BillCreateDto dto)
        {
            var created = await _billingService.CreateBillAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.BillId }, created);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _billingService.DeleteBillAsync(id);
            return NoContent();
        }
    }
}
