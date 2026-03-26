using EmployeeManagement.Common.Interfaces;
using EmployeeManagement.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _service;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IEmployeeService service, ILogger<EmployeesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResult<EmployeeDto>>>> GetEmployees(
            [FromQuery] GridRequest request)
        {
            var response = await _service.GetEmployeesAsync(request);
            return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<EmployeeDto?>>> GetEmployee(int id)
        {
            var response = await _service.GetEmployeeByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> AddEmployee([FromForm] EmployeeCreateRequest request, IFormFile? profileImage)
        {
            var response = await _service.AddEmployeeAsync(request, profileImage);
            return response.Success ? CreatedAtAction(nameof(GetEmployee), new { id = response.Data }, response)
                                    : StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateEmployee(int id, [FromForm] EmployeeUpdateRequest request, IFormFile? profileImage)
        {
            if (id != request.Row_Id)
                return BadRequest(ApiResponse<bool>.ErrorResponse("ID mismatch", 400));

            var response = await _service.UpdateEmployeeAsync(request, profileImage);
            return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteEmployee(int id)
        {
            var response = await _service.DeleteEmployeeAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("countries")]
        public async Task<ActionResult<ApiResponse<IEnumerable<LocationDto>>>> GetCountries()
        {
            var response = await _service.GetCountriesAsync();
            return Ok(response);
        }

        [HttpGet("countries/{countryId}/states")]
        public async Task<ActionResult<ApiResponse<IEnumerable<LocationDto>>>> GetStates(int countryId)
        {
            var response = await _service.GetStatesByCountryAsync(countryId);
            return Ok(response);
        }

        [HttpGet("states/{stateId}/cities")]
        public async Task<ActionResult<ApiResponse<IEnumerable<LocationDto>>>> GetCities(int stateId)
        {
            var response = await _service.GetCitiesByStateAsync(stateId);
            return Ok(response);
        }
    }
}
