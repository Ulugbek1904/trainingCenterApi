using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Enums;
using trainingCenter.Domain.Models;
using trainingCenter.Domain.Models.DTOs;
using trainingCenter.Services.Foundation.Interfaces;

namespace trainingCenter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(Role.Admin))]
public class CoursesController : ControllerBase
{
    private readonly ICourseService courseService;
    private readonly IMapper mapper;
    private readonly ICurrentUserService currentUser;

    public CoursesController(
        ICourseService courseService,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        this.courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCourse([FromBody] CourseCreateDto dto)
    {
        var course = mapper.Map<Course>(dto);
        course.TenantId = currentUser.TenantId;

        var created = await courseService.RegisterCourseAsync(course);
        return CreatedAtAction(nameof(GetCourseById), new { id = created.Id }, mapper.Map<CourseDto>(created));
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllCourses([FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        var allCourses = await courseService.RetrieveAllCoursesAsync();
        var filtered = allCourses.Where(c => c.TenantId == currentUser.TenantId);

        var paged = filtered.Skip((page - 1) * size).Take(size).ToList();
        var result = new PagedResult<CourseDto>
        {
            Items = mapper.Map<List<CourseDto>>(paged),
            TotalCount = filtered.Count(),
            PageNumber = page,
            PageSize = size
        };

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourseById(Guid id)
    {
        var course = await courseService.RetrieveCourseByIdAsync(id);
        if (course.TenantId != currentUser.TenantId)
            return Forbid("Bu kurs sizga tegishli emas.");

        return Ok(mapper.Map<CourseDto>(course));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] CourseUpdateDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mos kelmadi.");

        var existing = await courseService.RetrieveCourseByIdAsync(id);
        if (existing.TenantId != currentUser.TenantId)
            return Forbid("Siz bu kursni o‘zgartira olmaysiz.");

        var updated = await courseService.ModifyCourseAsync(mapper.Map<Course>(dto));
        return Ok(mapper.Map<CourseDto>(updated));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        var existing = await courseService.RetrieveCourseByIdAsync(id);
        if (existing.TenantId != currentUser.TenantId)
            return Forbid("Bu kurs sizga tegishli emas.");

        await courseService.RemoveCourseAsync(id);
        return NoContent();
    }
}
