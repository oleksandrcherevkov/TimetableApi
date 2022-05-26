using TimetableApi.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace TimetableApi.HelperExtensions
{
    public static class ModelStateErrorsForm
    {
        /// <summary>
        /// Add service errors to ModelStateDictionary.
        /// </summary>
        /// <param name="modelState"></param>
        /// <param name="service"></param>
        public static void AddServiceErrors(this ModelStateDictionary modelState, ServiceErrors service)
        {
            foreach (var error in service.Errors)
            {
                var properties = error.MemberNames.ToList();
                modelState.AddModelError(properties.Any() ? properties.First() : "", error.ErrorMessage);
            }
        }
    }
}
