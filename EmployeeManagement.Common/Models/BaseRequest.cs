using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Common.Models
{
    public abstract class BaseRequest
    {
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }
}
