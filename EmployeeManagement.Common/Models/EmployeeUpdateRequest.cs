using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Common.Models
{
    public class EmployeeUpdateRequest : EmployeeCreateRequest
    {
        public int Row_Id { get; set; }
    }
}
