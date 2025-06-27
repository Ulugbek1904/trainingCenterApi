using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Models;
using trainingCenter.Domain.Models.DTOs;
using trainingCenter.Services.Foundation.Interfaces;
using ArgumentException = trainingCenter.Common.Exceptions.ArgumentException;

namespace trainingCenter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GradesController : ControllerBase
    {
        private readonly IGradeService gradeService;
        private readonly IMapper mapper;

        public GradesController(IGradeService gradeService, IMapper mapper)
        {
            this.gradeService = gradeService ?? throw new NullArgumentException(nameof(gradeService));
            this.mapper = mapper ?? throw new NullArgumentException(nameof(mapper));
        }

        [HttpPost]
        public async Task<IActionResult> CreateGrade([FromBody] GradeCreateDto gradeDto)
        {
            try
            {
                var grade = mapper.Map<Grade>(gradeDto);
                var createdGrade = await gradeService.RegisterGradeAsync(grade);
                var resultDto = mapper.Map<GradeDto>(createdGrade);
                return CreatedAtAction(nameof(GetGradeById), new { id = resultDto.Id }, resultDto);
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
        public async Task<IActionResult> GetAllGrades([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            if (page < 1 || size < 1)
                return BadRequest("Page and size must be positive.");

            var grades = await gradeService.RetrieveAllGradesAsync();
            var totalCount = grades.Count;
            var pagedGrades = grades.Skip((page - 1) * size).Take(size).ToList();
            var resultDtos = mapper.Map<List<GradeDto>>(pagedGrades);

            var result = new PagedResult<GradeDto>
            {
                Items = resultDtos,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = size
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGradeById(Guid id)
        {
            var grade = await gradeService.RetrieveGradeByIdAsync(id);
            var resultDto = mapper.Map<GradeDto>(grade);
            return Ok(resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGrade(Guid id, [FromBody] GradeUpdateDto gradeDto)
        {
            if (id != gradeDto.Id)
                throw new ArgumentException("ID mismatch.");

            var grade = mapper.Map<Grade>(gradeDto);
            var updatedGrade = await gradeService.ModifyGradeAsync(grade);
            var resultDto = mapper.Map<GradeDto>(updatedGrade);
            return Ok(resultDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGrade(Guid id)
        {
            await gradeService.RemoveGradeAsync(id);
            return NoContent();
        }
    }
}