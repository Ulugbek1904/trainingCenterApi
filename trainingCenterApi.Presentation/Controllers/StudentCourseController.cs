using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Models;
using trainingCenter.Domain.Models.DTOs;
using trainingCenter.Services.Foundation.Interfaces;
using ArgumentException = trainingCenter.Common.Exceptions.ArgumentException;

namespace trainingCenter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Secretary")]
public class StudentCoursesController : ControllerBase
{
    private readonly IStudentCourseService studentCourseService;
    private readonly IMapper mapper;
    private readonly ICurrentUserService currentUser;

    public StudentCoursesController(
        IStudentCourseService studentCourseService,
        IMapper mapper, 
        ICurrentUserService currentUser)
    {
        this.studentCourseService = studentCourseService ??
            throw new NullArgumentException(nameof(studentCourseService));

        this.mapper = mapper ?? 
            throw new NullArgumentException(nameof(mapper));

        this.currentUser = currentUser ??
            throw new NullArgumentException(nameof(currentUser));
    }

    [HttpPost]
    public async Task<IActionResult> CreateStudentCourse([FromBody] StudentCourseCreateDto dto)
    {
        var studentCourse = mapper.Map<StudentCourse>(dto);
        studentCourse.TenantId = currentUser.TenantId;

        var created = await studentCourseService.RegisterStudentCourseAsync(studentCourse);

        return CreatedAtAction(nameof(GetStudentCourse), new
            { studentId = created.StudentId, courseId = created.CourseId },
            mapper.Map<StudentCourseDto>(created));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllStudentCourses(
        [FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        var all = await studentCourseService.RetrieveAllStudentCoursesAsync();

        var filtered = all
            .Where(sc => sc.TenantId == currentUser.TenantId).ToList();

        var paged = filtered.Skip((page - 1) * size).Take(size).ToList();
        var result = new PagedResult<StudentCourseDto>
        {
            Items = mapper.Map<List<StudentCourseDto>>(paged),
            TotalCount = filtered.Count,
            PageNumber = page,
            PageSize = size
        };
        return Ok(result);
    }

    [HttpGet("student/{studentId}/course/{courseId}")]
    public async Task<IActionResult> GetStudentCourse(Guid studentId, Guid courseId)
    {
        var studentCourse = await studentCourseService
            .RetrieveStudentCourseByIdsAsync(studentId, courseId);

        if (studentCourse.TenantId != currentUser.TenantId)
            return Forbid();

        return Ok(mapper.Map<StudentCourseDto>(studentCourse));
    }

    [HttpPut("student/{studentId}/course/{courseId}")]
    public async Task<IActionResult> UpdateStudentCourse(
        Guid studentId, Guid courseId, [FromBody] StudentCourseUpdateDto dto)
    {
        if (studentId != dto.StudentId || courseId != dto.CourseId)
            throw new ArgumentException("ID mismatch.");

        var studentCourse = mapper.Map<StudentCourse>(dto);
        studentCourse.TenantId = currentUser.TenantId;

        var updated = await studentCourseService.
            ModifyStudentCourseAsync(studentCourse);

        return Ok(mapper.Map<StudentCourseDto>(updated));
    }

    [HttpDelete("student/{studentId}/course/{courseId}")]
    public async Task<IActionResult> DeleteStudentCourse(Guid studentId, Guid courseId)
    {
        var sc = await studentCourseService.RetrieveStudentCourseByIdsAsync(studentId, courseId);
        if (sc.TenantId != currentUser.TenantId)
            return Forbid();

        await studentCourseService.RemoveStudentCourseAsync(studentId, courseId);
        return NoContent();
    }
}
