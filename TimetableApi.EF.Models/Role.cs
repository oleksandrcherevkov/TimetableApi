using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimetableApi.EF.Models.Base;

namespace TimetableApi.EF.Models
{
    public class Role : BaseModel
    {
        public string Name { get; set; }
    }
}
