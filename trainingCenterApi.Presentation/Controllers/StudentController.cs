using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Models;
using trainingCenter.Domain.Models.DTOs.Student;
using trainingCenter.Services.Foundation.Interfaces;
using ArgumentException = trainingCenter.Common.Exceptions.ArgumentException;

namespace trainingCenter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,Secretary,Admin,Teacher")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService studentService;
    private readonly IMapper mapper;
    private readonly ICurrentUserService currentUser;

    public StudentsController(
        IStudentService studentService,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        this.studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    [HttpPost]
    public async Task<IActionResult> CreateStudent([FromBody] StudentCreateDto studentDto)
    {
        var student = mapper.Map<Student>(studentDto);
        student.TenantId = currentUser.TenantId;

        var createdStudent = await studentService.RegisterStudentAsync(student);
        var resultDto = mapper.Map<StudentDto>(createdStudent);

        return CreatedAtAction(nameof(GetStudentById), new { id = resultDto.Id }, resultDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllStudents([FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        if (page < 1 || size < 1)
            return BadRequest("Page and size must be positive.");

        var students = await studentService.RetrieveAllStudents();
        var filtered = students.Where(s => s.TenantId == currentUser.TenantId).ToList();
        var paged = filtered.Skip((page - 1) * size).Take(size).ToList();
        var resultDtos = mapper.Map<List<StudentDto>>(paged);

        var result = new PagedResult<StudentDto>
        {
            Items = resultDtos,
            TotalCount = filtered.Count,
            PageNumber = page,
            PageSize = size
        };

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStudentById(Guid id)
    {
        var student = await studentService.RetrieveStudentByIdAsync(id);
        if (student.TenantId != currentUser.TenantId)
            return Forbid();

        var resultDto = mapper.Map<StudentDto>(student);
        return Ok(resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] StudentUpdateDto studentDto)
    {
        if (id != studentDto.Id)
            throw new ArgumentException("ID mismatch.");

        var existing = await studentService.RetrieveStudentByIdAsync(id);
        if (existing.TenantId != currentUser.TenantId)
            return Forbid();

        var student = mapper.Map<Student>(studentDto);
        student.TenantId = currentUser.TenantId;

        var updatedStudent = await studentService.ModifyStudentAsync(student);
        var resultDto = mapper.Map<StudentDto>(updatedStudent);
        return Ok(resultDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(Guid id)
    {
        var existing = await studentService.RetrieveStudentByIdAsync(id);
        if (existing.TenantId != currentUser.TenantId)
            return Forbid();

        await studentService.RemoveStudentAsync(id);
        return NoContent();
    }
}
