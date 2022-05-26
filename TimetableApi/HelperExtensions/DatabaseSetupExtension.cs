using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Logging;
using TimetableApi.EF;
using TimetableApi.DAL;
using TimetableApi;
using TimetableApi.DAL.Generic;
using TimetableApi.EF.Models;
using RandomNameGeneratorLibrary;

namespace ChatApi.HelperExtensions
{
    public static class DatabaseSetupExtension
    {
        /// <summary>
        /// Migrates and seeds database.
        /// 
        /// In case of exeption deletes whole database.
        /// </summary>
        /// <param name="webHost"></param>
        /// <returns></returns>
        public static async Task<IHost> SetupDatabaseAsync(this IHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())                                                      //#A
            {
                var services = scope.ServiceProvider;                                                               //#B

                var context             = services.GetRequiredService<AppDbContext>();                              //#B
                var projectsRepo        = services.GetRequiredService<IGenericRepository<Project>>();
                var employeesRepo       = services.GetRequiredService<IGenericRepository<Employee>>();
                var activitiesRepo      = services.GetRequiredService<IGenericRepository<Activity>>();
                var activityTypesRepo   = services.GetRequiredService<IGenericRepository<ActivityType>>();
                var rolesRepo           = services.GetRequiredService<IGenericRepository<Role>>();

                try
                {
                    var arePendingMigrations = context.Database                                                     //#C
                        .GetPendingMigrations()
                        .Any();
                    await context.Database.MigrateAsync();

                    if (arePendingMigrations)                                                                       //#C
                    {
                        Random random = new();
                        int employeesCount  = 20;
                        int projectsCount   = 5;

                        var employees = RandomEmployee(employeesCount, random);
                        var projects  = GetProjects(projectsCount);

                        await projectsRepo.CreateAsync(projects);
                        await activityTypesRepo.CreateAsync(activityTypes);
                        await rolesRepo.CreateAsync(roles);
                        await employeesRepo.CreateAsync(employees);

                        int activityTypesCount  = activityTypes.Length;
                        int rolesCount          = roles.Length;

                        int activitiesCount = 1000;
                        Activity[] activities = new Activity[activitiesCount];
                        Parallel.For(0, activitiesCount, i =>
                        {
                            int projectNum      = random.Next(projectsCount);
                            int employeeNum     = random.Next(employeesCount);
                            int activityTypeNum = i % activityTypesCount;
                            int roleNum         = i % rolesCount;

                            int projectDuration =
                             (projects[projectNum].EndDate.DayNumber
                            - projects[projectNum].StartDate.DayNumber);
                            int dayOfProject = i % projectDuration;
                            DateOnly date    = projects[projectNum].StartDate
                                                  .AddDays(dayOfProject);

                            int duration = i % 8 + 1;

                            activities[i] = new Activity()
                            {
                                ProjectId       = projects[projectNum].Id,
                                EmployeeId      = employees[employeeNum].Id,
                                ActivityTypeId  = activityTypes[activityTypeNum].Id,
                                RoleId          = roles[roleNum].Id,
                                Date            = date,
                                Duration        = duration
                            };
                        });

                        await activitiesRepo.CreateAsync(activities);
                    }
                }
                catch (Exception ex)                                                                                //#F
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogCritical(ex, "An error occurred while creating/migrating or seeding the database.");
                    throw;
                }
            }

            return webHost;
        }

        private static Employee[] RandomEmployee(int count, Random random)
        {
            var personGenerator   = new PersonNameGenerator();
            DateOnly birthdayDate = new DateOnly(2000, 1, 1);
            string[] names        = personGenerator
                .GenerateMultipleFirstAndLastNames(count)
                .ToArray();

            var employees = Enumerable.Range(0, count)
                .AsParallel()
                .Select(n => new Employee()
                {
                    Name     = names[n],
                    Sex      = "string",
                    Birthday = birthdayDate.AddDays(n)
                });
            return employees.ToArray();
        }

        private static Project[] GetProjects(int count)
        {
            DateOnly projectStartDate = new DateOnly(2022, 1, 1);
            Project[] projects = new Project[count];
            for (int i = 0; i < count; i++)
            {
                projects[i] = new Project()
                {
                    Name      = $"Project_{i}",
                    StartDate = projectStartDate.AddDays(i * 9),
                    EndDate   = projectStartDate.AddDays(i * 15 + 6)
                };
            }
            return projects;
        }

        private static readonly ActivityType[] activityTypes =
        {
            new(){Name = "regular work"},
            new(){Name = "overtime"}
        };

        private static readonly Role[] roles =
        {
            new(){Name = "software engineer"},
            new(){Name = "software architect"},
            new(){Name = "buisiness analyst"},
            new(){Name = "team lead"}
        };
    }
    /*====================================================
    #A: Create scope where all required services will be encapsulated.
    #B: Get repos for seed and DB context.
    #C: If there are penging migrations, try to fill the database.
    #F: If exeption occured, log error and ~~detete database~~. 
        If database if fully migarted and error occured on the seeding stage, the seeding script would not be executed next time.
    ====================================================*/
}
