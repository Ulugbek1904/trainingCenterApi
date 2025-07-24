using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using trainingCenter.Domain.Models;
using trainingCenter.Domain.Models.DTOs;
using trainingCenter.Services.Foundation.Interfaces;

namespace trainingCenter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Teacher,Admin")]
    public class AttendancesController : ControllerBase
    {
        private readonly IAttendanceService attendanceService;
        private readonly IMapper mapper;
        private readonly ICurrentUserService currentUser;

        public AttendancesController(
            IAttendanceService attendanceService,
            IMapper mapper,
            ICurrentUserService currentUser)
        {
            this.attendanceService = attendanceService;
            this.mapper = mapper;
            this.currentUser = currentUser;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAttendance([FromBody] AttendanceCreateDto attendanceDto)
        {
            var attendance = mapper.Map<Attendance>(attendanceDto);
            attendance.TenantId = currentUser.TenantId;

            var createdAttendance = await attendanceService.RegisterAttendanceAsync(attendance);
            var resultDto = mapper.Map<AttendanceDto>(createdAttendance);
            return CreatedAtAction(nameof(GetAttendanceById), new { id = resultDto.Id }, resultDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAttendances([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            if (page < 1 || size < 1)
                return BadRequest("Page and size must be positive.");

            var allAttendances = await attendanceService.RetrieveAllAttendancesAsync();
            var tenantAttendances = allAttendances
                .Where(a => a.TenantId == currentUser.TenantId)
                .ToList();

            var paged = tenantAttendances
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();

            var resultDtos = mapper.Map<List<AttendanceDto>>(paged);

            var result = new PagedResult<AttendanceDto>
            {
                Items = resultDtos,
                TotalCount = tenantAttendances.Count,
                PageNumber = page,
                PageSize = size
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAttendanceById(Guid id)
        {
            var attendance = await attendanceService.RetrieveAttendanceByIdAsync(id);

            if (attendance.TenantId != currentUser.TenantId)
                return Forbid("You cannot access attendance from another tenant.");

            var resultDto = mapper.Map<AttendanceDto>(attendance);
            return Ok(resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttendance(Guid id, [FromBody] AttendanceUpdateDto attendanceDto)
        {
            if (id != attendanceDto.Id)
                return BadRequest("ID mismatch.");

            var attendance = mapper.Map<Attendance>(attendanceDto);
            attendance.TenantId = currentUser.TenantId;

            var updatedAttendance = await attendanceService.ModifyAttendanceAsync(attendance);
            var resultDto = mapper.Map<AttendanceDto>(updatedAttendance);
            return Ok(resultDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendance(Guid id)
        {
            var attendance = await attendanceService.RetrieveAttendanceByIdAsync(id);

            if (attendance.TenantId != currentUser.TenantId)
                return Forbid("You cannot delete attendance from another tenant.");

            await attendanceService.RemoveAttendanceAsync(id);
            return NoContent();
        }
    }
}
