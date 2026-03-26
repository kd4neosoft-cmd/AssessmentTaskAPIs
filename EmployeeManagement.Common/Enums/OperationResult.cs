using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Common.Enums
{
    public enum OperationResult
    {
        Success = 200,
        Created = 201,
        BadRequest = 400,
        NotFound = 404,
        Conflict = 409,
        ServerError = 500
    }
}
