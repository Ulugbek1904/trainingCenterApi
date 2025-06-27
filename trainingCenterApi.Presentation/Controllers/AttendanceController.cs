using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Models;
using trainingCenter.Domain.Models.DTOs;
using trainingCenter.Services.Foundation.Interfaces;
using ArgumentException = trainingCenter.Common.Exceptions.ArgumentException;

namespace trainingCenter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendancesController : ControllerBase
    {
        private readonly IAttendanceService attendanceService;
        private readonly IMapper mapper;

        public AttendancesController(IAttendanceService attendanceService, IMapper mapper)
        {
            this.attendanceService = attendanceService ?? throw new ArgumentNullException(nameof(attendanceService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAttendance([FromBody] AttendanceCreateDto attendanceDto)
        {
            try
            {
                var attendance = mapper.Map<Attendance>(attendanceDto);
                var createdAttendance = await attendanceService.RegisterAttendanceAsync(attendance);
                var resultDto = mapper.Map<AttendanceDto>(createdAttendance);
                return CreatedAtAction(nameof(GetAttendanceById), new { id = resultDto.Id }, resultDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAttendances([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            if (page < 1 || size < 1)
                return BadRequest("Page and size must be positive.");

            var attendances = await attendanceService.RetrieveAllAttendancesAsync();
            var totalCount = attendances.Count;
            var pagedAttendances = attendances.Skip((page - 1) * size).Take(size).ToList();
            var resultDtos = mapper.Map<List<AttendanceDto>>(pagedAttendances);

            var result = new PagedResult<AttendanceDto>
            {
                Items = resultDtos,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = size
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAttendanceById(Guid id)
        {
            var attendance = await attendanceService.RetrieveAttendanceByIdAsync(id);
            var resultDto = mapper.Map<AttendanceDto>(attendance);
            return Ok(resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttendance(Guid id, [FromBody] AttendanceUpdateDto attendanceDto)
        {
            if (id != attendanceDto.Id)
                throw new ArgumentException("ID mismatch.");

            var attendance = mapper.Map<Attendance>(attendanceDto);
            var updatedAttendance = await attendanceService.ModifyAttendanceAsync(attendance);
            var resultDto = mapper.Map<AttendanceDto>(updatedAttendance);
            return Ok(resultDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendance(Guid id)
        {
            await attendanceService.RemoveAttendanceAsync(id);
            return NoContent();
        }
    }
}