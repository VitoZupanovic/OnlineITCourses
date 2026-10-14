using DAL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Dtos.CourseDtos;

namespace WebAPI.Controllers
{
    [Route("api/course")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseRepository _courseRepo;
        private readonly ILogRepository _logRepo;

        public CourseController(ICourseRepository courseRepo, ILogRepository logRepo)
        {
            _courseRepo = courseRepo;
            _logRepo = logRepo;
        }

        private static CourseReadDto MapCourseToReadDto(Course c)
        {
            return new CourseReadDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                DurationHours = c.DurationHours,
                Level = c.Level,
                Price = c.Price,

                CourseGroup = new CourseGroupDto
                {
                    Id = c.CourseGroup.Id,
                    Name = c.CourseGroup.Name,
                    Description = c.CourseGroup.Description
                }
            };
        }


        [HttpGet]
        public ActionResult<IEnumerable<CourseReadDto>> GetAll()
        {
            try
            {
                var courses = _courseRepo.RetrieveAll()
                    .Select(MapCourseToReadDto)
                    .ToList();

                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, $"Error while retrieving courses", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<CourseReadDto> GetById([FromRoute] int id)
        {
            try
            {
                var c = _courseRepo.RetrieveById(id);

                if (c == null)
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Course with id={id} was not found.");
                    return NotFound();
                }

                var dto = MapCourseToReadDto(c);

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, $"Error while retrieving course with id={id}.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}/instructors")]
        public ActionResult<IEnumerable<InstructorDto>> GetInstructorsForCourse([FromRoute] int id)
        {
            try
            {
                if (!_courseRepo.CourseExists(id))
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Course with id={id} was not found.");
                    return NotFound();
                }

                var instructors = _courseRepo.RetrieveInstructorsForCourse(id)
                    .Select(ci => new InstructorDto
                    {
                        Id = ci.Instructor.Id,
                        Name = ci.Instructor.Name,
                        Email = ci.Instructor.Email,
                        Biography = ci.Instructor.Biography
                    })
                    .ToList();

                return Ok(instructors);
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, $"Error while retrieving instructors for course with id={id}.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<CourseDto> Create([FromBody] CourseDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var course = new Course
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    DurationHours = dto.DurationHours,
                    Level = dto.Level,
                    Price = dto.Price,
                    CourseGroupId = dto.CourseGroupId
                };

                _courseRepo.Create(course);

                dto.Id = course.Id;

                _logRepo.AddLog(AppLogLevel.Information, $"Course with id={course.Id} has been created.");

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, "Error while creating course.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{courseId}/instructors/{instructorId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult AddInstructorToCourse([FromRoute] int courseId, [FromRoute] int instructorId)
        {
            try
            {
                if (!_courseRepo.CourseExists(courseId))
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Cannot assign instructor with id={instructorId}. Course with id={courseId} was not found.");
                    return NotFound("Course not found.");
                }

                if (!_courseRepo.InstructorExists(instructorId))
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Cannot assign instructor with id={instructorId} to course with id={courseId}. Instructor was not found.");
                    return NotFound("Instructor not found.");
                }

                if (_courseRepo.CourseInstructorExists(courseId, instructorId))
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Instructor with id={instructorId} is already assigned to course with id={courseId}.");
                    return BadRequest("Instructor is already assigned to this course.");
                }

                _courseRepo.AddInstructorToCourse(courseId, instructorId);

                _logRepo.AddLog(AppLogLevel.Information, $"Instructor with id={instructorId} has been assigned to course with id={courseId}.");

                return Ok("Instructor assigned to course.");
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, $"Error while assigning instructor with id={instructorId} to course with id={courseId}.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Update([FromRoute] int id, [FromBody] CourseDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!_courseRepo.CourseExists(id))
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Course with id={id} was not found.");
                    return NotFound();
                }

                var course = new Course
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    DurationHours = dto.DurationHours,
                    Level = dto.Level,
                    Price = dto.Price,
                    CourseGroupId = dto.CourseGroupId
                };

                _courseRepo.Update(id, course);

                _logRepo.AddLog(AppLogLevel.Information, $"Course with id={id} has been updated.");

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, $"Error while updating course with id={id}.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete([FromRoute] int id)
        {
            try
            {
                if (!_courseRepo.CourseExists(id))
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Course with id={id} was not found.");
                    return NotFound();
                }

                if (!_courseRepo.CanDeleteCourse(id))
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Cannot delete course with id={id} because it has instructors or applications.");
                    return BadRequest("Cannot delete course because it has instructors or applications.");
                }

                _courseRepo.Delete(id);

                _logRepo.AddLog(AppLogLevel.Information, $"Course with id={id} has been deleted.");

                return Ok();
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, $"Error while deleting course with id={id}.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{courseId}/instructors/{instructorId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult RemoveInstructorFromCourse([FromRoute] int courseId, [FromRoute] int instructorId)
        {
            try
            {
                if (!_courseRepo.CourseInstructorExists(courseId, instructorId))
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Cannot remove instructor with id={instructorId} from course with id={courseId} because the instructor is not assigned to this course.");
                    return NotFound("Instructor is not assigned to this course.");
                }
                _courseRepo.DeleteCourseInstructorEntry(courseId, instructorId);

                _logRepo.AddLog(AppLogLevel.Information, $"Instructor with id={instructorId} has been removed from course with id={courseId}.");

                return Ok("Instructor removed from course.");
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, $"Error while removing instructor with id={instructorId} from course with id={courseId}.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<CourseReadDto>> Search([FromQuery] string? text, [FromQuery] int page = 1,
        [FromQuery] int count = 10)
        {
            try
            {
                if (page < 1 || count < 1)
                    return BadRequest("Page and count must be greater than 0.");

                var courses = _courseRepo.Search(text, page, count)
                    .Select(MapCourseToReadDto)
                    .ToList();

                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, "Error while searching courses.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


    }
}
