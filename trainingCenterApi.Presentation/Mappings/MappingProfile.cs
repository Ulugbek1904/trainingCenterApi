using trainingCenter.Domain.Models.DTOs.Student;
using trainingCenter.Domain.Models;
using trainingCenter.Domain.Models.DTOs;

namespace trainingCenterApi.Presentation.Mappings;

public class MappingProfile : AutoMapper.Profile
{
    public MappingProfile()
    {
        CreateMap<StudentCreateDto, Student>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.EnrollmentDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.StudentCourses, opt => opt.MapFrom(src => new List<StudentCourse>()))
                .ForMember(dest => dest.ParentTelegramId, opt => opt.Ignore())
                .ForMember(dest => dest.Notes, opt => opt.Ignore());

        CreateMap<StudentUpdateDto, Student>();

        CreateMap<Student, StudentDto>();


        // Course mappings
        CreateMap<CourseCreateDto, Course>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.StudentCourses, opt => opt.MapFrom(src => new List<StudentCourse>()))
            .ForMember(dest => dest.Teacher, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore());
        CreateMap<CourseUpdateDto, Course>()
            .ForMember(dest => dest.Teacher, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore());
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => 
                src.Teacher != null ? src.Teacher.FullName : string.Empty))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src =>
                src.Category != null ? src.Category.Name : string.Empty));


        // Category mappings
        CreateMap<CategoryCreateDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Courses, opt => opt.MapFrom(src => new List<Course>()));
        CreateMap<CategoryUpdateDto, Category>();
        CreateMap<Category, CategoryDto>();

        // StudentCourse mappings
        CreateMap<StudentCourseCreateDto, StudentCourse>()
            .ForMember(dest => dest.Student, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore());
        CreateMap<StudentCourseUpdateDto, StudentCourse>()
            .ForMember(dest => dest.Student, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore());
        CreateMap<StudentCourse, StudentCourseDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.FullName : string.Empty))
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course != null ? src.Course.Name : string.Empty));

        // Payment mappings
        CreateMap<PaymentCreateDto, Payment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Student, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore());
        CreateMap<PaymentUpdateDto, Payment>()
            .ForMember(dest => dest.Student, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore());
        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => 
                src.Student != null ? src.Student.FullName : string.Empty))
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => 
                src.Course != null ? src.Course.Name : string.Empty));

        // Attendance mappings
        CreateMap<AttendanceCreateDto, Attendance>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Student, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore());
        CreateMap<AttendanceUpdateDto, Attendance>()
            .ForMember(dest => dest.Student, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore());
        CreateMap<Attendance, AttendanceDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.FullName : string.Empty))
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course != null ? src.Course.Name : string.Empty));

        // Grade mappings
        CreateMap<GradeCreateDto, Grade>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Student, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore())
            .ForMember(dest => dest.Teacher, opt => opt.Ignore());
        CreateMap<GradeUpdateDto, Grade>()
            .ForMember(dest => dest.Student, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore())
            .ForMember(dest => dest.Teacher, opt => opt.Ignore());
        CreateMap<Grade, GradeDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.FullName : string.Empty))
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course != null ? src.Course.Name : string.Empty))
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher != null ? src.Teacher.FullName : string.Empty));

        // User mappings
        CreateMap<UserCreateDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore());
        CreateMap<UserUpdateDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
            .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore());
        CreateMap<User, UserDto>();
    }

}
