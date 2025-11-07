

using AMI_Project.DTOs.Tariffs;
using AMI_Project.Models;
using AMI_Project.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AMI_Project.Controllers
{
    [ApiController]
    [Route("api/tariffs")] // ✅ consistent, lowercase and plural
    public class TariffController : ControllerBase
    {
        private readonly ITariffService _service;
        private readonly IMapper _mapper;

        public TariffController(ITariffService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var tariffs = await _service.GetAllAsync(ct);
            return Ok(_mapper.Map<IEnumerable<TariffReadDto>>(tariffs));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var tariff = await _service.GetByIdAsync(id, ct);
            if (tariff == null) return NotFound();
            return Ok(_mapper.Map<TariffReadDto>(tariff));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TariffCreateDto dto, CancellationToken ct)
        {
            var entity = _mapper.Map<Tariff>(dto);
            var created = await _service.CreateAsync(entity, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.TariffId }, _mapper.Map<TariffReadDto>(created));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TariffUpdateDto dto, CancellationToken ct)
        {
            var entity = _mapper.Map<Tariff>(dto);
            var updated = await _service.UpdateAsync(id, entity, ct);
            if (updated == null) return NotFound();
            return Ok(_mapper.Map<TariffReadDto>(updated));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await _service.DeleteAsync(id, ct);
            return NoContent();
        }
    }
}
