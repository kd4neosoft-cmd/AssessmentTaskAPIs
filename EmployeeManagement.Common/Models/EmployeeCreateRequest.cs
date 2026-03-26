using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Common.Models
{
    public class EmployeeCreateRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public string EmailAddress { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string PanNumber { get; set; } = string.Empty;
        public string PassportNumber { get; set; } = string.Empty;
        public int Gender { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime DateOfBirth { get; set; }
        public DateTime? DateOfJoinee { get; set; }
    }

}
