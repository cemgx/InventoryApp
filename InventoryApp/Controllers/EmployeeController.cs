using InventoryApp.Models.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly InventoryAppDbContext _context;

        public EmployeeController(InventoryAppDbContext context)
        {
            _context = context;
        }


    }
}
