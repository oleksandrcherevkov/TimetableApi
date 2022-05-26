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
using TimetableApi.Services.Employees.Models;
using TimetableApi.Services.Employees.QueryObjects;

namespace TimetableApi.Services.Employees
{
    public class EmployeeService : ServiceErrors
    {
        private const string NotFoundMessage = "Employee with ID [{0}] not found.";
        private const string ErrorMessage = "Error occured while {0} employee";
        private const string ErrorWithIDMessage = ErrorMessage + " with ID [{1}].";
        private const string DateErrorMessage = "String '{0}' is not recognized as a valid date.";

        private readonly IGenericRepository<Employee> _employeesRepo;
        private readonly ILogger<EmployeeService>     _logger;

        public EmployeeService(IGenericRepository<Employee> employeesRepo,
                               ILogger<EmployeeService> logger)
        {
            _employeesRepo = employeesRepo;
            _logger = logger;
        }
        public async Task<List<EmployeeListDto>> ListAsync()
        {
            return await _employeesRepo
                .Read()
                .SelectListDto()
                .ToListAsync();
        }
        public async Task<EmployeeListDto> ListAsync(int id)
        {
            EmployeeListDto project = await _employeesRepo
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

        public async Task<Employee> AddAsync(EmployeeAddUpdateDto addDto)
        {
            DateOnly birthday;
            try
            {
                birthday = DateOnlyJsonConverter.Parse(addDto.Birthday);
            }
            catch (Exception ex)
            {
                AddError(String.Format(DateErrorMessage, addDto.Birthday), nameof(addDto.Birthday));
                _logger.LogInformation(ex, DateErrorMessage, addDto.Birthday);
                return null;
            }

            var project = new Employee()
            {
                Name = addDto.Name,
                Sex = addDto.Sex,
                Birthday = birthday
            };
            try
            {
                await _employeesRepo.CreateAsync(project);
            }
            catch (Exception ex)
            {
                AddError(String.Format(ErrorMessage, "creating"), nameof(project));
                _logger.LogError(ex, ErrorMessage, "creating");
            }
            return project;
        }

        public async Task<Employee> UpdateAsync(int id, EmployeeAddUpdateDto updateDto)
        {
            Employee employee = await _employeesRepo.FindByIdAsync(id);
            if (employee is null)
            {
                AddError(String.Format(NotFoundMessage, id), nameof(id));
                _logger.LogInformation(NotFoundMessage, id);
                return employee;
            }

            DateOnly birthday;
            try
            {
                birthday = DateOnlyJsonConverter.Parse(updateDto.Birthday);
            }
            catch (Exception ex)
            {
                AddError(String.Format(DateErrorMessage, updateDto.Birthday), nameof(updateDto.Birthday));
                _logger.LogInformation(ex, DateErrorMessage, updateDto.Birthday);
                return employee;
            }

            employee.Name = updateDto.Name;
            employee.Sex = updateDto.Sex;
            employee.Birthday = birthday;
            try
            {
                await _employeesRepo.UpdateAsync(employee);
            }
            catch (Exception ex)
            {
                AddError(String.Format(ErrorWithIDMessage, "updating", id), nameof(id));
                _logger.LogInformation(ex, ErrorWithIDMessage, "updating", id);
            }

            return employee;
        }

        public async Task RemoveAsync(int id)
        {
            try
            {
                await _employeesRepo.RemoveByIdAsync(id);
            }
            catch (Exception ex)
            {
                AddError(String.Format(ErrorWithIDMessage, "deleting", id), "employee");

                if (!await _employeesRepo.Read(p => p.Id == id).AnyAsync())
                    AddError(String.Format(NotFoundMessage, id), nameof(id));
                else
                    _logger.LogError(ex, ErrorWithIDMessage, "deleting", id);
            }
        }
    }
}

