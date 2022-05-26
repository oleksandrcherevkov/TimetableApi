using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimetableApi.EF.Models;
using TimetableApi.Services.Employees.Models;
using TimetableApi.Services.Projects.Models;

namespace TimetableApi.Services.Employees.QueryObjects
{
    public static class SelectEmployeeListDto
    {
        public static IQueryable<EmployeeListDto> SelectListDto(this IQueryable<Employee> query)
        {
            return query.Select(e => new EmployeeListDto()
            {
                Name        = e.Name,
                Sex = e.Sex,
                Birthday = e.Birthday,
                Projects = e.Activities
                            .Select(a => a.Project)
                            .Distinct()
                            .OrderBy(p => p.StartDate)
                            .Select(p => p.Name)
                            .ToArray()
            });
        }
    }
}
