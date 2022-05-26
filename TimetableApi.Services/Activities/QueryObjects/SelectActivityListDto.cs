using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimetableApi.EF.Models;
using TimetableApi.Services.Activities.Models;

namespace TimetableApi.Services.Activities.QueryObjects
{
    public static class SelectActivityListDto
    {
        public static IQueryable<ActivityListDto> SelectListDto(this IQueryable<Activity> query)
        {
            return query.Select(a => new ActivityListDto()
            {
                ProjectName  = a.Project.Name,
                EmployeeName = a.Employee.Name,
                ActivityType = a.ActivityType.Name,
                Role         = a.Role.Name,
                Date         = a.Date,
                Duration     = a.Duration
            });
        }
    }
}
