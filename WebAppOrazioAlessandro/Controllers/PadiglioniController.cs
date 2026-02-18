using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAppOrazioAlessandro.DTOs.Padiglione;
using WebAppOrazioAlessandro.Entities;
using WebAppOrazioAlessandro.Services.DeletionService;
using WebAppOrazioAlessandro.Services.Interfaces;


namespace WebAppOrazioAlessandro.Controllers
{
    [Authorize(Roles = "Admin,Supervisor")]
    [ApiController]
    [Route("api/[controller]")]
    public class PadiglioniController : ControllerBase
    {
        private readonly IPadiglioneService _service;

        public PadiglioniController(IPadiglioneService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePadiglioneDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreatePadiglioneDto dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            BackgroundJob.Enqueue<PadiglioneDeletionService>(
                service => service.DeleteAsync(id));

            return Ok("Eliminazione pianificata");
        }
    }

}
