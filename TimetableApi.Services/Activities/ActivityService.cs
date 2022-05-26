using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimetableApi.Converters.DateOnly;
using TimetableApi.DAL.Generic;
using TimetableApi.EF.Models;
using TimetableApi.Services.Activities.Models;
using TimetableApi.Services.Activities.QueryObjects;

namespace TimetableApi.Services.Activities
{
    public class ActivityService : ServiceErrors
    {
        private readonly IGenericRepository<Activity>     _activitiesRepo;
        private readonly IGenericRepository<ActivityType> _activityTypesRepo;
        private readonly IGenericRepository<Role>         _rolesRepo;
        private readonly IGenericRepository<Employee>     _employeesRepo;
        private readonly IGenericRepository<Project>      _projectsRepo;
        private readonly ILogger<ActivityService>         _logger;

        public ActivityService(IGenericRepository<Activity>     activitiesRepo, 
                               IGenericRepository<ActivityType> activityTypesRepo, 
                               IGenericRepository<Role>         rolesRepo,
                               IGenericRepository<Employee>     employeesRepo,
                               IGenericRepository<Project>      projectsRepo,
                               ILogger<ActivityService>         logger)
        {
            _activitiesRepo     = activitiesRepo;
            _activityTypesRepo  = activityTypesRepo;
            _rolesRepo          = rolesRepo;
            _employeesRepo      = employeesRepo;
            _projectsRepo       = projectsRepo;
            _logger             = logger;
        }

        public async Task<List<ActivityListDto>> ListActivitiesAsync(int employeeId, DateOnly startDay, DateOnly endDay)
        {
            var query = _activitiesRepo
                .Read(a => a.EmployeeId == employeeId &&
                           a.Date >= startDay && a.Date <= endDay)
                .OrderBy(a => a.Date)
                .SelectListDto();
            return await query.ToListAsync();
        }
        public async Task<List<ActivityListDto>> ListActivitiesAsync(int employeeId, DateOnly date)
        {
            return await ListActivitiesAsync(employeeId, date, date);
        }
        public async Task<List<ActivityListDto>> ListActivitiesAsync(int employeeId, int week)
        {
            int      currentYear = DateTime.UtcNow.Year;
            DateOnly startDay    = new DateOnly(currentYear, 1, 1);
            DateOnly endDay;
            short    dayOfWeek   = (short)startDay.DayOfWeek;

            if (dayOfWeek == 0)
            {
                dayOfWeek = 7;
            }
            if (week == 1)
            {
                endDay = startDay.AddDays(7 - dayOfWeek);
            }
            else
            {
                startDay = startDay.AddDays(8 - dayOfWeek);
                week -= 2;

                startDay = startDay.AddDays(7 * week);
                endDay   = startDay.AddDays(6);

            }
            return await ListActivitiesAsync(employeeId, startDay, endDay);
        }

        public async Task<Activity> AddActivityAsync(ActivityAddDto addDto)
        {
            DateOnly date;
            try
            {
                date = DateOnlyJsonConverter.Parse(addDto.Date);
            }
            catch(Exception ex)
            {
                string message = "String '{0}' is not recognized as a valid date.";
                AddError(String.Format(message, addDto.Date), nameof(addDto.Date));
                _logger.LogInformation(ex, message, addDto.Date);
                return null;
            }
            
            
            var activity = new Activity
            {
                ProjectId = addDto.ProjectId,
                EmployeeId = addDto.EmployeeId,
                ActivityTypeId = addDto.ActivityTypeId,
                RoleId = addDto.RoleId,
                Date = date,
                Duration = addDto.Duration
            };

            try
            {
                await _activitiesRepo.CreateAsync(activity);
            }
            catch(Exception ex)
            {
                string message = "Error occured while creating activity of an employee with ID [{0}].";
                AddError(String.Format(message, activity.EmployeeId), nameof(activity));
                _logger.LogInformation(ex, message, activity.EmployeeId);

                if (!await _projectsRepo.Read(p => p.Id == activity.ProjectId).AnyAsync())
                    AddError($"Project with ID [{activity.ProjectId}] does not exist", nameof(activity.ProjectId));
                if (!await _employeesRepo.Read(p => p.Id == activity.EmployeeId).AnyAsync())
                    AddError($"Employee with ID [{activity.EmployeeId}] does not exist", nameof(activity.EmployeeId));
                if (!await _activityTypesRepo.Read(p => p.Id == activity.ActivityTypeId).AnyAsync())
                    AddError($"Activity type with ID [{activity.ActivityTypeId}] does not exist", nameof(activity.ActivityTypeId));
                if (!await _rolesRepo.Read(p => p.Id == activity.RoleId).AnyAsync())
                    AddError($"Role with ID [{activity.RoleId}] does not exist", nameof(activity.RoleId));
            }

            return activity;
        }

        public async Task<ActivityType> AddActivityTypeAsync(string name)
        {
            var activityType = new ActivityType()
            {
                Name = name,
            };

            try
            {
                await _activityTypesRepo.CreateAsync(activityType);
            }
            catch (Exception ex)
            {
                string message = "Error occured while creating activity type.";
                AddError(message, nameof(activityType));

                if(_activityTypesRepo.Read(a => a.Name == activityType.Name).Any())
                    AddError($"Activity type [{activityType.Name}] already exist.", nameof(activityType.Name));
                else
                    _logger.LogError(ex, message);
            }

            return activityType;
        }

        public async Task<Role> AddRoleAsync(string name)
        {
            var role = new Role()
            {
                Name = name,
            };

            try
            {
                await _rolesRepo.CreateAsync(role);
            }
            catch (Exception ex)
            {
                string message = "Error occured while creating role.";
                AddError(message, nameof(role));

                if (_rolesRepo.Read(r => r.Name == role.Name).Any())
                    AddError($"Role [{role.Name}] already exist.", nameof(role.Name));
                else
                    _logger.LogError(ex, message);
            }

            return role;
        }
    }
}

