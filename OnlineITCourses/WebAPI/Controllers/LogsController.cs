using DAL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/logs")]
    [ApiController]
    [Authorize]
    public class LogsController : ControllerBase
    {
        private readonly ILogRepository _logRepo;

        public LogsController(ILogRepository logRepo)
        {
            _logRepo = logRepo;
        }

        [HttpGet("get/{n}")]
        public ActionResult<IEnumerable<Log>> GetLast([FromRoute] int n)
        {
            try
            {
                return Ok(_logRepo.GetLast(n));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("count")]
        public ActionResult<int> Count()
        {
            try
            {
                return Ok(_logRepo.Count());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
