using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimetableApi.Converters.DateOnly;

namespace TimetableApi.Services.Activities.Models
{
    public class ActivityListDto
    {
        public string ProjectName { get; set; }
        public string EmployeeName { get; set; }
        public string Role { get; set; }
        public string ActivityType { get; set; }
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly Date { get; set; }
        public float Duration { get; set; }
    }
}
