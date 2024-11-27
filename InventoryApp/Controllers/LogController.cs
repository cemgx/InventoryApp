using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllLogs()
        {
            var logDirectory = "C:\\Work\\InventoryApp\\InventoryApp\\logs";
            if (!Directory.Exists(logDirectory))
            {
                return NotFound("Log path bulunamadı");
            }

            var logs = Directory.GetFiles(logDirectory, "log-*.txt")
                                .Select(Path.GetFileName)
                                .ToList();

            return Ok(logs);
        }

        [HttpGet("{logFileName}")]
        public IActionResult GetLogFile(string logFileName)
        {
            var logDirectory = "C:\\Work\\InventoryApp\\InventoryApp\\logs";
            var filePath = Path.Combine(logDirectory, logFileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Log dosyası bulunamadı");
            }

            var content = System.IO.File.ReadAllText(filePath);
            return Ok(content);
        }

        [HttpGet("search")]
        public IActionResult SearchLogs([FromQuery] string term)
        {
            var logDirectory = "C:\\Work\\InventoryApp\\InventoryApp\\logs";
            if (!Directory.Exists(logDirectory))
            {
                return NotFound("Log path bulunamadı");
            }

            var results = new List<string>();
            var logFiles = Directory.GetFiles(logDirectory, "log-*.txt");

            foreach (var logFile in logFiles)
            {
                using var stream = new FileStream(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(stream);
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line != null && line.Contains(term, StringComparison.OrdinalIgnoreCase))
                    {
                        results.Add(line);
                    }
                }
            }

            if (results.Count == 0 || results == null)
            {
                return NotFound("Aradığınız terim ile alakalı hiçbir sonuç bulunamadı");
            }

            return Ok(results);
        }
    }
}
