using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TimetableApi.DAL.Generic;
using TimetableApi.EF.Models;
using TimetableApi.Services.Activities;
using Xunit;

namespace TimetableApi.Tests
{
    public class ActivityServiceTests
    {
        [Fact]
        public async Task AddRole()
        {
            var mockActivitiesRepo = new Mock<IGenericRepository<Activity>>();
            var mockActivityTypesRepo = new Mock<IGenericRepository<ActivityType>>();
            var mockRolesRepo = new Mock<IGenericRepository<Role>>();
            var mockEmployeesRepo = new Mock<IGenericRepository<Employee>>();
            var mockProjectsRepo = new Mock<IGenericRepository<Project>>();
            var mockLogger = new Mock<ILogger<ActivityService>>();
            string roleName = "Test";

            // Arrange
            ActivityService service = new ActivityService(mockActivitiesRepo.Object, mockActivityTypesRepo.Object, mockRolesRepo.Object, mockEmployeesRepo.Object, mockProjectsRepo.Object, mockLogger.Object);

            // Act
            Role result = await service.AddRoleAsync(roleName);

            // Assert
            Assert.Equal(roleName, result.Name);
        }

        [Fact]
        public async Task AddDublicateRole()
        {
                var mockActivitiesRepo = new Mock<IGenericRepository<Activity>>();
                var mockActivityTypesRepo = new Mock<IGenericRepository<ActivityType>>();
                var mockRolesRepo = new Mock<IGenericRepository<Role>>();
                var mockEmployeesRepo = new Mock<IGenericRepository<Employee>>();
                var mockProjectsRepo = new Mock<IGenericRepository<Project>>();
                var mockLogger = new Mock<ILogger<ActivityService>>();
                string roleName = "Test";
                Role role = new() { Name = roleName };
                mockRolesRepo.Setup(repo => repo.CreateAsync(It.IsAny<Role>()))
                    .ThrowsAsync(new Exception());
                mockRolesRepo.Setup(repo => repo.Read(It.IsAny<Expression<Func<Role, bool>>>()))
                    .Returns(new List<Role>() { new Role { Name = roleName } }.AsQueryable());

                // Arrange
                ActivityService service = new ActivityService(mockActivitiesRepo.Object, mockActivityTypesRepo.Object, mockRolesRepo.Object, mockEmployeesRepo.Object, mockProjectsRepo.Object, mockLogger.Object);

                // Act
                Role result = await service.AddRoleAsync(roleName);


                // Assert
                Assert.True(service.HasErrors);
                Assert.NotEmpty(service.Errors);
                Assert.Equal(2, service.Errors.Count);
        }
    }
}
