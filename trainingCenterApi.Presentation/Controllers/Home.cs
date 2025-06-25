using Microsoft.AspNetCore.Mvc;
using trainingCenter.Domain.Models;
using trainingCenter.Infrastructure.brokers.storage;

namespace trainingCenterApi.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class Home : ControllerBase
{
    private readonly IStorageBroker storage;

    public Home(IStorageBroker storage)
    {
        this.storage = storage;
    }
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Welcome to the Training Center API!");
    }

    [HttpGet("create")]
    public async ValueTask<IActionResult> createStudent()
    {
        var student1 = new Student
        {
            Id = Guid.Parse("d7c9080c-7ea8-496d-83ac-6f62b1372a4b"),
            FullName = "Ali",
            EnrollmentDate = DateTime.UtcNow,
            IsActive = true,
            ParentPhoneNumber = "+998911032766",
            BirthDate = new DateTime(2008, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            StudentCourses = new List<StudentCourse>(),
            PhoneNumber = "+998940641904",
            Address = "sdsdas"
        };

        await storage.InsertAsync<Student>(student1);

        return Ok("Student created successfully!");
    }
}