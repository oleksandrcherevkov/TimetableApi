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
    public class Employee : BaseModel
    {
        public string Name { get; set; }
        public string Sex { get; set; }
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly Birthday { get; set; }
        public ICollection<Activity> Activities { get; set; }
    }
}
