using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TimetableApi.HelperExtensions;
using TimetableApi.Services.Employees;
using TimetableApi.Services.Employees.Models;

namespace TimetableApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeService _employeeService;
        public EmployeeController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetProjects()
        {
            var employees = await _employeeService.ListAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject([Range(1, int.MaxValue)] int id)
        {
            var employee = await _employeeService.ListAsync(id);

            if (_employeeService.HasErrors)
            {
                ModelState.AddServiceErrors(_employeeService);
                return BadRequest(ModelState);
            }

            return Ok(employee);
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateProject(
            [FromBody] EmployeeAddUpdateDto addDto)
        {
            var employee = await _employeeService.AddAsync(addDto);

            if (_employeeService.HasErrors)
            {
                ModelState.AddServiceErrors(_employeeService);
                return BadRequest(ModelState);
            }

            return Ok(employee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(
            [FromRoute][Range(0, int.MaxValue)] int id, [FromBody] EmployeeAddUpdateDto updateDto)
        {
            var employee = await _employeeService.UpdateAsync(id, updateDto);

            if (_employeeService.HasErrors)
            {
                ModelState.AddServiceErrors(_employeeService);
                return BadRequest(ModelState);
            }

            return Ok(employee);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject([Range(1, int.MaxValue)] int id)
        {
            await _employeeService.RemoveAsync(id);

            if (_employeeService.HasErrors)
            {
                ModelState.AddServiceErrors(_employeeService);
                return BadRequest(ModelState);
            }

            return Ok();
        }
    }
}
