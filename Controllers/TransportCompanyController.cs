using BachBinHoangManagement.DTO;
using BachBinHoangManagement.Models;
using BachBinHoangManagement.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;

namespace BachBinHoangManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransportCompanyController : ControllerBase
    {
        private readonly InternalManagementContext _context;
        private readonly PaginationSettings _paginationSettings;

        public TransportCompanyController(IConfiguration configuration, InternalManagementContext context, IOptions<PaginationSettings> paginationSettings)
        {
            _context = context;
            _paginationSettings = paginationSettings.Value;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllTransportCompany(
            int pageNumber = 1,
            int? pageSize = null
            )
        {
            int actualPageSize = pageSize ?? _paginationSettings.DefaultPageSize;
            var transportCompany = _context.TransportCompanies
                .Include(p => p.District)
                    .ThenInclude(p => p.Province)
                .Include(p => p.TransportCompanyServices)
                    .ThenInclude(p => p.Service)
                .AsQueryable();

            int totalTransportCompanyCount = await transportCompany.CountAsync();

            if (totalTransportCompanyCount == 0)
            {
                return NotFound("Không tìm thấy công ty vận chuyển nào.");
            }

            int totalPageCount = (int)Math.Ceiling(totalTransportCompanyCount / (double)actualPageSize);
            int nextPage = pageNumber + 1 > totalPageCount ? pageNumber : pageNumber + 1;
            int previousPage = pageNumber - 1 < 1 ? pageNumber : pageNumber - 1;

            var pagingResult = new PagingReturn
            {
                TotalPageCount = totalPageCount,
                CurrentPage = pageNumber,
                NextPage = nextPage,
                PreviousPage = previousPage
            };

            List<TransportDTO> transportWithPaging = await transportCompany
                .Skip((pageNumber - 1) * actualPageSize)
                .Take(actualPageSize)
                .Select(p => new TransportDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    SpecificAddress = p.SpecificAddress,
                    Contact = p.Contact,
                    Notes = p.Notes,
                    DistrictId = p.DistrictId,
                    DistrictName = p.District.Name,
                    ProvinceId = p.District.ProvinceId,
                    ProvinceName = p.District.Province.Name,
                    Services = p.TransportCompanyServices
                        .Select(pc => new ServiceDTO
                        {
                            Id = pc.Service.Id,
                            Name = pc.Service.Name
                        })
                    .ToList()
                })
            .ToListAsync();

            if (transportWithPaging == null || transportWithPaging.Count == 0)
            {
                return NotFound("Không tìm thấy công ty vận chuyển nào.");
            }
            var result = new
            {
                TransportCompany = transportWithPaging,
                Paging = pagingResult
            };

            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetTransportCompanyId(int id)
        {
            var transportCompany = await _context.TransportCompanies
                .Include(p => p.District)
                    .ThenInclude(p => p.Province)
                .Include(p => p.TransportCompanyServices)
                    .ThenInclude(p => p.Service)
                .Where(p => p.Id == id)
                .Select(p => new TransportDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    SpecificAddress = p.SpecificAddress,
                    Contact = p.Contact,
                    Notes = p.Notes,
                    DistrictId = p.DistrictId,
                    DistrictName = p.District.Name,
                    ProvinceId = p.District.ProvinceId,
                    ProvinceName = p.District.Province.Name,
                    Services = p.TransportCompanyServices
                        .Select(pc => new ServiceDTO
                        {
                            Id = pc.Service.Id,
                            Name = pc.Service.Name
                        })
                    .ToList()
                })
                .FirstOrDefaultAsync();

            if (transportCompany == null)
            {
                return NotFound();
            }

            return Ok(transportCompany);
        }

        [HttpPost("AddTransportCompany")]
        public async Task<IActionResult> AddTransportCompany([FromForm] TransportCompanyRequest model)
        {
            var transport = new TransportCompany
            {
                Name = model.Name,
                SpecificAddress = model.SpecificAddress,
                //ServiceId = model.ServiceId,
                Contact = model.Contact,
                Notes = model.Notes,
                DistrictId = model.DistrictId,
                TransportCompanyServices = model.ServiceIds != null ? model.ServiceIds.Select(serviceId => new TransportCompanyService { ServiceId = serviceId }).ToList() : new List<TransportCompanyService>()
            };

            _context.TransportCompanies.Add(transport);
            await _context.SaveChangesAsync();

            return Ok(new { transportCompanyId = transport.Id });
        }

        [HttpPut("UpdateTransportCompany/{id}")]
        public async Task<IActionResult> UpdateTransportCompany(int id, [FromForm] TransportCompanyRequest model)
        {
            var existingTransport = await _context.TransportCompanies
                .Include(tc => tc.TransportCompanyServices)
                .FirstOrDefaultAsync(tc => tc.Id == id);

            if (existingTransport == null)
            {
                return NotFound("Không tìm thấy công ty vận chuyển.");
            }

            // Cập nhật chỉ khi có giá trị mới
            existingTransport.Name = !string.IsNullOrEmpty(model.Name) ? model.Name : existingTransport.Name;
            existingTransport.SpecificAddress = !string.IsNullOrEmpty(model.SpecificAddress) ? model.SpecificAddress : existingTransport.SpecificAddress;
            existingTransport.Contact = !string.IsNullOrEmpty(model.Contact) ? model.Contact : existingTransport.Contact;
            existingTransport.Notes = model.Notes ?? existingTransport.Notes;
            existingTransport.DistrictId = model.DistrictId != 0 ? model.DistrictId : existingTransport.DistrictId;

            // Cập nhật dịch vụ nếu có
            if (model.ServiceIds != null && model.ServiceIds.Any())
            {
                _context.TransportCompanyServices.RemoveRange(existingTransport.TransportCompanyServices);

                foreach (var serviceId in model.ServiceIds)
                {
                    existingTransport.TransportCompanyServices.Add(new TransportCompanyService
                    {
                        ServiceId = serviceId
                    });
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật thành công!", transportCompanyId = existingTransport.Id });
        }

        [HttpPatch("UpdateTransportCompanyNotes/{id}")]
        public async Task<IActionResult> UpdateTransportCompanyNotes(int id, [FromBody] NotesRequest request)
        {
            var existingTransport = await _context.TransportCompanies
                .FirstOrDefaultAsync(tc => tc.Id == id);

            if (existingTransport == null)
            {
                return NotFound("Không tìm thấy công ty vận chuyển.");
            }

            existingTransport.Notes = request.Notes ?? existingTransport.Notes;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật ghi chú thành công!", transportCompanyId = existingTransport.Id });
        }


        [HttpDelete("DeleteTransportCompany/{id}")]
        public async Task<IActionResult> DeleteTransportCompany(int id)
        {
            var existingTransport = await _context.TransportCompanies
                .Include(tc => tc.TransportCompanyServices)
                .FirstOrDefaultAsync(tc => tc.Id == id);

            if (existingTransport == null)
            {
                return NotFound("Không tìm thấy công ty vận chuyển.");
            }

            // Xóa tất cả các dịch vụ liên quan
            _context.TransportCompanyServices.RemoveRange(existingTransport.TransportCompanyServices);

            // Xóa công ty vận chuyển
            _context.TransportCompanies.Remove(existingTransport);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa thành công!" });
        }

    }
}
