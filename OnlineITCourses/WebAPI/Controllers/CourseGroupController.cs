using DAL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Dtos.CourseDtos;

namespace WebAPI.Controllers
{
    [Route("api/coursegroup")]
    [ApiController]
    public class CourseGroupController : ControllerBase
    {
        private readonly ILogRepository _logRepo;
        private readonly ICourseGroupRepository _courseGroupRepo;

        public CourseGroupController(ICourseGroupRepository courseGroupRepo, ILogRepository logRepo)
        {
            _courseGroupRepo = courseGroupRepo;
            _logRepo = logRepo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CourseGroupDto>> GetAll()
        {
            try
            {
                var groups = _courseGroupRepo.RetrieveAll()
                    .Select(g => new CourseGroupDto
                    {
                        Id = g.Id,
                        Name = g.Name,
                        Description = g.Description
                    })
                    .ToList();

                return Ok(groups);
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, $"Error while retrieving course groups", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<CourseGroupDto> GetById([FromRoute] int id)
        {
            try
            {
                var group = _courseGroupRepo.RetrieveById(id);

                if (group == null)
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Course group with id={id} was not found.");
                    return NotFound();
                }
                var dto = new CourseGroupDto
                {
                    Id = group.Id,
                    Name = group.Name,
                    Description = group.Description
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, $"Error while retrieving course group with id={id}.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<CourseGroupDto> Create([FromBody] CourseGroupDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var group = new CourseGroup
                {
                    Name = dto.Name,
                    Description = dto.Description
                };

                _courseGroupRepo.Create(group);

                _logRepo.AddLog(AppLogLevel.Information, $"Course group with id={group.Id} has been created.");

                dto.Id = group.Id;

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, $"Error while creating course group.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Update([FromRoute] int id, [FromBody] CourseGroupDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!_courseGroupRepo.CourseGroupExists(id))
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Course group with id={id} was not found.");
                    return NotFound();
                }
                var courseGroup = new CourseGroup
                {
                    Name = dto.Name,
                    Description = dto.Description
                };

                _courseGroupRepo.Update(id, courseGroup);

                _logRepo.AddLog(AppLogLevel.Information, $"Course group with id={courseGroup.Id} has been updated.");

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, $"Error while updating course group with id={id}.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete([FromRoute] int id)
        {
            try
            {
                if (!_courseGroupRepo.CourseGroupExists(id))
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Course group with id={id} was not found.");
                    return NotFound();
                }

                if (!_courseGroupRepo.CanDeleteCourseGroup(id))
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Cannot delete course group with id={id} because courses are using it.");
                    return BadRequest("Cannot delete course group because courses are using it.");
                }

                _courseGroupRepo.DeleteCourseGroup(id);

                _logRepo.AddLog(AppLogLevel.Information, $"Course group with id={id} has been deleted.");

                return Ok();
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, $"Error while deleting course group with id={id}.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }



}
