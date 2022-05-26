using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimetableApi.EF.Models;
using TimetableApi.Services.Projects.Models;

namespace TimetableApi.Services.Projects.QueryObjects
{
    public static class SelectEmployeeListDto
    {
        public static IQueryable<ProjectListDto> SelectListDto(this IQueryable<Project> query)
        {
            return query.Select(p => new ProjectListDto()
            {
                Name        = p.Name,
                StartDate   = p.StartDate,
                EndDate     = p.EndDate,
                Employees   = p.Activities.Select(a => a.Employee)
                                    .Distinct()
                                    .Select(e => e.Name)
                                    .ToArray()
            });
        }
    }
}
