using AutoMapper;
namespace Talent;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mapping between Course and CourseDto
        CreateMap<Course, CourseDto>();
        CreateMap<CreateCourseDto, Course>();

        // Mapping between User and UserDto
        CreateMap<User, UserDto>();
        CreateMap<RegisterUserDto, User>();

        // Mapping between Lesson and LessonDto
        CreateMap<Lesson, LessonDto>();
        CreateMap<CreateLessonDto, Lesson>();
        CreateMap<Lesson,LessonPreviewDto>();

        // Mapping between Enrollment and EnrollmentDto
        CreateMap<Enrollment, EnrollmentDto>();

        // Mapping between Progress and ProgressDto
        CreateMap<Progress, ProgressDto>();
    }
}
