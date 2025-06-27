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
    public class StudentCoursesController : ControllerBase
    {
        private readonly IStudentCourseService studentCourseService;
        private readonly IMapper mapper;

        public StudentCoursesController(IStudentCourseService studentCourseService, IMapper mapper)
        {
            this.studentCourseService = studentCourseService ?? throw new NullArgumentException(nameof(studentCourseService));
            this.mapper = mapper ?? throw new NullArgumentException(nameof(mapper));
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudentCourse([FromBody] StudentCourseCreateDto studentCourseDto)
        {
            try
            {
                var studentCourse = mapper.Map<StudentCourse>(studentCourseDto);
                var createdStudentCourse = await studentCourseService.RegisterStudentCourseAsync(studentCourse);
                var resultDto = mapper.Map<StudentCourseDto>(createdStudentCourse);
                return CreatedAtAction(nameof(GetStudentCourse), new 
                    { studentId = resultDto.StudentId, courseId = resultDto.CourseId }, resultDto);
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
        public async Task<IActionResult> GetAllStudentCourses([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            if (page < 1 || size < 1)
                return BadRequest("Page and size must be positive.");

            var studentCourses = await studentCourseService.RetrieveAllStudentCoursesAsync();
            var totalCount = studentCourses.Count;
            var pagedStudentCourses = studentCourses.Skip((page - 1) * size).Take(size).ToList();
            var resultDtos = mapper.Map<List<StudentCourseDto>>(pagedStudentCourses);

            var result = new PagedResult<StudentCourseDto>
            {
                Items = resultDtos,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = size
            };

            return Ok(result);
        }

        [HttpGet("student/{studentId}/course/{courseId}")]
        public async Task<IActionResult> GetStudentCourse(Guid studentId, Guid courseId)
        {
            var studentCourse = await studentCourseService.RetrieveStudentCourseByIdsAsync(studentId, courseId);
            var resultDto = mapper.Map<StudentCourseDto>(studentCourse);
            return Ok(resultDto);
        }

        [HttpPut("student/{studentId}/course/{courseId}")]
        public async Task<IActionResult> UpdateStudentCourse(Guid studentId, Guid courseId, [FromBody] StudentCourseUpdateDto studentCourseDto)
        {
            if (studentId != studentCourseDto.StudentId || courseId != studentCourseDto.CourseId)
                throw new ArgumentException("ID mismatch.");

            var studentCourse = mapper.Map<StudentCourse>(studentCourseDto);
            var updatedStudentCourse = await studentCourseService.ModifyStudentCourseAsync(studentCourse);
            var resultDto = mapper.Map<StudentCourseDto>(updatedStudentCourse);
            return Ok(resultDto);
        }

        [HttpDelete("student/{studentId}/course/{courseId}")]
        public async Task<IActionResult> DeleteStudentCourse(Guid studentId, Guid courseId)
        {
            await studentCourseService.RemoveStudentCourseAsync(studentId, courseId);
            return NoContent();
        }
    }
}