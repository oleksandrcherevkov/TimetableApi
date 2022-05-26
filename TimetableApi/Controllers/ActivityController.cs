using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TimetableApi.Converters.DateOnly;
using TimetableApi.HelperExtensions;
using TimetableApi.Services.Activities;
using TimetableApi.Services.Activities.Models;

namespace TimetableApi.Controllers
{
    [Route("api/employee/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly ActivityService _activityService;
        public ActivityController(ActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetActivity(
            [Range(0, int.MaxValue)] int employeeId, [FromQuery] string dateString)
        {
            DateOnly date;
            try
            {
                date = DateOnlyJsonConverter.Parse(dateString);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(dateString), $"String '{dateString}' is not recognized as a valid date.");
                return BadRequest(ModelState);
            }
            var activities = await _activityService.ListActivitiesAsync(employeeId, date);
            return Ok(activities);
        }

        [HttpGet("{employeeId}/{week}")]
        public async Task<IActionResult> GetActivity(
            [Range(0, int.MaxValue)] int employeeId, int week)
        {
            var activities = await _activityService.ListActivitiesAsync(employeeId, week);
            return Ok(activities);
        }

        [HttpPost("")]
        public async Task<IActionResult> AddActivity([FromBody] ActivityAddDto addDto)
        {
            var activity = await _activityService.AddActivityAsync(addDto);

            if (_activityService.HasErrors)
            {
                ModelState.AddServiceErrors(_activityService);
                return BadRequest(ModelState);
            }

            return Ok(activity);
        }

        [HttpPost("type")]
        public async Task<IActionResult> AddActivityType([FromBody] string typeName)
        {
            var activityType = await _activityService.AddActivityTypeAsync(typeName);

            if (_activityService.HasErrors)
            {
                ModelState.AddServiceErrors(_activityService);
                return BadRequest(ModelState);
            }

            return Ok(activityType);
        }

        [HttpPost("role")]
        public async Task<IActionResult> AddRole([FromBody] string roleName)
        {
            var role = await _activityService.AddActivityTypeAsync(roleName);

            if (_activityService.HasErrors)
            {
                ModelState.AddServiceErrors(_activityService);
                return BadRequest(ModelState);
            }

            return Ok(role);
        }
    }
}
