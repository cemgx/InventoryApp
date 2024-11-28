using Microsoft.AspNetCore.Mvc;
using InventoryApp.Models.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace InventoryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly InventoryAppDbContext _context;

        public LogController(InventoryAppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllLogs()
        {
            var logs = _context.Logs
                               .OrderByDescending(l => l.TimeStamp)
                               .ToList();

            return Ok(logs);
        }

        [HttpGet("{id}")]
        public IActionResult GetLogById(int id)
        {
            var log = _context.Logs.FirstOrDefault(l => l.Id == id);

            if (log == null)
                return NotFound("Log bulunamadı.");

            return Ok(log);
        }

        [HttpGet("search")]
        public IActionResult SearchLogs([FromQuery] string term)
        {
            var results = _context.Logs
                                  .Where(l => EF.Functions.Like(l.Message, $"%{term}%") ||
                                              EF.Functions.Like(l.Exception, $"%{term}%") ||
                                              EF.Functions.Like(l.Properties, $"%{term}%"))
                                  .OrderByDescending(l => l.TimeStamp)
                                  .Take(100)
                                  .ToList();

            if (results.IsNullOrEmpty())
                return NotFound("Aradığınız terim ile alakalı hiçbir sonuç bulunamadı.");

            return Ok(results);
        }
    }
}
