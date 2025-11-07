using AMI_Project.Models;
using AMI_Project.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AMI_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TariffSlabController : ControllerBase
    {
        private readonly ITariffSlabRepository _repository;

        public TariffSlabController(ITariffSlabRepository repository)
        {
            _repository = repository;
        }

        // ✅ GET: api/TariffSlab
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var slabs = await _repository.GetAllAsync(ct);
            return Ok(slabs);
        }

        // ✅ GET: api/TariffSlab/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var slab = await _repository.GetByIdAsync(id, ct);
            if (slab == null)
                return NotFound($"Tariff slab with ID {id} not found.");

            return Ok(slab);
        }

        // ✅ POST: api/TariffSlab
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TariffSlab slab, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _repository.AddAsync(slab, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.TariffSlabId }, created);
        }

        // ✅ PUT: api/TariffSlab/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TariffSlab slab, CancellationToken ct)
        {
            if (id != slab.TariffSlabId)
                return BadRequest("ID in URL does not match ID in body.");

            var updated = await _repository.UpdateAsync(slab, ct);
            if (updated == null)
                return NotFound($"Tariff slab with ID {id} not found.");

            return Ok(updated);
        }

        // ✅ DELETE: api/TariffSlab/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var existing = await _repository.GetByIdAsync(id, ct);
            if (existing == null)
                return NotFound($"Tariff slab with ID {id} not found.");

            await _repository.DeleteAsync(id, ct);
            return NoContent();
        }
    }
}
