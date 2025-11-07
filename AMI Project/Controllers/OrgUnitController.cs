using AMI_Project.DTOs.OrgUnits;
using AMI_Project.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AMI_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrgUnitController : ControllerBase
    {
        private readonly IOrgUnitService _service;

        public OrgUnitController(IOrgUnitService service)
        {
            _service = service;
        }

        // ------------------------------------------------------------
        // 🔹 GET: All Org Units (complete hierarchy)
        // ------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _service.GetAllAsync(ct);
            return Ok(result);
        }

        // ------------------------------------------------------------
        // 🔹 GET: Org Unit by ID
        // ------------------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await _service.GetByIdAsync(id, ct);
            return result == null ? NotFound() : Ok(result);
        }

        // ------------------------------------------------------------
        // 🔹 GET: Children by ParentId (for hierarchy dropdowns)
        // Example: /api/orgunit/children/5
        // ------------------------------------------------------------
        [HttpGet("children/{parentId:int}")]
        public async Task<IActionResult> GetChildren(int parentId, CancellationToken ct)
        {
            var allUnits = await _service.GetAllAsync(ct);
            var children = allUnits.Where(u => u.ParentId == parentId).ToList();
            return Ok(children);
        }

        // ------------------------------------------------------------
        // 🔹 GET: Top-level Org Units (Zones)
        // Example: /api/orgunit/top
        // ------------------------------------------------------------
        [HttpGet("top")]
        public async Task<IActionResult> GetTopLevel(CancellationToken ct)
        {
            var allUnits = await _service.GetAllAsync(ct);
            var topUnits = allUnits.Where(u => u.ParentId == null).ToList();
            return Ok(topUnits);
        }

        // ------------------------------------------------------------
        // 🔹 GET: Org Units by Type (optional helper)
        // Example: /api/orgunit/type/Feeder
        // ------------------------------------------------------------
        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetByType(string type, CancellationToken ct)
        {
            var allUnits = await _service.GetAllAsync(ct);
            var filtered = allUnits.Where(u =>
                u.Type.Equals(type, StringComparison.OrdinalIgnoreCase)).ToList();
            return Ok(filtered);
        }

        // ------------------------------------------------------------
        // 🔹 POST: Create a new Org Unit
        // ------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrgUnitCreateDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.OrgUnitId }, result);
        }

        // ------------------------------------------------------------
        // 🔹 PUT: Update an existing Org Unit
        // ------------------------------------------------------------
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrgUnitUpdateDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateAsync(id, dto, ct);
            return result == null ? NotFound() : Ok(result);
        }

        // ------------------------------------------------------------
        // 🔹 DELETE: Remove an Org Unit
        // ------------------------------------------------------------
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await _service.DeleteAsync(id, ct);
            return NoContent();
        }
    }
}
