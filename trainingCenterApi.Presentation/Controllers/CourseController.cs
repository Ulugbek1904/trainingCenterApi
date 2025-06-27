using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using trainingCenter.Domain.Models;
using trainingCenter.Domain.Models.DTOs;
using trainingCenter.Services.Foundation.Interfaces;
using ArgumentException = trainingCenter.Common.Exceptions.ArgumentException;

namespace trainingCenter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService courseService;
        private readonly IMapper mapper;

        public CoursesController(ICourseService courseService, IMapper mapper)
        {
            this.courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CourseCreateDto courseDto)
        {
            try
            {
                var course = mapper.Map<Course>(courseDto);
                var createdCourse = await courseService.RegisterCourseAsync(course);
                var resultDto = mapper.Map<CourseDto>(createdCourse);
                return CreatedAtAction(nameof(GetCourseById), new { id = resultDto.Id }, resultDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCourses([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            if (page < 1 || size < 1)
                return BadRequest("Page and size must be positive.");

            var courses = await courseService.RetrieveAllCoursesAsync();
            var totalCount = courses.Count;
            var pagedCourses = courses.Skip((page - 1) * size).Take(size).ToList();
            var resultDtos = mapper.Map<List<CourseDto>>(pagedCourses);

            var result = new PagedResult<CourseDto>
            {
                Items = resultDtos,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = size
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(Guid id)
        {
            var course = await courseService.RetrieveCourseByIdAsync(id);
            var resultDto = mapper.Map<CourseDto>(course);
            return Ok(resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] CourseUpdateDto courseDto)
        {
            if (id != courseDto.Id)
                throw new ArgumentException("ID mismatch.");

            var course = mapper.Map<Course>(courseDto);
            var updatedCourse = await courseService.ModifyCourseAsync(course);
            var resultDto = mapper.Map<CourseDto>(updatedCourse);
            return Ok(resultDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            await courseService.RemoveCourseAsync(id);
            return NoContent();
        }
    }
}