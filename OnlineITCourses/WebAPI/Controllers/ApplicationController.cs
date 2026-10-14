using DAL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Dtos.ApplicationDtos;
using WebAPI.Dtos.AuthDtos;
using WebAPI.Dtos.CourseDtos;

namespace WebAPI.Controllers
{
    [Route("api/application")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationRepository _applicationRepo;
        private readonly ILogRepository _logRepo;

        public ApplicationController(IApplicationRepository applicationRepo, ILogRepository logRepo)
        {
            _applicationRepo = applicationRepo;
            _logRepo = logRepo;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<ApplicationReadDto>> GetAll()
        {
            try
            {
                var applications = _applicationRepo.RetrieveAll()
                    .Select(a => new ApplicationReadDto
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Course = new CourseSmallDto
                        {
                            Id = a.Course.Id,
                            Name = a.Course.Name
                        },

                        User = new UserSmallDto
                        {
                            Id = a.User.Id,
                            Username = a.User.Username
                        },
                        AppliedAt = a.AppliedAt,
                        Status = a.Status
                    })
                    .ToList();

                return Ok(applications);
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, "Error while retrieving all applications.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("course/{courseId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<ApplicationReadDto>> GetApplicationsForCourse([FromRoute] int courseId)
        {
            try
            {
                if (!_applicationRepo.CourseExists(courseId))
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Cannot retrieve applications. Course with id={courseId} was not found.");
                    return NotFound("Course not found.");
                }

                var applications = _applicationRepo.RetrieveForCourse(courseId)
                    .Select(a => new ApplicationReadDto
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Course = new CourseSmallDto
                        {
                            Id = a.Course.Id,
                            Name = a.Course.Name
                        },

                        User = new UserSmallDto
                        {
                            Id = a.User.Id,
                            Username = a.User.Username
                        },
                        AppliedAt = a.AppliedAt,
                        Status = a.Status
                    })
                    .ToList();

                return Ok(applications);
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, $"Error while retrieving applications for course with id={courseId}.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("myApplications")]
        [Authorize(Roles = "User")]
        public ActionResult<IEnumerable<ApplicationReadDto>> GetMyApplications()
        {
            try
            {
                var username = User.Identity!.Name!;

                var user = _applicationRepo.RetrieveUserByUsername(username);

                if (user == null)
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Cannot retrieve applications. User with username={username} was not found.");
                    return NotFound("User not found.");
                }

                var applications = _applicationRepo.RetrieveForUser(user.Id)
                    .Select(a => new ApplicationReadDto
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Course = new CourseSmallDto
                        {
                            Id = a.Course.Id,
                            Name = a.Course.Name
                        },

                        User = new UserSmallDto
                        {
                            Id = a.User.Id,
                            Username = a.User.Username
                        },
                        AppliedAt = a.AppliedAt,
                        Status = a.Status
                    })
                    .ToList();

                return Ok(applications);
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, "Error while retrieving applications for current user.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("apply")]
        [Authorize(Roles = "User")]
        public ActionResult<ApplicationDto> Apply([FromBody] ApplicationDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var username = User.Identity!.Name!;

                var user = _applicationRepo.RetrieveUserByUsername(username);

                if (user == null)
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Cannot apply to course with id={dto.CourseId}. User with username={username} was not found.");
                    return NotFound("User not found.");
                }

                var course = _applicationRepo.RetrieveCourseById(dto.CourseId);

                if (course == null)
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Cannot apply. Course with id={dto.CourseId} was not found.");
                    return NotFound("Course not found.");
                }

                if (_applicationRepo.UserAlreadyApplied(user.Id, dto.CourseId))
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"User with id={user.Id} already applied to course with id={dto.CourseId}.");
                    return BadRequest("You have already applied to this course.");
                }

                var application = new Application
                {
                    Name = $"Application for course {dto.CourseId}",
                    UserId = user.Id,
                    CourseId = dto.CourseId,
                    AppliedAt = DateTime.Now,
                    Status = "Pending"
                };

                _applicationRepo.Create(application);


                var readDto = new ApplicationReadDto
                {
                    Id = application.Id,
                    Name = application.Name,
                    Course = new CourseSmallDto
                    {
                        Id = course.Id,
                        Name = course.Name
                    },

                    User = new UserSmallDto
                    {
                        Id = user.Id,
                        Username = user.Username
                    },
                    AppliedAt = application.AppliedAt,
                    Status = application.Status
                };

                _logRepo.AddLog(AppLogLevel.Information, $"User with id={user.Id} applied to course with id={dto.CourseId}.");

                return Ok(readDto);
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, $"Error while applying to course with id={dto.CourseId}.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateStatus([FromRoute] int id, [FromBody] ApplicationStatusDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!_applicationRepo.ApplicationExists(id))
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Application with id={id} was not found.");
                    return NotFound();
                }

                var allowedStatuses = new List<string> { "Pending", "Approved", "Rejected" };

                if (!allowedStatuses.Contains(dto.Status))
                    return BadRequest("Status must be Pending, Approved, or Rejected.");

                _applicationRepo.UpdateStatus(id, dto.Status);

                _logRepo.AddLog(AppLogLevel.Information, $"Application with id={id} status has been changed to {dto.Status}.");

                return Ok($"Application status changed to {dto.Status}.");
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, $"Error while updating application status for application with id={id}.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete([FromRoute] int id)
        {
            try
            {
                if (!_applicationRepo.ApplicationExists(id))
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Application with id={id} was not found.");
                    return NotFound();
                }

                _applicationRepo.DeleteApplication(id);

                _logRepo.AddLog(AppLogLevel.Information, $"Application with id={id} has been deleted.");

                return Ok();
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, $"Error while deleting application with id={id}.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }



    }
}
