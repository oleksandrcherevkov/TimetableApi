using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimetableApi.Converters.DateOnly;

namespace TimetableApi.Services.Employees.Models
{
    public class EmployeeListDto : EmployeeAddUpdateDto
    {

        public string Name { get; set; }
        public string Sex { get; set; }
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly Birthday { get; set; }
        public string[] Projects { get; set; }
    }
}
