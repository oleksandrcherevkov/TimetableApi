using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimetableApi.Converters.DateOnly;
using TimetableApi.EF.Models.Base;

namespace TimetableApi.EF.Models
{
    public class Activity : BaseModel
    {
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
        public int RoleId { get; set; }
        public int ActivityTypeId { get; set; }
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly Date { get; set; }
        public float Duration { get; set; }

        public Role         Role { get; set; }
        public ActivityType ActivityType { get; set; }
        public Project      Project { get; set; }
        public Employee     Employee { get; set; }
    }
}
