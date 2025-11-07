using AMI_Project.DTOs.Consumers;
using AMI_Project.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace AMI_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsumerController : ControllerBase
    {
        private readonly IConsumerService _service;

        public ConsumerController(IConsumerService service)
        {
            _service = service;
        }

        // -------------------------
        // GET: api/consumer
        // -------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var result = await _service.GetAllAsync(ct);
            return Ok(result);
        }

        // -------------------------
        // GET: api/consumer/{id}
        // -------------------------
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id, CancellationToken ct = default)
        {
            var result = await _service.GetByIdAsync(id, ct);
            if (result == null)
                return NotFound(new { message = $"Consumer with ID {id} not found." });

            return Ok(result);
        }

        // -------------------------
        // POST: api/consumer
        // -------------------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ConsumerCreateDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string createdBy = "System"; // TODO: Replace with user context later
            var result = await _service.CreateAsync(dto, createdBy, ct);

            return CreatedAtAction(nameof(GetById), new { id = result.ConsumerId }, result);
        }

        // -------------------------
        // PUT: api/consumer/{id}
        // -------------------------
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] ConsumerUpdateDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string updatedBy = "System"; // TODO: Replace with user context later
            var result = await _service.UpdateAsync(id, dto, updatedBy, ct);

            if (result == null)
                return NotFound(new { message = $"Consumer with ID {id} not found." });

            return Ok(result);
        }

        // -------------------------
        // DELETE: api/consumer/{id}
        // -------------------------
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id, CancellationToken ct = default)
        {
            await _service.DeleteAsync(id, ct);
            return NoContent();
        }
    }
}
