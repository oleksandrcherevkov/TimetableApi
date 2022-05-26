using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TimetableApi.HelperExtensions;
using TimetableApi.Services.Projects;
using TimetableApi.Services.Projects.Models;

namespace TimetableApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectService _projectService;
        public ProjectController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await _projectService.ListAsync();
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject([Range(1, int.MaxValue)] int id)
        {
            var project = await _projectService.ListAsync(id);

            if (_projectService.HasErrors)
            {
                ModelState.AddServiceErrors(_projectService);
                return BadRequest(ModelState);
            }

            return Ok(project);
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateProject(
            [FromBody] ProjectAddUpdateDto addDto)
        {
            var project = await _projectService.AddAsync(addDto);

            if (_projectService.HasErrors)
            {
                ModelState.AddServiceErrors(_projectService);
                return BadRequest(ModelState);
            }

            return Ok(project);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(
            [FromRoute][Range(1, int.MaxValue)] int id, [FromBody] ProjectAddUpdateDto updateDto)
        {
            var project = await _projectService.UpdateAsync(id, updateDto);

            if (_projectService.HasErrors)
            {
                ModelState.AddServiceErrors(_projectService);
                return BadRequest(ModelState);
            }

            return Ok(project);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject([Range(1, int.MaxValue)] int id)
        {
            await _projectService.RemoveAsync(id);

            if (_projectService.HasErrors)
            {
                ModelState.AddServiceErrors(_projectService);
                return BadRequest(ModelState);
            }

            return Ok();
        }
    }
}
