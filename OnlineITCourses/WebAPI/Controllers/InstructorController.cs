using DAL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Dtos.CourseDtos;

namespace WebAPI.Controllers
{
    [Route("api/instructor")]
    [ApiController]
    public class InstructorController : ControllerBase
    {
        private readonly IInstructorRepository _instructorRepo;
        private readonly ILogRepository _logRepo;

        public InstructorController(IInstructorRepository instructorRepo, ILogRepository logRepo)
        {
            _instructorRepo = instructorRepo;
            _logRepo = logRepo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<InstructorDto>> GetAll()
        {
            try
            {
                var instructors = _instructorRepo.RetrieveAll()
                    .Select(i => new InstructorDto
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Email = i.Email,
                        Biography = i.Biography
                    })
                    .ToList();

                return Ok(instructors);
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, "Error while retrieving all instructors.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<InstructorDto> GetById([FromRoute] int id)
        {
            try
            {
                var i = _instructorRepo.RetrieveById(id);

                if (i == null)
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Instructor with id={id} was not found.");
                    return NotFound();
                }
                var dto = new InstructorDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Email = i.Email,
                    Biography = i.Biography
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, $"Error while retrieving instructor with id={id}.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<InstructorDto> Create([FromBody] InstructorDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var instructor = new Instructor
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Biography = dto.Biography
                };

                _instructorRepo.Create(instructor);

                dto.Id = instructor.Id;

                _logRepo.AddLog(AppLogLevel.Information, $"Instructor with id={instructor.Id} has been created.");

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, "Error while creating instructor.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Update([FromRoute] int id, [FromBody] InstructorDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!_instructorRepo.InstructorExists(id))
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Instructor with id={id} was not found.");
                    return NotFound();
                }

                var instructor = new Instructor
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Biography = dto.Biography
                };

                _instructorRepo.Update(id, instructor);

                _logRepo.AddLog(AppLogLevel.Information, $"Instructor with id={id} has been updated.");

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, $"Error while updating instructor with id={id}.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            try
            {
                if (!_instructorRepo.InstructorExists(id))
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Instructor with id={id} was not found.");
                    return NotFound();
                }

                if (!_instructorRepo.CanDeleteInstructor(id))
                {
                    _logRepo.AddLog(AppLogLevel.Warning, $"Cannot delete instructor with id={id} because the instructor is assigned to one or more courses.");
                    return BadRequest("Cannot delete instructor because it is assigned to one or more courses.");
                }

                _instructorRepo.DeleteInstructor(id);

                _logRepo.AddLog(AppLogLevel.Information, $"Instructor with id={id} has been deleted.");

                return Ok();
            }
            catch (Exception ex)
            {
                _logRepo.AddLog(AppLogLevel.Error, $"Error while deleting instructor with id={id}.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }


}
