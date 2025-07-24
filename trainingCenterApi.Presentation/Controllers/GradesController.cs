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
[Authorize(Roles = "Admin,Teacher")]
public class GradesController : ControllerBase
{
    private readonly IGradeService gradeService;
    private readonly IMapper mapper;
    private readonly ICurrentUserService currentUser;

    public GradesController(IGradeService gradeService, IMapper mapper, ICurrentUserService currentUser)
    {
        this.gradeService = gradeService ?? throw new NullArgumentException(nameof(gradeService));
        this.mapper = mapper ?? throw new NullArgumentException(nameof(mapper));
        this.currentUser = currentUser ?? throw new NullArgumentException(nameof(currentUser));
    }

    [HttpPost]
    public async Task<IActionResult> CreateGrade([FromBody] GradeCreateDto gradeDto)
    {
        var grade = mapper.Map<Grade>(gradeDto);
        grade.TenantId = currentUser.TenantId;

        var created = await gradeService.RegisterGradeAsync(grade);
        return CreatedAtAction(nameof(GetGradeById), new { id = created.Id }, mapper.Map<GradeDto>(created));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGrades([FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        var grades = await gradeService.RetrieveAllGradesAsync();
        var filtered = grades.Where(g => g.TenantId == currentUser.TenantId).ToList();

        var paged = filtered.Skip((page - 1) * size).Take(size).ToList();
        var result = new PagedResult<GradeDto>
        {
            Items = mapper.Map<List<GradeDto>>(paged),
            TotalCount = filtered.Count,
            PageNumber = page,
            PageSize = size
        };
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGradeById(Guid id)
    {
        var grade = await gradeService.RetrieveGradeByIdAsync(id);
        if (grade.TenantId != currentUser.TenantId)
            return Forbid();

        return Ok(mapper.Map<GradeDto>(grade));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGrade(Guid id, [FromBody] GradeUpdateDto gradeDto)
    {
        if (id != gradeDto.Id)
            throw new ArgumentException("ID mismatch.");

        var grade = mapper.Map<Grade>(gradeDto);
        grade.TenantId = currentUser.TenantId;

        var updated = await gradeService.ModifyGradeAsync(grade);
        return Ok(mapper.Map<GradeDto>(updated));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGrade(Guid id)
    {
        var grade = await gradeService.RetrieveGradeByIdAsync(id);
        if (grade.TenantId != currentUser.TenantId)
            return Forbid();

        await gradeService.RemoveGradeAsync(id);
        return NoContent();
    }
}
