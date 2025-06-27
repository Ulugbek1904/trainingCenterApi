using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Models;
using trainingCenter.Domain.Models.DTOs.Student;
using trainingCenter.Services.Foundation.Interfaces;
using ArgumentException = trainingCenter.Common.Exceptions.ArgumentException;

namespace trainingCenter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService studentService;
        private readonly IMapper mapper;

        public StudentsController(IStudentService studentService, IMapper mapper)
        {
            this.studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] StudentCreateDto studentDto)
        {
            try
            {
                var student = mapper.Map<Student>(studentDto);
                var createdStudent = await studentService.RegisterStudentAsync(student);
                var resultDto = mapper.Map<StudentDto>(createdStudent);
                return CreatedAtAction(nameof(GetStudentById), new { id = resultDto.Id }, resultDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStudents([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            if (page < 1 || size < 1)
                return BadRequest("Page and size must be positive.");

            var students = await studentService.RetrieveAllStudents();
            var totalCount = students.Count();
            var pagedStudents = students.Skip((page - 1) * size).Take(size).ToList();
            var resultDtos = mapper.Map<List<StudentDto>>(pagedStudents);

            var result = new PagedResult<StudentDto>
            {
                Items = resultDtos,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = size
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(Guid id)
        {
            var student = await studentService.RetrieveStudentByIdAsync(id);
            var resultDto = mapper.Map<StudentDto>(student);
            return Ok(resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] StudentUpdateDto studentDto)
        {
            if (id != studentDto.Id)
                throw new ArgumentException("ID mismatch.");

            var student = mapper.Map<Student>(studentDto);
            var updatedStudent = await studentService.ModifyStudentAsync(student);
            var resultDto = mapper.Map<StudentDto>(updatedStudent);
            return Ok(resultDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(Guid id)
        {
            await studentService.RemoveStudentAsync(id);
            return NoContent();
        }
    }
}