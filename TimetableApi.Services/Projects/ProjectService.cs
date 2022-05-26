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
using TimetableApi.Services.Projects.Models;
using TimetableApi.Services.Projects.QueryObjects;

namespace TimetableApi.Services.Projects
{
    public class ProjectService : ServiceErrors
    {
        private const string NotFoundMessage = "Project with ID [{0}] not found.";
        private const string ErrorMessage = "Error occured while {0} project";
        private const string ErrorWithIDMessage = ErrorMessage + " with ID [{1}].";
        private const string DateErrorMessage = "String '{0}' is not recognized as a valid date.";

        private readonly IGenericRepository<Project> _projectsRepo;
        private readonly ILogger<ProjectService>     _logger;

        public ProjectService(IGenericRepository<Project> projectsRepo,
                              ILogger<ProjectService> logger)
        {
            _projectsRepo = projectsRepo;
            _logger = logger;
        }
        public async Task<List<ProjectListDto>> ListAsync()
        {
            return await _projectsRepo
                .Read()
                .SelectListDto()
                .ToListAsync();
        }
        public async Task<ProjectListDto> ListAsync(int id)
        {
            ProjectListDto project = await _projectsRepo
                .Read(p => p.Id == id)
                .SelectListDto()
                .FirstOrDefaultAsync();

            if (project is null)
            {
                AddError(String.Format(NotFoundMessage, id), nameof(id));
                _logger.LogInformation(NotFoundMessage, id);
            }

            return project;
        }

        public async Task<Project> AddAsync(ProjectAddUpdateDto addDto)
        {
            DateOnly startDate;
            try
            {
                startDate = DateOnlyJsonConverter.Parse(addDto.StartDate);
            }
            catch (Exception ex)
            {
                AddError(String.Format(DateErrorMessage, addDto.StartDate), nameof(addDto.StartDate));
                _logger.LogInformation(ex, DateErrorMessage, addDto.StartDate);
            }
            DateOnly endDate;
            try
            {
                endDate = DateOnlyJsonConverter.Parse(addDto.EndDate);
            }
            catch (Exception ex)
            {
                AddError(String.Format(DateErrorMessage, addDto.EndDate), nameof(addDto.EndDate));
                _logger.LogInformation(ex, DateErrorMessage, addDto.EndDate);
            }

            if (HasErrors)
                return null;

            var project = new Project()
            {
                Name = addDto.Name,
                StartDate = startDate,
                EndDate = endDate
            };
            try
            {
                await _projectsRepo.CreateAsync(project);
            }
            catch (Exception ex)
            {
                string message = String.Format(ErrorMessage, "creating");
                AddError(message, nameof(project));
                _logger.LogWarning(ex, message);
            }
            return project;
        }

        public async Task<Project> UpdateAsync(int id, ProjectAddUpdateDto updateDto)
        {
            Project project = await _projectsRepo.FindByIdAsync(id);
            if (project is null)
            {
                AddError(String.Format(NotFoundMessage, id), nameof(id));
                _logger.LogInformation(NotFoundMessage, id);
            }


            DateOnly startDate;
            try
            {
                startDate = DateOnlyJsonConverter.Parse(updateDto.StartDate);
            }
            catch (Exception ex)
            {
                AddError(String.Format(DateErrorMessage, updateDto.StartDate), nameof(updateDto.StartDate));
                _logger.LogInformation(ex, DateErrorMessage, updateDto.StartDate);
            }
            DateOnly endDate;
            try
            {
                endDate = DateOnlyJsonConverter.Parse(updateDto.EndDate);
            }
            catch (Exception ex)
            {
                AddError(String.Format(DateErrorMessage, updateDto.EndDate), nameof(updateDto.EndDate));
                _logger.LogInformation(ex, DateErrorMessage, updateDto.EndDate);
            }

            if (HasErrors)
                return null;

            project.Name = updateDto.Name;
            project.StartDate = startDate;
            project.EndDate = endDate;
            try 
            {
                await _projectsRepo.UpdateAsync(project);
            }
            catch (Exception ex)
            {
                AddError(String.Format(ErrorWithIDMessage, "updating", id), nameof(id));
                _logger.LogWarning(ex, ErrorWithIDMessage, "updating", id);
            }

            return project;
        }

        public async Task RemoveAsync(int id)
        {
            try
            {
                await _projectsRepo.RemoveByIdAsync(id);
            }
            catch (Exception ex)
            {
                AddError(String.Format(ErrorWithIDMessage, "deleting", id), "project");
                
                if(! await _projectsRepo.Read(p => p.Id == id).AnyAsync())
                {
                    AddError(String.Format(NotFoundMessage, id), nameof(id));
                    _logger.LogInformation(NotFoundMessage, id);
                }
                else
                    _logger.LogError(ex, ErrorWithIDMessage, "deleting", id);

            }
        }
    }
}
