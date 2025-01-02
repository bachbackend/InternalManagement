using BachBinHoangManagement.DTO;
using BachBinHoangManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Authentication.ExtendedProtection;

namespace BachBinHoangManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly InternalManagementContext _context;

        public ServiceController(IConfiguration configuration, InternalManagementContext context)
        {
            _context = context;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllService()
        {
            var services = _context.Services.ToList();
            if(services == null)
            {
                return NotFound("Không tìm thấy dịch vụ nào.");
            }
            return Ok(services);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddService([FromBody] ServiceRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingService = await _context.Services
                .FirstOrDefaultAsync(c => c.Name == dto.Name);

            if (existingService != null)
            {
                return BadRequest("Service này đã tồn tại.");
            }

            var newService = new Models.Service
            {
                Name = dto.Name,
            };

            _context.Services.Add(newService);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetServiceById), new { id = newService.Id }, newService);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound("Service không tồn tại.");
            }
            return Ok(service);
        }

        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> EditService(int id, [FromBody] ServiceRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingService = await _context.Services.FindAsync(id);
            if (existingService == null)
            {
                return NotFound("Service không tồn tại.");
            }

            var duplicateService = await _context.Services
                .FirstOrDefaultAsync(c => c.Name == dto.Name && c.Id != id);
            if (duplicateService != null)
            {
                return BadRequest("Tên service đã tồn tại.");
            }

            existingService.Name = dto.Name;

            _context.Services.Update(existingService);
            await _context.SaveChangesAsync();

            return Ok(existingService);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var existingService = await _context.Services.FindAsync(id);
            if (existingService == null)
            {
                return NotFound("Service không tồn tại.");
            }

            _context.Services.Remove(existingService);
            await _context.SaveChangesAsync();

            return Ok("Xóa Service thành công.");
        }

    }
}
