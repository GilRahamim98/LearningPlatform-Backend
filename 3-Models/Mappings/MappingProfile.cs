using AutoMapper;
namespace Talent;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mapping between Course, CourseDto and CreateCourseDto
        CreateMap<Course, CourseDto>();
        CreateMap<CreateCourseDto, Course>();

        // Mapping between User, UserDto and RegisterUserDto
        CreateMap<User, UserDto>();
        CreateMap<RegisterUserDto, User>();

        // Mapping between Lesson and LessonDto with reverse mapping, and between Lesson to CreateLessonDto
        CreateMap<Lesson, LessonDto>().ReverseMap();
        CreateMap<CreateLessonDto, Lesson>();
        CreateMap<Lesson,LessonPreviewDto>();

        // Mapping between Enrollment and EnrollmentDto
        CreateMap<Enrollment, EnrollmentDto>();

        // Mapping between Progress and ProgressDto
        CreateMap<Progress, ProgressDto>();
    }
}
