using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimetableApi.Converters.DateOnly;

namespace TimetableApi.Services.Activities.Models
{
    public class ActivityAddDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ProjectId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int EmployeeId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int RoleId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int ActivityTypeId { get; set; }
        [Required]
        public string Date { get; set; }
        [Required]
        [Range(0.5, int.MaxValue)]
        public float Duration { get; set; }
    }
}
