using AutoMapper;
namespace Talent;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mapping between Course and CourseDto
        CreateMap<Course, CourseDTO>();
        CreateMap<CreateCourseDto, Course>();

        // Mapping between User and UserDto
        CreateMap<User, UserDTO>();
        CreateMap<RegisterUserDTO, User>();

        // Mapping between Lesson and LessonDto
        CreateMap<Lesson, LessonDTO>();
        CreateMap<CreateLessonDto, Lesson>();

        // Mapping between Enrollment and EnrollmentDto
        CreateMap<Enrollment, EnrollmentDto>();

        // Mapping between Progress and ProgressDto
        CreateMap<Progress, ProgressDto>();
    }
}
